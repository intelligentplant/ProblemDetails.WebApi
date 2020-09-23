// Copyright (c) Intelligent Plant Ltd. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Web.Http;

using Microsoft.Owin;

using Owin;

namespace ProblemDetails.WebApi.Sample {
    public class Startup {
        public void Configuration(IAppBuilder app) {
            var config = new HttpConfiguration();
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
            config.AddProblemDetails();
            config.MapHttpAttributeRoutes();

            app.UseWebApi(config);

            app.UseProblemDetails(new PathString("/"));
        }
    }
}
