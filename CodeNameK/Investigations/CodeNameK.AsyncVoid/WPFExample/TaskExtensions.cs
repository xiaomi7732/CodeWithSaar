using System;
using System.Threading.Tasks;

namespace WPFExample
{
    public static class TaskExtensions
    {
        /// <summary>
        /// Runs a Task with various callbacks. This allows calling to async tasks with in
        /// void methods like an event handler, and at the same time, providing a way to deal
        /// with the completion or exception.
        /// </summary>
        /// <param name="task">The task to run.</param>
        /// <param name="onException">Call back when exception is thrown.</param>
        /// <param name="onCompleted">Call back when the task completes.</param>
        public static async void RunTaskWithCallbacks(
            this Task task, Action<Exception> onException, Action onCompleted)
        {
            try
            {
                await task;
                onCompleted();
            }
            catch (Exception ex)
            {
                onException(ex);
            }
        }
    }
}