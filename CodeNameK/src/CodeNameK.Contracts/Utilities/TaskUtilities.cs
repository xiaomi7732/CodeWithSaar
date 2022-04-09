using System;
using System.Threading.Tasks;

namespace CodeNameK.Core.Utilities
{
    public static class TaskUtilities
    {
        /// <summary>
        /// Fire and forget a task. Handles exception when handler is specified.
        /// </summary>
        /// <param name="task">The target task.</param>
        /// <param name="exceptionHandler">The exception handler.</param>
        /// <param name="onSuccess">Success callback.</param>
        /// <param name="continueOnCapturedContext"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static async void FireWithExceptionHandler(
            this Task task,
            Action<Exception>? exceptionHandler = null,
            Action? onSuccess = null)
        {
            if (task is null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            try
            {
                await task;
                onSuccess?.Invoke();
            }
            catch (Exception ex)
            {
                exceptionHandler?.Invoke(ex);
            }
        }
    }
}
