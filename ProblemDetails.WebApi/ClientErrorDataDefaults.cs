// Copyright (c) .NET Foundation. All rights reserved.
// Copyright (c) Intelligent Plant Ltd. All rights reserved.
// Copyright (c) Other contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace IntelligentPlant.ProblemDetails {

    /// <summary>
    /// Provides constants and defaults for use in the production of RFC 7807 responses.
    /// </summary>
    internal static class ClientErrorDataDefaults {

        /// <summary>
        /// Media type to use for problem details.
        /// </summary>
        internal const string MediaType = Constants.MediaType;


        /// <summary>
        /// Adds default client error mappings into the specified lookup.
        /// </summary>
        /// <param name="clientErrorMapping">
        ///   The client error mapping lookup.
        /// </param>
        internal static void ApplyDefaults(IDictionary<int, ClientErrorData> clientErrorMapping) {
            clientErrorMapping[400] = new ClientErrorData {
                Link = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = Resources.ApiConventions_Title_400,
            };

            clientErrorMapping[401] = new ClientErrorData {
                Link = "https://tools.ietf.org/html/rfc7235#section-3.1",
                Title = Resources.ApiConventions_Title_401,
            };

            clientErrorMapping[403] = new ClientErrorData {
                Link = "https://tools.ietf.org/html/rfc7231#section-6.5.3",
                Title = Resources.ApiConventions_Title_403,
            };

            clientErrorMapping[404] = new ClientErrorData {
                Link = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                Title = Resources.ApiConventions_Title_404,
            };

            clientErrorMapping[406] = new ClientErrorData {
                Link = "https://tools.ietf.org/html/rfc7231#section-6.5.6",
                Title = Resources.ApiConventions_Title_406,
            };

            clientErrorMapping[409] = new ClientErrorData {
                Link = "https://tools.ietf.org/html/rfc7231#section-6.5.8",
                Title = Resources.ApiConventions_Title_409,
            };

            clientErrorMapping[415] = new ClientErrorData {
                Link = "https://tools.ietf.org/html/rfc7231#section-6.5.13",
                Title = Resources.ApiConventions_Title_415,
            };

            clientErrorMapping[422] = new ClientErrorData {
                Link = "https://tools.ietf.org/html/rfc4918#section-11.2",
                Title = Resources.ApiConventions_Title_422,
            };

            clientErrorMapping[500] = new ClientErrorData {
                Link = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Title = Resources.ApiConventions_Title_500,
            };
        }

    }
}
