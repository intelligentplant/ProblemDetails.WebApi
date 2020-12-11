// Copyright (c) Intelligent Plant Ltd. All rights reserved.
// Copyright (c) Other contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProblemDetails.WebApi.Sample {
    public class Program {

        public static async Task Main() {
            using (var host = Microsoft.Owin.Hosting.WebApp.Start<Startup>("http://localhost:5000")) {
                var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

                Console.CancelKeyPress += (sender, args) => {
                    tcs.TrySetResult(true);
                };

                Console.WriteLine("Press CTRL+C to exit");

                await tcs.Task.ConfigureAwait(false);
            }
        }

    }
}
