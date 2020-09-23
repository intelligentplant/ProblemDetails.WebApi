// Copyright (c) .NET Foundation. All rights reserved.
// Copyright (c) Intelligent Plant Ltd. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace IntelligentPlant.ProblemDetails {

    /// <summary>
    /// Information for producing client errors.
    /// </summary>
    public class ClientErrorData {
        /// <summary>
        /// Gets or sets a link (URI) that describes the client error.
        /// </summary>
        /// <remarks>
        /// By default, this maps to <see cref="ProblemDetails.Type"/>.
        /// </remarks>
        public string Link { get; set; } = default!;

        /// <summary>
        /// Gets or sets the summary of the client error.
        /// </summary>
        /// <remarks>
        /// By default, this maps to <see cref="ProblemDetails.Title"/> and should not change
        /// between multiple occurrences of the same error.
        /// </remarks>
        public string Title { get; set; } = default!;
    }
}
