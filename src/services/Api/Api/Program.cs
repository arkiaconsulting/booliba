// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using Booliba.Api.Infrastructure;
using Booliba.QuerySide;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new DateOnlyConverter());
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

builder.Services
    .AddApplicationCore()
    .AddInMemoryEventStore()
    .AddNullEmailNotifier()
    .AddQuerySide()
    .AddInMemoryProjection();

builder.Services.AddAuthentication()
    .AddJwtBearer("oauth", "AKC Auth", options =>
    {
        options.Audience = "booliba";
        options.TokenValidationParameters.ValidateIssuer = true;
        options.TokenValidationParameters.ValidateIssuerSigningKey = true;
        options.TokenValidationParameters.ValidIssuer = "https://auth.arkia.dev";
        options.TokenValidationParameters.ValidateAudience = true;
    });

builder.Services.AddQuerySide();
builder.Services.AddApplicationInsightsTelemetry();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();
