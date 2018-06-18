using Orleans;
using Orleans.Concurrency;
using System;
using System.Threading.Tasks;

namespace common
{
    public interface ICacheGrain<T> : IGrainWithStringKey
    {
        Task Set(Immutable<T> item, TimeSpan timeToLive);
        Task<Immutable<T>> Get();
        Task Clear();
        Task Refresh();
    }
}
