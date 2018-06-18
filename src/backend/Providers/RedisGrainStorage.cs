using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using Orleans.Storage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace silo.Providers
{
    public class RedisGrainStorage : IGrainStorage, ILifecycleParticipant<ISiloLifecycle>
    {
        private readonly ILogger<RedisGrainStorage> logger;

        public RedisGrainStorage(ILogger<RedisGrainStorage> logger)
        {
            this.logger = logger;
        }

        public Task ClearStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            throw new NotImplementedException();
        }

        public void Participate(ISiloLifecycle lifecycle)
        {
            throw new NotImplementedException();
        }

        public Task ReadStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            throw new NotImplementedException();
        }

        public Task WriteStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            throw new NotImplementedException();
        }
    }
}
