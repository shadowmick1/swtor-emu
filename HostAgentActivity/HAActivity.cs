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
using System.Activities;
using Microsoft.TeamFoundation.Build.Client;
using System.ServiceModel;
using WCFContracts;

namespace HostAgentActivity
{
    [BuildActivity(HostEnvironmentOption.All)]
    [BuildExtension(HostEnvironmentOption.All)]
    public sealed class HasProcessActivity : CodeActivity<bool>
    {
        // Define an activity input argument of type string
        public InArgument<string> TargetMachine { get; set; }
        public InArgument<string> ProcessName { get; set; }

        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.
        protected override bool Execute(CodeActivityContext context)
        {
            ChannelFactory<I_TFS_HostAgent_Contract> factory = new ChannelFactory<I_TFS_HostAgent_Contract>(new NetTcpBinding(), new EndpointAddress("net.tcp://" + context.GetValue<string>(TargetMachine) + ":14631/TOR/HostAgent"));
            I_TFS_HostAgent_Contract channel = factory.CreateChannel();
            return channel.HasProcess(context.GetValue(this.ProcessName));
        }
    }
}
