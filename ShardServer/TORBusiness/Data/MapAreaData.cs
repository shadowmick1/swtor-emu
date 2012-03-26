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
    public class MapAreaStore : Dictionary<MapAreas, MapAreaData>
    {
        static MapAreaStore _instance;
        public static MapAreaStore Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new MapAreaStore();
                return _instance;
            }
        }

        public MapAreaStore()
        {
            Add(MapAreas.AlderaanS, new MapAreaData() { NamePrefix = "ald" });
        }

    }

    public class MapAreaData
    {
        public string NamePrefix;
    }

    public enum MapAreas : ulong
    {
        AlderaanS = 4611686023868513034,
        BalmorraI = 4611686022220993772,
        BalmorraR = 4611686040187270001,
        BelsavisS = 4611686025952170568,
        CoruscantR = 4611686019047870293,
        DromundKaasI = 4611686019618382666,
        FlashPointTransitionRegionalImperial = 4611686027378771320,
        FlashPointTransitionRegionalRepublic = 4611686033507370314,
        HothS = 4611686025038950088,
        HuttaI = 4611686019802841877,
        IlumS = 4611686039258170278,
        KorribanI = 4611686022825732973,
        NarShaddaaS = 4611686027681070173,
        OrdMantellR = 4611686019802843831,

        PCShip_DefenderConsular = 4611686051284471420,
        PCShip_DefenderKnight = 4611686051284476033,
        PCShip_FuryWarrior = 4611686051284477081,
        PCShip_FuryInquisitor = 4611686051284477465,
        PCShip_MantisD5 = 4611686051849071023,
        PCShip_PhantomX70B = 4611686051849071159,
        PCShip_ThunderclapBT7 = 4611686053134031672,
        PCShip_XSFreighter = 4611686053487831502,

        QueshS = 4611686030163371363,
        TarisI = 4611686070551329793,
        TarisR = 4611686023014030644,
        TatooineS = 4611686018427650020,
        TythonR = 4611686019869492753,
        VossS = 4611686025383332936,
    }

}
