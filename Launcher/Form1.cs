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
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Swtor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private string extractLang()
        {
            string[] file = Directory.GetFiles(Directory.GetCurrentDirectory() + "\\Assets\\", "assets_swtor_??_??_version.txt");
            Match matches = Regex.Match(file[0], @"(.*)[\\/]assets_swtor_([a-z]{2})_([a-z]{2})_version.txt");
            return matches.Groups[2].Value + "-" + matches.Groups[3].Value;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string lang = extractLang();
            ProcessStartInfo info = new ProcessStartInfo(Directory.GetCurrentDirectory() + "\\swtor\\client\\swtor.exe", "-set username " + username.Text +
                " -set password d9G2zHvR48gt1ODJfdGymH/UtJoqhOjMLtWzzXqE6Mx80unJedTpzi3T5s0q1eKaK4TnnXfXsZ59gbHMLNbnyXzR4Ml6hrXDetXgmHqEtckqgLHCLoDjnnfVtMl3geLMdoPhwnnR5Jh40uHIeNHkynzb5ph8gLLJLtHmz3jX6cJ71uLLe4Hnmi7atMgqhLbPeNGymiyA48521OPNLtuxzn+H5ch81eHJfYHizXjQ6cN72+meLYGxzHbX5Mx5gejCK4GymCvU5MJ62+POKofmzXnb48ks1OTNLdOzwnuG4p5+2+POf4ayw3yA5Jh407LMKdbowiqG5p5+07XLd4exyWLRtMgr1uHIfYfozHbR4pks2rWfd4Pgz3yHsZ12hrSZeQ==" +
                " -set platform swtor.server.com:8080 -set environment swtor " +
                "-set lang " + lang + " -set torsets main," + lang + " @swtor_dual.icb");

            info.WorkingDirectory = Directory.GetCurrentDirectory() + "\\swtor\\client";

            Process.Start(info);
        }
    }
}