using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MemoirsOfThePast.Infrastructure.AgentFrameworkSample
{
    public class AgentExecutor
    {
        /// <summary>
        /// A class representing the output of the slogan writer agent.
        /// </summary>
        public sealed class SloganResult
        {
            [JsonPropertyName("task")]
            public required string Task { get; set; }

            [JsonPropertyName("slogan")]
            public required string Slogan { get; set; }
        }

        /// <summary>
        /// A class representing the output of the feedback agent.
        /// </summary>
        public sealed class FeedbackResult
        {
            [JsonPropertyName("comments")]
            public string Comments { get; set; } = string.Empty;

            [JsonPropertyName("rating")]
            public int Rating { get; set; }

            [JsonPropertyName("actions")]
            public string Actions { get; set; } = string.Empty;
        }

        /// <summary>
        /// A custom event to indicate that a slogan has been generated.
        /// </summary>
        public sealed class SloganGeneratedEvent(SloganResult sloganResult) : WorkflowEvent(sloganResult)
        {
            public override string ToString() => $"Slogan: {sloganResult.Slogan}";
        }

        /// <summary>
        /// A custom executor that uses an AI agent to generate slogans based on a given task.
        /// Note that this executor has two message handlers:
        /// 1. HandleAsync(string message): Handles the initial task to create a slogan.
        /// 2. HandleAsync(Feedback message): Handles feedback to improve the slogan.
        /// </summary>
        public sealed class SloganWriterExecutor : Executor
        {
            private readonly AIAgent _agent;
            private readonly AgentThread _thread;

            /// <summary>
            /// Initializes a new instance of the <see cref="SloganWriterExecutor"/> class.
            /// </summary>
            /// <param name="id">A unique identifier for the executor.</param>
            /// <param name="chatClient">The chat client to use for the AI agent.</param>
            public SloganWriterExecutor(string id, IChatClient chatClient) : base(id)
            {
                ChatClientAgentOptions agentOptions = new(instructions: "You are a professional slogan writer. You will be given a task to create a slogan.")
                {
                    ChatOptions = new()
                    {
                        ResponseFormat = ChatResponseFormat.ForJsonSchema<SloganResult>()
                    }
                };

                this._agent = new ChatClientAgent(chatClient, agentOptions);
                this._thread = this._agent.GetNewThread();
            }

            protected override RouteBuilder ConfigureRoutes(RouteBuilder routeBuilder) =>
                routeBuilder.AddHandler<string, SloganResult>(this.HandleAsync)
                            .AddHandler<FeedbackResult, SloganResult>(this.HandleAsync);

            /// <summary>
            /// 接受用户输入消息
            /// </summary>
            /// <param name="message"></param>
            /// <param name="context"></param>
            /// <param name="cancellationToken"></param>
            /// <returns></returns>
            /// <exception cref="InvalidOperationException"></exception>
            public async ValueTask<SloganResult> HandleAsync(string message, IWorkflowContext context, CancellationToken cancellationToken = default)
            {
                var result = await this._agent.RunAsync(message, this._thread, cancellationToken: cancellationToken);

                var sloganResult = JsonSerializer.Deserialize<SloganResult>(result.Text) ?? throw new InvalidOperationException("Failed to deserialize slogan result.");

                await context.AddEventAsync(new SloganGeneratedEvent(sloganResult), cancellationToken);
                return sloganResult;
            }

            public async ValueTask<SloganResult> HandleAsync(FeedbackResult message, IWorkflowContext context, CancellationToken cancellationToken = default)
            {
                var feedbackMessage = $"""
            Here is the feedback on your previous slogan:
            Comments: {message.Comments}
            Rating: {message.Rating}
            Suggested Actions: {message.Actions}

            Please use this feedback to improve your slogan.
            """;

                var result = await this._agent.RunAsync(feedbackMessage, this._thread, cancellationToken: cancellationToken);
                var sloganResult = JsonSerializer.Deserialize<SloganResult>(result.Text) ?? throw new InvalidOperationException("Failed to deserialize slogan result.");

                await context.AddEventAsync(new SloganGeneratedEvent(sloganResult), cancellationToken);
                return sloganResult;
            }
        }

        /// <summary>
        /// A custom event to indicate that feedback has been provided.
        /// </summary>
        public sealed class FeedbackEvent(FeedbackResult feedbackResult) : WorkflowEvent(feedbackResult)
        {
            private readonly JsonSerializerOptions _options = new() { WriteIndented = true };
            public override string ToString() => $"Feedback:\n{JsonSerializer.Serialize(feedbackResult, this._options)}";
        }

        /// <summary>
        /// A custom executor that uses an AI agent to provide feedback on a slogan.
        /// </summary>
        public sealed class FeedbackExecutor : Executor<SloganResult>
        {
            private readonly AIAgent _agent;
            private readonly AgentThread _thread;

            public int MinimumRating { get; init; } = 8;

            public int MaxAttempts { get; init; } = 3;

            private int _attempts;

            /// <summary>
            /// Initializes a new instance of the <see cref="FeedbackExecutor"/> class.
            /// </summary>
            /// <param name="id">A unique identifier for the executor.</param>
            /// <param name="chatClient">The chat client to use for the AI agent.</param>
            public FeedbackExecutor(string id, IChatClient chatClient) : base(id)
            {
                ChatClientAgentOptions agentOptions = new(instructions: "You are a professional editor. You will be given a slogan and the task it is meant to accomplish.")
                {
                    ChatOptions = new()
                    {
                        ResponseFormat = ChatResponseFormat.ForJsonSchema<FeedbackResult>()
                    }
                };

                this._agent = new ChatClientAgent(chatClient, agentOptions);
                this._thread = this._agent.GetNewThread();
            }

            /// <summary>
            /// 接受 SloganResult Message
            /// </summary>
            /// <param name="message"></param>
            /// <param name="context"></param>
            /// <param name="cancellationToken"></param>
            /// <returns></returns>
            /// <exception cref="InvalidOperationException"></exception>
            public override async ValueTask HandleAsync(SloganResult message, IWorkflowContext context, CancellationToken cancellationToken = default)
            {
                var sloganMessage = $"""
            Here is a slogan for the task '{message.Task}':
            Slogan: {message.Slogan}
            Please provide feedback on this slogan, including comments, a rating from 1 to 10, and suggested actions for improvement.
            """;

                var response = await this._agent.RunAsync(sloganMessage, this._thread, cancellationToken: cancellationToken);
                var feedback = JsonSerializer.Deserialize<FeedbackResult>(response.Text) ?? throw new InvalidOperationException("Failed to deserialize feedback.");

                await context.AddEventAsync(new FeedbackEvent(feedback), cancellationToken);

                if (feedback.Rating >= this.MinimumRating)
                {
                    await context.YieldOutputAsync($"The following slogan was accepted:\n\n{message.Slogan}", cancellationToken);
                    return;
                }

                if (this._attempts >= this.MaxAttempts)
                {
                    await context.YieldOutputAsync($"The slogan was rejected after {this.MaxAttempts} attempts. Final slogan:\n\n{message.Slogan}", cancellationToken);
                    return;
                }

                await context.SendMessageAsync(feedback, cancellationToken: cancellationToken);
                this._attempts++;
            }
        }
    }
}
