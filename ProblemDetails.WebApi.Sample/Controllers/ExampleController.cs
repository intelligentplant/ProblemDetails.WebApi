// Copyright (c) Intelligent Plant Ltd. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Web.Http;

using IntelligentPlant.ProblemDetails.WebApi;

namespace ProblemDetails.WebApi.Sample.Controllers {

    [RoutePrefix("example")]
    public class ExampleController : ApiController {

        [HttpGet]
        [Route("")]
        [Route("requires-auth")]
        public IHttpActionResult Unauthorized() {
            return StatusCode(HttpStatusCode.Unauthorized); // 401
        }


        [HttpGet]
        [Route("not-found")]
        public new IHttpActionResult NotFound() {
            return base.NotFound(); // 404
        }


        [HttpGet]
        [Route("bad-request")]
        public new IHttpActionResult BadRequest() {
            return base.BadRequest("You don't have enough credits!"); // 400
        }


        [HttpGet]
        [Route("error")]
        public IHttpActionResult Error() {
            throw new Exception("An error occurred!");
        }


        [HttpGet]
        [Route("http-error")]
        public IHttpActionResult HttpError() {
            throw new HttpResponseException(HttpStatusCode.Forbidden);
        }


        [HttpGet]
        [Route("model-validation")]
        public IHttpActionResult ModelValidation([FromUri] Model model) {
            return Ok();
        }


        [HttpGet]
        [Route("create-direct")]
        public IHttpActionResult CreateDirect() {
            return this.CreateProblemDetailsResponse(HttpStatusCode.Forbidden, detail: "Problem details was directly created");
        }


        [HttpGet]
        [Route("create-from-error")]
        public IHttpActionResult CreateFromError(bool? detail = null) {
            try {
                throw new Exception("An error occurred!");
            }
            catch (Exception e) {
                return this.CreateServerErrorProblemDetailsResponse(e, detail);
            }
        }


        [HttpGet]
        [Route("create-from-validation-error")]
        public IHttpActionResult CreateFromValidationError() {
            var model = new Model();
            try {
                Validator.ValidateObject(model, new ValidationContext(model), true);
            }
            catch (ValidationException e) {
                return this.CreateValidationProblemDetailsResponse(e);
            }

            return Ok();
        }


        [HttpGet]
        [Route("create-from-validation-result")]
        public IHttpActionResult CreateFromValidationResult() {
            var model = new Model();
            var errors = new List<ValidationResult>();
            if (!Validator.TryValidateObject(model, new ValidationContext(model), errors, true)) {
                return this.CreateValidationProblemDetailsResponse(errors);
            }

            return Ok();
        }

    }


    public class Model {

        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

    }
}
