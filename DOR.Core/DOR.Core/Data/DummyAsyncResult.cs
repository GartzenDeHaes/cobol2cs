using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace DOR.Core.Data
{
	public class DummyAsyncResult : IAsyncResult
	{
		AsyncContext _ctx;

		public Object AsyncState 
		{
			get { return _ctx; }
		}

		public WaitHandle AsyncWaitHandle 
		{
			get { return null; }
		}

		public bool CompletedSynchronously 
		{
			get { return true; }
		}

		public bool IsCompleted 
		{
			get { return true; }
		}

		public DummyAsyncResult(AsyncContext ctx)
		{
			_ctx = ctx;
			_ctx.CompletedEvent.Set();
		}
	}
}
