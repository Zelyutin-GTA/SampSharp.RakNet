# SampSharp.RakNet
Wrapper of the Pawn.RakNet plugin for [SampSharp]

Installation
-----
1. Add dependency with NuGet
2. Copy Initialization from TestMode.GameMode to your GameMode class
3. Include SampSharp.RakNet.inc into your Pawn GameMode (.pwn). **RakNet callbacks handling won't work without it.**
4. You will also need to add Pawn.RakNet.inc to the directory near SampSharp.RakNet.inc. You can find it at [Pawn.RakNet] releases page.
5. Compile your .pwn.

[SampSharp]:https://github.com/ikkentim/SampSharp/
[Pawn.RakNet]:https://github.com/urShadow/Pawn.RakNet/releases
