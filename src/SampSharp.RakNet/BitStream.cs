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
using System.Collections.Generic;

using SampSharp.RakNet.Events;
using SampSharp.RakNet.Definitions;

namespace SampSharp.RakNet
{
    public partial class BitStream : IDisposable
    {
        public int Id { get; private set; }
        public bool IsHandMade{ get; private set; }
        public BitStream(int id, bool isHandMade = false)
        {
            if(id == 0)
            {
                Console.WriteLine("[SampSharp.RakNet][Warning] Trying to create BitStream with id(handle) = 0");
            }
            IsHandMade = isHandMade;
            Id = id;
        }
        public bool IsEmptyHandle()
        {
            return Id == 0;
        }
        public void Reset()
        {
            Internal.BS_Reset(Id);
        }
        public void ResetReadPointer()
        {
            Internal.BS_ResetReadPointer(Id);
        }
        public void ResetWritePointer()
        {
            Internal.BS_ResetWritePointer(Id);
        }
        public void IgnoreBits(int number)
        {
            Internal.BS_IgnoreBits(Id, number);
        }
        public int WriteOffset
        {
            get
            {
                Internal.BS_GetWriteOffset(Id, out int offset);
                return offset;
            }
            set
            {
                Internal.BS_SetWriteOffset(Id, value);
            }
        }
        public int ReadOffset
        {
            get
            {
                Internal.BS_GetReadOffset(Id, out int offset);
                return offset;
            }
            set
            {
                Internal.BS_SetReadOffset(Id, value);
            }
        }

        public int NumberOfBitsUsed
        {
            get
            {
                Internal.BS_GetNumberOfBitsUsed(Id, out int number);
                return number;
            }
        }
        public int NumberOfBytesUsed
        {
            get
            {
                Internal.BS_GetNumberOfBytesUsed(Id, out int number);
                return number;
            }
        }
        public int NumberOfUnreadBits
        {
            get
            {
                Internal.BS_GetNumberOfUnreadBits(Id, out int number);
                return number;
            }
        }
        public int NumberOfBitsAllocated
        {
            get
            {
                Internal.BS_GetNumberOfBitsAllocated(Id, out int number);
                return number;
            }
        }
        public void WriteValue(params object[] arguments)
        {
            Internal.BS_WriteValue(Id, arguments);
        }
        public Dictionary<string, object> ReadValue(params object[] arguments)
        {
            var values = Internal.BS_ReadValue(Id, arguments);
            return values;
        }

        public void SendRpc(int rpcId, int playerId, PacketPriority priority = PacketPriority.HighPriority, PacketReliability reliability = PacketReliability.ReliableOrdered)
        {
            var result = Internal.BS_RPC(Id, playerId, rpcId, (int)priority, (int)reliability);
        }
        public void SendPacket(int playerId, PacketPriority priority = PacketPriority.HighPriority, PacketReliability reliability = PacketReliability.ReliableOrdered)
        {
            var result = Internal.BS_Send(Id, playerId, (int)priority, (int)reliability);
        }

        public void Dispose()
        {
            if (IsHandMade)
            {
                Internal.BS_Delete(out int id);
                Id = id;
            }
        }
        public static BitStream Create()
        {
            int id = Internal.BS_New();
            return new BitStream(id, true);
        }
    }
}
