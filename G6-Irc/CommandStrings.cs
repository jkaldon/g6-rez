//
//  CommandStrings.cs
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
  public class CommandStrings
  {
    private CommandStrings ()
    {
    }
    public const string PASS = "PASS";
    public const string NICK = "NICK";
    public const string USER = "USER";
    public const string QUIT = "QUIT";

    public const string JOIN = "JOIN";
    public const string PART = "PART";

    public const string TOPIC = "TOPIC";
    public const string KICK = "KICK";
    public const string INVITE = "INVITE";


    public const string PING = "PING";
    public const string PONG = "PONG";

    public const string PRIVMSG = "PRIVMSG" ;
  }
}

