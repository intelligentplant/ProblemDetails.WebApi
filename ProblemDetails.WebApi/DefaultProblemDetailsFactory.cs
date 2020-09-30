// Copyright (c) .NET Foundation. All rights reserved.
// Copyright (c) Intelligent Plant Ltd. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Web.Http.ModelBinding;

using Microsoft.Owin;

namespace IntelligentPlant.ProblemDetails {

    /// <summary>
    /// Default <see cref="ProblemDetailsFactory"/> implementation.
    /// </summary>
    internal sealed class DefaultProblemDetailsFactory : ProblemDetailsFactory {

        /// <inheritdoc/>
        public override ProblemDetails CreateProblemDetails(
            IOwinContext httpContext,
            int? statusCode = null,
            string? title = null,
            string? type = null,
            string? detail = null,
            string? instance = null
        ) {
            statusCode ??= 500;
            
            var problemDetails = new ProblemDetails {
                Status = statusCode,
                Title = title,
                Type = type,
                Detail = detail,
                Instance = instance,
            };

            ApplyProblemDetailsDefaults(httpContext, problemDetails, statusCode.Value);

            return problemDetails;
        }


        /// <inheritdoc/>
        public override ServerErrorProblemDetails CreateServerErrorProblemDetails(
            IOwinContext httpContext, 
            Exception? error = null, 
            int? statusCode = null, 
            string? title = null, 
            string? type = null, 
            string? detail = null, 
            string? instance = null
        ) {
            statusCode ??= 500;

            var problemDetails = new ServerErrorProblemDetails(error) {
                Status = statusCode,
                Type = type,
                Instance = instance,
            };

            if (detail != null) {
                problemDetails.Detail = detail;
            }

            if (title != null) {
                problemDetails.Title = title;
            }

            ApplyProblemDetailsDefaults(httpContext, problemDetails, statusCode.Value);

            return problemDetails;
        }


        /// <inheritdoc/>
        public override ValidationProblemDetails CreateValidationProblemDetails(
            IOwinContext httpContext,
            ModelStateDictionary modelStateDictionary,
            int? statusCode = null,
            string? title = null,
            string? type = null,
            string? detail = null,
            string? instance = null
        ) {
            if (modelStateDictionary == null) {
                throw new ArgumentNullException(nameof(modelStateDictionary));
            }

            statusCode ??= 400;

            var problemDetails = new ValidationProblemDetails(modelStateDictionary) {
                Status = statusCode,
                Type = type,
                Detail = detail,
                Instance = instance,
            };

            if (title != null) {
                problemDetails.Title = title;
            }

            ApplyProblemDetailsDefaults(httpContext, problemDetails, statusCode.Value);

            return problemDetails;
        }


        /// <summary>
        /// Applies default settings to a <see cref="ProblemDetails"/> instance.
        /// </summary>
        /// <param name="httpContext">
        ///   The HTTP context.
        /// </param>
        /// <param name="problemDetails">
        ///   The <see cref="ProblemDetails"/> instance.
        /// </param>
        /// <param name="statusCode">
        ///   The status code to use when looking up default values for the error type.
        /// </param>
        private void ApplyProblemDetailsDefaults(IOwinContext httpContext, ProblemDetails problemDetails, int statusCode) {
            problemDetails.Status ??= statusCode;
            problemDetails.Instance ??= string.Concat(httpContext.Request.PathBase, httpContext.Request.Path);

            var clientErrorMapping = new Dictionary<int, ClientErrorData>();
            ClientErrorDataDefaults.ApplyDefaults(clientErrorMapping);

            if (clientErrorMapping.TryGetValue(statusCode, out var clientErrorData)) {
                problemDetails.Title ??= clientErrorData.Title;
                problemDetails.Type ??= clientErrorData.Link;
            }
        }
    }
}
