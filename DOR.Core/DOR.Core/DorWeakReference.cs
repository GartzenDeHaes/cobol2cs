using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DOR.Core
{
	/// <summary>
	/// Allows implicit casting of the weak reference.
	/// </summary>
	/// <remarks>The purpose of a weak reference is to avoid circular references that
	/// prevent garbage collection.</remarks>
	/// <typeparam name="T"></typeparam>
	public class DorWeakReference<T>
	{
		private WeakReference _ref;

		public DorWeakReference(T t)
		{
			_ref = new WeakReference(t, false);
		}

		public T Get()
		{
			return (T)_ref.Target;
		}

		public static explicit operator T(DorWeakReference<T> t)
		{
			return (T)t._ref.Target;
		}
	}
}
