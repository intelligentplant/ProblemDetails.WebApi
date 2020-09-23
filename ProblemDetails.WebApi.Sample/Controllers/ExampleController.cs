// Copyright (c) Intelligent Plant Ltd. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

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

    }


    public class Model {

        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

    }
}
