namespace CodeNameK.LearnDisposable
{
    sealed class MyTask : IDisposable
    {
        private bool _isDisposed;
        private readonly Task _theTask;
        private CancellationTokenSource _cancellationTokenSource;
        public MyTask(Action body)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _theTask = new Task(body, _cancellationTokenSource.Token);
        }


        public void Dispose()
        {
            if(_isDisposed)
            {
                return;
            }
            _isDisposed = true;
            _cancellationTokenSource.Cancel();
            _theTask.Dispose();
            _cancellationTokenSource.Dispose();
        }
    }
}