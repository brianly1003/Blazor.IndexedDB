﻿using System.Reflection;

namespace Blazor.IndexedDB.Extensions
{
    internal static class TaskExtensions
    {
        internal static async Task<object> InvokeAsyncWithResult(this MethodInfo @this, object obj, params object[] parameters)
        {
            var task = (Task)@this.Invoke(obj, parameters);
            await task.ConfigureAwait(false);
            var resultProperty = task.GetType().GetProperty("Result");
            return resultProperty.GetValue(task);
        }

        internal static async Task InvokeAsync(this MethodInfo @this, object obj, params object[] parameters)
        {
            var task = (Task)@this.Invoke(obj, parameters);

            await task.ConfigureAwait(false);
        }
    }
}
