using SampSharp.Core;

namespace SampSharp.RakNet
{
    public interface IHasClient
    {
        IGameModeClient GameModeClient { get; }
    }
}
