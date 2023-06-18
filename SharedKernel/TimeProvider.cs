using System;

namespace SharedKernel
{
    public abstract class TimeProvider
    {
        private static TimeProvider _current =
            DefaultTimeProvider.Instance;

        public static TimeProvider Current
        {
            get => TimeProvider._current;
            set => _current = value;
        }

        public abstract DateTime UtcNow { get; }
    }

    public class DefaultTimeProvider : TimeProvider
    {
        
        private static readonly  DefaultTimeProvider _instance =
            new DefaultTimeProvider();

        public override DateTime UtcNow => DateTime.UtcNow;

        public static DefaultTimeProvider Instance => DefaultTimeProvider._instance;
    }
}