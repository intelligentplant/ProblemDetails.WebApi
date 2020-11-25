# ProblemDetails.WebApi

Adds [RFC 7807](https://tools.ietf.org/html/rfc7807) support to ASP.NET 4.x Web API applications. Note that this library supports ASP.NET 4.x; **ASP.NET Core is not supported**.


## Installation

Add a reference to the [IntelligentPlant.ProblemDetails.WebApi](https://www.nuget.org/packages/IntelligentPlant.ProblemDetails.WebApi) NuGet package.


## Configuration

To add support for all Web API controller actions via global action and exception filters on a `System.Web.Http.HttpConfiguration` object, use the `AddProblemDetails` extension method:

```csharp
public void Configuration(IAppBuilder app) {
    var config = new HttpConfiguration();
    // Enable detailed exceptions to include error messages and stack traces in 500 responses.
    //config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
    config.AddProblemDetails();
    config.MapHttpAttributeRoutes();

    app.UseWebApi(config);
}
```

When using an OWIN application to host Web API, the OWIN application will return a 401 response in the authorization stage of the pipeline if access to a route is denied, and a 404 response if it cannot identify a matching route for a request, meaning that the Web API filters cannot return problem details for these requests. You can return problem details for 401 and 404 responses by adding a middleware directly to the OWIN pipeline using the `UseProblemDetails` extension method:

```csharp
public void Configuration(IAppBuilder app) {
    // We only want to return a problem details response on API routes.
    app.UseProblemDetails(new PathString("/api"));
    
    var config = new HttpConfiguration();
    // Enable detailed exceptions to include error messages and stack traces in 500 responses.
    //config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
    config.AddProblemDetails();
    config.MapHttpAttributeRoutes();

    app.UseWebApi(config);
}
```


## Manually Creating Problem Details Responses

The `IntelligentPlant.ProblemDetails.WebApi` namespace contains extension methods for the `ApiController` type to allow direct creation of responses containing problem details objects in API controller methods, allowing you to return a problem details response that is appropriate for the exception that occurred. For example:

```csharp
[HttpGet]
[Route("greet")]
public async Task Greet(string? name = null, CancellationToken cancellationToken = default) {
    try {
        var result = await ProcessGreeting(Request.GetOwinContext(), name, cancellationToken).ConfigureAwait(false);
        return Ok(result);
    }
    catch (ArgumentException e) {
        return this.CreateProblemDetailsResponse(statusCode: HttpStatusCode.BadRequest, detail: e.Message);
    }
    catch (System.Security.SecurityException e) {
        return this.CreateProblemDetailsResponse(statusCode: HttpStatusCode.Forbidden, detail: e.Message);
    }
    catch (System.ComponentModel.DataAnnotations.ValidationException e) {
        return this.CreateValidationProblemDetailsResponse(e);
    }
    catch (Exception e) {
        return this.CreateServerErrorProblemDetailsResponse(e);
    }
}
```
