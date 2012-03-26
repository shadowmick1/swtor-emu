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
using ShardServer.TORBusiness.Data;

namespace ShardServer.TORBusiness.Model
{
    public class Character
    {
        public string Name;
        public byte Level;
        public MapAreas AreaSpec;
        public CharacterClass Class;

        public Tuple<byte, byte>[] Appearance = new Tuple<byte, byte>[]
        {
            new Tuple<byte, byte>(0x01, 0x5D),
            new Tuple<byte, byte>(0x05, 0x8B),
            new Tuple<byte, byte>(0x07, 0x3D),
            new Tuple<byte, byte>(0x0A, 0x51),
            new Tuple<byte, byte>(0x0B, 0x46),
            new Tuple<byte, byte>(0x0C, 0x83),
            new Tuple<byte, byte>(0x0E, 0x20),
            new Tuple<byte, byte>(0x10, 0xA8)
        };

        public uint spec1 = 0x4e7511aa;
        public uint spec2 = 0x899;
        public byte spec3 = 0x80;
        public ushort spec4;
        public byte spec5;

        public uint APP1 = 0xf233f656;
        public uint APP2 = 0x7774dedd;
        public uint APP3 = 0xb19c6ccb;
        public ushort APP4 = 0x1a7;

    }
}
