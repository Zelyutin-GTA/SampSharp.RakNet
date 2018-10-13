#include "a_samp.inc"
#include "Pawn.Raknet.inc"

new bool:blockNextRPC = false;
new bool:blockNextPacket = false;
public OnPlayerConnect(playerid)
{
	CallRemoteFunction("OnTestRemote", "d", playerid);
}
public OnIncomingRPC(playerid, rpcid, BitStream:bs)
{
	printf("[Pawn] Incoming RPC. %d, %d, %d", playerid, rpcid, bs);

	if(blockNextRPC)
	{
		print("[Pawn] RPC blocked");
		blockNextRPC = false;
		return 0;
	}
	return 1;
}
public OnOutcomingRPC(playerid, rpcid, BitStream:bs)
{
	printf("[Pawn] Outcoming RPC. %d, %d, %d", playerid, rpcid, bs);

	if(blockNextRPC)
	{
		print("[Pawn] RPC blocked");
		blockNextRPC = false;
		return 0;
	}
	return 1;
}
public OnIncomingPacket(playerid, packetid, BitStream:bs)
{
	printf("[Pawn] Incoming Packet. %d, %d, %d", playerid, packetid, bs);

	if(blockNextPacket)
	{
		print("[Pawn] Packet blocked");
		blockNextPacket = false;
		return 0;
	}
	return 1;
}
public OnOutcomingPacket(playerid, packetid, BitStream:bs)
{
	printf("[Pawn] Outcoming Packet. %d, %d, %d", playerid, packetid, bs);

	if(blockNextPacket)
	{
		print("[Pawn] Packet blocked");
		blockNextPacket = false;
		return 0;
	}
	return 1;
}
forward BlockNextRPC();
public BlockNextRPC()
{
	blockNextRPC = true;
	printf("[Pawn] Blocking Next RPC");
}
forward BlockNextPacket();
public BlockNextPacket()
{
	blockNextPacket = true;
	printf("[Pawn] Blocking Next Packet");
}
public OnPlayerCommandText(playerid, cmdtext[])
{
    if(!strcmp(cmdtext, "/changenamepawnrpc", true))
    {
        new BitStream:bs = BS_New();

        new name[MAX_PLAYER_NAME] = "Change_Pawn";
	    BS_WriteValue(
	        bs,
	        PR_UINT16, playerid,
	        PR_UINT8, strlen(name), // Size should be exactly the same
	        PR_STRING, name
	    );

	    BS_RPC(bs, playerid, 11);

	    //BS_Delete(bs);
        return 1;
    }
    return 0;
}