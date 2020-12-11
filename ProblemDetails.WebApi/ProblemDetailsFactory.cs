﻿// Copyright (c) .NET Foundation. All rights reserved.
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
    /// Factory to produce <see cref="ProblemDetails"/>, <see cref="ServerErrorProblemDetails"/>, 
    /// and <see cref="ValidationProblemDetails"/>.
    /// </summary>
    public abstract class ProblemDetailsFactory {

        /// <summary>
        /// Default <see cref="ProblemDetailsFactory"/> instance.
        /// </summary>
        public static ProblemDetailsFactory Default { get; } = new DefaultProblemDetailsFactory();

        /// <summary>
        /// A callback that can be used to transform a created <see cref="ProblemDetails"/> 
        /// instance after it has been created by the factory.
        /// </summary>
        public virtual Action<IOwinContext, ProblemDetails>? OnDetailsCreated { get; set; }


        /// <summary>
        /// Creates a <see cref="ProblemDetails"/> instance that configures defaults based on 
        /// values specified in <see cref="ClientErrorDataDefaults" />.
        /// </summary>
        /// <param name="httpContext">
        ///   The <see cref="IOwinContext"/>.
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
        /// <returns>
        ///   The <see cref="ProblemDetails"/> instance.
        /// </returns>
        public abstract ProblemDetails CreateProblemDetails(
            IOwinContext httpContext,
            int? statusCode = null,
            string? title = null,
            string? type = null,
            string? detail = null,
            string? instance = null
        );


        /// <summary>
        /// Creates a <see cref="ServerErrorProblemDetails"/> instance that configures defaults 
        /// based on values specified in <see cref="ClientErrorDataDefaults"/>.
        /// </summary>
        /// <param name="httpContext">
        ///   The <see cref="IOwinContext"/>.
        /// </param>
        /// <param name="error">
        ///   The exception that occurred. If specified, the <see cref="ProblemDetails.Detail"/> 
        ///   will be configured using the <see cref="Exception.Message"/>.
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
        ///   The value for <see cref="ProblemDetails.Detail"/>. This parameter is ignored if 
        ///   <paramref name="error"/> is specified.
        /// </param>
        /// <param name="instance">
        ///   The value for <see cref="ProblemDetails.Instance"/>.
        /// </param>
        /// <returns>
        ///   The <see cref="ProblemDetails"/> instance.
        /// </returns>
        public abstract ServerErrorProblemDetails CreateServerErrorProblemDetails(
            IOwinContext httpContext,
            Exception? error = null,
            int? statusCode = null,
            string? title = null,
            string? type = null,
            string? detail = null,
            string? instance = null
        );


        /// <summary>
        /// Creates a <see cref="ValidationProblemDetails"/> instance that configures defaults 
        /// based on values specified in <see cref="ClientErrorDataDefaults"/>.
        /// </summary>
        /// <param name="httpContext">
        ///   The <see cref="IOwinContext"/>.
        /// </param>
        /// <param name="modelStateDictionary">
        ///   The <see cref="ModelStateDictionary"/>.
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
        /// <returns>
        ///   The <see cref="ValidationProblemDetails"/> instance.
        /// </returns>
        public abstract ValidationProblemDetails CreateValidationProblemDetails(
            IOwinContext httpContext,
            ModelStateDictionary modelStateDictionary,
            int? statusCode = null,
            string? title = null,
            string? type = null,
            string? detail = null,
            string? instance = null
        );


        /// <summary>
        /// Creates a <see cref="ValidationProblemDetails"/> instance that configures defaults 
        /// based on values specified in <see cref="ClientErrorDataDefaults"/>.
        /// </summary>
        /// <param name="httpContext">
        ///   The <see cref="IOwinContext"/>.
        /// </param>
        /// <param name="error">
        ///   The <see cref="ValidationException"/>.
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
        /// <returns>
        ///   The <see cref="ValidationProblemDetails"/> instance.
        /// </returns>
        public abstract ValidationProblemDetails CreateValidationProblemDetails(
            IOwinContext httpContext,
            ValidationException error,
            int? statusCode = null,
            string? title = null,
            string? type = null,
            string? detail = null,
            string? instance = null
        );


        /// <summary>
        /// Creates a <see cref="ValidationProblemDetails"/> instance that configures defaults 
        /// based on values specified in <see cref="ClientErrorDataDefaults"/>.
        /// </summary>
        /// <param name="httpContext">
        ///   The <see cref="IOwinContext"/>.
        /// </param>
        /// <param name="errors">
        ///   The validation errors.
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
        /// <returns>
        ///   The <see cref="ValidationProblemDetails"/> instance.
        /// </returns>
        public abstract ValidationProblemDetails CreateValidationProblemDetails(
            IOwinContext httpContext,
            IEnumerable<ValidationResult> errors,
            int? statusCode = null,
            string? title = null,
            string? type = null,
            string? detail = null,
            string? instance = null
        );


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
        protected void ApplyProblemDetailsDefaults(IOwinContext httpContext, ProblemDetails problemDetails, int statusCode) {
            problemDetails.Status ??= statusCode;
            problemDetails.Instance ??= string.Concat(httpContext.Request.PathBase, httpContext.Request.Path);

            var clientErrorMapping = new Dictionary<int, ClientErrorData>();
            ClientErrorDataDefaults.ApplyDefaults(clientErrorMapping);

            if (clientErrorMapping.TryGetValue(statusCode, out var clientErrorData)) {
                problemDetails.Title ??= clientErrorData.Title;
                problemDetails.Type ??= clientErrorData.Link;
            }

            OnDetailsCreated?.Invoke(httpContext, problemDetails);
        }

    }
}
