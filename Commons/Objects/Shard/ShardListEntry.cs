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
using System.Xml.Serialization;

namespace Commons.Objects.Shard
{
    public class ShardListEntry
    {
        [XmlAttribute("Region")]
        public ShardRegion Region;
        [XmlIgnore]
        public bool Online = true;
        [XmlAttribute("TZ")]
        public short Timezone;
        [XmlAttribute("ID")]
        public string ServerID;
        [XmlAttribute("Name")]
        public string Name;
        [XmlIgnore]
        public int QueueWait = 0;
        [XmlIgnore]
        public ShardLoadLevel LoadLevel = ShardLoadLevel.Standard;
        [XmlAttribute("Lang")]
        public string Language;
    }

    public enum ShardRegion : byte
    {
        [XmlEnum("NA")]
        NorthAmerica = 1,
        [XmlEnum("EU")]
        Europe = 2
    }

    public enum ShardLoadLevel : byte
    {
        Standard = 0,
        Light = 1,
        Heavy = 2,
        VeryHeavy = 3,
    }

}
