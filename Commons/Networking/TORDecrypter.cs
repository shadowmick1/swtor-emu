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
using System.Runtime.InteropServices;

namespace Commons.Networking
{
    public class TORDecrypter
    {
        [DllImport("TORCrypto.dll", EntryPoint = "TCipherCreate", SetLastError = true)]
        static extern IntPtr TCipherCreate();
        [DllImport("TORCrypto.dll", EntryPoint = "TCipherLoadKeys", SetLastError = true)]
        static extern void TCipherLoadKeys(IntPtr cipher, bool login);
        [DllImport("TORCrypto.dll", EntryPoint = "TCipherDecryptSetKeys", SetLastError = true)]
        static extern void TCipherDecryptSetKeys(IntPtr cipher);

        [DllImport("TORCrypto.dll", EntryPoint = "TCipherDecryptPutRaw", SetLastError = true)]
        static extern void TCipherDecryptPutRaw(IntPtr cipher, bool client, byte[] data, uint data_len);
        [DllImport("TORCrypto.dll", EntryPoint = "TCipherDecryptGetSize", SetLastError = true)]
        static extern int TCipherDecryptGetSize(IntPtr cipher, bool client);
        [DllImport("TORCrypto.dll", EntryPoint = "TCipherDecryptGetPacket", SetLastError = true)]
        static extern int TCipherDecryptGetPacket(IntPtr cipher, bool client, byte[] data, uint data_len);

        [DllImport("TORCrypto.dll", EntryPoint = "RSADecrypt", SetLastError = true)]
        public static extern int RSADecrypt(byte[] rsaString, byte[] destination, ref uint dstSize);

        byte _type;
        IntPtr cipher;

        public TORDecrypter(byte type)
        {
            _type = type;
            cipher = TCipherCreate();
        }

        public void LoadKeys()
        {
            TCipherLoadKeys(cipher, _type == 0);
            TCipherDecryptSetKeys(cipher);
        }

        static object mutex = new object();

        public List<byte[]> Decrypt(byte[] data, bool client)
        {
            lock(mutex)
            {
                List<byte[]> packets = new List<byte[]>();
                TCipherDecryptPutRaw(cipher, client, data, (uint)data.Length);
                int packetSize = 0;
                while (true)
                {
                    packetSize = TCipherDecryptGetSize(cipher, client);

                    if (packetSize == -1) break;

                    if (packetSize < 0)
                    {
                        Console.WriteLine("ERROR: DecryptGetSize returned " + packetSize);
                        break;
                    }

                    byte[] packet = new byte[packetSize];
                    int res = TCipherDecryptGetPacket(cipher, client, packet, (uint)packetSize);
                    if (res == 0)
                    {
                        packets.Add(packet);
                    }
                }
                return packets;
                }
        }

    }
}
