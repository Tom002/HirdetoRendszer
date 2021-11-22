using Hellang.Middleware.ProblemDetails;
using HirdetoRendszer.Common.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;

using MvcProblemDetailsFactory = Microsoft.AspNetCore.Mvc.Infrastructure.ProblemDetailsFactory;

namespace HirdetoRendszer.Api.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddProblemDetailsForExceptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddProblemDetails(c =>
            {
                c.IncludeExceptionDetails = (ctx, ex) => true;
                c.ExceptionDetailsPropertyName = "exceptions";
                c.SourceCodeLineCount = 0;

                c.Map<EntityNotFoundException>((ctx, ex) =>
                    ctx.RequestServices
                        .GetRequiredService<MvcProblemDetailsFactory>()
                        .CreateProblemDetails(
                            ctx, StatusCodes.Status404NotFound, string.IsNullOrEmpty(ex.Title) ? "Keresett rekord nem található!" : ex.Title));

                c.Map<ValidationException>((ctx, ex) =>
                    ctx.RequestServices
                        .GetRequiredService<MvcProblemDetailsFactory>()
                        .CreateValidationProblemDetails(
                            ctx,
                            CreateModelStateDictionary(ex.Errors),
                            StatusCodes.Status400BadRequest,
                            string.IsNullOrEmpty(ex.Title) ? "Validációs hiba!" : ex.Title,
                            ex.Type?.ToString()));

                c.Map<ForbiddenException>((ctx, ex) =>
                  ctx.RequestServices
                      .GetRequiredService<MvcProblemDetailsFactory>()
                      .CreateProblemDetails(
                          ctx, StatusCodes.Status403Forbidden, string.IsNullOrEmpty(ex.Title) ? "Hozzáférés megtagadva!" : ex.Title));

                c.Map<BusinessException>((ctx, ex) =>
                    ctx.RequestServices
                        .GetRequiredService<MvcProblemDetailsFactory>()
                        .CreateProblemDetails(
                            ctx,
                            StatusCodes.Status500InternalServerError,
                            string.IsNullOrEmpty(ex.Title) ? "Hiba történt!" : ex.Title,
                            ex.Type?.ToString(),
                            ex.Message));

                c.Map<NotImplementedException>((ctx, ex) =>
                    ctx.RequestServices
                        .GetRequiredService<MvcProblemDetailsFactory>()
                        .CreateProblemDetails(ctx, StatusCodes.Status501NotImplemented, "Nem megvalósított funkció!"));

                // https://httpstatuses.com/499
                c.Map<OperationCanceledException>((ctx, ex) =>
                    ctx.RequestServices
                        .GetRequiredService<MvcProblemDetailsFactory>()
                        .CreateProblemDetails(ctx, 499, "Művelet kliens által megszakítva!"));

                // minden más
                c.Map<Exception>((ctx, ex) =>
                    ctx.RequestServices
                        .GetRequiredService<MvcProblemDetailsFactory>()
                        .CreateProblemDetails(ctx, StatusCodes.Status500InternalServerError, "Hiba történt!"));
            });

            return services;
        }

        public static IServiceCollection AddCorsPolicies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCors(o => o.AddPolicy("LocalCors", builder =>
            {
                builder.WithOrigins("http://localhost:3000")
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .AllowCredentials();
            }));

            return services;
        }

        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Hirdeto rendszer",
                    Version = "v1"
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                   {
                     new OpenApiSecurityScheme
                     {
                       Reference = new OpenApiReference
                       {
                         Type = ReferenceType.SecurityScheme,
                         Id = "Bearer"
                       }
                      },
                      new string[] { }
                    }
                  });
            });

            return services;
        }

        private static ModelStateDictionary CreateModelStateDictionary(IEnumerable<ValidationError> validationErrors)
        {
            var modelState = new ModelStateDictionary();

            foreach (var item in validationErrors)
            {
                modelState.AddModelError(item.Key, item.Error);
            }

            return modelState;
        }
    }
}
