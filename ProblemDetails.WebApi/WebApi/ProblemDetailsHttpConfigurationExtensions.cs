// Copyright (c) Intelligent Plant Ltd. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using IntelligentPlant.ProblemDetails.WebApi;

namespace System.Web.Http {

    /// <summary>
    /// Extensions for <see cref="HttpConfiguration"/> objects.
    /// </summary>
    public static class ProblemDetailsHttpConfigurationExtensions {

        /// <summary>
        /// Add support for creating RFC 7807 problem details responses when a controller action 
        /// returns a non-good status code.
        /// </summary>
        /// <param name="httpConfiguration">
        ///   The <see cref="HttpConfiguration"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="httpConfiguration"/> is <see langword="null"/>.
        /// </exception>
        public static void AddProblemDetails(this HttpConfiguration httpConfiguration) {
            if (httpConfiguration == null) {
                throw new ArgumentNullException(nameof(httpConfiguration));
            }

            httpConfiguration.Filters.Add(new ProblemDetailsErrorFilter());
            httpConfiguration.Filters.Add(new ProblemDetailsActionFilter());
        }

    }
}
