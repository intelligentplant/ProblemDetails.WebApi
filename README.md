# ProblemDetails.WebApi

Adds [RFC 7807](https://tools.ietf.org/html/rfc7807) support to ASP.NET 4.x Web API applications. Note that this library supports ASP.NET 4.x; **ASP.NET Core is not supported**.


## Installation

Add a reference to the [IntelligentPlant.ProblemDetails.WebApi](https://www.nuget.org/packages/IntelligentPlant.ProblemDetails.WebApi) NuGet package.


## Examples

An example self-hosted application can be found [here](/ProblemDetails.WebApi.Sample).


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


## Manually Creating Problem Details Responses in API Controllers

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


## Handling Exceptions in the OWIN Pipeline

It is also possible to generate problem details responses from unhandled exceptions in the OWIN pipeline. To do this, you can assign a callback function to the `ExceptionHandler` property on the `ProblemDetailsMiddlewareOptions` class:

```csharp
public void Configuration(IAppBuilder app) {
    // We only want to return a problem details response on API routes.
    app.UseProblemDetails(new ProblemDetailsMiddlewareOptions() {
        IncludePaths = new [] {
            new PathString("/api")
        },
        ExceptionHandler = ProblemDetailsErrorHandler
    });
}


private static ProblemDetails? ProblemDetailsErrorHandler(IOwinContext context, Exception error, ProblemDetailsFactory factory) {
    if (error is System.Security.SecurityException) {
        return factory.CreateProblemDetails(context, 403);
    }

    // - handling for other exceptions here -

    // Return null to rethrow the unhandled exception.
    return null;
}

```


## Handling Web API Exceptions in the OWIN Pipeline

By default, Web API ensures that all unhandled exception in the Web API pipeline are handled using its `IExceptionHandler` service. However, you may wish to handle Web API exceptions directly in the OWIN pipeline, so that all unhandled application errors pass through the same set of OWIN middleware prior to generating a problem details response for an API call.

You can disable Web API exception handling completely when registering the Web API problem details configuration with the Web API configuration:

```csharp
public void Configuration(IAppBuilder app) {
    // We only want to return a problem details response on API routes.
    app.UseProblemDetails(new ProblemDetailsMiddlewareOptions() {
        IncludePaths = new [] {
            new PathString("/api")
        },
        ExceptionHandler = ProblemDetailsErrorHandler
    });

    var config = new HttpConfiguration();
    config.AddProblemDetails(handleExceptions: false);
    config.MapHttpAttributeRoutes();

    app.UseWebApi(config);
}


private static ProblemDetails? ProblemDetailsErrorHandler(IOwinContext context, Exception error, ProblemDetailsFactory factory) {
    if (error is System.Security.SecurityException) {
        return factory.CreateProblemDetails(context, 403);
    }

    // - handling for other exceptions here -

    // Return null to rethrow the unhandled exception.
    return null;
}
```


## Transforming Problem Details via Callback

`ProblemDetailsFactory` defines an `OnDetailsCreated` callback that will be invoked whenever the factory creates a problem details instance. You can use this callback to e.g. add custom properties to your problem details objects.

```csharp
public void Configuration(IAppBuilder app) {
    ProblemDetailsFactory.Default.OnDetailsCreated = (context, details) => {
        // Add these properties to every problem detail.
        details.Extensions["callback-added"] = "some-value";
        details.Extensions["utc-time"] = DateTime.UtcNow.ToString("u");
    };

    app.UseProblemDetails(new ProblemDetailsMiddlewareOptions() {
        IncludePaths = new [] {
            new PathString("/api")
        }
    });

    var config = new HttpConfiguration();
    config.AddProblemDetails();
    config.MapHttpAttributeRoutes();

    app.UseWebApi(config);
}
```

Note that, rather than adding an `extensions` property to the problem details JSON representation, the JSON.NET serializer will add a new property to the JSON object for each entry in the `Extensions` dictionary on the `ProblemDetails` instance e.g.

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.3",
  "title": "Forbidden",
  "status": 403,
  "detail": null,
  "instance": "/api/delete-item/1",
  "callback-added": "some-value",
  "utc-time": "2020-12-11 07:20:00Z"
}
```
