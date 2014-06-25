//
//  Plugins.cs
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
using System.Collections.Generic;
using G6RezBot.Plugin;

namespace G6RezBot
{
  public class Plugins
  {
    public Plugins(AppDomain pluginDomain)
    {
      _PluginDomain = pluginDomain;
    }

    AppDomain _PluginDomain;

    public List<PluginInfo> ScanPlugins() 
    {
      var Results = new List<PluginInfo>();
      var PluginDirectory = GetPluginDirectory();

      System.Console.WriteLine("");
      System.Console.WriteLine("Searching for plugins in {0}...", PluginDirectory);

      var discovery = (PluginDiscovery) _PluginDomain.CreateInstanceAndUnwrap (
        typeof (PluginDiscovery).Assembly.FullName,
        typeof (PluginDiscovery).FullName
      );
        
      var DllFiles = System.IO.Directory.GetFiles(PluginDirectory , "*.dll");

      foreach(var DllFile in DllFiles) {
        Console.WriteLine("  Looking for IPlugin implementations in {0}...", System.IO.Path.GetFileName(DllFile));
        PluginInfo[] PluginInfos = discovery.GetPlugins(DllFile);

        foreach (var PluginInfo in PluginInfos)
        {
          Results.Add(PluginInfo);
          Console.WriteLine("    Found \"{0}\": \"{1}\"", PluginInfo.AssemblyName, PluginInfo.TypeName);
        }
        Console.WriteLine("  Done.");
      }
      Console.WriteLine("Done.");
        
      return Results;
    }

    public Dictionary<PluginTypeEnum, List<IPlugin>> InitializePlugins(List<PluginInfo> plugins, EventHandler<StatusChangedEventArgs> stateChangeEventHandler)
    {
      var Results = new Dictionary<PluginTypeEnum, List<IPlugin>>();

      Console.WriteLine();
      foreach (var Plugin in plugins) {
        var PluginInstance = (IPlugin)_PluginDomain.CreateInstanceAndUnwrap (
          Plugin.AssemblyName,
          Plugin.TypeName
        );

        PluginInstance.StatusChanged += stateChangeEventHandler;

        if (! Results.ContainsKey(PluginInstance.PluginType) ) {
          Results.Add(PluginInstance.PluginType, new List<IPlugin>());
        }
        Results[PluginInstance.PluginType].Add(PluginInstance);
        Console.WriteLine("Created \"{0} Plugin\" of type \"{1}\"", PluginInstance.PluginType, Plugin.TypeName);
      }

      return Results;
    }

    private static string GetPluginDirectory() {
      var ApplicationDirectory = System.Reflection.Assembly.GetExecutingAssembly().Location;
      ApplicationDirectory = System.IO.Path.GetDirectoryName(ApplicationDirectory);
      Console.WriteLine("ApplicationDirectory={0}", ApplicationDirectory);     

      var PluginDirectory = System.IO.Path.Combine(ApplicationDirectory, "Plugins");

      if (!System.IO.Directory.Exists(PluginDirectory)) {
        Console.Write("PluginDirectory not found, creating subdirectory...");     
        System.IO.Directory.CreateDirectory(PluginDirectory);
        Console.WriteLine("Done.");
      }

      Console.WriteLine("PluginDirectory={0}", PluginDirectory);     

      return PluginDirectory;
    }
  }
}

