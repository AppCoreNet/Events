// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using System.Threading;
using System.Threading.Tasks;

namespace AppCore.Events.Storage
{
    public interface IBackgroundTask
    {
        Task StartAsync(CancellationToken cancellationToken);

        Task StopAsync(CancellationToken cancellationToken);
    }
}