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
	public class DispMsg
	{
		public object Context;
		public MethodInfo MethodInfo;
		public object[] Parameters;
		public BlockingCollection<object> Result = new BlockingCollection<object>(1);
	}

	public class ApartmentThreadedObject : ThreadedMessageQueue<DispMsg>
	{
		private bool _isAsync;

		/// <summary>
		/// A thread safe execution disptcher.  NOTE: this uses reflection to make its calls.
		/// </summary>
		/// <example>
		/// <code>
		///	public class ObjectContextApartment : ApartmentThreadedObject
		///	{
		///		private ObjectContext _ctx;
		///
		/// 	public void AcceptAllChanges()
		///		{
		///			Dispatch(_ctx, "AcceptAllChanges");
		///		}
		/// }
		/// </code>
		/// </example>
		/// <remarks>Using async can cause the caller to miss exceptions thrown by the dispatch target.</remarks>
		/// <param name="isAsync">If true, the calling thread will not wait for results on the call where there is no return value.</param>
		protected ApartmentThreadedObject(bool isAsync = false)
		{
			_isAsync = isAsync;
		}

		protected override void Receive(DispMsg msg)
		{
			msg.Result.Add(msg.MethodInfo.Invoke(msg.Context, msg.Parameters));
		}

		protected override void Error(DispMsg msg, Exception ex)
		{
			msg.Result.Add(ex);
		}

		protected T Dispatch<T>(DispMsg msg)
		{
			Enqueue(msg);
			object o = msg.Result.Take();
			if (o is Exception)
			{
				throw new Exception("Call failed, see inner exception for details", (Exception)o);
			}
			return (T)o;
		}

		protected void Dispatch(DispMsg msg)
		{
			Enqueue(msg);

			// An optimization would be to remove this result check.  However,
			// I'm not sure if that would cause synchronization problems (context
			// not being in the expected state after a call is omplete).
			if (_isAsync)
			{
				object o = msg.Result.Take();
				if (o is Exception)
				{
					throw new Exception("Call failed, see inner exception for details", (Exception)o);
				}
			}
		}

		protected void Dispatch(object context, MethodInfo method, object[] parameters)
		{
			DispMsg msg = new DispMsg();
			msg.Context = context;
			msg.Parameters = parameters;
			msg.MethodInfo = method;
			Dispatch(msg);
		}

		protected T Dispatch<T>(object context, MethodInfo method, object[] parameters)
		{
			DispMsg msg = new DispMsg();
			msg.Context = context;
			msg.Parameters = parameters;
			msg.MethodInfo = method;
			return Dispatch<T>(msg);
		}

		protected T Dispatch<T>(object context, string methodName, object[] parameters)
		{
			MethodInfo method;
			if (null == parameters)
			{
				method = context.GetType().GetMethod(methodName);
			}
			else
			{
				method = context.GetType().GetMethod(methodName, (from o in parameters select o.GetType()).ToArray());
			}
			return Dispatch<T>(context, method, parameters);
		}

		protected void Dispatch(object context, string methodName, object[] parameters)
		{
			MethodInfo method;
			if (null == parameters)
			{
				method = context.GetType().GetMethod(methodName);
			}
			else
			{
				method = context.GetType().GetMethod(methodName, (from o in parameters select o.GetType()).ToArray());
			}
			Dispatch(context, method, parameters);
		}

		protected MethodInfo Search4Method(object context, string methodName, Type returnType, object[] parameters)
		{
			MethodInfo[] methods = (from m in context.GetType().GetMethods() where m.Name == methodName select m).ToArray();

			foreach (MethodInfo m in methods)
			{
				ParameterInfo[] mp = m.GetParameters();
				if (null == parameters && mp.Length != 0)
				{
					continue;
				}
				if (null == parameters)
				{
					if (mp.Length != 0)
					{
						continue;
					}
				}
				else
				{
					if (mp.Length != parameters.Length)
					{
						continue;
					}
				}
				if (m.IsGenericMethodDefinition && !m.ReturnType.BaseType.Equals(returnType.BaseType))
				{
					continue;
				}
				if (!m.IsGenericMethodDefinition && !m.ReturnType.Equals(returnType))
				{
					continue;
				}
				for (int x = 0; null != parameters && x < parameters.Length; x++)
				{
					if (!mp[x].ParameterType.Equals(parameters[x].GetType()))
					{
						continue;
					}
				}
				return m;
			}
			throw new ArgumentException("Cannot locate method " + methodName);
		}

		protected T DispatchSearch<T>(object context, string methodName, object[] parameters)
		{
			MethodInfo theMethod = Search4Method(context, methodName, typeof(T), parameters);
			return Dispatch<T>(context, theMethod, parameters);
		}

		protected T DispatchTemplated<T>(object context, string methodName, Type templType)
		{
			return DispatchTemplated<T>(context, methodName, templType, null);
		}

		protected T DispatchTemplated<T>(object context, string methodName, Type templType, object[] parameters)
		{
			MethodInfo method;
			if (null == parameters)
			{
				method = context.GetType().GetMethod(methodName);
			}
			else
			{
				Type[] ptypes = (from o in parameters select o.GetType()).ToArray();
				method = context.GetType().GetMethod(methodName, ptypes);
			}
			method = method.MakeGenericMethod(new Type[] { templType });
			return Dispatch<T>(context, method, parameters);
		}

		protected T DispatchTemplatedSearch<T>(object context, string methodName, Type templType, object[] parameters)
		{
			MethodInfo theMethod = Search4Method(context, methodName, typeof(T), parameters);
			theMethod = theMethod.MakeGenericMethod(new Type[] { templType });
			return Dispatch<T>(context, theMethod, parameters);
		}

		protected void DispatchTemplated(object context, string methodName, Type templType, object[] parameters)
		{
			MethodInfo method;
			if (null == parameters)
			{
				method = context.GetType().GetMethod(methodName);
			}
			else
			{
				method = context.GetType().GetMethod(methodName, (from o in parameters select o.GetType()).ToArray());
			}
			method = method.MakeGenericMethod(new Type[] { templType });
			Dispatch(context, method, parameters);
		}

		protected T Dispatch<T>(object context, string methodName)
		{
			return Dispatch<T>(context, methodName, null);
		}

		protected T DispatchSearch<T>(object context, string methodName)
		{
			return DispatchSearch<T>(context, methodName, null);
		}

		protected void Dispatch(object context, string methodName)
		{
			Dispatch(context, methodName, null);
		}

		protected T Dispatch<T>(object context, string methodName, object arg1)
		{
			return Dispatch<T>(context, methodName, new object[] { arg1 });
		}

		protected T Dispatch<T>(object context, string methodName, object arg1, object arg2)
		{
			return Dispatch<T>(context, methodName, new object[] { arg1, arg2 });
		}

		protected T DispatchSearch<T>(object context, string methodName, object arg1, object arg2)
		{
			return DispatchSearch<T>(context, methodName, new object[] { arg1, arg2 });
		}

		protected T Dispatch<T>(object context, string methodName, object arg1, object arg2, object arg3)
		{
			return Dispatch<T>(context, methodName, new object[] { arg1, arg2, arg3 });
		}

		protected void Dispatch(object context, string methodName, object arg1)
		{
			Dispatch(context, methodName, new object[] { arg1 });
		}

		protected void Dispatch(object context, string methodName, object arg1, object arg2)
		{
			Dispatch(context, methodName, new object[] { arg1, arg2 });
		}

		protected void Dispatch(object context, string methodName, object arg1, object arg2, object arg3)
		{
			Dispatch(context, methodName, new object[] { arg1, arg2, arg3 });
		}

		protected T DispatchTemplated<T>(object context, string methodName, Type templType, object arg1)
		{
			return DispatchTemplated<T>(context, methodName, templType, new object[] { arg1 });
		}

		protected T DispatchTemplated<T>(object context, string methodName, Type templType, object arg1, object arg2)
		{
			return DispatchTemplated<T>(context, methodName, templType, new object[] { arg1, arg2 });
		}

		protected T DispatchTemplatedSearch<T>(object context, string methodName, Type templType, object arg1, object arg2)
		{
			return DispatchTemplatedSearch<T>(context, methodName, templType, new object[] { arg1, arg2 });
		}

		protected T DispatchTemplated<T>(object context, string methodName, Type templType, object arg1, object arg2, object arg3)
		{
			return DispatchTemplated<T>(context, methodName, templType, new object[] { arg1, arg2, arg3 });
		}

		protected T DispatchTemplatedSearch<T>(object context, string methodName, Type templType, object arg1, object arg2, object arg3)
		{
			return DispatchTemplatedSearch<T>(context, methodName, templType, new object[] { arg1, arg2, arg3 });
		}

		protected T DispatchTemplated<T>(object context, string methodName, Type templType, object arg1, object arg2, object arg3, object arg4)
		{
			return DispatchTemplated<T>(context, methodName, templType, new object[] { arg1, arg2, arg3, arg4 });
		}

		protected void DispatchTemplated(object context, string methodName, Type templType, object arg1)
		{
			DispatchTemplated(context, methodName, templType, new object[] { arg1 });
		}

		protected void DispatchTemplated(object context, string methodName, Type templType, object arg1, object arg2)
		{
			DispatchTemplated(context, methodName, templType, new object[] { arg1, arg2 });
		}

		protected void DispatchTemplated(object context, string methodName, Type templType, object arg1, object arg2, object arg3)
		{
			DispatchTemplated(context, methodName, templType, new object[] { arg1, arg2, arg3 });
		}

		protected void DispatchTemplated(object context, string methodName, Type templType, object arg1, object arg2, object arg3, object arg4)
		{
			DispatchTemplated(context, methodName, templType, new object[] { arg1, arg2, arg3, arg4 });
		}

		public void DispatchPropertySet<T>(object context, string propName, T value)
		{
			DispMsg msg = new DispMsg();
			msg.Context = context;
			msg.Parameters = new object[] { value };
			msg.MethodInfo = context.GetType().GetProperty(propName, typeof(T)).GetSetMethod(); ;
			Dispatch(msg);
		}
	}
}
