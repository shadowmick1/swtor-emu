using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commons.Networking;
using System.Runtime.InteropServices;
using Commons;
using System.IO;
using Cryptography;
using System.Globalization;

namespace ProxyServer
{
    public class PacketProcessor
    {
        
        [DllImport("TORCrypto.dll", EntryPoint = "TRSAInit", SetLastError = true)]
        public static extern void TRSAInit();
        [DllImport("TORCrypto.dll", EntryPoint = "TRSADecrypt", SetLastError = true)]
        static extern IntPtr TRSADecrypt(byte[] data, uint data_len, ref uint resultSize);

        public static void ProcessPacket(byte[] data, AsyncConnection connection)
        {
            AsyncConnectionData acd = (connection.AsyncState as AsyncConnectionData);
            ByteBuffer buffer = new ByteBuffer(ByteOrder.LittleEndian, data, 6, data.Length - 6); // 1 opcode 4 len 1 chk
            if (connection.State == 1)
            {
                buffer.ReadInt();
                string rsaBytes = Encoding.UTF8.GetString(buffer.ReadBytes(1024));
                byte[] rsa = new byte[512];
                for (int i = 0; i < rsaBytes.Length; i += 2)
                {
                    rsa[i / 2] = byte.Parse(rsaBytes.Substring(i, 2), NumberStyles.HexNumber);
                }
                uint resultSize = 0;
                IntPtr ptr = TRSADecrypt(rsa, 512, ref resultSize);
                byte[] decrypted = new byte[resultSize];
                Marshal.Copy(ptr, decrypted, 0, (int)resultSize);
                Console.WriteLine("Decrypted Size = " + resultSize);
                Console.WriteLine(Utility.HexDump(decrypted));

                // set salsa keys
                ByteBuffer packetbuffer = new ByteBuffer(ByteOrder.LittleEndian, decrypted);
                string username = packetbuffer.ReadString();
                string hash = packetbuffer.ReadString();

                byte[] key1 = packetbuffer.ReadBytes(32);
                byte[] key2 = packetbuffer.ReadBytes(32); // decrypts client
                byte[] iv1 = packetbuffer.ReadBytes(8);
                byte[] iv2 = packetbuffer.ReadBytes(8); // decrypts client

                connection.CipheredStream.EnableEncryption(key2, iv2, key1, iv1);

                connection.SetState(2); // State 2 = Ready to decrypt
            }
            else
            {
                switch(data.Length)
                {
                    case 14: // discarded client packet
                        break;
                    case 45:
                        byte[] response_45 = new byte[]
                            {
                                0x00, 0x48, 0x00, 0x00, 0x00, 0x48, 0xAF, 0xC5, 0x31, 0x67, 0xFF, 0xFF, 0xFF, 0xFF, 0x04, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x63, 0x61, 
                                0x73, 0x74, 0x6C, 0x65, 0x68, 0x69, 0x6C, 0x6C, 0x74, 0x65, 0x73, 0x74, 0x00, 0x09, 0x00, 0x00, 
                                0x00, 0x61, 0x66, 0x63, 0x31, 0x62, 0x62, 0x35, 0x61, 0x00, 0x0C, 0x00, 0x00, 0x00, 0x6C, 0x6F, 
                                0x67, 0x69, 0x6E, 0x73, 0x65, 0x72, 0x76, 0x65, 0x72, 0x00, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 
                                0x00, 0x00, 
                            };
                        connection.SendRegularPacket(response_45);
                        break;
                    case 126: //omega packet
                        ByteBuffer pkt_omega = new ByteBuffer(ByteOrder.LittleEndian);
                        pkt_omega.WriteBytes(new byte[] {
                            0x00, 0x60, // Length
                            0x00, 0x00, 0x00, 0x60, // Length
                            0x84, 0xD0, 0xF2, 0x90, // PacketID (with client hack)
                            0x04, 0x00, 0x00, 0x00 // ObjectCode
                        });
                        pkt_omega.WriteString("swtor-game-lab-1.swtor.com:20063"); // Server Address
                        pkt_omega.WriteString("WIKRQEOYULPBIEHHADRWAAPNVRYGQHMNRXGHBUIV"); // Some key ?

                        Console.WriteLine(Utility.HexDump(pkt_omega.ToArray()));

                        connection.SendRegularPacket(pkt_omega.ToArray());
                        break;
                    default:
                        Console.WriteLine("recv packet, size = " + data.Length);
                        Console.WriteLine(Utility.HexDump(data));
                        break;
                }

                /*List<byte[]> packets = acd.Crypter.ProcessServerFromClient(data);

                foreach (byte[] packet in packets)
                {
                    ByteBuffer pkt = new ByteBuffer(ByteOrder.BigEndian, packet);
                    short opcode = pkt.ReadShort();
                    int length = pkt.ReadInt();

                    switch (opcode)
                    {
                        case 45:
                            // Client Selecting a Shard Server
                            Console.WriteLine(connection.GetHashCode() + " selecting shard.");
                            MemoryStream response_45 = new MemoryStream(new byte[]
                            {
                                0x00, 0x48, 0x0, 0x00, 0x00, 0x48, 0xAF, 0xC5, 0x31, 0x67, 0xFF, 0xFF, 0xFF, 0xFF, 0x04, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x63, 0x61, 
                                0x73, 0x74, 0x6C, 0x65, 0x68, 0x69, 0x6C, 0x6C, 0x74, 0x65, 0x73, 0x74, 0x00, 0x09, 0x00, 0x00, 
                                0x00, 0x61, 0x66, 0x63, 0x31, 0x62, 0x62, 0x35, 0x61, 0x00, 0x0C, 0x00, 0x00, 0x00, 0x6C, 0x6F, 
                                0x67, 0x69, 0x6E, 0x73, 0x65, 0x72, 0x76, 0x65, 0x72, 0x00, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 
                                0x00, 0x00, 
                            });
                            SendPacket(connection, response_45);
                            break;
                        default:
                            Console.WriteLine("PKTRECV Opcode = " + opcode + " Size = " + length);
                            Console.WriteLine("Hex Dump = " + Utility.ToHexString(pkt, true));
                            Console.WriteLine("String Dump = " + Encoding.UTF8.GetString(pkt.ToArray()));
                            break;
                    }
                }*/
            }            
        }

    }

    public class AsyncConnectionData
    {
        public TORCrypter Crypter;
    }

}
