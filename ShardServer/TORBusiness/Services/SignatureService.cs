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
using ShardServer.Packets;
using ShardServer.Packets.Server;
using Commons;
using System.Threading;

namespace ShardServer.TORBusiness.Services
{
    public class SignatureService
    {
        public static void RequestClientSignature(AsyncConnection con, string objectName, uint id)
        {
            con.SendPacket(new SMsg_ClientSignatureRequest(objectName, id));
            TORLog.Info("RequestClientSignature: " + objectName + "@" + id);
        }
        public static void ClientSignatureResponse(AsyncConnection con, string objectName, uint id, string signature)
        {
            TORLog.Info("ClentSignatureResponse: " + objectName + "@" + id + " = " + signature);
            if (objectName == "OmegaServerProxyObjectName")
            {
                con.SendPacket(new SMsg_ServerSignatureResponse(0x5050, "u796", "userentrypoint13", "9cf74d45:1a6cc459:6fa57dc2"));
                con.SendPacket(new SMsg_XMLSettings());
            }
        }
        public static void RequestServerSignature(AsyncConnection con, string objectName)
        {
            uint responseid = 0;
            string data1 = "";
            string data2 = "userentrypoint13";
            string sig = "";
            switch(objectName)
            {
                case "timesource":
                    responseid = 0x010006;
                    data1 = "timesource";
                    sig = "462a9d1f";
                    break;
                case "biomon":
                    responseid = 0x02505e;
                    data1 = "sp1u796[biomonserver:biomon]sylar501.biomon";
                    sig = "389fc8f0:9adcb7e6";
                    break;
                case "worldserver":
                    responseid = 0x035059;
                    data1 = "sp2u796[WorldServer:worldserver]sylar501.worldserver";
                    sig = "51e518d9:92367cb3:29f2db17";
                    break;
                case "gamesystemsserver":
                    responseid = 0x05505a;
                    data1 = "sp3u796[GameSystemsServer:gamesystemsserver]sylar501.gamesystemsserver";
                    sig = "450a2825";
                    break;
                case "chatgateway":
                    responseid = 0x06505c;
                    data1 = "sp4u796[ChatGateway:chatgateway]sylar501.chatgateway";
                    sig = "900005df";
                    break;
                case "auctionserver":
                    responseid = 0x07505b;
                    data1 = "sp5u796[AuctionServer:auctionserver]sylar501.auctionserver";
                    sig = "9ce839f7";
                    break;
                case "mailserver":
                    responseid = 0x085058;
                    data1 = "sp6u796[Mail:mailserver]sylar501.mailserver";
                    sig = "9759aa23";
                    break;
                case "trackingserver":
                    responseid = 0x09505d;
                    data1 = "sp7u796[TrackingServer:trackingserver]sylar501.trackingserver";
                    sig = "c5b320c1:8cebf93e";
                    break;
                case "areaserver":
                    responseid = 0x046953;
                    data1 = "sp8u796[AreaServer-ord_main-4611686019802843831-1-:areaserver]sylar501.areaserver";
                    sig = "91ac5777:62060b0:29f2db17";
                    break;
                default:
                    TORLog.Error("Unknown server signature for " + objectName);
                    return;
            }
            con.SendPacket(new SMsg_ServerSignatureResponse(responseid, data1, data2, sig));
        }
    }
}
