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
using System.Threading;

namespace Commons.Networking
{
    public class AsyncServer
    {
        Socket serverSocket;

        public delegate void ConnectionAcceptedHandler(AsyncConnection connection);

        public event ConnectionAcceptedHandler ConnectionAccepted;

        public AsyncServer(IPAddress address, int port)
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(address, port));
        }

        public void Start()
        {
            serverSocket.Listen(100);
            BeginAccept();
        }

        void BeginAccept()
        {
            serverSocket.BeginAccept(new AsyncCallback(EndAccept), null);
        }

        void EndAccept(IAsyncResult ar)
        {
            Socket clientSocket = serverSocket.EndAccept(ar);
            AsyncConnection con = new AsyncConnection(clientSocket);
            if (ConnectionAccepted != null)
            {
                new Thread(new ThreadStart(() =>
                    {
                        ConnectionAccepted(con);
                    })).Start();
            }
            BeginAccept();
        }

        public void Stop()
        {
            // broadcast connection close to all clients
            serverSocket.Close();
        }

    }
}
