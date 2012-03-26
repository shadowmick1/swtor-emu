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
using System.Net;
using Commons;
using Commons.Networking;
using ShardServer.Packets;
using ShardServer.Packets.Server;
using ShardServer.TORBusiness.Model;

namespace ShardServer
{
    class Program
    {
        public static Character LastCreatedChar;

        static void Main(string[] args)
        {
            Console.Title = "Nexus Shard Server";

            Console.WriteLine("Nexus Shard Server\n");
            Utility.WriteLegal();
            Console.WriteLine("");

            AsyncServer server = new AsyncServer(IPAddress.Parse("0.0.0.0"), 20063);
            server.Start();

            server.ConnectionAccepted += new AsyncServer.ConnectionAcceptedHandler(connection =>
            {
                TORLog.Network("CnxAccept => " + connection.GetHashCode());
                connection.DataReceived += new AsyncConnection.ConnectionDataReceivedHandler(data =>
                {
                    if (connection.State == 1)
                    {
                        // rsa packet
                        connection.SetState(2);
                        connection.SendPacket(new SMsg_ClientSignatureRequest("OmegaServerProxyObjectName", 0x0174F5));
                        return;
                    }

                    ByteBuffer buffer = new ByteBuffer(ByteOrder.LittleEndian, data);
                    byte opcode = (byte)buffer.ReadByte();

                    byte[] len_data = buffer.ReadBytes(4);
                    byte chk = (byte)buffer.ReadByte();
                    uint packetid = buffer.ReadUInt();

                    if (chk != (byte)(opcode ^ len_data[0] ^ len_data[1] ^ len_data[2] ^ len_data[3]))
                    {
                        TORLog.Warn("Received packet with invalid checksum!");
                    }

                    if (opcode == 1) // Client Ping Packet
                    {
                        TORLog.Network("Ping from " + connection.GetHashCode() + " Seq = " + packetid);
                        ByteBuffer response = new ByteBuffer(ByteOrder.LittleEndian);
                        response.WriteByte(2); // Pong OPC = 2
                        response.WriteInt(0); // Pong Len
                        response.WriteByte(0); // Pong Chk
                        response.WriteUInt(packetid); // Pong
                        response.WriteInt(0);
                        response.WriteInt(1);
                        response.WriteByte(0);
                        connection.SendTORPacket(response);
                        return;
                    }

                    string packetname = OpcodeManager.Instance.GetPacketName(opcode, packetid);

                    if (packetname == "")
                    {
                        TORLog.Warn("Received unknown packet from SWTOR client\nOpcode = 0x" + opcode.ToString("X2") + " -- PacketID = 0x" + packetid.ToString("X8"));
                        TORLog.Warn("--- dump ---");
                        TORLog.Warn(Utility.HexDump(data));
                    }
                    else
                    {
                        try
                        {
                            IClientPacket pkt = Activator.CreateInstance(Type.GetType("ShardServer.Packets.Client." + packetname)) as IClientPacket;
                            TORLog.Network("PktRecv @ " + connection.GetHashCode() + " << " + packetname);
                            pkt.ExecutePacket(connection, buffer);
                        }
                        catch (Exception ex)
                        {
                            TORLog.Error("Exception occured while processing " + packetname, ex);
                        }
                    }
                });
                // Send HELLO packet
                connection.SendClear(new byte[] { 0x03, 0x0e, 0x00, 0x00, 0x00, 0x0d, 0x11, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
                // State is 1
                connection.SetState(1);
                connection.EngageReading();
            });

            TORLog.Info("SHARD Server running, press Enter to exit ...");
            Console.ReadLine();
        }
    }
}