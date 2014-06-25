//
//  RezBotAsyncResult.cs
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
using System.Threading;

namespace G6RezBot.Plugin
{
  public class AsyncResult<T> : MarshalByRefObject, IAsyncResult, IDisposable
  {
    private readonly AsyncCallback _Callback;
    private bool _Completed;
    private bool _CompletedSynchronously;
    private readonly object _AsyncState;
    private readonly ManualResetEvent _WaitHandle;
    private T _Result;
    private Exception _e;
    private readonly object _SyncRoot;

    public AsyncResult(AsyncCallback cb, object state)
      : this(cb, state, false)
    {
    }

    public AsyncResult(AsyncCallback cb, object state,
      bool completed)
    {
      this._Callback = cb;
      this._AsyncState = state;
      this._Completed = completed;
      this._CompletedSynchronously = completed;

      this._WaitHandle = new ManualResetEvent(false);
      this._SyncRoot = new object();
    }

    #region IAsyncResult Members

    public object AsyncState
    {
      get { return this._AsyncState; }
    }

    public WaitHandle AsyncWaitHandle
    {
      get { return this._WaitHandle; }
    }

    public bool CompletedSynchronously
    {
      get 
      {
        lock (this._SyncRoot)
        {
          return this._CompletedSynchronously;
        }
      }
    }

    public bool IsCompleted
    {
      get 
      {
        lock (this._SyncRoot)
        {
          return this._Completed;
        }
      }
    }

    #endregion

    #region IDisposable Members

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize(this);
    }

    #endregion

    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
      {
        lock (this._SyncRoot)
        {
          if (this._WaitHandle != null)
          {
            ((IDisposable)this._WaitHandle).Dispose();
          }
        }
      }
    }

    public Exception Exception
    {
      get
      {
        lock (this._SyncRoot)
        {
          return this._e;
        }
      }
    }

    public T Result
    {
      get 
      {
        lock (this._SyncRoot)
        {
          return this._Result;
        }
      }
    }

    public void Complete(T result, bool completedSynchronously)
    {
      lock (this._SyncRoot)
      {
        this._Completed = true;
        this._CompletedSynchronously = completedSynchronously;
        this._Result = result;
      }

      this.SignalCompletion();
    }

    public void HandleException(Exception e, bool completedSynchronously)
    {
      lock (this._SyncRoot)
      {
        this._Completed = true;
        this._CompletedSynchronously = completedSynchronously;
        this._e = e;
      }

      this.SignalCompletion();
    }

    private void SignalCompletion()
    {
      this._WaitHandle.Set();

      ThreadPool.QueueUserWorkItem(new WaitCallback(this.InvokeCallback));
    }

    private void InvokeCallback(object state)
    {
      if (this._Callback != null)
      {
        this._Callback(this);
      }
    }
  }
}

