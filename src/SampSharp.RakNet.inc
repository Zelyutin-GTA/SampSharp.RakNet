#include "Pawn.Raknet.inc"

new bool:blockNextRPC = false;
new bool:blockNextPacket = false;

public OnIncomingRPC(playerid, rpcid, BitStream:bs)
{
	if(blockNextRPC)
	{
		//RPC blocked
		blockNextRPC = false;
		return 0;
	}
	return 1;
}
public OnOutcomingRPC(playerid, rpcid, BitStream:bs)
{
	if(blockNextRPC)
	{
		//RPC blocked
		blockNextRPC = false;
		return 0;
	}
	return 1;
}
public OnIncomingPacket(playerid, packetid, BitStream:bs)
{
	if(blockNextPacket)
	{
		//Packet blocked
		blockNextPacket = false;
		return 0;
	}
	return 1;
}
public OnOutcomingPacket(playerid, packetid, BitStream:bs)
{
	if(blockNextPacket)
	{
		//Packet blocked
		blockNextPacket = false;
		return 0;
	}
	return 1;
}
forward BlockNextRPC();
public BlockNextRPC()
{
	blockNextRPC = true;
}
forward BlockNextPacket();
public BlockNextPacket()
{
	blockNextPacket = true;
}