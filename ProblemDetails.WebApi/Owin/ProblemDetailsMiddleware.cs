using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;

using Microsoft.Owin;

namespace IntelligentPlant.ProblemDetails.Owin {

    /// <summary>
    /// OWIN middleware that can be used to return RFC 7807 problem details when a request results 
    /// in a non-good status code.
    /// </summary>
    public class ProblemDetailsMiddleware : OwinMiddleware {

        /// <summary>
        /// Path string for the root of the application.
        /// </summary>
        private static readonly PathString s_rootPath = new PathString("/");

        /// <summary>
        /// Middleware options.
        /// </summary>
        private readonly ProblemDetailsMiddlewareOptions _options;


        /// <summary>
        /// Creates a new <see cref="ProblemDetailsMiddleware"/> object.
        /// </summary>
        /// <param name="next">
        ///   The next middleware in the pipeline.
        /// </param>
        /// <param name="options">
        ///   The middleware options. Can be <see langword="null"/>.
        /// </param>
        public ProblemDetailsMiddleware(OwinMiddleware next, ProblemDetailsMiddlewareOptions? options)
            : base(next) {
            _options = options ?? new ProblemDetailsMiddlewareOptions();
        }


        /// <inheritdoc/>
        public override async Task Invoke(IOwinContext context) {
            // Check if we are allowed to return a problem details object for this route.
            var canReturnProblemDetails = _options.IncludePaths != null && _options.IncludePaths.Any(x => context.Request.Path.StartsWithSegments(x) || x.Equals(s_rootPath)) && (_options.ExcludePaths == null || !_options.ExcludePaths.Any(x => context.Request.Path.StartsWithSegments(x) || x.Equals(s_rootPath)));
            if (!canReturnProblemDetails) {
                // We can't return a problem details object, so just pass to the next middleware and exit.
                await Next.Invoke(context).ConfigureAwait(false);
                return;
            }

            // We are allowed to return a problem details object for this route if required. We 
            // will create an in-memory buffer that will temporarily replace the output stream 
            // for the response. This will allow us to replace the response as written by the 
            // route handler if required.

            var response = context.Response;
            var responseBodyStream = response.Body;

            using (var buffer = new MemoryStream()) {
                // Replace the original output stream with our buffer.
                response.Body = buffer;
                // Flag that defines if we need to copy the contents of the buffer into the 
                // original output stream in the finally block below.
                var copyFromBuffer = true;
                try {
                    // Let the inner middleware handle the request.
                    await Next.Invoke(context).ConfigureAwait(false);

                    if (context.Response.StatusCode < 400 || context.Response.StatusCode > 599) {
                        // We only care about error status codes.
                        return;
                    }

                    if (context.Response.ContentType?.StartsWith(ClientErrorDataDefaults.MediaType, StringComparison.OrdinalIgnoreCase) ?? false) {
                        // The response already contains a problem details object.
                        return;
                    }

                    // We need to manually create a problem details response. We will discard any 
                    // existing response data that has been written to the buffer.
                    copyFromBuffer = false;

                    // Create a new buffer, and then create and serialize a problem details object 
                    // into the new buffer.
                    using (var ms = new MemoryStream()) {
                        var problemDetails = ProblemDetailsFactory.Default.CreateProblemDetails(context, context.Response.StatusCode);
                        var content = new ObjectContent(
                            problemDetails.GetType(),
                            problemDetails,
                            new JsonMediaTypeFormatter(),
                            ClientErrorDataDefaults.MediaType
                        );
                        await content.CopyToAsync(ms).ConfigureAwait(false);

                        // Now set the content type and content length on the response, and copy 
                        // the content of the new buffer into the original response stream.
                        response.ContentType = content.Headers.ContentType.ToString();
                        response.ContentLength = ms.Length;
                        ms.Position = 0;
                        await ms.CopyToAsync(responseBodyStream).ConfigureAwait(false);
                    }
                }
                finally {
                    if (copyFromBuffer) {
                        // We need to copy the contents of the buffer into the original response 
                        // stream.
                        buffer.Position = 0;
                        await buffer.CopyToAsync(responseBodyStream).ConfigureAwait(false);
                    }

                    // Finally, we need to restore the orignal response stream.
                    response.Body = responseBodyStream;
                }
            }
        }

    }
}
