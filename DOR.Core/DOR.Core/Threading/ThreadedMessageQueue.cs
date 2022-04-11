using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Data.EntityClient;
using System.Data.Metadata.Edm;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;

namespace DOR.Core.Threading
{
	public class ThreadedMessageQueue<T> : IDisposable
	{
		private Thread _myThread;
		private volatile bool _running;
		private BlockingCollection<T> _q = new BlockingCollection<T>(50);

		public ThreadPriority Priority
		{
			get { return _myThread.Priority; }
			set { _myThread.Priority = value; }
		}

		public bool IsRunning
		{
			get { return _running; }
		}

		protected ThreadedMessageQueue()
		{
			_myThread = new Thread(new ThreadStart(Run));
			_myThread.IsBackground = true;
			_myThread.Start();
		}

		private void Run()
		{
			_running = true;

			while (_running)
			{
				try
				{
					T msg = _q.Take();
					if (!_running)
					{
						return;
					}

					try
					{
						Receive(msg);
					}
					catch (Exception ex)
					{
						Error(msg, ex);
					}
				}
				catch (InvalidOperationException)
				{
					Debug.Assert(!_running);
					return;
				}
			}
		}

		public void Enqueue(T msg)
		{
			_q.Add(msg);
		}

		protected virtual void Receive(T msg)
		{
			throw new NotImplementedException();
		}

		protected virtual void Error(T msg, Exception ex)
		{
			throw new NotImplementedException();
		}

		public void Stop()
		{
			if (_running)
			{
				_running = false;
				_q.CompleteAdding();
			}
		}

		//
		// Summary:
		//     Releases the resources used by the object context.
		public virtual void Dispose()
		{
			Dispose(true);
		}

		//
		// Summary:
		//     Releases the resources used by the object context.
		//
		// Parameters:
		//   disposing:
		//     true to release both managed and unmanaged resources; false to release only
		//     unmanaged resources.
		/*virtual*/
		public virtual void Dispose(bool disposing)
		{
			Stop();
		}
	}
}
