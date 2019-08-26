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
                "Processing event type {eventType}.");

        private static readonly LoggerEventDelegate<string, long> _pipelineProcessed =
            LoggerEvent.Define<string, long>(
                LogLevel.Debug,
                LogEventIds.PipelineProcessed,
                "Processed event type {eventType} in {elapsedTime} ms.");

        private static readonly LoggerEventDelegate<string, long> _pipelineFailed =
            LoggerEvent.Define<string, long>(
                LogLevel.Error,
                LogEventIds.PipelineFailed,
                "Processed event type {eventType} failed after {elapsedTime} ms.");

        private static readonly LoggerEventDelegate<string, string, long> _pipelineShortCircuited =
            LoggerEvent.Define<string, string, long>(
                LogLevel.Debug,
                LogEventIds.PipelineShortCircuited,
                "Processing of event type {eventType} was short-circuited by behavior {eventPipelineBehaviorType} in {elapsedTime} ms.");

        private static readonly LoggerEventDelegate<int, string> _invokingPreEventHandlers =
            LoggerEvent.Define<int, string>(
                LogLevel.Trace,
                LogEventIds.InvokingPreEventHandlers,
                "Invoking {preEventHandlerCount} pre-handlers for event type {eventType}.");

        private static readonly LoggerEventDelegate<int, string> _invokingPostEventHandlers =
            LoggerEvent.Define<int, string>(
                LogLevel.Trace,
                LogEventIds.InvokingPostEventHandlers,
                "Invoking {postEventHandlerCount} post-handlers for event type {eventType}.");

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

        public static void InvokingPreEventHandlers(this ILogger logger, Type eventType, int count)
        {
            _invokingPreEventHandlers(logger, count, eventType.GetDisplayName());
        }

        public static void InvokingPostEventHandlers(this ILogger logger, Type eventType, int count)
        {
            _invokingPostEventHandlers(logger, count, eventType.GetDisplayName());
        }
    }
}