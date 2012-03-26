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
using System.ServiceModel;
using WCFContracts;
using System.Diagnostics;

namespace NexusHostAgent
{
    class Program
    {
        static void Main(string[] args)
        {
            // This program is aimed to run on each server system of the Nexus emulator project
            // that run a TOR service.

            // 1. Start a WCF Server on TCP Port 14631
            ServiceHost servicehost = new ServiceHost(typeof(TFS_HostAgent_Contract_Impl), new Uri[] { new Uri("net.tcp://0.0.0.0:14631/TOR/") });
            servicehost.AddServiceEndpoint(typeof(I_TFS_HostAgent_Contract), new NetTcpBinding(), "HostAgent");
            servicehost.Open();

            Console.WriteLine("WCF Server Ready.");

            while (true) Console.ReadLine();
        }
    }

    public class TFS_HostAgent_Contract_Impl : I_TFS_HostAgent_Contract
    {
        public bool HasProcess(string exeName)
        {
            return Process.GetProcessesByName(exeName).Count() > 0;
        }

        public int GetProcessID(string exeName)
        {
            throw new NotImplementedException();
        }

        public bool KillProcess(int pid)
        {
            throw new NotImplementedException();
        }
    }

}
