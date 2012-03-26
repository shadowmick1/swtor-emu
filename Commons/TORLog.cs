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

namespace Commons
{
    public class TORLog
    {
        static void WriteToConsole(string message, ConsoleColor color)
        {
            ConsoleColor current = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = current;
        }

        static void _Log(string type, string message, ConsoleColor color)
        {
            WriteToConsole(DateTime.Now.ToString() + " [" + type + "] " + message, color);
        }

        public static void Info(string message)
        {
            _Log("Info", message, ConsoleColor.White);
        }

        public static void Warn(string message)
        {
            _Log("Warning", message, ConsoleColor.Yellow);
        }

        public static void Warn(string message, Exception e)
        {
            Warn(message += "\n" + e.Message + "\n" + e.StackTrace);
        }

        public static void Error(string message)
        {
            _Log("Error", message, ConsoleColor.Red);
        }

        public static void Error(string message, Exception e)
        {
            Error(message += "\n" + e.Message + "\n" + e.StackTrace);
        }

        public static void Network(string message)
        {
            _Log("Network", message, ConsoleColor.DarkCyan);
        }

    }
}
