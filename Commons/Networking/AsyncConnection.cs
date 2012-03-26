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
using System.Net;

namespace Commons.Networking
{
    public class AsyncConnection
    {
        Socket clientSocket;
        int _state = 0;
        public int State
        {
            get { return _state; }
        }
        public readonly TORCipheredStream CipheredStream;

        byte[] receiveBuffer = new byte[8192];

        public delegate void ConnectionDataReceivedHandler(byte[] data);

        public event ConnectionDataReceivedHandler DataReceived;

        public object AsyncState;

        public AsyncConnection(Socket socket)
        {
            clientSocket = socket;
            CipheredStream = new TORCipheredStream(socket, true);
        }

        public void SetState(int state)
        {
            _state = state;
        }

        public void EngageReading()
        {
            BeginRead();
        }

        public void Close()
        {
            clientSocket.Close();
        }

        void BeginRead()
        {
            if(clientSocket != null && clientSocket.Connected)
                CipheredStream.BeginRead(receiveBuffer, 0, 8192, new AsyncCallback(EndRead), null);
        }

        void EndRead(IAsyncResult ar)
        {
            if (clientSocket == null || !clientSocket.Connected)
                return;
            int recv = CipheredStream.EndRead(ar);
            if (recv <= 0)
            {
                Console.WriteLine("CnxDisconnect: " + GetHashCode());
                return;
            }
            List<byte[]> packets = (ar.AsyncState as ObjectContainer).obj as List<byte[]>;
            foreach(byte[] packet in packets)
            {
                if (DataReceived != null)
                {
                    DataReceived(packet);
                }
            }
            BeginRead();
        }

        public void SendClear(byte[] data)
        {
            clientSocket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback((ar) =>
                {
                    int sent = clientSocket.EndSend(ar);
                    Console.WriteLine("Sent " + sent + " unencrypted bytes.");
                }), null);
        }

        public void SendTORPacket(ByteBuffer buffer)
        {
            // here we receive a buffer prepared for length insertion
            int length = (int)buffer.Position;
            buffer.Position = 1;
            buffer.WriteInt(length);

            byte[] data = buffer.ToArray();
            byte chk = (byte)(data[0] ^ data[1] ^ data[2] ^ data[3] ^ data[4]);

            buffer.WriteByte(chk);

            SendRegularPacket(buffer.ToArray());
        }

        public void SendRegularPacket(byte[] payload)
        {
            CipheredStream.BeginWrite(payload, 0, payload.Length, new AsyncCallback(EndSend), null);
        }

        void EndSend(IAsyncResult ar)
        {
            CipheredStream.EndWrite(ar);
        }

    }
}
