// SampSharp.RakNet
// Copyright 2018 Danil Zelyutin
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
using System;
using SampSharp.GameMode;
using SampSharp.GameMode.API;

using SampSharp.RakNet;

using SampSharp.Core;
[assembly: SampSharpExtension(typeof(RakNet))]

namespace SampSharp.RakNet
{
    public partial class RakNet : Extension, IRakNet
    {
        internal static BaseMode Mode;
        internal static IGameModeClient Client => ((IHasClient)Mode).GameModeClient;

        #region Implementation of IService

        /// <summary>
        ///     Gets the game mode.
        /// </summary>
        public BaseMode GameMode { get; private set; }

        #endregion

        #region Overrides of Extension

        /// <summary>
        ///     Loads services provided by this extensions.
        /// </summary>
        /// <param name="gameMode">The game mode.</param>
        public override void LoadServices(BaseMode gameMode)
        {
            if (!typeof(IHasClient).IsAssignableFrom(gameMode.GetType()))
            {
                throw new RakNetException("[SampSharp.RakNet] Gamemode should implement IHasClient interface to use SampSharp.RakNet");
            }

            GameMode = gameMode;
            RakNet.Mode = gameMode;
            gameMode.Services.AddService<IRakNet>(this);

            base.LoadServices(gameMode);
        }

        #endregion

        public bool LoggingIncomingRpc { get; set; } = false;
        public bool LoggingOutcomingRpc { get; set; } = false;
        public bool LoggingIncomingPacket { get; set; } = false;
        public bool LoggingOutcomingPacket { get; set; } = false;
        public bool LoggingBlockingRpc { get; set; } = false;
        public bool LoggingBlockingPacket { get; set; } = false;

        public void SetLogging(bool incomingRpc, bool outcomingRpc, bool incomingPacket, bool outcomingPacket, bool blockingRpc, bool blockingPacket)
        {
            LoggingIncomingRpc = incomingRpc;
            LoggingOutcomingRpc = outcomingRpc;
            LoggingIncomingPacket = incomingPacket;
            LoggingOutcomingPacket = outcomingPacket;
            LoggingBlockingRpc = blockingRpc;
            LoggingBlockingPacket = blockingPacket;
        }

        public void BlockRpc()
        {
            if (LoggingBlockingRpc) Console.WriteLine($"[S#] Blocking next Rpc");
            Internal.CallRemoteFunction("BlockNextRpc", "");
        }
        public void BlockPacket()
        {
            if (LoggingBlockingPacket) Console.WriteLine($"[S#] Blocking next Packet");
            Internal.CallRemoteFunction("BlockNextPacket", "");
        }

    }
}
