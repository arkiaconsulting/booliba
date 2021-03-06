// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using Booliba.Data;
using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using System;
using System.Text.Json;

namespace Booliba
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .RegisterBlazorMauiWebView()
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddBlazorWebView();
            builder.Services.AddHttpClient<BoolibaService>(
                httpClient => httpClient.BaseAddress = new Uri("https://booliba.azurewebsites.net/api/")
            );
            builder.Services.AddSingleton(_ =>
            {
                var serializationOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                serializationOptions.Converters.Add(new DateOnlyConverter());

                return serializationOptions;
            });


            return builder.Build();
        }
    }
}