using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace SharedKernel
{
    public class TimerService
    {
        private Timer _timer;
        private static SemaphoreSlim _semaphore;

        public void SetTimer(double interval)
        {
            _semaphore = new SemaphoreSlim(1, 1);

            _timer = new Timer(interval)
            {
                AutoReset = true,
                Enabled = true
            };
            _timer.Elapsed += NotifyTimerElapsed;
        }

        public void Stop() {
            _timer?.Stop();
        }

        public event Action<SemaphoreSlim> OnElapsed;

        private void NotifyTimerElapsed(object source, ElapsedEventArgs e)
        {
            Run();
        }

        private async Task Run()
        {
            await _semaphore.WaitAsync();
            OnElapsed?.Invoke(_semaphore);
        }

        public async Task RunImmediately()
        {
            await Run();
        }
    }
}