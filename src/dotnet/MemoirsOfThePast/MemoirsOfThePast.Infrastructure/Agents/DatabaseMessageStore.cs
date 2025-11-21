using MemoirsOfThePast.Infrastructure.Domain;
using MemoirsOfThePast.Infrastructure.EntityFrameworkCore;
using MemoirsOfThePast.Infrastructure.JwtAuthentication;
using Microsoft.Agents.AI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace MemoirsOfThePast.Infrastructure.Agents
{
    /// <summary>
    /// postgresql 向量存储消息
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="serializedStoreState"></param>
    /// <param name="jsonSerializerOptions"></param>
    public class DatabaseMessageStore(string agentId,IServiceProvider serviceProvider) : ChatMessageStore
    {
        private string TheadId = Guid.NewGuid().ToString("N");

        public override async Task AddMessagesAsync(IEnumerable<ChatMessage> messages, CancellationToken cancellationToken = default)
        {
            List<ChatVectorMessageEntity> data = new(messages.Count());

            ChatVectorMessageEntity entity = null;

            await using var scope = serviceProvider.CreateAsyncScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<IDbContext>();
            var userContext = scope.ServiceProvider.GetRequiredService<IUserContext>();

            foreach (var item  in messages)
            {
                entity = new ChatVectorMessageEntity
                {
                    UserId = userContext.UserId,
                    AgentId = agentId,
                    Key = item.MessageId,
                    MessageText = item.Text,
                    Timestamp = item.CreatedAt,
                    ThreadId = TheadId,
                    SerializedMessage = JsonSerializer.Serialize(item),
                };
            }

            await dbContext.ChatVectorMessages.AddRangeAsync(data, cancellationToken);
            await dbContext.SaveChangesAsync();
        }

        public async override Task<IEnumerable<ChatMessage>> GetMessagesAsync(CancellationToken cancellationToken = default)
        {
            await using var scope = serviceProvider.CreateAsyncScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<IDbContext>();
            var userContext = scope.ServiceProvider.GetRequiredService<IUserContext>();

            var messages = await dbContext.ChatVectorMessages.OrderByDescending(x => x.Timestamp).Take(10).ToListAsync();

            var chatMessages = new List<ChatMessage>(messages.Count);

            foreach (var message in messages)
            {
                chatMessages.Add(JsonSerializer.Deserialize<ChatMessage>(message.SerializedMessage));
            }

            return chatMessages;
        }

        public override JsonElement Serialize(JsonSerializerOptions jsonSerializerOptions = null)
        {
            throw new NotImplementedException();
        }
    }
}
