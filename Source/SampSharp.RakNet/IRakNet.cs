using System;
using SampSharp.GameMode;

namespace SampSharp.RakNet
{
    public interface IRakNet : IService
    {
        void PostLoad(BaseMode gameMode);
    }
}
