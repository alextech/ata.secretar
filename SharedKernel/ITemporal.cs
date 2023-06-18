using System;

namespace SharedKernel
{
    public interface ITemporal
    {
        DateTimeOffset AsOf { get; }
    }
}