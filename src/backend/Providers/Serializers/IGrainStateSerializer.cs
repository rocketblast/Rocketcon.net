using Newtonsoft.Json.Linq;
using Orleans;
using System;
using System.Collections.Generic;
using System.Text;

namespace silo.Providers.Serializers
{
    public interface IGrainStateSerializer
    {
        JObject Serializer(IGrainState grainState);
        void Deserialize(IGrainState grainState, JObject entityData);
    }
}
