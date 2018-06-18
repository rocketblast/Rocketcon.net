using common;
using Orleans;
using Orleans.Concurrency;
using Orleans.Providers;
using System;
using System.Threading.Tasks;

namespace grains
{
    /// <summary>
    /// This class will be used for storing cached values in an grain instead of 
    /// external caching service. This solution will however not replace those services
    /// completly and thus should serve as asfasdbfgajhsdfj
    /// </summary>
    [StorageProvider(ProviderName = "RedisStore")]
    public class CacheGrain<T> : Grain, ICacheGrain<T>
    {
        Immutable<T> _item = new Immutable<T>(default(T));
        TimeSpan _timeToLive = TimeSpan.Zero;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="timeToLive"></param>
        /// <returns></returns>
        public Task Set(Immutable<T> item, TimeSpan timeToLive)
        {
            this._item = item;
            // Defaults to 2h if no value was given
            this._timeToLive = timeToLive == TimeSpan.Zero ? TimeSpan.FromHours(2) : timeToLive;

            // Makes sure orleans(current silo) won't deactivate our cache grain before TTL has passed
            this.DelayDeactivation(timeToLive);

            return Task.FromResult(0);
        }

        public Task<Immutable<T>> Get()
        {
            return Task.FromResult(this._item);
        }

        public Task Clear()
        {
            // Deactivates current grain after current request has been returned
            // and thus clearing current value
            this.DeactivateOnIdle();

            return Task.FromResult(0);
        }

        public Task Refresh()
        {
            // Refresh ttl for current grain
            this.DelayDeactivation(_timeToLive);

            return Task.FromResult(0);
        }
    }
}
