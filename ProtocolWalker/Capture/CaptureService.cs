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
using System.Windows;
using SharpPcap;
using PacketDotNet;

namespace ProtocolWalker.Capture
{
    public class CaptureService
    {
        public static int[] ProxyCapturePorts;
        static ICaptureDevice captureDevice;
        static string CaptureFilter;
        public static CaptureSession captureSession;
        public static int gameCapturePort;
        
        public static MainWindow mw;

        public static void StartCapture()
        {
            CaptureFilter = "tcp port 8995 or tcp portrange 20050-20100";
            mw.lbl_capturingOn.Content = CaptureFilter + " @ " + captureDevice.Description;

            captureDevice.OnPacketArrival += new PacketArrivalEventHandler(captureDevice_OnPacketArrival);
            captureDevice.Open(DeviceMode.Promiscuous, 1000);
            captureDevice.Filter = CaptureFilter;
            captureDevice.StartCapture();
            Console.WriteLine("Starting capture ...");
        }

        public static void StopCapture()
        {
            captureDevice.StopCapture();
        }

        static object PacketProcessMutex = new object();

        static void captureDevice_OnPacketArrival(object sender, CaptureEventArgs e)
        {
            lock (PacketProcessMutex)
            {
                TcpPacket tcpPacket = TcpPacket.GetEncapsulated(Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data) as EthernetPacket);
                if (tcpPacket.Psh)
                {
                    PacketDirection direction = (tcpPacket.SourcePort == 8995 || (tcpPacket.SourcePort > 20050 && tcpPacket.SourcePort < 20100)) ? PacketDirection.Server2Client : PacketDirection.Client2Server;
                    byte[] data = new byte[tcpPacket.Bytes.Length - tcpPacket.Header.Length];
                    Array.Copy(tcpPacket.Bytes, tcpPacket.Header.Length, data, 0, data.Length);

                    // calculate session id
                    int sessionid = tcpPacket.DestinationPort * tcpPacket.SourcePort;
                    CaptureSession session = CaptureSessionList.Instance.Find(s => s.ID == sessionid);

                    if (session == null)
                    {
                        bool login = false;
                        if((direction == PacketDirection.Server2Client && tcpPacket.SourcePort == 8995) || (direction == PacketDirection.Client2Server && tcpPacket.DestinationPort == 8995))
                            login = true;
                        session = new CaptureSession(sessionid, (login ? (byte)0 : (byte)4));
                        CaptureSessionList.Instance.Add(session);
                    }

                    session.PacketReceived(data, direction); 
                }
            }
        }

        public static List<string> GetAllDevices()
        {
            List<string> result = new List<string>();
            foreach(ICaptureDevice dev in CaptureDeviceList.Instance)
            {
                result.Add(dev.Description);
            }
            return result;
        }

        public static void SelectDevice(int offset)
        {
            captureDevice = CaptureDeviceList.Instance[offset];
        }

    }
}
