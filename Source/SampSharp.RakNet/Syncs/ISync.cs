using System;
using SampSharp.RakNet.Events;

namespace SampSharp.RakNet.Syncs
{
    public interface ISync
    {
        event EventHandler<SyncReadEventArgs> ReadCompleted;

        void ReadIncoming();
        void ReadOutcoming();
    }
}
