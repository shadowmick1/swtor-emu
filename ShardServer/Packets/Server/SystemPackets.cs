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
using System.Xml;

namespace ShardServer.Packets.Server
{
    public class SMsg_ServerSignatureResponse : IServerPacket
    {
        uint _id;
        string _data1;
        string _data2;
        string _sig;

        public SMsg_ServerSignatureResponse(uint id, string data1, string data2, string signature)
        {
            _id = id;
            _data1 = data1;
            _data2 = data2;
            _sig = signature;
        }

        public void WritePacket(Commons.Networking.AsyncConnection connection, Commons.Networking.ByteBuffer packet)
        {
            packet.WriteUInt(0xFFFFFFFF);
            packet.WriteUInt(_id);
            packet.WriteString(_data1);
            packet.WriteString(_data2);
            packet.WriteString(_sig);
            packet.WriteLong(0);
            packet.WriteInt(1);
            packet.WriteByte(0);
            packet.WriteLong(1);
            packet.WriteInt(0);
            packet.WriteByte(0);
        }
    }
    public class SMsg_XMLSettings : IServerPacket
    {
        public void WritePacket(Commons.Networking.AsyncConnection connection, Commons.Networking.ByteBuffer packet)
        {
            packet.WriteLong(0x5050);
            string xml = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>";
            xml += "<client title=\"Test Client\" useSyncClock=\"true\" loglevel=\"debug\" repositoryserver=\"RepositoryServer:repositoryserver\" ";
            xml += "worldserver=\"WorldServer:worldserver\" trackingserver=\"TrackingServer:trackingserver\" gamesystemsserver=\"GameSystemsServer:gamesystemsserver\" ";
            xml += "serverscriptcompiler=\"ScriptCompilerServer:scriptcompiler\" clientscriptcompiler=\"ScriptCompilerClient:scriptcompiler\" ";
            xml += "searchserver=\"SearchServer:searchserver\" chatgateway=\"ChatGateway:chatgateway\" mailserver=\"Mail:mailserver\" auctionserver=\"AuctionServer:auctionserver\" ";
            xml += "universeID=\"he1012\" networkNapTimeMS=\"20\" worldServiceDirectoryConfigs=";
            xml += "\"CacheFilePath=HeroEngineCache\\Dev;cmdScriptActivity=;gameName=Dev;\" ";
            xml += "baseServiceDirectoryConfigs=\"ClientCachePath=REPLACEWITHLOCALAPPDATAPATH\\HeroEngine\\REPLACEWITHUNIVERSEID;cmdScriptActivity=;DynamicDetailAltPathName=search all Repository;";
            xml += "DynamicDetailAltTexturePath=/;DynamicDetailTexturePath=/art;HeightMapBillbaordAltPathName=search all Repository;HeightMapBillboardAltPath=/;HeightmapBillboardPath=/art;";
            xml += "HeightmapTerrainMeshAltPath=/;HeightmapTerrainMeshAltPathName=search all Repository;HeightmapTerrainMeshPath=/art;HeightmapTextureAltPath=/;";
            xml += "HeightmapTextureAltPathName=search all Repository;HeightmapTexturePath=/art;HeroEngineScriptWarning=This script is part of HeroEngine and should not be modified for game specific purposes.;";
            xml += "KNOWN_ISSUES_URL=;STATUS_HOST=bwa-dev-uvs02;STATUS_SERVER_PORT=61111;VERSION_NOTES_URL=;ArtDirectory=\\mmo1\\;SplashLogoPath=/bwa_splash.png;ShaderPath=/art/shaders;\" ";
            xml += "clientCachePath=\"REPLACEWITHLOCALAPPDATAPATH\\HeroEngine\\REPLACEWITHUNIVERSEID\" ";
            xml += "additionalClientConfigs=\"WorldName=he1012;SHARD_PUBLIC_NAME=he1012;CacheFilePath=HeroEngineCache\\Dev;cmdScriptActivity=&apos;&apos;;gameName=Dev;";
            xml += "DynamicDetailAltPathName=&apos;search all Repository&apos;;DynamicDetailAltTexturePath=/;DynamicDetailTexturePath=/art;HeightMapBillbaordAltPathName=&apos;search all Repository&apos;;";
            xml += "HeightMapBillboardAltPath=/;HeightmapBillboardPath=/art;HeightmapTerrainMeshAltPath=/;HeightmapTerrainMeshAltPathName=&apos;search all Repository&apos;;";
            xml += "HeightmapTerrainMeshPath=/art;HeightmapTextureAltPath=/;HeightmapTextureAltPathName=&apos;search all Repository&apos;;";
            xml += "HeightmapTexturePath=/art;HeroEngineScriptWarning=&apos;This script is part of HeroEngine and should not be modified for game specific purposes.&apos;;";
            xml += "KNOWN_ISSUES_URL=&apos;&apos;;STATUS_HOST=bwa-dev-uvs02;STATUS_SERVER_PORT=61111;VERSION_NOTES_URL=&apos;&apos;;";
            xml += "ArtDirectory=\\mmo1\\;SplashLogoPath=/bwa_splash.png;ShaderPath=/art/shaders;ScreencatcherURL=screencatcher.swtor.com;AssetTimestamp=AssetTimestamp;eGCSS_URL=swtor-game-lab.nexus.lan:443\">\x0a";
            xml += "                        <gamesystemsservers first=\"GameSystemsServer:gamesystemsserver\"/><biomon metricspublisherserver=\"biomonserver:biomon\">\x0a";
            xml += "                            <biomon-sampler service_family=\"he1012\" service_type=\"gameclient\">\x0a";
            xml += "                            </biomon-sampler></biomon><access-rights>\x0a";
            xml += "                            <client name=\"Automaton.exe\">\x0a";
            xml += "                                <network name=\"BWA\" address=\"10.2.0.0/15\"/><network name=\"AUS\" address=\"10.64.10.0/24\"/><network name=\"BWE\" address=\"10.0.0.0/15\"/>";
            xml += "<network name=\"Mythic\" address=\"10.18.11.0/24\"/></client><client name=\"HeroBlade.exe\">\x0a";
            xml += "                                <network name=\"Ultizen\" address=\"172.16.0.0/24\"/><network name=\"CheQ\" address=\"172.16.1.0/24\"/>";
            xml += "<network name=\"SperaSoft\" address=\"172.16.2.0/24\"/><network name=\"SperaSoftQA\" address=\"172.16.4.0/24\"/><network name=\"Ringtail\" address=\"172.16.11.0/24\"/>";
            xml += "<network name=\"BWA\" address=\"10.2.0.0/15\"/><network name=\"BWE\" address=\"10.0.0.0/15\"/><network name=\"Tiburon\" address=\"10.8.0.0/16\"/>";
            xml += "<network name=\"IE\" address=\"10.148.3.0/24\"/><network name=\"EARS\" address=\"10.14.161.0/24\"/><network name=\"Mythic\" address=\"10.18.11.0/24\"/>";
            xml += "<network name=\"Globant\" address=\"172.16.6.0/24\"/><network name=\"EAMadrid\" address=\"10.20.136.0/24\"/><network name=\"AUS\" address=\"10.64.10.0/24\"/>";
            xml += "<network name=\"BWI\" address=\"10.22.128.0/17\"/></client></access-rights></client>";
            packet.WriteString(xml);
        }
    }

    public class SMsg_ClientSignatureRequest : IServerPacket
    {
        string objectType;
        uint _id;

        /// <summary>
        /// Requests client signature for a given object name
        /// </summary>
        /// <param name="type">object name</param>
        /// <param name="id">some id</param>
        public SMsg_ClientSignatureRequest(string type, uint id)
        {
            objectType = type;
            _id = id;
        }

        public void WritePacket(Commons.Networking.AsyncConnection connection, Commons.Networking.ByteBuffer packet)
        {
            packet.WriteUInt(0xFFFFFFFF);
            packet.WriteString(objectType);
            packet.WriteUInt(_id);
            packet.WriteLong(0);
        }
    }

    public class SMsg_ServerAddressResponse : IServerPacket
    {
        uint _id;
        string _address;
        public SMsg_ServerAddressResponse(uint objId, string address)
        {
            _id = objId;
            _address = address;
        }

        void IServerPacket.WritePacket(Commons.Networking.AsyncConnection connection, Commons.Networking.ByteBuffer packet)
        {
            packet.WriteUInt(_id);
            packet.WriteString(_address);
            packet.WriteUInt(20065); // port
            packet.WriteUInt(0x14);
            packet.WriteBytes(new byte[] { 0x6F, 0xFC, 0xEE, 0xBF, 0x2B, 0x82, 0x23, 0x5D, 0xF2, 0x99, 0x78, 0xCD, 0x70, 0xB6, 0x0D, 0xA7, 0x92, 0xED, 0x5E, 0x33 });
        }
    }
}
