

#nullable enable
using Apizr.Extending.Configuring.Registry;

namespace StarCellar.Services.Apis
{
    // Please make sure to complete the following steps resulting from your configuration:
    // - dotnet add package Apizr.Integrations.FileTransfer.MediatR, then register MediatR
    // - dotnet add package Apizr.Extensions.Microsoft.Caching, then register your caching provider
    // - dotnet add package Apizr.Integrations.AutoMapper, then register AutoMapper
    // - dotnet add package Apizr.Integrations.Fusillade
    // - Add your file transfer manager while calling ConfigureStarCellarApizrManagers method thanks to its options builder parameter

    using System;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Http.Resilience;
    using AutoMapper;
    using MediatR;
    using Apizr;
    using Apizr.Configuring;
    using Apizr.Extending.Configuring.Common;

  
    public static partial class IServiceCollectionExtensions
    {
        /// <summary>
        /// Register all your Apizr managed apis with common shared options.
        /// You may call WithConfiguration option to adjust settings to your need.
        /// </summary>
        /// <param name="optionsBuilder">Adjust common shared options</param>
        /// <returns></returns>
        public static IServiceCollection ConfigureStarCellarApizrManagers(
            this IServiceCollection services,
            Action<IApizrExtendedCommonOptionsBuilder>? optionsBuilder = null)
        {
            optionsBuilder ??= _ => { }; // Default empty options if null
            optionsBuilder += options => options
                .WithBaseAddress("https://rx6z0kd7-7015.uks1.devtunnels.ms", ApizrDuplicateStrategy.Ignore)
                .ConfigureHttpClientBuilder(builder => builder
                    .AddStandardResilienceHandler(config =>
                    {
                        config.Retry = new HttpRetryStrategyOptions
                        {
                            UseJitter = true,
                            MaxRetryAttempts = 3,
                            Delay = TimeSpan.FromSeconds(0.5)
                        };
                    }))
                .WithInMemoryCacheHandler()
                .WithAutoMapperMappingHandler()
                .WithPriority()
                .WithMediation()
                .WithFileTransferMediation();
            
            return services.AddApizr(
                registry => registry
                  .AddManagerFor<ICellarApi>()
                  .AddManagerFor<IFileApi>()
                  .AddManagerFor<IUserApi>(),
                optionsBuilder);

        }
    }
}

