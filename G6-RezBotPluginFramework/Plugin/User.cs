//
//  IPluginChat.cs
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
  public class User : MarshalByRefObject
	{
    public string UserName { get; private set; }
    public bool IsAuthenticated { get; private set; }

    public User(string userName, bool isAuthenticated) {
      UserName = userName;
      IsAuthenticated = isAuthenticated;
    }
	}
}

