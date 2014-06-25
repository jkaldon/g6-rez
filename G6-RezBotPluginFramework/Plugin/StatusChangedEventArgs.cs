//
//  StatusChangedEventArgs.cs
//
//  Author:
//       Joshua Garvin <G6-RezBot@kaldon.com>
//
//  Copyright (c) 2014 Copyright (c) 2014 Joshua Garvin
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
using G6RezBot.Plugin;

namespace G6RezBot.Plugin
{
  [Serializable]
  public class StatusChangedEventArgs : EventArgs
  {
    public IPlugin Plugin { get; private set; }
    public PluginStatusEnum NewStatus { get; private set; }

    public StatusChangedEventArgs(PluginStatusEnum newStatus)
    {
      NewStatus = newStatus;
    }
  }
}

