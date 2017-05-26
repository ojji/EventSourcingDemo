using System;
using Board.Api.Domain.Normalizers;
using Board.Api.Domain.ReadModels;
using Board.Api.Domain.Repositories;
using Board.Api.Domain.Services;
using Board.Common.Events;
using Board.Common.Utils;
using EventStore.ClientAPI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Board.Api
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddTransient<IEventStore, Board.Common.Events.EventStore>();
            services.AddTransient<IProjectManagerService, ProjectManagerService>();
            services.AddTransient<ProjectRepository>();
            services.AddSingleton<RedisProjectViewNormalizer>();
            services.AddSingleton<IEventStoreConnection>(provider =>
            {
                var connectionSettings = ConnectionSettings
                    .Create()
                    .UseConsoleLogger()
                    .Build();
                var connection =
                    EventStoreConnection.Create(connectionSettings, new Uri("tcp://admin:changeit@localhost:1113"));
                connection.ConnectAsync().Wait();
                return connection;
            });

            services.AddSingleton<IConnectionMultiplexer>(provider => ConnectionMultiplexer.Connect("localhost"));
            RedisMapper.RegisterType<ProjectReadModel>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            var normalizer = app.ApplicationServices.GetService<RedisProjectViewNormalizer>();
            normalizer.Init();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseMvcWithDefaultRoute();
        }
    }
}