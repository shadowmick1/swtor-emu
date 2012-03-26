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
using System.Globalization;

namespace NumericConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Convert Numeric <=> Hex");
                Console.Write("Numeric = ");
                string num = Console.ReadLine();

                if (num.StartsWith("0x"))
                {
                    ulong id = ulong.Parse(num.Replace("0x", ""), NumberStyles.HexNumber);
                    Console.WriteLine("Numeric = " + id);
                }
                else
                {

                    ulong id = ulong.Parse(num);

                    Console.WriteLine("Hex = " + id.ToString("X8"));
                }
                Console.ReadLine();
            }
        }
    }
}
