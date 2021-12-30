// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Browser;
using Microsoft.AspNetCore.Components;
using Microsoft.Maui.Essentials;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Booliba
{
    internal class AuthService
    {
        private readonly OidcClientOptions _options;
        private readonly OidcClient _client;

        public AuthService()
        {
            _options = new OidcClientOptions
            {
                Authority = "https://auth.arkia.dev",
                ClientId = "booliba",
                Scope = "booliba.basic",
                RedirectUri = "io.identitymodel.native://callback",
                Browser = new WebAuthenticatorBrowser()
            };

            _client = new OidcClient(_options);
        }

        public async Task<string> Login()
        {
            var result = await _client.LoginAsync();

            return result.AccessToken;
        }
    }

    internal class NavigationManagerBrowser : IBrowser
    {
        private readonly NavigationManager _navigation;

        public NavigationManagerBrowser(NavigationManager navigation) => _navigation = navigation;

        public Task<BrowserResult> InvokeAsync(BrowserOptions options,
               CancellationToken cancellationToken = default)
        {
            _navigation.NavigateTo(options.StartUrl, true);

            return Task.FromResult(new BrowserResult
            {
                Response = "sdfsdfds"
            });
        }
    }

    internal class WebAuthenticatorBrowser : IBrowser
    {
        public async Task<BrowserResult> InvokeAsync(BrowserOptions options,
               CancellationToken cancellationToken = default)
        {
            try
            {
                WebAuthenticatorResult authResult =
                    await WebAuthenticator.AuthenticateAsync(
                        new Uri(options.StartUrl),
                          new Uri(options.EndUrl)
                    );

                var authorizeResponse = ToRawIdentityUrl(options.EndUrl, authResult);

                return new BrowserResult
                {
                    Response = authorizeResponse
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return new BrowserResult()
                {
                    ResultType = BrowserResultType.UnknownError,
                    Error = ex.ToString()
                };
            }
        }

        private static string ToRawIdentityUrl(string redirectUrl, WebAuthenticatorResult result)
        {
            IEnumerable<string> parameters =
                 result.Properties.Select(pair => $"{pair.Key}={pair.Value}");
            var values = string.Join("&", parameters);

            return $"{redirectUrl}#{values}";
        }
    }
}
