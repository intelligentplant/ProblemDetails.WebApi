// Copyright (c) Intelligent Plant Ltd. All rights reserved.
// Copyright (c) Other contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using System.Web.Http.Results;

namespace IntelligentPlant.ProblemDetails.WebApi {

    /// <summary>
    /// Extensions for <see cref="ApiController"/> and <see cref="HttpRequestMessage"/> to allow 
    /// <see cref="ProblemDetails"/> (and derived types) to be directly returned from API controller 
    /// actions.
    /// </summary>
    public static class ApiControllerExtensions {

        /// <summary>
        /// Creates a new <see cref="HttpResponseMessage"/> with a <see cref="HttpStatusCode.InternalServerError"/> 
        /// status code and a <see cref="ServerErrorProblemDetails"/> object in the response body.
        /// </summary>
        /// <param name="request">
        ///   The HTTP request.
        /// </param>
        /// <param name="error">
        ///   The exception.
        /// </param>
        /// <param name="includeErrorDetails">
        ///   <see langword="true"/> to include the error message and stack trace in the created 
        ///   <see cref="ServerErrorProblemDetails"/> object, or <see langword="false"/> to include 
        ///   only a general error message. 
        /// </param>
        /// <param name="factory">
        ///   The <see cref="ProblemDetailsFactory"/> to create the <see cref="ProblemDetails"/> 
        ///   with. Specify <see langword="null"/> to use <see cref="ProblemDetailsFactory.Default"/>.
        /// </param>
        /// <returns>
        ///   A new <see cref="HttpResponseMessage"/> containing a <see cref="ProblemDetails"/> 
        ///   object.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="request"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="error"/> is <see langword="null"/>.
        /// </exception>
        /// <remarks>
        ///   See <a href="https://tools.ietf.org/html/rfc7807#section-3.1">here</a> for information 
        ///   about the members of a <see cref="ProblemDetails"/> object.
        /// </remarks>
        public static HttpResponseMessage CreateServerErrorProblemDetailsResponse(
            this HttpRequestMessage request, 
            Exception error, 
            bool includeErrorDetails,
            ProblemDetailsFactory? factory = null
        ) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }
            if (error == null) {
                throw new ArgumentNullException(nameof(error));
            }

            if (factory == null) {
                factory = ProblemDetailsFactory.Default;
            }

            var problemDetails = factory.CreateServerErrorProblemDetails(
                request.GetOwinContext(),
                includeErrorDetails 
                    ? error 
                    : null
            );
            var response = request.CreateResponse(
                HttpStatusCode.InternalServerError, 
                problemDetails, 
                new JsonMediaTypeFormatter(),
                ClientErrorDataDefaults.MediaType
            );
            return response;
        }


        /// <summary>
        /// Creates a new <see cref="HttpResponseMessage"/> with a <see cref="HttpStatusCode.InternalServerError"/> 
        /// status code and a <see cref="ServerErrorProblemDetails"/> object in the response body.
        /// </summary>
        /// <param name="controller">
        ///   The API controller.
        /// </param>
        /// <param name="error">
        ///   The exception.
        /// </param>
        /// <param name="includeErrorDetail">
        ///   Specifies if the error message and stack trace should be included in the <see cref="ServerErrorProblemDetails"/>. 
        ///   Specify <see langword="null"/> to defer this choice to the <see cref="HttpRequestContext.IncludeErrorDetail"/>  
        ///   flag for the request.
        /// </param>
        /// <param name="factory">
        ///   The <see cref="ProblemDetailsFactory"/> to create the <see cref="ProblemDetails"/> 
        ///   with. Specify <see langword="null"/> to use <see cref="ProblemDetailsFactory.Default"/>.
        /// </param>
        /// <returns>
        ///   A new <see cref="ResponseMessageResult"/> containing a <see cref="ProblemDetails"/> 
        ///   object.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="controller"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="error"/> is <see langword="null"/>.
        /// </exception>
        /// <remarks>
        ///   See <a href="https://tools.ietf.org/html/rfc7807#section-3.1">here</a> for information 
        ///   about the members of a <see cref="ProblemDetails"/> object. 
        /// </remarks>
        public static IHttpActionResult CreateServerErrorProblemDetailsResponse(
            this ApiController controller, 
            Exception error,
            bool? includeErrorDetail = null,
            ProblemDetailsFactory? factory = null
        ) {
            if (controller == null) {
                throw new ArgumentNullException(nameof(controller));
            }
            if (error == null) {
                throw new ArgumentNullException(nameof(error));
            }

            return new ResponseMessageResult(CreateServerErrorProblemDetailsResponse(
                controller.Request, 
                error, 
                includeErrorDetail ?? controller.RequestContext.IncludeErrorDetail, 
                factory
            ));
        }


        /// <summary>
        /// Creates a new <see cref="HttpResponseMessage"/> with a <see cref="HttpStatusCode.BadRequest"/> 
        /// status code and a <see cref="ValidationProblemDetails"/> object in the response body.
        /// </summary>
        /// <param name="request">
        ///   The HTTP request.
        /// </param>
        /// <param name="modelStateDictionary">
        ///   The <see cref="ModelStateDictionary"/> containing the validation errors.
        /// </param>
        /// <param name="factory">
        ///   The <see cref="ProblemDetailsFactory"/> to create the <see cref="ProblemDetails"/> 
        ///   with. Specify <see langword="null"/> to use <see cref="ProblemDetailsFactory.Default"/>.
        /// </param>
        /// <returns>
        ///   A new <see cref="HttpResponseMessage"/> containing a <see cref="ValidationProblemDetails"/> 
        ///   object.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="request"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="modelStateDictionary"/> is <see langword="null"/>.
        /// </exception>
        /// <remarks>
        ///   See <a href="https://tools.ietf.org/html/rfc7807#section-3.1">here</a> for information 
        ///   about the members of a <see cref="ProblemDetails"/> object.
        /// </remarks>
        public static HttpResponseMessage CreateValidationProblemDetailsResponse(
            this HttpRequestMessage request, 
            ModelStateDictionary modelStateDictionary, 
            ProblemDetailsFactory? factory = null
        ) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }
            if (modelStateDictionary == null) {
                throw new ArgumentNullException(nameof(modelStateDictionary));
            }

            if (factory == null) {
                factory = ProblemDetailsFactory.Default;
            }

            var problemDetails = factory.CreateValidationProblemDetails(
                request.GetOwinContext(),
                modelStateDictionary,
                (int) HttpStatusCode.BadRequest,
                detail: Resources.ValidationProblemDescription_Details
            );
            var response = request.CreateResponse(
                HttpStatusCode.BadRequest, 
                problemDetails,
                new JsonMediaTypeFormatter(),
                ClientErrorDataDefaults.MediaType
            );
            return response;
        }


        /// <summary>
        /// Creates a new <see cref="ResponseMessageResult"/> with a <see cref="HttpStatusCode.BadRequest"/> 
        /// status code and a <see cref="ValidationProblemDetails"/> object in the response body.
        /// </summary>
        /// <param name="controller">
        ///   The API controller.
        /// </param>
        /// <param name="modelStateDictionary">
        ///   The <see cref="ModelStateDictionary"/> containing the validation errors.
        /// </param>
        /// <param name="factory">
        ///   The <see cref="ProblemDetailsFactory"/> to create the <see cref="ProblemDetails"/> 
        ///   with. Specify <see langword="null"/> to use <see cref="ProblemDetailsFactory.Default"/>.
        /// </param>
        /// <returns>
        ///   A new <see cref="ResponseMessageResult"/> containing a <see cref="ValidationProblemDetails"/> 
        ///   object.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="controller"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="modelStateDictionary"/> is <see langword="null"/>.
        /// </exception>
        /// <remarks>
        ///   See <a href="https://tools.ietf.org/html/rfc7807#section-3.1">here</a> for information 
        ///   about the members of a <see cref="ProblemDetails"/> object.
        /// </remarks>
        public static IHttpActionResult CreateValidationProblemDetailsResponse(
            this ApiController controller, 
            ModelStateDictionary modelStateDictionary, 
            ProblemDetailsFactory? factory = null
        ) {
            if (controller == null) {
                throw new ArgumentNullException(nameof(controller));
            }
            if (modelStateDictionary == null) {
                throw new ArgumentNullException(nameof(modelStateDictionary));
            }

            return new ResponseMessageResult(CreateValidationProblemDetailsResponse(controller.Request, modelStateDictionary, factory));
        }


        /// <summary>
        /// Creates a new <see cref="HttpResponseMessage"/> with a <see cref="HttpStatusCode.BadRequest"/> 
        /// status code and a <see cref="ValidationProblemDetails"/> object in the response body.
        /// </summary>
        /// <param name="request">
        ///   The HTTP request.
        /// </param>
        /// <param name="error">
        ///   The <see cref="ValidationException"/> containing the validation errors.
        /// </param>
        /// <param name="factory">
        ///   The <see cref="ProblemDetailsFactory"/> to create the <see cref="ProblemDetails"/> 
        ///   with. Specify <see langword="null"/> to use <see cref="ProblemDetailsFactory.Default"/>.
        /// </param>
        /// <returns>
        ///   A new <see cref="HttpResponseMessage"/> containing a <see cref="ValidationProblemDetails"/> 
        ///   object.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="request"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="error"/> is <see langword="null"/>.
        /// </exception>
        /// <remarks>
        ///   See <a href="https://tools.ietf.org/html/rfc7807#section-3.1">here</a> for information 
        ///   about the members of a <see cref="ProblemDetails"/> object.
        /// </remarks>
        public static HttpResponseMessage CreateValidationProblemDetailsResponse(
            this HttpRequestMessage request,
            ValidationException error,
            ProblemDetailsFactory? factory = null
        ) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }
            if (error == null) {
                throw new ArgumentNullException(nameof(error));
            }

            if (factory == null) {
                factory = ProblemDetailsFactory.Default;
            }

            var problemDetails = factory.CreateValidationProblemDetails(
                request.GetOwinContext(),
                error,
                (int) HttpStatusCode.BadRequest,
                detail: Resources.ValidationProblemDescription_Details
            );
            var response = request.CreateResponse(
                HttpStatusCode.BadRequest,
                problemDetails,
                new JsonMediaTypeFormatter(),
                ClientErrorDataDefaults.MediaType
            );
            return response;
        }


        /// <summary>
        /// Creates a new <see cref="ResponseMessageResult"/> with a <see cref="HttpStatusCode.BadRequest"/> 
        /// status code and a <see cref="ValidationProblemDetails"/> object in the response body.
        /// </summary>
        /// <param name="controller">
        ///   The API controller.
        /// </param>
        /// <param name="error">
        ///   The <see cref="ValidationException"/> containing the validation errors.
        /// </param>
        /// <param name="factory">
        ///   The <see cref="ProblemDetailsFactory"/> to create the <see cref="ProblemDetails"/> 
        ///   with. Specify <see langword="null"/> to use <see cref="ProblemDetailsFactory.Default"/>.
        /// </param>
        /// <returns>
        ///   A new <see cref="ResponseMessageResult"/> containing a <see cref="ValidationProblemDetails"/> 
        ///   object.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="controller"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="error"/> is <see langword="null"/>.
        /// </exception>
        /// <remarks>
        ///   See <a href="https://tools.ietf.org/html/rfc7807#section-3.1">here</a> for information 
        ///   about the members of a <see cref="ProblemDetails"/> object.
        /// </remarks>
        public static IHttpActionResult CreateValidationProblemDetailsResponse(
            this ApiController controller,
            ValidationException error,
            ProblemDetailsFactory? factory = null
        ) {
            if (controller == null) {
                throw new ArgumentNullException(nameof(controller));
            }
            if (error == null) {
                throw new ArgumentNullException(nameof(error));
            }

            return new ResponseMessageResult(CreateValidationProblemDetailsResponse(controller.Request, error, factory));
        }


        /// <summary>
        /// Creates a new <see cref="HttpResponseMessage"/> with a <see cref="HttpStatusCode.BadRequest"/> 
        /// status code and a <see cref="ValidationProblemDetails"/> object in the response body.
        /// </summary>
        /// <param name="request">
        ///   The HTTP request.
        /// </param>
        /// <param name="errors">
        ///   The validation errors.
        /// </param>
        /// <param name="factory">
        ///   The <see cref="ProblemDetailsFactory"/> to create the <see cref="ProblemDetails"/> 
        ///   with. Specify <see langword="null"/> to use <see cref="ProblemDetailsFactory.Default"/>.
        /// </param>
        /// <returns>
        ///   A new <see cref="HttpResponseMessage"/> containing a <see cref="ValidationProblemDetails"/> 
        ///   object.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="request"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="errors"/> is <see langword="null"/>.
        /// </exception>
        /// <remarks>
        ///   See <a href="https://tools.ietf.org/html/rfc7807#section-3.1">here</a> for information 
        ///   about the members of a <see cref="ProblemDetails"/> object.
        /// </remarks>
        public static HttpResponseMessage CreateValidationProblemDetailsResponse(
            this HttpRequestMessage request,
            IEnumerable<ValidationResult> errors,
            ProblemDetailsFactory? factory = null
        ) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }
            if (errors == null) {
                throw new ArgumentNullException(nameof(errors));
            }

            if (factory == null) {
                factory = ProblemDetailsFactory.Default;
            }

            var problemDetails = factory.CreateValidationProblemDetails(
                request.GetOwinContext(),
                errors,
                (int) HttpStatusCode.BadRequest,
                detail: Resources.ValidationProblemDescription_Details
            );
            var response = request.CreateResponse(
                HttpStatusCode.BadRequest,
                problemDetails,
                new JsonMediaTypeFormatter(),
                ClientErrorDataDefaults.MediaType
            );
            return response;
        }


        /// <summary>
        /// Creates a new <see cref="ResponseMessageResult"/> with a <see cref="HttpStatusCode.BadRequest"/> 
        /// status code and a <see cref="ValidationProblemDetails"/> object in the response body.
        /// </summary>
        /// <param name="controller">
        ///   The API controller.
        /// </param>
        /// <param name="errors">
        ///   The validation errors.
        /// </param>
        /// <param name="factory">
        ///   The <see cref="ProblemDetailsFactory"/> to create the <see cref="ProblemDetails"/> 
        ///   with. Specify <see langword="null"/> to use <see cref="ProblemDetailsFactory.Default"/>.
        /// </param>
        /// <returns>
        ///   A new <see cref="ResponseMessageResult"/> containing a <see cref="ValidationProblemDetails"/> 
        ///   object.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="controller"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="errors"/> is <see langword="null"/>.
        /// </exception>
        /// <remarks>
        ///   See <a href="https://tools.ietf.org/html/rfc7807#section-3.1">here</a> for information 
        ///   about the members of a <see cref="ProblemDetails"/> object.
        /// </remarks>
        public static IHttpActionResult CreateValidationProblemDetailsResponse(
            this ApiController controller,
            IEnumerable<ValidationResult> errors,
            ProblemDetailsFactory? factory = null
        ) {
            if (controller == null) {
                throw new ArgumentNullException(nameof(controller));
            }
            if (errors == null) {
                throw new ArgumentNullException(nameof(errors));
            }

            return new ResponseMessageResult(CreateValidationProblemDetailsResponse(controller.Request, errors, factory));
        }


        /// <summary>
        /// Creates a new <see cref="HttpResponseMessage"/> containing a <see cref="ProblemDetails"/> 
        /// object in the response body.
        /// </summary>
        /// <param name="request">
        ///   The HTTP request.
        /// </param>
        /// <param name="statusCode">
        ///   The status code for the response.
        /// </param>
        /// <param name="title">
        ///   The title for the problem. Specify <see langword="null"/> to infer a title from the 
        ///   status code.
        /// </param>
        /// <param name="problemTypeUri">
        ///   The problem type URI. Specify <see langword="null"/> to infer a URI from the status code.
        /// </param>
        /// <param name="detail">
        ///   A human-readable explanation of the problem.
        /// </param>
        /// <param name="factory">
        ///   The <see cref="ProblemDetailsFactory"/> to create the <see cref="ProblemDetails"/> 
        ///   with. Specify <see langword="null"/> to use <see cref="ProblemDetailsFactory.Default"/>.
        /// </param>
        /// <returns>
        ///   A new <see cref="HttpResponseMessage"/> containing a <see cref="ProblemDetails"/> 
        ///   object.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="request"/> is <see langword="null"/>.
        /// </exception>
        /// <remarks>
        ///   See <a href="https://tools.ietf.org/html/rfc7807#section-3.1">here</a> for information 
        ///   about the members of a <see cref="ProblemDetails"/> object.
        /// </remarks>
        public static HttpResponseMessage CreateProblemDetailsResponse(
            this HttpRequestMessage request, 
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError, 
            string? title = null, 
            string? problemTypeUri = null, 
            string? detail = null, 
            ProblemDetailsFactory? factory = null
        ) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }

            if (factory == null) {
                factory = ProblemDetailsFactory.Default;
            }

            var problemDetails = factory.CreateProblemDetails(
                request.GetOwinContext(),
                (int) statusCode,
                title,
                problemTypeUri,
                detail
            );
            var response = request.CreateResponse(
                statusCode,
                problemDetails,
                new JsonMediaTypeFormatter(),
                ClientErrorDataDefaults.MediaType
            );
            return response;
        }


        /// <summary>
        /// Creates a new <see cref="ResponseMessageResult"/> containing a <see cref="ProblemDetails"/> 
        /// object in the response body.
        /// </summary>
        /// <param name="controller">
        ///   The API controller.
        /// </param>
        /// <param name="statusCode">
        ///   The status code for the response.
        /// </param>
        /// <param name="title">
        ///   The title for the problem. Specify <see langword="null"/> to infer a title from the 
        ///   status code.
        /// </param>
        /// <param name="problemTypeUri">
        ///   The problem type URI. Specify <see langword="null"/> to infer a URI from the status code.
        /// </param>
        /// <param name="detail">
        ///   A human-readable explanation of the problem.
        /// </param>
        /// <param name="factory">
        ///   The <see cref="ProblemDetailsFactory"/> to create the <see cref="ProblemDetails"/> 
        ///   with. Specify <see langword="null"/> to use <see cref="ProblemDetailsFactory.Default"/>.
        /// </param>
        /// <returns>
        ///   A new <see cref="ResponseMessageResult"/> containing a <see cref="ProblemDetails"/> 
        ///   object.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="controller"/> is <see langword="null"/>.
        /// </exception>
        /// <remarks>
        ///   See <a href="https://tools.ietf.org/html/rfc7807#section-3.1">here</a> for information 
        ///   about the members of a <see cref="ProblemDetails"/> object.
        /// </remarks>
        public static IHttpActionResult CreateProblemDetailsResponse(
            this ApiController controller,
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError,
            string? title = null,
            string? problemTypeUri = null,
            string? detail = null,
            ProblemDetailsFactory? factory = null
        ) {
            if (controller == null) {
                throw new ArgumentNullException(nameof(controller));
            }

            return new ResponseMessageResult(CreateProblemDetailsResponse(
                controller.Request, 
                statusCode, 
                title, 
                problemTypeUri, 
                detail, 
                factory
            ));
        }

    }
}
