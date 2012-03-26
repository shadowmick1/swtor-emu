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
using System.IO;

namespace ClientShardListPatcher
{
    class Program
    {
        [System.Runtime.InteropServices.DllImport("Kernel32")]
        private extern static Boolean CloseHandle(IntPtr handle);
        /// <summary>
        /// This file patches swtor.exe (and backs it up to swtor.DateTime.Now.bak) for use with personal SSL servers.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            if (!File.Exists("swtor.exe"))
            {
                Console.WriteLine("Cannot find SWTOR.exe. Please check that the patcher is in the 'retailclient' directory.");
            }
            else
            {
                FileStream swtorExeStream;
                int writePosition = 0;
                try
                {
                    swtorExeStream = new FileStream("swtor.exe", FileMode.Open, FileAccess.Read);

                    Console.WriteLine("Searching the file for required offset ...");
                    byte[] pattern = new byte[] { 0x00, 0x6A, 0x01, 0x50, 0xE8, 0xFF, 0xFF, 0x01, 0xFF, 0x33, 0xDB };
                    byte[] buffer = new byte[1];

                    int matchcount = 0;
                    int position = 0;

                    while (swtorExeStream.Read(buffer, 0, 1) != 0)
                    {
                        byte b = buffer[0];
                        if (b == pattern[matchcount] || pattern[matchcount] == 0xFF)
                            matchcount++;
                        else if(matchcount > 0)
                            matchcount = 0;
                        if (matchcount == 11)
                        {
                            writePosition = position - 8;
                            break;
                        }
                        position++;
                    }
                    CloseHandle(swtorExeStream.Handle);
                    swtorExeStream.Close();
                }
                catch (Exception)
                {
                    Console.WriteLine("Cannot open swtor.exe. Check program is not running and you have adequate permissions.");
                }
                if (writePosition > 0)
                {
                    FileStream stream = new FileStream("swtor.exe", FileMode.OpenOrCreate, FileAccess.ReadWrite);

                    Console.WriteLine("Backing up swtor.exe...");

                    FileStream bkp = File.Open("swtor." + DateTime.Now.ToFileTime() + ".bak", FileMode.CreateNew);
                    stream.CopyTo(bkp);
                    bkp.Flush();
                    bkp.Close();
                    bkp.Dispose();

                    Console.WriteLine("Patching ...");

                    stream.Position = writePosition;
                    stream.WriteByte(0);
                    stream.Flush();
                    stream.Close();
                    stream.Dispose();

                    Console.WriteLine("Patching done.");
                }
            }


            Console.WriteLine("");
            Console.WriteLine("Press Enter to exit ...");
            Console.ReadLine();
        }
    }
}
