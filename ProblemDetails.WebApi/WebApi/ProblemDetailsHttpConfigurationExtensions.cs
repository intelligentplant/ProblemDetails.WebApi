// Copyright (c) Intelligent Plant Ltd. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using IntelligentPlant.ProblemDetails.WebApi;

namespace System.Web.Http {

    /// <summary>
    /// Extensions for <see cref="HttpConfiguration"/> objects.
    /// </summary>
    public static class ProblemDetailsHttpConfigurationExtensions {

        /// <summary>
        /// Add support for creating RFC 7807 problem details responses when a controller action 
        /// returns a non-good status code.
        /// </summary>
        /// <param name="httpConfiguration">
        ///   The <see cref="HttpConfiguration"/>.
        /// </param>
        /// <param name="handleExceptions">
        /// 
        /// <para>
        ///   Specifies if the Web API pipeline should handle exceptions. When <see langword="true"/>, 
        ///   a <see cref="ProblemDetailsErrorFilterAttribute"/> filter will be added to the global 
        ///   Web API configuration. When <see langword="false"/>, the default <see cref="ExceptionHandling.IExceptionHandler"/> 
        ///   service for the Web API pipeline will be replaced with a <see cref="RethrowErrorExceptionHandler"/>, 
        ///   which will cause all unhandled Web API exceptions to propagate to the web host's pipeline.
        /// </para>
        /// 
        /// <para>
        ///   You should specify <see langword="false"/> here if you intend to generate problem 
        ///   details responses for errors in the OWIN pipeline (via e.g. the 
        ///   <see cref="IntelligentPlant.ProblemDetails.Owin.ProblemDetailsMiddlewareOptions.ExceptionHandler"/> 
        ///   property) instead of in the Web API pipeline.
        /// </para>
        /// 
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="httpConfiguration"/> is <see langword="null"/>.
        /// </exception>
        public static void AddProblemDetails(this HttpConfiguration httpConfiguration, bool handleExceptions = true) {
            if (httpConfiguration == null) {
                throw new ArgumentNullException(nameof(httpConfiguration));
            }

            if (handleExceptions) {
                httpConfiguration.Filters.Add(new ProblemDetailsErrorFilterAttribute());
            }
            else {
                httpConfiguration.Services.Replace(typeof(ExceptionHandling.IExceptionHandler), new RethrowErrorExceptionHandler());
            }
            httpConfiguration.Filters.Add(new ProblemDetailsActionFilterAttribute());
        }

    }
}
