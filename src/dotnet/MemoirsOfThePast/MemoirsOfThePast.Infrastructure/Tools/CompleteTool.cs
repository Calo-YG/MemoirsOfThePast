using System.ComponentModel;


public class CompleteTool(Func<(string name, string result), Task> completeTaskFunc)
{
    [Description(
         "Use this tool when you have completed the task to provide your final answer")]
    public async Task<string> CompleteTask(
        [Description("The name of the completed task")]
        string name,
        [Description(
            "The final result of the task")]
        string result)
    {
        await completeTaskFunc.Invoke((name, result));

        return "Task completed.";
    }
}
