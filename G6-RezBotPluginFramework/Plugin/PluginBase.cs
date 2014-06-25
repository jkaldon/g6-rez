//
//  PluginBase.cs
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

namespace G6RezBot.Plugin
{
  public abstract class PluginBase : MarshalByRefObject, IPlugin
  {
    protected PluginBase()
    {
      _PluginStatus = PluginStatusEnum.Initializing;
    }

    #region IDisposable implementation
    protected virtual void Dispose(bool itIsSafeToAlsoFreeManagedObjects) 
    {
      if(itIsSafeToAlsoFreeManagedObjects) {
        PluginStatus = PluginStatusEnum.ShuttingDown;
      }
      // 
      // Note to self:
      // The base object here does not implement IDisposable.
      //
    }

    public void Dispose() 
    {
      Dispose(true); 
      GC.SuppressFinalize(this);
    }
    #endregion

    #region IPlugin implementation
    protected IMain Main { get; private set;}

    public abstract PluginTypeEnum PluginType
    {
      get;
    }

    public virtual event EventHandler<StatusChangedEventArgs> StatusChanged;

    private PluginStatusEnum _PluginStatus;
    public virtual PluginStatusEnum PluginStatus { 
      get{
        return _PluginStatus;
      }
      protected set {
        var RaiseStatusChanged = StatusChanged;
        var IsChanged = _PluginStatus != value;
        _PluginStatus = value;
        if (RaiseStatusChanged != null && IsChanged) 
        {
          RaiseStatusChanged(this, new StatusChangedEventArgs(value));
        }
      } 
    }

    public virtual void StartAsync(IMain main){
      Main = main;
    }
    #endregion

    ~PluginBase()
    {
      Dispose(false);
    }
  }
}

