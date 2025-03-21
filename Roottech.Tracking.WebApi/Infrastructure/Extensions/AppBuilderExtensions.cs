﻿using System;
using System.Threading;
using Owin;

namespace Roottech.Tracking.WebApi.Infrastructure.Extensions
{
    public static class AppBuilderExtensions
    {
        const string AppDisposingKey = "host.OnAppDisposing";

        /// <summary>
        /// Registers the given disposable to be disposed when the application shuts down.
        /// </summary>
        public static void RegisterForDisposal(this IAppBuilder appBuilder, IDisposable disposable)
        {
            if (appBuilder.Properties.ContainsKey(AppDisposingKey))
            {
                if (appBuilder.Properties[AppDisposingKey] is CancellationToken token)
                {
                    if (token != CancellationToken.None)
                    {
                        token.Register(disposable.Dispose);
                    }
                }
            }
        }

        /// <summary>
        /// Detects whether the request is using the http scheme, redirecting to the appropriate https route if necessary
        /// </summary>
        public static void ForceHttps(this IAppBuilder appBuilder)
        {
            appBuilder.Use((context, next) =>
            {
                var requestUri = context.Request.Uri;

                if (requestUri.Scheme == "http")
                {
                    var port = requestUri.Port;
                    var httpsUrl = $"https://{requestUri.Host}:{port}{requestUri.PathAndQuery}";

                    context.Response.Redirect(httpsUrl);
                }

                return next();
            });
        }
    }
}