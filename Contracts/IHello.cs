using System.Threading.Tasks;
using Orleans;

namespace Contracts
{
    public interface IHello : IGrainWithIntegerKey
    {
        Task<string> SayHello(string greeting);
    }
}