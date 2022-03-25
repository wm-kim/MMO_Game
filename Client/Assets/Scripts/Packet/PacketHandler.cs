using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class PacketHandler
{
	public static void S_EnterGameHandler(PacketSession session, IMessage packet)
	{
		S_EnterGame enterGamePacket = packet as S_EnterGame;
		// custumhandler로 인해서 PacketQueue에 넣어넣고 networkmanager에서 실행됨
		// MyPlayer 생성
        Managers.Object.Add(enterGamePacket.Player, myPlayer: true);
	}

	public static void S_LeaveGameHandler(PacketSession session, IMessage packet)
	{
		S_LeaveGame leaveGameHandler = packet as S_LeaveGame;

		Managers.Object.RemoveMyPlayer();
	}
	public static void S_SpawnHandler(PacketSession session, IMessage packet)
	{
		S_Spawn spawnPacket = packet as S_Spawn;
		
		foreach(ObjectInfo obj in spawnPacket.Objects)
        {
			Managers.Object.Add(obj, myPlayer: false);
		}
	}

	public static void S_DespawnHandler(PacketSession session, IMessage packet)
	{
		S_Despawn despawnPacket = packet as S_Despawn;

		foreach (int id in despawnPacket.ObjectIds)
		{
			Managers.Object.Remove(id);
		}
	}
	public static void S_MoveHandler(PacketSession session, IMessage packet)
	{
		S_Move movePacket = packet as S_Move;
		ServerSession serverSession = session as ServerSession;

		GameObject go = Managers.Object.FindById(movePacket.ObjectId);
		if (go == null) return;

		CreatureController cc = go.GetComponent<CreatureController>();
		if (cc == null) return;

		// state,dir,posx,y UpdateAnimation 자동호출
		// Broadcast에서 자기자신도 S_Move를 받는중
		// 일반적으로 myplayer은 client가 담당하여 이동하므로 덮어쓰지 않는다.

		cc.PosInfo = movePacket.PosInfo;

		// dir를 이용하여 client 쪽에서 알아서 이동하도록 처리하고 있다.
		// 때문에 이동하다가 말고 이동 못하는 쪽으로 꺾으면 
		// State는 여전히 Moving State이고 CellPos도 바뀌지 않으므로
		// 갱신패킷을 Server에 안보내지 않는다. 결과 바라 보던 방향으로 계속 이동한다.
		// moveToNextPos를 CreatureController에서 MyPlayerConroller로 이전함
	}

	public static void S_SkillHandler(PacketSession session, IMessage packet)
	{
		S_Skill skillPacket = packet as S_Skill;
		ServerSession serverSession = session as ServerSession;

		GameObject go = Managers.Object.FindById(skillPacket.ObjectId);
		if (go == null) return;

		PlayerController pc = go.GetComponent<PlayerController>();
		if (pc != null)
        {
			pc.UseSkill(skillPacket.Info.SkillId);
        }
	}
}
