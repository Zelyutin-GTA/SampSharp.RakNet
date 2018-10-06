using System;

using SampSharp.Core;

namespace ASGameMode
{
    class Program
    {
        static void Main(string[] args)
        {
            new GameModeBuilder()
            //.UseLogLevel(SampSharp.Core.Logging.CoreLogLevel.Debug)
            .Use<GameMode>()
            .Run();
        }
    }
}
