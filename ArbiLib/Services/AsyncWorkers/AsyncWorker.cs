namespace ArbiLib.Services.Worker
{
    public abstract class AsyncWorker(ArbiService Service) : IDisposable
    {
        public bool IsRunning { get; protected set; } = false;
        public ArbiService Arbi { get; protected set; } = Service;
        protected CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        protected Task? _task = null;

        public void Dispose()
        {
            StopWork();
        }

        public void StopWork()
        {
            _cancellationTokenSource.Cancel();
            try
            {
                _task?.Wait();
            }
            catch (Exception) { }
            _cancellationTokenSource.Dispose();
            IsRunning = false;
        }

        public void StartWork()
        {
            if (IsRunning) return;

            IsRunning = true;
            _cancellationTokenSource = new CancellationTokenSource();
            _task = Task.Run(async () => await StartWork(_cancellationTokenSource.Token));
        }

        private async Task StartWork(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                Task t = DoWork();
                await t;
            }
        }

        protected abstract Task DoWork();
    }
}
