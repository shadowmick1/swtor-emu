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
using System.Xml;

namespace ProtocolWalker.Capture
{
    public class XmlFactory
    {
        public static XmlElement CreatePacketElement(XmlDocument doc, TORCapturedPacket packet)
        {
            XmlElement element = doc.CreateElement("Packet");
            element.Attributes.Append(CreateAttribute(doc, "Type", packet.Type.ToString()));
            return element;
        }

        static XmlAttribute CreateAttribute(XmlDocument doc, string attributeName, string value)
        {
            XmlAttribute attr = doc.CreateAttribute(attributeName);
            attr.Value = value;
            return attr;
        }

        public static XmlDocument CreateSessionDocument(CaptureSession session)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement root_element = doc.CreateElement("CaptureSession");
            root_element.Attributes.Append(CreateAttribute(doc, "SaveTime", DateTime.Now.ToString()));
            foreach (TORCapturedPacket pkt in session.CapturedPackets)
            {
                root_element.AppendChild(CreatePacketElement(doc, pkt));
            }
            doc.AppendChild(root_element);
            return doc;
        }

    }
}
