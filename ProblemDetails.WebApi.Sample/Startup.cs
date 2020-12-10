// Copyright (c) Intelligent Plant Ltd. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using System.Web.Http;

using Microsoft.Owin;

using Owin;

namespace ProblemDetails.WebApi.Sample {
    public class Startup {
        public void Configuration(IAppBuilder app) {
            app.UseProblemDetails(new IntelligentPlant.ProblemDetails.Owin.ProblemDetailsMiddlewareOptions() { 
                IncludePaths = new[] { new PathString("/") },
                ExceptionHandler = (context, error, factory) => { 
                    if (error is HttpResponseException httpError) {
                        return factory.CreateProblemDetails(context, (int) httpError.Response.StatusCode, detail: httpError.Message);
                    }
                    if (error is System.Security.SecurityException) {
                        return factory.CreateProblemDetails(context, 403);
                    }
                    if (error is ArgumentException) {
                        return factory.CreateProblemDetails(context, 400, detail: error.Message);
                    }
                    if (error is OperationCanceledException) {
                        return factory.CreateProblemDetails(context, 0, detail: error.Message);
                    }

                    return factory.CreateServerErrorProblemDetails(context, error, detail: $"[OWIN PIPELINE] {error.Message}");
                }
            });

            var config = new HttpConfiguration();
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
            // Specify true below to handle exceptions in the Web API pipeline and false to handle 
            // exceptions in the OWIN pipeline.
            config.AddProblemDetails(handleExceptions: false);
            config.MapHttpAttributeRoutes();

            app.UseWebApi(config);

            // Additional OWIN routes for testing the OWIN pipeline error handling.
            app.Map("/owin", owinPipeline => {
                owinPipeline.Use((ctx, next) => {
                    // /owin/forbidden
                    if (ctx.Request.Path.Equals(new PathString("/forbidden"))) {
                        throw new System.Security.SecurityException();
                    }

                    // /owin/invalid-argument
                    if (ctx.Request.Path.Equals(new PathString("/invalid-argument"))) {
                        throw new ArgumentException();
                    }

                    // /owin/cancelled
                    if (ctx.Request.Path.Equals(new PathString("/cancelled"))) {
                        throw new OperationCanceledException();
                    }

                    ctx.Response.StatusCode = 404;
                    return Task.CompletedTask;
                });
            });
        }
    }
}
