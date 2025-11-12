using System.ComponentModel;

namespace MemoirsOfThePast.Infrastructure.Tools
{
    public class TaskTool
    {
        [Description("Execute a task step and return the current agent name.")]
        public static void StepTask(
            [Description("The name of the person or agent performing the task.")] string name,
            [Description("The description of the step being executed.")] string step)
        {
            var str = $"Currently, {name} is executing step: {step}";

            Console.WriteLine(str);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="taskName"></param>
        /// <returns></returns>
        [Description("Complete a specific task and return a message.")]
        public static void CompleteTask(
            [Description("The name of the task to complete.")] string taskName)
        {

            var str = $"Task '{taskName}' has been completed successfully.";

            Console.WriteLine(str);
        }
    }
}
