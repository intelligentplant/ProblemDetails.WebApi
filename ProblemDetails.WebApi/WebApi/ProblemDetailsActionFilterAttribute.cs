// Copyright (c) Intelligent Plant Ltd. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace IntelligentPlant.ProblemDetails.WebApi {

    /// <summary>
    /// Web API action filter that returns <see cref="ProblemDetails"/> responses for actions that 
    /// return non-good status codes.
    /// </summary>
    public class ProblemDetailsActionFilterAttribute : ActionFilterAttribute {

        /// <summary>
        /// The <see cref="ProblemDetailsFactory"/> to use.
        /// </summary>
        private readonly ProblemDetailsFactory _factory = ProblemDetailsFactory.Default;


        /// <inheritdoc/>
        public override void OnActionExecuting(HttpActionContext actionContext) {
            // Prior to executing any action, ensure that the model state is valid and return a 
            // 400 response containing a ProblemDetails object if it is not.
            if (!actionContext.ModelState.IsValid) {
                actionContext.Response = actionContext.Request.CreateResponse(
                    HttpStatusCode.BadRequest,
                    _factory.CreateValidationProblemDetails(
                        actionContext.Request.GetOwinContext(),
                        actionContext.ModelState,
                        detail: Resources.ValidationProblemDescription_Details
                    ),
                    new JsonMediaTypeFormatter(),
                    ClientErrorDataDefaults.MediaType
                );
            }
        }


        /// <inheritdoc/>
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext) {
            // After an action has executed, determine if we need to modify the response to 
            // contain a ProblemDetails object.

            if (actionExecutedContext.Response == null || actionExecutedContext.Response.IsSuccessStatusCode) {
                // Response has good status or we can't infer what the status is.
                return;
            }

            if (actionExecutedContext.Response?.Content?.Headers?.ContentType?.MediaType?.Equals(ClientErrorDataDefaults.MediaType) ?? false) {
                // The response already contains a ProblemDetails object.
                return;
            }

            string? detail = null;

            if (actionExecutedContext.Response?.Content is ObjectContent oc) {
                if (oc.Value is ProblemDetails) {
                    // This is already a ProblemDetails response.
                    return;
                }

                if (oc.Value is HttpError httpError && httpError.TryGetValue(HttpErrorKeys.MessageKey, out var message)) {
                    // The response was created using e.g. the BadRequest method on an ApiController.
                    detail = message?.ToString();
                }
            }

            actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(
                actionExecutedContext.Response!.StatusCode,
                _factory.CreateProblemDetails(
                    actionExecutedContext.Request.GetOwinContext(), 
                    (int) actionExecutedContext.Response.StatusCode,
                    detail: detail
                ),
                new JsonMediaTypeFormatter(),
                ClientErrorDataDefaults.MediaType
            );
        }

    }
}
