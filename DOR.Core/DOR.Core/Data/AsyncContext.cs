using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace DOR.Core.Data
{
	public class AsyncContext
	{
		public object UserData
		{
			get;
			set;
		}

		public Exception AsyncException
		{
			get;
			set;
		}

		public ManualResetEventSlim CompletedEvent
		{
			get;
			private set;
		}

		List<IAsyncResult> _subQueries;
		public IList<IAsyncResult> SubQueries
		{
			get { return _subQueries; }
		}

		public int? CacheDurationInMinutes
		{
			get;
			protected set;
		}

		public AsyncContext()
		{
			CompletedEvent = new ManualResetEventSlim(false);
		}

		public void AddSubQuery(IAsyncResult async)
		{
			if (_subQueries == null)
			{
				_subQueries = new List<IAsyncResult>();
			}

			_subQueries.Add(async);
		}

		public void RethrowAnyAsyncException()
		{
			if (AsyncException != null)
			{
				throw new Exception("See inner exception", AsyncException);
			}
		}

		public static void AddSubQuery(IAsyncResult rootQuery, IAsyncResult subQuery)
		{
			((AsyncContext)rootQuery.AsyncState).AddSubQuery(subQuery);
		}

		public static void SetUserData(IAsyncResult rootQuery, object data)
		{
			((AsyncContext)rootQuery.AsyncState).UserData = data;
		}

		public static object GetUserData(IAsyncResult rootQuery)
		{
			return ((AsyncContext)rootQuery.AsyncState).UserData;
		}

		public static IAsyncResult GetSubQuery(IAsyncResult rootQuery, int idx)
		{
			return ((AsyncContext)rootQuery.AsyncState).SubQueries[idx];
		}

		public static int? GetCacheDurationInMinutes(IAsyncResult rootQuery)
		{
			return ((AsyncContext)rootQuery.AsyncState).CacheDurationInMinutes;
		}
	}
}
