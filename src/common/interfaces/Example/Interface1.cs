using Orleans;
using System.Threading.Tasks;

namespace interfaces.Example
{
    public interface IValueGrain : IGrainWithIntegerKey
    {
        Task<string> GetValue();
        Task SetValue(string value);
    }
}
