// Licensed under the MIT License.
// Copyright (c) 2018 the AppCore .NET project.

using System.Threading;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace AppCore.Events.Pipeline
{
    internal interface IEventPipeline
    {
        Task InvokeAsync(IEventContext eventContext, CancellationToken cancellationToken);
    }
}