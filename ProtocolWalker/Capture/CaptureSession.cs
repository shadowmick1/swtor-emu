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
using System.Windows.Controls;
using Commons.Networking;

namespace ProtocolWalker.Capture
{
    public class CaptureSession
    {
        int _id;
        public int ID
        {
            get { return _id; }
        }

        // Captured Packets in this Session
        public EventAwareList<TORCapturedPacket> CapturedPackets = new EventAwareList<TORCapturedPacket>();
        // State of this session
        public int State = 0;
        // TOR ciphers for decrypting protocol
        public TORDecrypter decrypter;

        public ListBox _uiPacketList;
        public TextBox _uiPacketRawView;

        public int RelatedShardSessionID = 0;

        byte _type;

        public CaptureSession(int id, byte type)
        {
            _id = id;
            _type = type;
            decrypter = new TORDecrypter(type);
            CapturedPackets.OnAdd += new EventAwareListAddEvent<TORCapturedPacket>(CapturedPackets_OnAdd);
            Console.WriteLine("new session " + id + " type " + type);
        }

        void CapturedPackets_OnAdd(object sender, EventArgs<TORCapturedPacket> e)
        {
            if (_uiPacketList != null)
            {
                CaptureService.mw.Dispatcher.Invoke(new Action(() =>
                {
                    _uiPacketList.Items.Add("[" + e.Value.ID + "] " + (e.Value.Direction == PacketDirection.Server2Client ? "S" : "C") + " - " + e.Value.Name + " (" + e.Value.Length + ")");
                }), null);
            }
        }

        public TORCapturedPacket GetPacket(int id)
        {
            return CapturedPackets.Find(pkt => pkt.ID == id);
        }

        public void PacketReceived(byte[] data, PacketDirection direction)
        {
            // let's split that into chunks of byte arrays
            List<byte[]> packets;
            switch (State)
            {
                case 0:
                case 1:
                    packets = new List<byte[]>();
                    packets.Add(data);
                    if (State == 1 || State == 3)
                    {
                        decrypter.LoadKeys();
                    }
                    break;
                case 2:
                    packets = decrypter.Decrypt(data, direction == PacketDirection.Client2Server);
                    break;
                default: packets = new List<byte[]>(); break;
            }

            if (State == 0 || State == 1) State++;

            foreach (byte[] pktarray in packets)
            {
                int newid = CapturedPackets.Count<TORCapturedPacket>() > 0 ? CapturedPackets.Last<TORCapturedPacket>().ID + 1 : 1;
                TORCapturedPacket tpkt = new TORCapturedPacket(newid, _type == 4 ? TORPacketType.Shard : TORPacketType.Proxy, pktarray, this, direction);
                
                // todo: get packet name and opcode
                /*uint opcode = PacketIds.GetData(pktarray);
                if (opcode == 0x90F2D084)
                {
                    tpkt.Name = "SMsg_ShardServerAddress";
                }*/

                CapturedPackets.Add(tpkt);
            }             
        }

    }
}
