// Copyright (c) Intelligent Plant Ltd. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;

namespace IntelligentPlant.ProblemDetails.WebApi {

    /// <summary>
    /// <see cref="IExceptionHandler"/> class that intentionally leaves exceptions unhandled so 
    /// that they can be handled by the host application's request pipeline instead.
    /// </summary>
    public class RethrowErrorExceptionHandler : IExceptionHandler {

        /// <summary>
        /// Completed task.
        /// </summary>
        private static readonly Task s_completedTask =
#if NET45
            Task.FromResult(0);
#else
            Task.CompletedTask;
#endif

        /// <inheritdoc/>
        public Task HandleAsync(ExceptionHandlerContext context, CancellationToken cancellationToken) {
            context.Result = null;
            return s_completedTask;
        }
    }
}
