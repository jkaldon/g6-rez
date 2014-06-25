//
//  UserModes.cs
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

namespace G6Irc
{
  public enum UserRegistrationModes : byte
  {
    /// <summary>
    /// - user is registered normally
    /// </summary>
    NONE = 0,
    /// <summary>
    /// w - user receives wallops;
    /// </summary>
    WALLOPS=4,
    /// <summary>
    /// i - marks a users as invisible;
    /// </summary>
    INVISIBLE = 8,
    /// <summary>
    /// wi - user receives wallops and marks user as invisible
    /// </summary>
    BOTH = 12
  }
}

