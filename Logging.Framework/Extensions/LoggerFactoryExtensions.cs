using Logging.Framework.Interfaces;
using Logging.Framework.Internal;
using Logging.Framework.Models;
using Logging.Framework.Provider;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logging.Framework.Extensions
{
   public static class LoggerFactoryExtensions
    {

        #region IServiceCollection DbLogger
        public static ILoggingBuilder AddDatabaseLogger(this ILoggingBuilder builder, IConfiguration parentConfig, string loggingSectionName = "Logging", string connectionStringSectionName = "ConnectionStrings")
        {
            builder = builder ?? throw new ArgumentNullException(nameof(builder));
            builder.Services.AddDatabaseLogger(parentConfig, loggingSectionName, connectionStringSectionName);
            return builder;
        }
        public static ILoggingBuilder AddDatabaseLogger(this ILoggingBuilder builder, IConfiguration loggerSettings, IConfiguration connectionStrings)
        {
            builder = builder ?? throw new ArgumentNullException(nameof(builder));
            builder.Services.AddDatabaseLogger(loggerSettings, connectionStrings);
            return builder;
        }

        public static ILoggingBuilder AddDatabaseLogger(this ILoggingBuilder builder, LoggerSettings loggerSettings)
        {
            builder = builder ?? throw new ArgumentNullException(nameof(builder));
            builder.Services.AddDatabaseLoggerRequiredServices(loggerSettings);
            return builder;
        }

        public static IServiceCollection AddDatabaseLogger(this IServiceCollection services, IConfiguration parentConfig, string loggingSectionName = "Logging", string connectionStringSectionName = "ConnectionStrings")
        {
            services = services ?? throw new ArgumentNullException(nameof(services));
            var loggerSettings = parentConfig.GetSection(loggingSectionName);
            var connectionString = parentConfig.GetSection(connectionStringSectionName);
            return services.AddDatabaseLogger(loggerSettings, connectionString);
        }

        public static IServiceCollection AddDatabaseLogger(this IServiceCollection services, IConfiguration loggerSettings, IConfiguration connectionStrings)
        {
            services = services ?? throw new ArgumentNullException(nameof(services));
            loggerSettings = loggerSettings ?? throw new ArgumentNullException(nameof(loggerSettings));
            connectionStrings = connectionStrings ?? throw new ArgumentNullException(nameof(connectionStrings));

            var settings = new LoggerSettings(loggerSettings, connectionStrings);
            services.AddDatabaseLoggerRequiredServices(settings);
            return services;
        }

        public static void AddDatabaseLoggerRequiredServices(this IServiceCollection services, LoggerSettings settings)
        {
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<ILoggerProvider, DbLoggerProvider>(pvd =>
           {
               var processor = new DbLoggerProcessor(settings);
               return new DbLoggerProvider(pvd, settings, processor);
           });           
        }
        #endregion

        #region FileLogger
        public static ILoggingBuilder AddFileLogger(this ILoggingBuilder builder, IConfiguration parentConfig, string loggingSectionName = "Logging")
        {
            builder = builder ?? throw new ArgumentNullException(nameof(builder));
            var loggerSettings = parentConfig.GetSection(loggingSectionName);
            builder.Services.AddFileLogger(loggerSettings);            
            return builder;
        }
        public static ILoggingBuilder AddFileLogger(this ILoggingBuilder builder, LoggerSettings settings)
        {
            builder = builder ?? throw new ArgumentNullException(nameof(builder));           
            builder.Services.AddFileLoggerRequiredServices(settings);
            return builder;
        }
        //Service collection
        public static IServiceCollection AddFileLogger(this IServiceCollection services, IConfiguration loggerSettings)
        {
            services = services ?? throw new ArgumentNullException(nameof(services));

            loggerSettings = loggerSettings ?? throw new ArgumentNullException(nameof(loggerSettings));
            var settings = new LoggerSettings(loggerSettings);
            services.AddFileLogger(settings);
            return services;
        }
        public static IServiceCollection AddFileLogger(this IServiceCollection services, LoggerSettings loggerSettings)
        {
            services = services ?? throw new ArgumentNullException(nameof(services));           
            services.AddFileLoggerRequiredServices(loggerSettings);
            return services;
        }

        public static void AddFileLoggerRequiredServices(this IServiceCollection services, LoggerSettings loggerSettings)
        {
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<ILoggerProvider, FileLoggerProvider>(pvd =>
           {
               var processor = new FileLogger(loggerSettings);
               return new FileLoggerProvider(pvd, loggerSettings, processor);
           });           
        }

        #endregion
        public static Task[] GetLoggerProviderWaitTasks(this IServiceProvider provider)
        {
            return provider.GetServices<ILoggerProvider>()
                .Where(pvd => pvd is ICanWait)
                .Cast<ICanWait>()
                .Select(pvd => pvd.WaitToComplete())
                .ToArray();
        }

        public static bool BlockForAsyncLoggerToComplete(this IServiceProvider provider, int ms = 2000)
        {
            var tasks = provider.GetLoggerProviderWaitTasks();

            return tasks.Length == 0 || Task.WaitAll(tasks, ms);
        }
    }
}
