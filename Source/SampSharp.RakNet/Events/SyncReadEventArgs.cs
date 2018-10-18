using System;

using SampSharp.RakNet.Syncs;

namespace SampSharp.RakNet.Events
{
    public class SyncReadEventArgs : EventArgs
    {
        public ISync Sync { get; private set; }
        public SyncReadEventArgs(ISync sync)
        {
            Sync = sync;
        }
    }
}