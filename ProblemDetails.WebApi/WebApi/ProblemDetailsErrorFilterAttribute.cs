// Copyright (c) Intelligent Plant Ltd. All rights reserved.
// Copyright (c) Other contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace IntelligentPlant.ProblemDetails.WebApi {

    /// <summary>
    /// Web API exception filter that returns <see cref="ProblemDetails"/> responses for actions 
    /// that throw unhandled exceptions.
    /// </summary>
    /// <remarks>
    ///   If an unhandled exception occurs, the error message and stack trace will be included in 
    ///   the returned <see cref="ProblemDetails"/> object if the <see cref="HttpRequestContext.IncludeErrorDetail"/> 
    ///   property for the request is <see langword="true"/>.
    /// </remarks>
    public class ProblemDetailsErrorFilterAttribute : ExceptionFilterAttribute {

        /// <summary>
        /// The <see cref="ProblemDetailsFactory"/> to use.
        /// </summary>
        private readonly ProblemDetailsFactory _factory;


        /// <summary>
        /// Creates a new <see cref="ProblemDetailsErrorFilterAttribute"/> instance.
        /// </summary>
        /// <param name="factory">
        ///   The <see cref="ProblemDetailsFactory"/> to use. Specify <see langword="null"/> to 
        ///   use <see cref="ProblemDetailsFactory.Default"/>.
        /// </param>
        public ProblemDetailsErrorFilterAttribute(ProblemDetailsFactory? factory = null) {
            _factory = factory ?? ProblemDetailsFactory.Default;
        }


        /// <inheritdoc/>
        public override void OnException(HttpActionExecutedContext actionExecutedContext) {
            // Make a special case for HttpResponseException, as this can be thrown by 
            // controllers for any non-good status code.
            if (actionExecutedContext.Exception is HttpResponseException responseException) {
                actionExecutedContext.Response = actionExecutedContext.Request.CreateProblemDetailsResponse(
                    responseException.Response.StatusCode,
                    factory: _factory
                );
                return;
            }

            // Otherwise, we are returning a 500 error. We will include the exception detail in the 
            // ProblemDetails instance only if we are allowed to according to the request context.
            actionExecutedContext.Response = actionExecutedContext.Request.CreateServerErrorProblemDetailsResponse(
                actionExecutedContext.Exception,
                actionExecutedContext.ActionContext.RequestContext.IncludeErrorDetail,
                factory: _factory
            );
        }

    }
}
