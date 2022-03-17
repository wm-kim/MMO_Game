using Server;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


class PacketHandler
{
    // 어떤 Session에서 조립이 되었고 어떤 packet인지
    public static void C_LeaveGameHandler(PacketSession session, IPacket packet)
    {
        ClientSession clientSession = session as ClientSession;

        if (clientSession.Room == null) return;

        GameRoom room = clientSession.Room;
        room.Push(() => room.Leave(clientSession));
    }

    public static void C_MoveHandler(PacketSession session, IPacket packet)
    {
        C_Move movePacket = packet as C_Move;
        ClientSession clientSession = session as ClientSession;

        if (clientSession.Room == null) return;

        // Console.WriteLine($"{movePacket.posX}, {movePacket.posY}, {movePacket.posZ}");

        GameRoom room = clientSession.Room;
        room.Push(() => room.Move(clientSession, movePacket));
    }

}

