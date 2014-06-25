//
//  IPlugin.cs
//
//  Author:
//       Joshua Garvin <G6-RezBot@kaldon.com>
//
//  Copyright (c) 2014 Joshua Garvin 
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Net;

namespace G6RezBot.Plugin
{
  public enum PluginStatusEnum : byte
	{
    Unknown = 0,
    /// <summary>
    /// Plugin is initializing, reading configuration, etc.  (The plugin should not access network resources in this phase.)
    /// </summary>
    Initializing,
    /// <summary>
    /// The plugin has finished initializing and is ready to be started.  (The plugin should not access network resources in this phase.)
    /// </summary>
    Initialized,
    /// <summary>
    /// The plugin is performing startup actions such as connecting to network resources and enabling timers.
    /// </summary>
    Starting,
    /// <summary>
    /// The plugin is operational and/or operating.
    /// </summary>
    Started,
    /// <summary>
    /// The plugin is in the process of closing network connections, freeing memory allocations, and cleaning up unmanaged resources.
    /// </summary>
    ShuttingDown
	}
}

