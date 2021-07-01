// Copyright (c) Intelligent Plant Ltd. All rights reserved.
// Copyright (c) Other contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ProblemDetails.WebApi.Sample {
    public class Program {

        public static async Task Main() {
            await new HostBuilder().ConfigureServices(services => {
                services.AddHostedService<OwinWebHost>();
            }).RunConsoleAsync().ConfigureAwait(false);
        }


        private class OwinWebHost : IHostedService {

            private IDisposable? _webHost; 

            public Task StartAsync(CancellationToken cancellationToken) {
                const string url = "http://localhost:5000";
                _webHost ??= Microsoft.Owin.Hosting.WebApp.Start<Startup>(url);
                
                Console.WriteLine($"Listening on: {url}");
                Console.WriteLine("Press CTRL+C to exit");

                return Task.CompletedTask;
            }

            public Task StopAsync(CancellationToken cancellationToken) {
                _webHost?.Dispose();
                return Task.CompletedTask;
            }
        }

    }
}
