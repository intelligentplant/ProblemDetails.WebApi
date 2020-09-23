using System;
using System.Collections.Generic;
using System.Linq;

using IntelligentPlant.ProblemDetails.Owin;

using Microsoft.Owin;

namespace Owin {

    /// <summary>
    /// <see cref="IAppBuilder"/> extensions for registering <see cref="ProblemDetailsMiddleware"/>.
    /// </summary>
    public static class ProblemDetailsMiddlewareExtensions {

        /// <summary>
        /// Adds a <see cref="ProblemDetailsMiddleware"/> to the request pipeline.
        /// </summary>
        /// <param name="appBuilder">
        ///   The OWIN app builder.
        /// </param>
        /// <param name="includePath">
        ///   The path to include. Specify <c>/</c> to match all routes.
        /// </param>
        /// <param name="additionalIncludePaths">
        ///   Additional paths to include.
        /// </param>
        /// <returns>
        ///   The OWIN app builder.
        /// </returns>
        public static IAppBuilder UseProblemDetails(this IAppBuilder appBuilder, PathString includePath, params PathString[] additionalIncludePaths) {
            return appBuilder.UseProblemDetails(new ProblemDetailsMiddlewareOptions() { 
                IncludePaths = new [] { includePath }.Concat(additionalIncludePaths).ToArray()
            });
        }


        /// <summary>
        /// Adds a <see cref="ProblemDetailsMiddleware"/> to the request pipeline.
        /// </summary>
        /// <param name="appBuilder">
        ///   The OWIN app builder.
        /// </param>
        /// <param name="includePaths">
        ///   The paths to include. Specify <c>/</c> to match all routes.
        /// </param>
        /// <param name="excludePaths">
        ///   The paths to exclude.
        /// </param>
        /// <returns>
        ///   The OWIN app builder.
        /// </returns>
        public static IAppBuilder UseProblemDetails(this IAppBuilder appBuilder, IEnumerable<PathString>? includePaths, IEnumerable<PathString>? excludePaths) {
            return appBuilder.UseProblemDetails(new ProblemDetailsMiddlewareOptions() { 
                IncludePaths = includePaths,
                ExcludePaths = excludePaths
            });
        }


        /// <summary>
        /// Adds a <see cref="ProblemDetailsMiddleware"/> to the request pipeline.
        /// </summary>
        /// <param name="appBuilder">
        ///   The OWIN app builder.
        /// </param>
        /// <param name="options">
        ///   The middleware options.
        /// </param>
        /// <returns>
        ///   The OWIN app builder.
        /// </returns>
        public static IAppBuilder UseProblemDetails(this IAppBuilder appBuilder, ProblemDetailsMiddlewareOptions? options) {
            if (appBuilder == null) {
                throw new ArgumentNullException(nameof(appBuilder));
            }

            appBuilder.Use<ProblemDetailsMiddleware>(options);

            return appBuilder;
        }

    }
}
