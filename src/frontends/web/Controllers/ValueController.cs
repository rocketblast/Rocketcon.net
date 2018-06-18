using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using interfaces.Example;
using Microsoft.AspNetCore.Mvc;
using Orleans;

namespace Rocketcon_web.Controllers
{
    [Route("api/[controller]")]
    public class ValueController : Controller
    {
        private IClusterClient client;

        public ValueController(IClusterClient client)
        {
            this.client = client;
        }

        [HttpGet("{id}")]
        public async Task<object> Get(int id)
        {
            var grain = this.client.GetGrain<IValueGrain>(id);
            var returnValue = await grain.GetValue();

            return Ok(new
            {
                id = id,
                value = (string)returnValue
            });
        }

        [HttpPost("{id}")]
        public async Task<object> Post(int id, [FromQuery]string value)
        {
            var grain = this.client.GetGrain<IValueGrain>(id);
            await grain.SetValue(value);

            return Ok(new
            {
                id = id,
                value = (string)value
            });
        }
    }
}