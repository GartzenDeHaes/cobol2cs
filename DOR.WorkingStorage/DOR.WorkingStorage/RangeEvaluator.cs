using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DOR.WorkingStorage
{
	public class RangeEvaluator
	{
		public long Low
		{
			get;
			private set;
		}

		public long High
		{
			get;
			private set;
		}

		public RangeEvaluator(long low, long hi)
		{
			Low = low;
			High = hi;
		}

		public override bool Equals(object obj)
		{
			if (obj is long)
			{
				return (long)obj >= Low && (long)obj <= High;
			}

			IBufferOffset o = obj as IBufferOffset;
			if ((object)o != null)
			{
				return Equals(o.ToInt());
			}

			WsRecord w = obj as WsRecord;
			if ((object)w != null)
			{
				return Equals(w.ToInt());
			}

			throw new ArgumentException("Don't know how to range " + obj.ToString());
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
