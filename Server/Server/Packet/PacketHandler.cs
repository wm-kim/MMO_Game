using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server;
using Server.Game;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

// handler 쪽은 항상 red zone
class PacketHandler
{
	// client의 CreatureController에서 dir, state, cellpos 셋중하나에 변경하면 C_Move 보냄
	public static void C_MoveHandler(PacketSession session, IMessage packet)
	{
		C_Move movePacket = packet as C_Move;
		ClientSession clientSession = session as ClientSession;

		Console.WriteLine($"C_Move ({movePacket.PosInfo.PosX}, {movePacket.PosInfo.PosY})");

		// Player가 입장해 있는 방을 찾아서 방에 있는 모든 player들에게 broadcasting
		// if (clientSession.MyPlayer == null) return;
		// 누군가 다른 Thread에서 MyPlayer를 null로 바꿀 수 있음

		Player player = clientSession.MyPlayer;
		if (player == null) return;

		// if (clientSession.MyPlayer.Room == null) return;

		GameRoom room = player.Room;
		if(room == null) return;

		// 경합조건 발생. 한쪽에 몰아서 lock으로 처리
		room.HandleMove(player, movePacket);
	}

	// 
	public static void C_SkillHandler(PacketSession session, IMessage packet)
    {
		C_Skill skillPacket = packet as C_Skill;
		ClientSession clientSession = session as ClientSession;

		Player player = clientSession.MyPlayer;
		if (player == null) return;

		GameRoom room = player.Room;
		if (room == null) return;

		// skill id에 따라 처리
		room.HandleSkill(player, skillPacket);
	}
}
