// Licensed under the MIT License.
// Copyright (c) 2018,2019 the AppCore .NET project.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace AppCore.Events.Storage
{
    public abstract class AsyncBackgroundTask : IBackgroundTask
    {
        private CancellationTokenSource _shutdownCts;
        private Task _backgroundTask;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (_backgroundTask != null && !_backgroundTask.IsCompleted)
                throw new InvalidOperationException("Background task already running.");

            _shutdownCts = new CancellationTokenSource();
            _backgroundTask = RunAsync(_shutdownCts.Token);
            return Task.FromResult(true);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_backgroundTask == null)
                return;

            try
            {
                _shutdownCts.Cancel();
            }
            finally
            {
                var tcs = new TaskCompletionSource<bool>();
                using (cancellationToken.Register(() => tcs.SetCanceled()))
                    await Task.WhenAny(_backgroundTask, tcs.Task);
            }
        }

        protected abstract Task RunAsync(CancellationToken cancellationToken);
    }
}