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
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;

namespace Commons.Networking
{
    public class TORCipheredStream : NetworkStream
    {
        [DllImport("TORCrypto.dll", EntryPoint = "TCryptoInteropWrapperCreate", SetLastError = true)]
        static extern IntPtr TCryptoInteropWrapperCreate(bool type);
        [DllImport("TORCrypto.dll", EntryPoint = "TCryptoInteropWrapperLoadKeys", SetLastError = true)]
        static extern IntPtr TCryptoInteropWrapperLoadKeys(IntPtr wrapper, byte[] key1, byte[] iv1, byte[] key2, byte[] iv2);
        [DllImport("TORCrypto.dll", EntryPoint = "TCryptoProcessClient", SetLastError = true)]
        static extern int TCryptoProcessClient(IntPtr wrapper, byte[] data, uint data_len, byte[] buffer);
        [DllImport("TORCrypto.dll", EntryPoint = "TCryptoProcessServer", SetLastError = true)]
        static extern int TCryptoProcessServer(IntPtr wrapper, byte[] data, uint data_len, byte[] buffer);

        Socket _clientSocket;
        IntPtr tCipherPtr;

        bool _encrypted = false;
        object mutex = new object();

        public TORCipheredStream(Socket clientSocket, bool type) : base(clientSocket)
        {
            _clientSocket = clientSocket;
            tCipherPtr = TCryptoInteropWrapperCreate(type);
        }

        public void EnableEncryption(byte[] key1, byte[] iv1, byte[] key2, byte[] iv2)
        {
            lock(mutex)
            {
                TCryptoInteropWrapperLoadKeys(tCipherPtr, key1, iv1, key2, iv2);
                Console.WriteLine("Keys loaded.");
                _encrypted = true;
            }
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int size, AsyncCallback callback, object state)
        {
            return base.BeginRead(buffer, offset, size, callback, new ObjectContainer() { obj = buffer });
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            lock (mutex)
            {
                // End Read data from socket
                int socketRead = base.EndRead(asyncResult);
                // check if disconnection
                if (socketRead <= 0)
                    return socketRead;
                byte[] buffer = (asyncResult.AsyncState as ObjectContainer).obj as byte[];
                // decrypt the buffer, in place
                List<byte[]> packets = new List<byte[]>();
                if (!_encrypted)
                {
                    Array.Resize<byte>(ref buffer, socketRead);
                    packets.Add(buffer);
                    (asyncResult.AsyncState as ObjectContainer).obj = packets;
                    return 1;
                }
                
                byte[] decrypted = new byte[socketRead * 2];
                int decryptedSize = TCryptoProcessClient(tCipherPtr, buffer, (uint)socketRead, decrypted);
                Array.Resize<byte>(ref decrypted, decryptedSize);
                
                Console.WriteLine(Utility.HexDump(decrypted));

                packets.Add(decrypted);

                (asyncResult.AsyncState as ObjectContainer).obj = packets;
                return 1;
            }
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int size, AsyncCallback callback, object state)
        {
            lock (mutex)
            {
                Console.WriteLine("sending packet " + size + " bytes");
                byte[] compressed = new byte[buffer.Length + 6];
                int newsize = TCryptoProcessServer(tCipherPtr, buffer, (uint)size, compressed);
                Array.Resize<byte>(ref compressed, newsize);
                Console.WriteLine("compressed size = " + newsize);
                Console.WriteLine(Utility.HexDump(compressed));                
                return base.BeginWrite(compressed, 0, compressed.Length, callback, state);
            }
        }

    }

    public class ObjectContainer
    {
        public object obj;
    }

}
