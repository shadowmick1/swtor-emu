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
using ShardServer.Packets.Server;
using Commons.Networking;
using Commons;
using ShardServer.TORBusiness.Services;

namespace ShardServer.Packets.Client
{
    public class CMsg_ClientSignatureResponse : IClientPacket
    {
        public void ExecutePacket(Commons.Networking.AsyncConnection connection, Commons.Networking.ByteBuffer packet)
        {
            packet.ReadUInt();
            packet.ReadShort();
            string requestdeObjectName = packet.ReadString(); // OmegaServerProxyObjectName
            string signature = packet.ReadString();    // b7a6bba3:8ab55405:d7b5d3e1:5bc541f9
            packet.ReadString();   // Client
            uint objectId = packet.ReadUInt();
            SignatureService.ClientSignatureResponse(connection, requestdeObjectName, objectId, signature);
        }
    }

    public class CMsg_SocketClose : IClientPacket
    {
        public void ExecutePacket(AsyncConnection connection, ByteBuffer packet)
        {
            TORLog.Info(connection.GetHashCode() + " leaving the Shard Server.");
            connection.Close();
        }
    }

    public class CMsg_ServerSignatureRequest : IClientPacket
    {
        public void ExecutePacket(AsyncConnection connection, ByteBuffer packet)
        {
            uint RequestedRootID = packet.ReadUInt(); // 0x50500000
            string cnx_type_1 = packet.ReadString(); // *
            string cnx_type_2 = packet.ReadString(); // timesource

            packet.ReadString(); // String.Empty
            packet.ReadString(); // String.Empty

            string cnx_type_3 = packet.ReadString(); // Application_TimeRequester

            packet.ReadString(); // String.Empty

            string objectId = packet.ReadString();
            
            SignatureService.RequestServerSignature(connection, cnx_type_2);
        }
    }

    public class CMsg_ClientModules : IClientPacket
    {
        void IClientPacket.ExecutePacket(AsyncConnection connection, ByteBuffer packet)
        {
            connection.SendPacket(new SMsg_0x25ACBEF4());
            connection.SendPacket(new SMsg_0x35BEBAA5());
            connection.SendPacket(new SMsg_0x04CCE2BB());
            connection.SendPacket(new SMsg_0x4BD75535());
        }
    }

    public class CMsg_ServerAddressRequest : IClientPacket
    {
        void IClientPacket.ExecutePacket(AsyncConnection connection, ByteBuffer packet)
        {
            uint objid = packet.ReadObjectIDRev();
            TORLog.Info("GetServerAddress@" + objid);
            //connection.SendPacket(new SMsg_ServerAddressResponse(objid, "swtor.privateserver.com"));
        }
    }
}
