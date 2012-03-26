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
using ShardServer.Packets.Server;
using ShardServer.TORBusiness.Model;
using ShardServer.TORBusiness.Data;

namespace ShardServer.Packets.Client
{
    public class CMsg_CharacterListRequest : IClientPacket
    {
        void IClientPacket.ExecutePacket(AsyncConnection connection, ByteBuffer packet)
        {
            uint objid = packet.ReadObjectIDRev();
            TORLog.Info("CharacterListRequest@" + objid);
            connection.SendPacket(new SMsg_CharacterListResponse(objid));
        }
    }

    public class CMsg_CharacterCreateRequest : IClientPacket
    {
        static byte[] lastpacket;
        void IClientPacket.ExecutePacket(AsyncConnection connection, ByteBuffer packet)
        {
            uint id = packet.ReadObjectIDRev();
            int namelen = packet.ReadInt();
            string name = "";
            for (int i = 0; i < namelen; i++)
            {
                name += (char)packet.ReadByte();
            }
            packet.ReadInt(); packet.ReadInt();
            packet.ReadInt();

            uint CHARSPEC1 = packet.ReadUInt();
            byte CHARSPEC2 = (byte)packet.ReadByte();

            packet.ReadBytes(36);

            uint CHARSPECAPP1 = packet.ReadUInt();
            uint CHARSPECAPP2 = packet.ReadUInt();
            uint CHARSPECAPP3 = packet.ReadUInt();
            ushort CHARSPECAPP4 = packet.ReadUShort();

            packet.ReadByte(); // separator

            byte[] classBytes = packet.ReadBytes(8);
            Array.Reverse(classBytes);
            ulong classNodeRefId = BitConverter.ToUInt64(classBytes, 0);

            Console.WriteLine("----- Creating Character -----");
            Console.WriteLine("Name = " + name);
            Console.WriteLine("Class ID = " + classNodeRefId);
            Console.WriteLine("App1 = " + CHARSPECAPP1);
            Console.WriteLine("App2 = " + CHARSPECAPP2);
            Console.WriteLine("App3 = " + CHARSPECAPP3);
            Console.WriteLine("App4 = " + CHARSPECAPP4);

            /*byte[] known_offs = new byte[] { 84, 88, 91, 92, 95, 96, 100, 104, 107, 108, 112 };
            long originalPositon = packet.Position;
            byte[] thispkt = packet.ReadBytes((int)(packet.Length - packet.Position));
            packet.Position = originalPositon;

            if (lastpacket != null)
            {      

                for (byte i = 0; i < thispkt.Length; i++)
                {
                    if (lastpacket[i] != thispkt[i] && !known_offs.Contains<byte>(i))
                        Console.WriteLine("Data differs at " + i + " : " + lastpacket[i] + " => " + thispkt[i]);
                }
            }

            lastpacket = thispkt;*/

            Character c = new Character()
            {
                //APP1 = CHARSPECAPP1,
                //APP2 = CHARSPECAPP2,
                //APP3 = CHARSPECAPP3,
                //APP4 = CHARSPECAPP4,
                AreaSpec = TORBusiness.Data.MapAreas.PCShip_XSFreighter,
                Class = (CharacterClass)classNodeRefId,
                Level = 44,
                Name = name,
                spec2 = CHARSPEC1,
                spec3 = CHARSPEC2,
            };

            packet.ReadBytes(78);
            packet.ReadByte();
            byte appDataCount = (byte)packet.ReadByte();
            c.Appearance = new Tuple<byte, byte>[appDataCount];
            packet.ReadByte();
            Console.WriteLine("appearance entries: " + appDataCount);
            for (int i = 0; i < appDataCount; i++)
            {
                byte[] data = packet.ReadBytes(4);
                c.Appearance[i] = new Tuple<byte, byte>(data[0], data[3]);
                Console.WriteLine("Appearance = " + data[3]);
            }

            Program.LastCreatedChar = c;

            connection.SendPacket(new SMsg_CharacterCreateResponse(id, name, 0x07));

        }
    }

    public class CMsg_CharacterSelectRequest : IClientPacket
    {
        void IClientPacket.ExecutePacket(AsyncConnection connection, ByteBuffer packet)
        {
            uint objid = packet.ReadObjectIDRev();
            packet.ReadByte();
            uint charid = packet.ReadUInt();
            TORLog.Info("CharacterSelectRequest@" + objid + " - " + charid);
            connection.SendPacket(new SMsg_CharacterSelectResponse(objid));
            connection.SendPacket(new SMsg_CharacterCurrentMap(objid, "ord_main", "4611686019802843831"));
            connection.SendPacket(new SMsg_CharacterAreaEnter(objid));
            connection.SendPacket(new SMsg_CharacterAreaServerSpec(objid, "ord_main", "4611686019802843831"));
        }
    }
    public class CMsg_ProcessesResponse : IClientPacket
    {
        void IClientPacket.ExecutePacket(AsyncConnection connection, ByteBuffer packet)
        {

        }
    }
    public class CMsg_C26464A9 : IClientPacket
    {
        void IClientPacket.ExecutePacket(AsyncConnection connection, ByteBuffer packet)
        {
            //uint objid = packet.ReadObjectIDRev();
            //packet.ReadByte();
            //TORLog.Info("@unknown Object" + objid);
            //connection.SendPacket(new SMsg_CharacterSelectResponse(objid));
        }
    }

    public class CMsg_0C0BA34F : IClientPacket
    {
        void IClientPacket.ExecutePacket(AsyncConnection connection, ByteBuffer packet)
        {

        }
    }

    public class CMsg_D0D38F43 : IClientPacket
    {
        void IClientPacket.ExecutePacket(AsyncConnection connection, ByteBuffer packet)
        {

        }
    }
}
