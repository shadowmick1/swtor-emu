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

namespace ShardServer.TORBusiness.Data
{
    class CharacterSpecsData
    {

    }

    public enum CharacterClass : ulong
    {
        // Base Classes
        JediKnight = 16141119516274073244,
        JediConsular = 16141179471541245792,
        Smuggler = 16140912704077491401,
        Trooper = 16140973599688231714,
        SithWarrior = 16140902893827567561,
        SithInquisitor = 16141010271067846579,
        BountyHunter = 16141170711935532310,
        ImperialAgent = 16140943676484767978,
        // Advanced Classes
        JediSentinel,
        JediGuardian,
        JediSage,
        JediShadow,
        Gunslinger,
        Scoundrel,
        Commando,
        Vanguard,
        SithJuggernaut,
        SithMarauder,
        SithAssassin,
        SithSorcerer,
        Powertech,
        Mercenary,
        Operative,
        Sniper
    }

}
