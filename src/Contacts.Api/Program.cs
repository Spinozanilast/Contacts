using Asp.Versioning;
using Contacts.Api;
using Contacts.Application.Extensions;
using Contacts.DataAccess.Extensions;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSerilog((_, loggerConfig) =>
{
    loggerConfig.WriteTo.Console();
    loggerConfig.ReadFrom.Configuration(builder.Configuration);
});

builder.Services.AddOpenApi();

builder.Services.AddContactsContext(builder.Configuration);
builder.Services
    .AddContactsCommands()
    .AddContactsQueries();

builder.Services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
        options.ApiVersionReader = new UrlSegmentApiVersionReader();
    })
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'V";
        options.SubstituteApiVersionInUrl = true;
    });

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAllOrigins",
            policyBuilder => policyBuilder.AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin()
                //.AllowCredentials()
                .SetIsOriginAllowed(_ => true)
        );
    });
}

var app = builder.Build();

var apiVersionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1, 0))
    .ReportApiVersions()
    .Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapGroup("/api/v{version:apiVersion}/contacts")
    .MapContactsEndpoints()
    .WithApiVersionSet(apiVersionSet)
    .HasApiVersion(new ApiVersion(1, 0));

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    await app.ApplyMigrations();
    app.UseCors("AllowAllOrigins");

    using var scope = app.Services.CreateScope();
    await scope.ServiceProvider.SeedContacts();
}

app.UseHttpsRedirection();

app.Run();