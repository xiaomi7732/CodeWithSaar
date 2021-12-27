﻿using System;
using System.Threading.Tasks;

namespace CodeNameK.Core.Utilities
{
    public static class TaskUtilities
    {
        /// <summary>
        /// Fire & forget a task. Handles exception when handler is specified.
        /// </summary>
        /// <param name="task">The target task.</param>
        /// <param name="exceptionHandler">The exception handler.</param>
        /// <param name="continueOnCapturedContext"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static async void FireWithExceptionHandler(
            this Task task,
            Action<Exception>? exceptionHandler = null)
        {
            if (task is null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            try
            {
                await task;
            }
            catch (Exception ex)
            {
                exceptionHandler?.Invoke(ex);
            }
        }
    }
}