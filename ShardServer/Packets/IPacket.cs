/*
* Copyright (C) 2008-2012 Emulator Nexus <http://emulatornexus.com//>
*
* This program is free software; you can redistribute it and/or modify it
* under the terms of the GNU General Public License as published by the
* Free Software Foundation; either version 3 of the License, or (at your
* option) any later version.
*
* This program is distributed in the hope that it will be useful, but WITHOUT
* ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
* FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for
* more details.
*
* You should have received a copy of the GNU General Public License along
* with this program. If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commons;
using Commons.Networking;
using System.Threading;

namespace ShardServer.Packets
{
    public interface IPacket
    {
    }

    public interface IClientPacket : IPacket
    {
        void ExecutePacket(AsyncConnection connection, ByteBuffer packet);
    }

    public interface IServerPacket : IPacket
    {
        void WritePacket(AsyncConnection connection, ByteBuffer packet);
    }

    public static class AsyncConnectionExtensions
    {
        public static void SendPacket(this AsyncConnection con, IServerPacket pkt)
        {
            byte opcode = OpcodeManager.Instance.GetOpcode(pkt.GetType().Name);
            uint packetid = OpcodeManager.Instance.GetPacketID(pkt.GetType().Name);
            if (packetid == 0)
            {
                TORLog.Error("ERROR: No PacketID defined for " + pkt.GetType().Name);
                return;
            }
            ByteBuffer packet = new ByteBuffer(ByteOrder.LittleEndian);
            packet.WriteByte(opcode);
            packet.WriteInt(0); // Length
            packet.WriteByte(0); // ChkByte
            packet.WriteUInt(packetid);
            pkt.WritePacket(con, packet);
            con.SendTORPacket(packet);
            TORLog.Network("PktSend @ " + con.GetHashCode() + " >> " + pkt.GetType().Name);
        }
    }

}
