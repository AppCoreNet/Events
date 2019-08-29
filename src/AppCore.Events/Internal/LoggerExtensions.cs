// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using System;
using AppCore.Logging;

namespace AppCore.Events
{
    internal static class LoggerExtensions
    {
        private static readonly LoggerEventDelegate<string> _pipelineProcessing =
            LoggerEvent.Define<string>(
                LogLevel.Trace,
                LogEventIds.PipelineProcessing,
                "Processing event {eventType} ...");

        private static readonly LoggerEventDelegate<string, long> _pipelineProcessed =
            LoggerEvent.Define<string, long>(
                LogLevel.Debug,
                LogEventIds.PipelineProcessed,
                "Successfully processed event {eventType} in {elapsedTime} ms.");

        private static readonly LoggerEventDelegate<string, long> _pipelineFailed =
            LoggerEvent.Define<string, long>(
                LogLevel.Error,
                LogEventIds.PipelineFailed,
                "Failed to process event {eventType} after {elapsedTime} ms.");

        private static readonly LoggerEventDelegate<string, string, long> _pipelineShortCircuited =
            LoggerEvent.Define<string, string, long>(
                LogLevel.Debug,
                LogEventIds.PipelineShortCircuited,
                "Processing of event {eventType} was short-circuited by behavior {eventPipelineBehaviorType} in {elapsedTime} ms.");

        private static readonly LoggerEventDelegate<string, string> _invokingBehavior =
            LoggerEvent.Define<string, string>(
                LogLevel.Trace,
                LogEventIds.InvokingBehavior,
                "Invoking behavior {pipelineBehaviorType} for event {eventType} ...");

        private static readonly LoggerEventDelegate<string, string> _invokingPreEventHandler =
            LoggerEvent.Define<string, string>(
                LogLevel.Trace,
                LogEventIds.InvokingPreEventHandler,
                "Invoking pre-handler {preEventHandlerType} for event {eventType} ...");

        private static readonly LoggerEventDelegate<string, string> _invokingPostEventHandler =
            LoggerEvent.Define<string, string>(
                LogLevel.Trace,
                LogEventIds.InvokingPostEventHandler,
                "Invoking post-handler {postEventHandlerType} for event {eventType} ...");

        public static void PipelineProcessing(this ILogger logger, Type eventType)
        {
            _pipelineProcessing(logger, eventType.GetDisplayName());
        }

        public static void PipelineProcessed(this ILogger logger, Type eventType, TimeSpan elapsed)
        {
            _pipelineProcessed(logger, eventType.GetDisplayName(), (long) elapsed.TotalMilliseconds);
        }

        public static void PipelineFailed(this ILogger logger, Type eventType, TimeSpan elapsed, Exception exception)
        {
            _pipelineFailed(logger, eventType.GetDisplayName(), (long) elapsed.TotalMilliseconds, exception:exception);
        }

        public static void PipelineShortCircuited(this ILogger logger, Type eventType, Type eventPipelineBehavior, TimeSpan elapsed)
        {
            _pipelineShortCircuited(
                logger,
                eventType.GetDisplayName(),
                eventPipelineBehavior.GetDisplayName(),
                (long) elapsed.TotalMilliseconds);
        }

        public static void InvokingBehavior(this ILogger logger, Type eventType, Type pipelineBehaviorType)
        {
            _invokingBehavior(logger, pipelineBehaviorType.GetDisplayName(), eventType.GetDisplayName());
        }

        public static void InvokingPreEventHandler(this ILogger logger, Type eventType, Type preHandlerType)
        {
            _invokingPreEventHandler(logger, preHandlerType.GetDisplayName(), eventType.GetDisplayName());
        }

        public static void InvokingPostEventHandler(this ILogger logger, Type eventType, Type postHandlerType)
        {
            _invokingPostEventHandler(logger, postHandlerType.GetDisplayName(), eventType.GetDisplayName());
        }
    }
}