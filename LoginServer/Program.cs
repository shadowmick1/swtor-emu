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
using Commons.Networking;
using System.Net;
using Commons;
using System.IO;
using System.Runtime.InteropServices;

namespace ProxyServer
{
	class Program
	{
		/*
		 * Packet flow
		 * 
		 * 0001. S => C : unk
		 * 0002. C => S : RSA Encrypted chunk (Salsa20 Key encrypted with RSA)
		 * 0003. C => S : Password (1st packet)
		 * 0004. C => S : Ping Packet (encrypted Salsa20)
		 * 
		 */
		static void Main(string[] args)
		{
			PacketProcessor.TRSAInit();
			AsyncServer server = new AsyncServer(IPAddress.Parse("0.0.0.0"), 7979);
			server.Start();

			server.ConnectionAccepted += new AsyncServer.ConnectionAcceptedHandler(connection =>
			{
				Console.WriteLine("New connection: " + connection.GetHashCode());
				// Create AsyncState object
				connection.AsyncState = new AsyncConnectionData();
				// Send Packet 0001

				connection.DataReceived += new AsyncConnection.ConnectionDataReceivedHandler(data =>
				{
					PacketProcessor.ProcessPacket(data, connection);
				});

				connection.SendClear(new byte[] {
					0x03, 0x16, 0x00, 0x00, 0x00, 0x15, 0x11, 0x00, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00, 0x36, 0x40, 0xce, 0x72, 0x71, 0xfe, 0x8d, 0x7e 
				});
				// State is 1
				connection.SetState(1);

				connection.EngageReading();
			});

			Console.WriteLine("Server running, press Enter to exit ...");
			Console.ReadLine();
		}
	}
}
