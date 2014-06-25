//
//  MainClass.cs
//
//  Author:
//       Joshua Garvin <G6-RezBot@kaldon.com>
//
//  Copyright (c) 2014 Joshua Garvin 
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Affero General Public License as
//  published by the Free Software Foundation, either version 3 of the
//  License, or (at your option) any later version.
// 
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Affero General Public License for more details.
//
//  You should have received a copy of the GNU Affero General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.


using System;
using System.Linq;
using System.Collections.Generic;
using G6RezBot;
using G6RezBot.Plugin;


namespace G6RezBot
{
  class MainClass : MarshalByRefObject, IMain, IDisposable
	{
    private static MainClass _MainClass;

    private MainClass(AppDomain pluginDomain){
      _PluginDomain = pluginDomain;
      _Plugins = new Plugins(_PluginDomain);

      _Timer = new System.Timers.Timer(300);
      _Timer.AutoReset = true;
      _Timer.Elapsed += HandleElapsedTimer;
    }

    private System.Timers.Timer _Timer;
    private AppDomain _PluginDomain;
    private Plugins _Plugins;
    private Dictionary<PluginTypeEnum, List<IPlugin>> _PluginInstances = new Dictionary<PluginTypeEnum, List<IPlugin>>();

 		public static void Main (string[] args)
    {
      //AppDomain PluginDomain = AppDomain.CreateDomain ("Plugin Domain");
      AppDomain PluginDomain = AppDomain.CurrentDomain;

      using(_MainClass = new MainClass(PluginDomain)) {
        _MainClass.Initialize();
        _MainClass.StartPlugins();

        Console.ReadKey();
      }

      //AppDomain.Unload(PluginDomain);
		}

    private void Initialize() {
      var PluginInfos = _Plugins.ScanPlugins();
      _PluginInstances = _Plugins.InitializePlugins(PluginInfos, PluginStatusChanged);
    }

    private void StartPlugins()
    {
      foreach (var TheType in _PluginInstances.Keys) {
        // The extra call to ToArray() guarentees that plugins don't 
        // dissapear from our list while attempting to start them.
        foreach (var Plugin in _PluginInstances[TheType].ToArray())
        {
          Plugin.StartAsync(this);
        }
      }
    }

    private void PluginStatusChanged(object sender, StatusChangedEventArgs e){
      var Plugin = (IPlugin)sender;
      Console.WriteLine("{0} {1} plugin entered the {2} state.", Plugin.PluginType, Plugin.GetType().FullName, e.NewStatus);

      switch(e.NewStatus){
        case PluginStatusEnum.Initialized:
          break;
        case PluginStatusEnum.Starting:
          break;
        case PluginStatusEnum.Started:
          // The extra call to ToArray() guarentees that plugins don't 
          // dissapear from our list while attempting to check thir status.
          if( _PluginInstances.All( InstanceType => InstanceType.Value.ToArray().All( Instance => Instance.PluginStatus == PluginStatusEnum.Started || Instance.PluginStatus == PluginStatusEnum.ShuttingDown ))) 
          {
            Console.WriteLine("All plugins finished starting.");
            Console.WriteLine("Starting timer...");
            _Timer.Start();
          }
          break;
        case PluginStatusEnum.ShuttingDown:
          Plugin.StatusChanged -= PluginStatusChanged;
          _PluginInstances[Plugin.PluginType].Remove(Plugin);
          break;
      }
    }


    private void HandleElapsedTimer (object sender, System.Timers.ElapsedEventArgs e)
    {
      try{
        var Elapsed = TimerElapsed;
        if (TimerElapsed != null)
        {
          var NewE = new G6RezBot.Plugin.ElapsedEventArgs(e);
          Elapsed(this, NewE);
        }
      }
      catch(Exception ex){
        Console.WriteLine("Error in MainClass.HandleElapsedTimer():  " + ex.ToString());
      }
    }

    #region IMain implementation
    private Dictionary<string, User> _Users = new Dictionary<string, User>();

    public event EventHandler<G6RezBot.Plugin.ElapsedEventArgs> TimerElapsed;

    public User ConstructUser(string userName)
    {
      User Result;
      if(! _Users.TryGetValue(userName, out Result))
      {
        Result = new User(userName, false);
      }
      return Result;
    }

    #endregion


    #region IDisposable implementation
    private void Dispose(bool itIsSafeToAlsoFreeManagedObjects){
      if (itIsSafeToAlsoFreeManagedObjects) {
        foreach (var TheType in _PluginInstances.Keys) {
          // The extra call to ToArray() guarentees that objects don't 
          // dissapear from our list while attempting to dispose them.
          foreach (var Plugin in _PluginInstances[TheType].ToArray())
          {
            try {
              Plugin.Dispose();
            }
            catch(Exception ex) {
              Console.WriteLine("Error disposing \"{0}\": {1}", Plugin.GetType().FullName, ex.Message);
              Console.WriteLine(ex.StackTrace.ToString());
            }
          }
        }
      }
    }

    public void Dispose()
    {
      try
      {
        Dispose(true);
      }
      finally
      {
        GC.SuppressFinalize(this);
      }
    }

    #endregion
	}
}
