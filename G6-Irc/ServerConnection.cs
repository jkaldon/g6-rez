//
//  ServerConnection.cs
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
using System.Net.Sockets;
using System.IO;
using G6RezBot.Plugin;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using G6Irc;

namespace G6Irc
{
  public class ServerConnection : IDisposable
  {
    public ServerConnection ()
    {
    }

    private TcpClient _Client;
    private StreamReader _Reader;
    private StreamWriter _Writer;
    private Thread _ReaderThread;

    public event EventHandler<ChannelEventArgs> ChannelEvent;
    public event EventHandler<ServerEventArgs> ServerEvent;

    private void TcpConnectionFinished(IAsyncResult asyncResult){
      try{
        Console.WriteLine("TcpClient connected.");
        var ServerConnectionAsyncResult = (AsyncResult<ServerConnection>)asyncResult.AsyncState;

        _Client.EndConnect(asyncResult);

        _Reader = new StreamReader (_Client.GetStream ());
        _Writer = new StreamWriter (_Client.GetStream ());

        _Writer.AutoFlush = true;

        _ReaderThread = new Thread(ReaderTask);
        _ReaderThread.Priority = ThreadPriority.AboveNormal;
        _ReaderThread.Start();

        ServerConnectionAsyncResult.Complete(this, asyncResult.CompletedSynchronously);
      }
      catch(Exception ex){
        Console.WriteLine(ex.ToString());
      }
    }

    private void ReaderTask(){
      while(_Client.Connected) 
      {
        String ReceivedInfo;
        try {
          ReceivedInfo = _Reader.ReadLine();
        }
        catch {
          ReceivedInfo= null;
        }

        if (ReceivedInfo != null)
        {
          parseMessage(ReceivedInfo);
        }
      }
    }

    protected virtual void RaiseChannelEvent(string channelName, string eventName, string userName, string message)
    {
      var e = new ChannelEventArgs(channelName, eventName, userName, message);
      // Make a temporary copy of the event to avoid possibility of 
      // a race condition if the last subscriber unsubscribes 
      // immediately after the null check and before the event is raised.
      EventHandler<ChannelEventArgs> handler = ChannelEvent;

      // Event will be null if there are no subscribers 
      if (handler != null)
      {
        // Use the () operator to raise the event.
        handler(this, e);
      }
    }

    protected virtual void RaiseServerEvent(string eventName, string userName, string message)
    {
      var e = new ServerEventArgs(eventName, userName, message);
      // Make a temporary copy of the event to avoid possibility of 
      // a race condition if the last subscriber unsubscribes 
      // immediately after the null check and before the event is raised.
      EventHandler<ServerEventArgs> handler = ServerEvent;

      // Event will be null if there are no subscribers 
      if (handler != null)
      {

        // Use the () operator to raise the event.
        handler(this, e);
      }
    }

    public IAsyncResult BeginConnect (System.Net.IPAddress address, int port, AsyncCallback callback) {
      Console.WriteLine("TcpClient connecting to {0}:{1}...", address, port);
      _Client = new TcpClient ();
      var ServerConnectionAsyncResult = new AsyncResult<ServerConnection>(callback, this);
      _Client.BeginConnect(address, port, TcpConnectionFinished, ServerConnectionAsyncResult);
      return ServerConnectionAsyncResult;
    } 

    public void Authenticate (string password, string nickname) {
      Authenticate (password, nickname, nickname);
    }

    public void Authenticate (string password, string nickname, string realName) {
      if (!string.IsNullOrWhiteSpace(password))
      {
        write(CommandStrings.PASS + " " + password);
      }
      write (CommandStrings.NICK + " " +  nickname);
      write (CommandStrings.USER + " " +  nickname + " 8 * :" + realName);
    }

    public void SetNickname (string nickname) {
      write (CommandStrings.NICK + " " + nickname);
    }

    public void Join (string channel) {
      write (CommandStrings.JOIN + " " + channel);
    }

    public void Join (string channel, string key) {
      write (CommandStrings.JOIN + " " + channel + " " + key);
    }

    public void Part (string channel) {
      write (CommandStrings.PART + " " + channel);
    }

    public void Kick (string channel) {
      write (CommandStrings.KICK + " " + channel);
    }

    public void Invite (string channel) {
      write (CommandStrings.INVITE + " " + channel);
    }

    protected void Quit (string message) {
      write (CommandStrings.QUIT  + " :" + message);
    }

    public void SendMessage(string channel, string message, params object[] args)
    {
      write(CommandStrings.PRIVMSG + " " + channel + " :" + message, args);
    }

    private void write(String message, params object[] args)
    {
      _Writer.WriteLine (message, args);
    }

    private void parseMessage(String message)
    {
      Console.WriteLine(message);
      String[] msg = message.Split(' ');

      if (msg[0] == CommandStrings.PING) {
        write (CommandStrings.PONG + " " + msg [1]);
      }
      else {
        switch (msg[1]) {
          case CommandStrings.PRIVMSG:
            {
              //:ashdfohe!~jkaldon@localhost PRIVMSG #Testing :Hello World
              var ActorName = msg[0];
              var Channel = msg[2];
              var Message = string.Join(" ", msg.Skip(3).ToArray());
              RaiseChannelEvent(Channel, CommandStrings.PRIVMSG, ActorName, Message);
            }
          break;
          case CommandStrings.JOIN:
            {
              //:Galapagos!~Galapagos@localhost JOIN :#Testing
              var ActorName = msg[0];
              var Channel = msg[2];
              RaiseChannelEvent(Channel, CommandStrings.JOIN, ActorName, String.Empty);
            }
            break;
          case ResponseCodes.RPL_NAMREPLY:
            {
              //:irc.kaldon.com 353 Galapagos = #Testing :Galapagos jkaldon
              //:irc.kaldon.com 366 Galapagos #Testing :End of NAMES list
              var Channel = msg[4];
              var ActorName = msg[2];
              var UserList = string.Join(" ", msg.Skip(5).ToArray());
              RaiseChannelEvent(Channel, "CHANNEL_REPLY_USER_LIST", ActorName, UserList);
            }
          break;
          case CommandStrings.PART:
            {
              //:ashdfohe!~jkaldon@localhost PART #Testing :ashdfohe
              var Channel = msg[2];
              var ActorName = msg[0];
              var UserList = string.Join(" ", msg.Skip(3).ToArray());
              RaiseChannelEvent(Channel, CommandStrings.PART, ActorName, UserList);
            }
          break;
//        case ResponseCodes.RPL_WHOREPLY:
//          //  addUserToList(capName(msg[4]));
//          break;
          case CommandStrings.KICK:
            {
              var ActorName = msg[0];
              var Channel = msg[2];
              var Reason = string.Join(" ", msg.Skip(4).ToArray());
              RaiseChannelEvent(Channel, CommandStrings.KICK, ActorName, Reason);
            }
            break;
        }
      }
    }

    #region IDisposable implementation
    protected virtual void Dispose(bool itIsSafeToAlsoFreeManagedObjects)
    {
      if(itIsSafeToAlsoFreeManagedObjects)
      {
        var Writer = _Writer;
        if(Writer != null)
        {
          Quit("I'm disposing!!!");
          Thread.Sleep(100);
          Writer.Dispose();
        }

        var Reader = _Reader;
        if (Reader != null)
        {
          Reader.Dispose();
        }

        var Client = _Client;
        if(Client != null) {
          Client.Close();
        }

      }
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }
    #endregion
  }
}

