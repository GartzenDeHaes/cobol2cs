using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Collections;

namespace DOR.Core.Collections
{
	/// <summary>
	/// This class is for programs that need access to the internal node pointers
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class SimpleList<T> : IEnumerable where T : class, IListNode<T>, new()
	{
		public T HeadSentinal
		{
			get;
			private set;
		}

		public T TailSentinal
		{
			get;
			private set;
		}

		public T First
		{
			get
			{
				if (HeadSentinal.Next == TailSentinal)
				{
					return default(T);
				}
				return (T)HeadSentinal.Next;
			}
		}

		public T Last
		{
			get
			{
				if (TailSentinal.Prev == HeadSentinal)
				{
					return default(T);
				}
				return (T)TailSentinal.Prev;
			}
		}

		public T Current
		{
			get;
			private set;
		}

		public bool IsEmpty
		{
			get { return HeadSentinal.Next == TailSentinal; }
		}

		public int Count
		{
			get;
			private set;
		}

		public SimpleList()
		{
			Count = 0;

			HeadSentinal = new T();
			TailSentinal = new T();

			HeadSentinal.Next = TailSentinal;
			TailSentinal.Prev = HeadSentinal;
		}

		public void Add(T t)
		{
			t.Next = TailSentinal;
			t.Prev = TailSentinal.Prev;
			TailSentinal.Prev.Next = t;
			TailSentinal.Prev = t;

			Count++;
		}

		public void AddAtHead(T t)
		{
			t.Prev = HeadSentinal;
			t.Next = HeadSentinal.Next;
			t.Next.Prev = t;
			HeadSentinal.Next = t;

			Count++;
		}

		public void Remove(T t)
		{
			Debug.Assert(t.Prev.Next == t);
			Debug.Assert(t.Next.Prev == t);

			if (t == HeadSentinal || t == TailSentinal)
			{
				return;
			}

			t.Prev.Next = t.Next;
			t.Next.Prev = t.Prev;
			t.Next = null;
			t.Prev = null;

			Count--;
		}

		public void Remove()
		{
			T newCurrent = (T)Current.Next;
			Remove(Current);
			Current = newCurrent;
		}

		public void Insert(T t)
		{
			t.Next = Current;
			t.Prev = Current.Prev;
			t.Prev.Next = t;
			Current.Prev = t;

			Current = t;

			Count++;

			Debug.Assert(t.Prev.Next == t);
			Debug.Assert(t.Next.Prev == t);
		}

		public void BeginIteration()
		{
			Current = HeadSentinal;
		}

		public bool IsIterationComplete
		{
			get { return null == Current || Current == TailSentinal; }
		}

		public bool Next()
		{
			if (null == Current || Current == TailSentinal)
			{
				return false;
			}

			Current = Current.Next;
			Debug.Assert(Current.Prev.Next == Current);
			Debug.Assert(Current == TailSentinal || Current.Next.Prev == Current);

			return Current != TailSentinal;
		}

		public bool Prev()
		{
			if (null == Current || Current == HeadSentinal)
			{
				return false;
			}

			Current = Current.Prev;
			Debug.Assert(Current == HeadSentinal || Current.Prev.Next == Current);
			Debug.Assert(Current.Next.Prev == Current);

			return Current != HeadSentinal;
		}

		public T ElementAtFromNode(T node, int count)
		{
			Debug.Assert(count > -1);

			while (node != TailSentinal && --count >= 0)
			{
				node = node.Next;
			}

			if (count < 0)
			{
				return (T)node;
			}

			return null;
		}

		public T ElementAtFromHead(int count)
		{
			Debug.Assert(count > -1);

			IListNode<T> node = HeadSentinal.Next;
			while (node != TailSentinal && --count >= 0)
			{
				node = node.Next;
			}

			if (count < 0)
			{
				return (T)node;
			}

			return null;
		}

		public T ElementAtFromTail(int count)
		{
			Debug.Assert(count > -1);

			IListNode<T> node = TailSentinal.Prev;
			while (node != HeadSentinal && --count >= 0)
			{
				node = node.Prev;
			}

			if (count < 0)
			{
				return (T)node;
			}

			return null;
		}

		public void PopHead()
		{
			Remove(HeadSentinal.Next);
		}

		public void PopTail()
		{
			Remove(TailSentinal.Prev);
		}

		public SimpleList<T> Extract(T from, T to)
		{
			SimpleList<T> lst = new SimpleList<T>();

			from.Prev.Next = to.Next;
			to.Next.Prev = from.Prev;

			from.Prev = lst.HeadSentinal;
			lst.HeadSentinal.Next = from;

			to.Next = lst.TailSentinal;
			lst.TailSentinal.Prev = to;

			AssertListIntegrety();
			lst.AssertListIntegrety();

			return lst;
		}

		public void InjectAfterCurrent(SimpleList<T> lst)
		{
			lst.AssertListIntegrety();

			if (lst.Count == 0)
			{
				return;
			}

			T next = Current.Next;
			Current.Next = lst.First;
			lst.First.Prev = Current;

			next.Prev = lst.Last;
			lst.Last.Next = next;

			AssertListIntegrety();
		}

		public void InsertAfterCurrent(T node)
		{
			node.Next = Current.Next;
			node.Next.Prev = node;
			node.Prev = Current;
			Current.Next = node;

			AssertListIntegrety();
		}

		public void AssertListIntegrety()
		{
#if DEBUG
			Debug.Assert(HeadSentinal.Prev == null);
			Debug.Assert(TailSentinal.Next == null);

			T n = HeadSentinal.Next;

			while (n != TailSentinal)
			{
				Debug.Assert(n.Prev.Next == n);
				Debug.Assert(n.Next.Prev == n);
				n = n.Next;
			}
#endif
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return (IEnumerator)GetEnumerator();
		}

		public SimpleListEnum<T> GetEnumerator()
		{
			return new SimpleListEnum<T>(HeadSentinal, TailSentinal);
		}
	}

	public class SimpleListEnum<T> : IEnumerator where T : class, IListNode<T>, new()
	{
		private T _head;
		private T _tail;
		private T _current;

		public SimpleListEnum(T head, T tail)
		{
			_head = head;
			_tail = tail;
			_current = head;
		}

		public bool MoveNext()
		{
			if (_current == _tail)
			{
				return false;
			}
			_current = _current.Next;
			return _tail != _current;
		}

		public void Reset()
		{
			_current = _head;
		}

		object IEnumerator.Current
		{
			get
			{
				return Current;
			}
		}

		public T Current
		{
			get
			{
				Debug.Assert(_current != _tail);
				return _current == _tail ? null : _current;
			}
		}
	}
}
