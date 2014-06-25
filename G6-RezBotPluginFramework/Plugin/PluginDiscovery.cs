//
//  PluginDiscovery.cs
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
using System.Reflection;
using System.Collections.Generic;

namespace G6RezBot.Plugin
{
  class PluginDiscovery : MarshalByRefObject
  {
    public PluginInfo[] GetPlugins (string assemblyPath)
    {
      Assembly TheAssembly = Assembly.LoadFrom (assemblyPath);
      List<PluginInfo> TypeNames = new List<PluginInfo> ();

      foreach (Type TheType in TheAssembly.GetTypes ())
      {
        if (TheType.IsPublic
          && TheType.IsMarshalByRef
          && typeof(IPlugin).IsAssignableFrom(TheType))
        {
          TypeNames.Add(new PluginInfo(TheType.Assembly.FullName, TheType.FullName));
        }
      }
      return TypeNames.ToArray ();
    }
  }
}

