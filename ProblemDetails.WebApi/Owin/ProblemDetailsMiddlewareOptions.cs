using System;
using System.Collections.Generic;

using Microsoft.Owin;

namespace IntelligentPlant.ProblemDetails.Owin {

    /// <summary>
    /// Options for <see cref="ProblemDetailsMiddleware"/>.
    /// </summary>
    public class ProblemDetailsMiddlewareOptions {

        /// <summary>
        /// The paths that are allowed to return a problem details response. Specify <c>/</c> to 
        /// match all routes.
        /// </summary>
        public IEnumerable<PathString>? IncludePaths { get; set; }

        /// <summary>
        /// The paths that are not allowed to return a problem details response. Any path that 
        /// matches an <see cref="IncludePaths"/> entry and be excluded via an entry here.
        /// </summary>
        public IEnumerable<PathString>? ExcludePaths { get; set; }

        /// <summary>
        /// A delegate that can be used to generate a custom <see cref="ProblemDetails"/> object 
        /// for an unhandled exception in the OWIN pipeline. Return <see langword="null"/> to 
        /// rethrow the unhandled exception.
        /// </summary>
        public Func<IOwinContext, Exception, ProblemDetailsFactory, ProblemDetails?>? ExceptionHandler { get; set; }

    }
}
