//
//  MainClass.cs
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
using System.Net;
using G6Irc;

namespace G6Irc
{
  public class IrcPlugin : PluginBase
  {
    private ServerConnection _IrcConnection;
    private IPAddress _Address;
    private int _Port;
    private string _Nickname;
    private string _Realname;
    private string _Password;
    private string _ChannelName;

    public IrcPlugin()
    {
      _Address = System.Net.Dns.GetHostAddresses("irc.twitch.tv")[0];
      //_Address = System.Net.Dns.GetHostAddresses("localhost")[0];
      _Port = 6667;
      _Password = "oauth:gcwzhjcgg8784gqan83dq1006xit28f";
      _Nickname = "GalapagosPenguin";
      _Realname = "G6-REZBot Test";
      _ChannelName = "#GalapagosPenguin";
      Console.WriteLine("G6Irc.IrcPlugin constructor completed!");
    }

    #region IDisposable implementation

    protected override void Dispose(bool itIsSafeToAlsoFreeManagedObjects){
      try
      {
        if (itIsSafeToAlsoFreeManagedObjects) {
          PluginStatus = PluginStatusEnum.ShuttingDown;

          var Connection = _IrcConnection;
          if (Connection != null)
          {
            Connection.Dispose();
          }
        }
      }
      finally
      {
        base.Dispose(itIsSafeToAlsoFreeManagedObjects);
      }
    }

    #endregion

    #region implemented abstract members of PluginBase

    public override PluginTypeEnum PluginType
    {
      get
      {
        return PluginTypeEnum.Chat;
      }
    }

    public override void StartAsync(IMain main)
    {
      Console.WriteLine("IrcPlugin starting...");

      base.StartAsync(main);

      PluginStatus = PluginStatusEnum.Starting;

      _IrcConnection = new ServerConnection();
      _IrcConnection.ChannelEvent += HandleChannelEvent;
      _IrcConnection.ServerEvent += HandleServerEvent;
      Console.WriteLine("IrcPlugin connecting to {0}:{1}...", _Address, _Port);
      _IrcConnection.BeginConnect(_Address, _Port, IrcConnectionFinished);
    }

    void HandleServerEvent (object sender, ServerEventArgs e)
    {
      Console.WriteLine("User {0} sent {1} : {3}", e.UserName, e.EventName, e.Message);
    }

    void HandleChannelEvent (object sender, ChannelEventArgs e)
    {
      Console.WriteLine("User {0} sent {1} from {2}: {3}", e.UserName, e.EventName, e.ChannelName, e.Message);
    }

    private void IrcConnectionFinished(IAsyncResult asyncResult)
    {
      try
      {
        Console.WriteLine("IrcPlugin connected to {0}:{1}.", _Address, _Port);

        Console.WriteLine("IrcPlugin authenticating...");
        _IrcConnection.Authenticate(_Password, _Nickname, _Realname);

        Console.WriteLine("IrcPlugin joining channel {0}...", _ChannelName);
        _IrcConnection.Join(_ChannelName);

        PluginStatus = PluginStatusEnum.Started;
      }
      catch(Exception ex) {
        Console.WriteLine(ex.ToString());
      }
    }

    #endregion

  }
}

