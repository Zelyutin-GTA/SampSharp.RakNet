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

using SampSharp.Core.Callbacks;
using SampSharp.RakNet.Definitions;
using SampSharp.RakNet.Events;

namespace SampSharp.RakNet
{
    public partial class RakNet
    {
        public event EventHandler<PacketRpcEventArgs> IncomingRpc;
        public event EventHandler<PacketRpcEventArgs> OutcomingRpc;
        public event EventHandler<PacketRpcEventArgs> IncomingPacket;
        public event EventHandler<PacketRpcEventArgs> OutcomingPacket;

        [Callback]
        internal void OnIncomingRPC(int playerid, int rpcid, int bs)
        {
            IncomingRpc?.Invoke(this, new PacketRpcEventArgs(rpcid, playerid, bs));
            if(LoggingIncomingRpc) Console.WriteLine($"[SampSharp.RakNet] Hooking Incoming RPC {playerid}, {rpcid}, {bs}");   
        }

        [Callback]
        internal void OnOutcomingRPC(int playerid, int rpcid, int bs)
        {
            OutcomingRpc?.Invoke(this, new PacketRpcEventArgs(rpcid, playerid, bs));
            if (LoggingOutcomingRpc) Console.WriteLine($"[SampSharp.RakNet] Hooking Outcoming RPC {playerid}, {rpcid}, {bs}");
        }
        [Callback]
        internal void OnIncomingPacket(int playerid, int packetid, int bs)
        {
            IncomingPacket?.Invoke(this, new PacketRpcEventArgs(packetid, playerid, bs));
            if (LoggingIncomingPacket) Console.WriteLine($"[SampSharp.RakNet] Hooking Incoming Packet {playerid}, {packetid}, {bs}");
        }
        [Callback]
        internal void OnOutcomingPacket(int playerid, int packetid, int bs)
        {
            OutcomingPacket?.Invoke(this, new PacketRpcEventArgs(packetid, playerid, bs));
            if (LoggingOutcomingPacket) Console.WriteLine($"[SampSharp.RakNet] Hooking Outcoming Packet {playerid}, {packetid}, {bs}");
        }
    }
}
