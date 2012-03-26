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

namespace DumpKeys
{
    class Program
    {
        [DllImport("KeyDump.dll", EntryPoint = "GetKeys", SetLastError = true)]
        public static extern bool GetKeys(IntPtr hKeysFile, uint dwOffset);

        static void Main(string[] args)
        {
            Console.WriteLine("press enter to dump keys ...");
            Console.ReadLine();
            IntPtr keyFile = FileInteropFunctions.CreateFile("c:\\AA.login.salsa", FileInteropFunctions.GENERIC_WRITE, 0, IntPtr.Zero, FileInteropFunctions.CREATE_ALWAYS, 0, 0);

            while (true)
            {
                if (GetKeys(keyFile, 0x00))
                {
                    FileInteropFunctions.CloseHandle(keyFile);
                    break;
                }
            }
            Console.WriteLine("Keys dumped.");
        }
    }

    class FileInteropFunctions
    {
        public const uint GENERIC_READ = (0x80000000);
        public const uint GENERIC_WRITE = (0x40000000);
        public const uint GENERIC_EXECUTE = (0x20000000);
        public const uint GENERIC_ALL = (0x10000000);

        public const uint CREATE_NEW = 1;
        public const uint CREATE_ALWAYS = 2;
        public const uint OPEN_EXISTING = 3;
        public const uint OPEN_ALWAYS = 4;
        public const uint TRUNCATE_EXISTING = 5;

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateFile(
            String lpFileName,              // Filename
            uint dwDesiredAccess,              // Access mode
            uint dwShareMode,              // Share mode
            IntPtr attr,                   // Security Descriptor
            uint dwCreationDisposition,           // How to create
            uint dwFlagsAndAttributes,           // File attributes
            uint hTemplateFile);               // Handle to template file

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool FlushFileBuffers(IntPtr hFile);
    }

}
