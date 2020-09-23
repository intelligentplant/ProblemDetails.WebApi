﻿// Copyright (c) .NET Foundation. All rights reserved.
// Copyright (c) Intelligent Plant Ltd. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Web.Http.ModelBinding;

using Newtonsoft.Json;

namespace IntelligentPlant.ProblemDetails {
    /// <summary>
    /// A <see cref="ProblemDetails"/> for validation errors.
    /// </summary>
    public class ValidationProblemDetails : ProblemDetails {

        /// <summary>
        /// Gets the validation errors associated with this instance of <see cref="ValidationProblemDetails"/>.
        /// </summary>
        [JsonProperty("errors")]
        public IDictionary<string, string[]> Errors { get; } = new Dictionary<string, string[]>(StringComparer.Ordinal);


        /// <summary>
        /// Initializes a new instance of <see cref="ValidationProblemDetails"/>.
        /// </summary>
        public ValidationProblemDetails() {
            Detail = Resources.ValidationProblemDescription_Details;
        }


        /// <summary>
        /// Initializes a new instance of <see cref="ValidationProblemDetails"/> using the specified <paramref name="modelState"/>.
        /// </summary>
        /// <param name="modelState"><see cref="ModelStateDictionary"/> containing the validation errors.</param>
        public ValidationProblemDetails(ModelStateDictionary modelState)
            : this() {
            if (modelState == null) {
                throw new ArgumentNullException(nameof(modelState));
            }

            foreach (var keyModelStatePair in modelState) {
                var key = keyModelStatePair.Key;
                var errors = keyModelStatePair.Value.Errors;
                if (errors != null && errors.Count > 0) {
                    if (errors.Count == 1) {
                        var errorMessage = GetErrorMessage(errors[0]);
                        Errors.Add(key, new[] { errorMessage });
                    }
                    else {
                        var errorMessages = new string[errors.Count];
                        for (var i = 0; i < errors.Count; i++) {
                            errorMessages[i] = GetErrorMessage(errors[i]);
                        }

                        Errors.Add(key, errorMessages);
                    }
                }
            }

            string GetErrorMessage(ModelError error) {
                return string.IsNullOrEmpty(error.ErrorMessage) ?
                    Resources.SerializableError_DefaultError :
                    error.ErrorMessage;
            }
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ValidationProblemDetails"/> using the specified <paramref name="errors"/>.
        /// </summary>
        /// <param name="errors">The validation errors.</param>
        public ValidationProblemDetails(IDictionary<string, string[]> errors)
            : this() {
            if (errors == null) {
                throw new ArgumentNullException(nameof(errors));
            }

            Errors = new Dictionary<string, string[]>(errors, StringComparer.Ordinal);
        }

    }
}
