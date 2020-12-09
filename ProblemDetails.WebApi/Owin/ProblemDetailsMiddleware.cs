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

                    if (response.StatusCode < 400 || response.StatusCode > 599) {
                        // We only care about error status codes.
                        return;
                    }

                    if (response.ContentType?.StartsWith(ClientErrorDataDefaults.MediaType, StringComparison.OrdinalIgnoreCase) ?? false) {
                        // The response already contains a problem details object.
                        return;
                    }

                    // We need to manually create a problem details response. We will discard any 
                    // existing response data that has been written to the buffer.
                    copyFromBuffer = false;

                    var problemDetails = ProblemDetailsFactory.Default.CreateProblemDetails(context, response.StatusCode);
                    await WriteProblemDetailsToStream(problemDetails, response, responseBodyStream).ConfigureAwait(false);
                }
                catch (Exception e) {
                    var errorDetails = _options.ExceptionHandler?.Invoke(e, ProblemDetailsFactory.Default);
                    if (errorDetails == null) {
                        // No problem details provided; rethrow the exception.
                        throw;
                    }

                    await WriteProblemDetailsToStream(errorDetails, response, responseBodyStream).ConfigureAwait(false);
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


        /// <summary>
        /// Writes the specified problem details object to the HTTP response stream.
        /// </summary>
        /// <param name="problemDetails">
        ///   The problem details.
        /// </param>
        /// <param name="response">
        ///   The HTTP response.
        /// </param>
        /// <param name="stream">
        ///   The destination response stream.
        /// </param>
        /// <returns>
        ///   A <see cref="Task"/> that will perform the write.
        /// </returns>
        private async Task WriteProblemDetailsToStream(ProblemDetails problemDetails, IOwinResponse response, Stream stream) {
            // Create a new buffer, and then create and serialize a problem details object 
            // into the new buffer.
            using (var ms = new MemoryStream()) {
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
                await ms.CopyToAsync(stream).ConfigureAwait(false);
            }
        }

    }
}
