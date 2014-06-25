//
//  ChannelEventArgs.cs
//
//  Author:
//       Joshua Garvin <G6-RezBot@kaldon.com>
//
//  Copyright (c) 2014 Copyright (c) 2014 Joshua Garvin
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
//
using System;
using G6RezBot.Plugin;

namespace G6Irc
{
  public class ServerEventArgs : EventArgs
  {
    private readonly string _EventName;
    private readonly string _UserName;
    private readonly string _Message;

    public ServerEventArgs(string eventName, string userName, string message)
    {
      _EventName = eventName;
      _UserName = userName;
      _Message = message;
    }

    public string EventName { get{ return _EventName; } }
    public string UserName { get{ return _UserName; } }
    public string Message { get{ return _Message; } }
  }
}

