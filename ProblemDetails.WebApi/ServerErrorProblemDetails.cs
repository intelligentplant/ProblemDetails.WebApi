// Copyright (c) Intelligent Plant Ltd. All rights reserved.
// Copyright (c) Other contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Newtonsoft.Json;

namespace IntelligentPlant.ProblemDetails {

    /// <summary>
    /// A <see cref="ProblemDetails"/> for unhandled exceptions.
    /// </summary>
    public class ServerErrorProblemDetails : ProblemDetails {

        /// <summary>
        /// The stack trace for the error.
        /// </summary>
        [JsonProperty("stackTrace")]
        public string? StackTrace { get; set; }


        /// <summary>
        /// Creates a new <see cref="ServerErrorProblemDetails"/> object.
        /// </summary>
        public ServerErrorProblemDetails() {
            Title = Resources.ApiConventions_Title_500;
        }


        /// <summary>
        /// Creates a new <see cref="ServerErrorProblemDetails"/> object that uses the specified 
        /// exception to configure the <see cref="ProblemDetails.Detail"/> and <see cref="StackTrace"/> 
        /// properties..
        /// </summary>
        public ServerErrorProblemDetails(Exception? error) : this() { 
            if (error != null) {
                Detail = error.Message;
                StackTrace = error.StackTrace;
            }
        }

    }
}
