// Copyright (c) .NET Foundation. All rights reserved.
// Copyright (c) Intelligent Plant Ltd. All rights reserved.
// Copyright (c) Other contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Http.ModelBinding;

using Microsoft.Owin;

namespace IntelligentPlant.ProblemDetails {

    /// <summary>
    /// Default <see cref="ProblemDetailsFactory"/> implementation.
    /// </summary>
    public class DefaultProblemDetailsFactory : ProblemDetailsFactory {

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

            var problemDetails = new ValidationProblemDetails(modelStateDictionary);
            ConfigureValidationProblemDetails(httpContext, problemDetails, statusCode, title, type, detail, instance);

            return problemDetails;
        }


        /// <inheritdoc/>
        public override ValidationProblemDetails CreateValidationProblemDetails(
            IOwinContext httpContext,
            ValidationException error,
            int? statusCode = null,
            string? title = null,
            string? type = null,
            string? detail = null,
            string? instance = null
        ) {
            if (error == null) {
                throw new ArgumentNullException(nameof(error));
            }

            var problemDetails = new ValidationProblemDetails(error);
            ConfigureValidationProblemDetails(httpContext, problemDetails, statusCode, title, type, detail, instance);

            return problemDetails;
        }


        /// <inheritdoc/>
        public override ValidationProblemDetails CreateValidationProblemDetails(
            IOwinContext httpContext,
            IEnumerable<ValidationResult> errors,
            int? statusCode = null,
            string? title = null,
            string? type = null,
            string? detail = null,
            string? instance = null
        ) {
            if (errors == null) {
                throw new ArgumentNullException(nameof(errors));
            }

            var problemDetails = new ValidationProblemDetails(errors);
            ConfigureValidationProblemDetails(httpContext, problemDetails, statusCode, title, type, detail, instance);

            return problemDetails;
        }


        /// <summary>
        /// Configures a <see cref="ValidationProblemDetails"/> instance.
        /// </summary>
        /// <param name="httpContext">
        ///   The <see cref="IOwinContext"/>.
        /// </param>
        /// <param name="problemDetails">
        ///   The <see cref="ValidationProblemDetails"/>.
        /// </param>
        /// <param name="statusCode">
        ///   The value for <see cref="ProblemDetails.Status"/>.
        /// </param>
        /// <param name="title">
        ///   The value for <see cref="ProblemDetails.Title"/>.
        /// </param>
        /// <param name="type">
        ///   The value for <see cref="ProblemDetails.Type"/>.
        /// </param>
        /// <param name="detail">
        ///   The value for <see cref="ProblemDetails.Detail"/>.
        /// </param>
        /// <param name="instance">
        ///   The value for <see cref="ProblemDetails.Instance"/>.
        /// </param>
        private void ConfigureValidationProblemDetails(
            IOwinContext httpContext,
            ValidationProblemDetails problemDetails,
            int? statusCode = null,
            string? title = null,
            string? type = null,
            string? detail = null,
            string? instance = null
        ) {
            statusCode ??= 400;

            problemDetails.Status = statusCode;
            problemDetails.Type = type;
            problemDetails.Detail = detail;
            problemDetails.Instance = instance;

            if (title != null) {
                problemDetails.Title = title;
            }

            ApplyProblemDetailsDefaults(httpContext, problemDetails, statusCode.Value);
        }

    }
}
