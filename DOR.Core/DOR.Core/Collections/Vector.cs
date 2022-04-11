/*
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace DOR.Core.Collections
{
	[Serializable]
	public class Vector<T> : IEnumerable
	{
		protected T[] m_data;
		protected int m_used;

		public Vector()
		{
			m_data = new T[10];
		}

		public Vector(int size)
		{
			m_data = new T[size];
		}

		public T Pop()
		{
			if (m_used > 0)
			{
				return m_data[--m_used];
			}
			else
			{
				return default(T);
			}
		}

		public T Peek()
		{
			if (m_used > 0)
			{
				return m_data[m_used-1];
			}
			else
			{
				return default(T);
			}
		}

		public T Head()
		{
			return m_data[0];
		}

		public T Tail()
		{
			return m_data[m_used - 1];
		}

		public int Capacity()
		{
			return m_data.Length;
		}

		public int AddElement(T o)
		{
			Extend();
			m_data[m_used] = o;
			return m_used++;
		}

		public void Add(T o)
		{
			Extend();
			m_data[m_used++] = o;
		}

		public void Add(T[] ta)
		{
			for (int x = 0; x < ta.Length; x++)
			{
				Add(ta[x]);
			}
		}

		public void AddRange(Vector<T> v)
		{
			for (int x = 0; x < v.Count; x++)
			{
				Add(v[x]);
			}
		}

		public void AddRange(T[] v)
		{
			for (int x = 0; x < v.Length; x++)
			{
				Add(v[x]);
			}
		}

		public void AddRangeWithPad(T[] a, int count)
		{
			for (int x = 0; x < count; x++)
			{
				if (x < a.Length)
				{
					Add(a[x]);
				}
				else
				{
					Add(default(T));
				}
			}
		}

		public void Extend()
		{
			if (m_used >= m_data.Length)
			{
				int space = m_data.Length;
				while (m_used >= space)
				{
					space <<= 1;
				}
				T[] array2 = new T[space];
				int i;
				for (i = 0; (i < m_used); i++)
				{
					array2[i] = m_data[i];
				}
				m_data = array2;
			}
		}

		public int Size
		{
			get { return m_used; }
		}

		public int Count
		{
			get { return m_used; }
		}

		public void SetSize(int s)
		{
			if (m_data.Length < s)
			{
				m_data = new T[s];
			}
			m_used = s;
		}

		public void Clear()
		{
			m_used = 0;
		}

		public T ElementAt(int at)
		{
			return m_data[at];
		}

		public T this[int idx]
		{
			get { return ElementAt(idx); }
			set { SetElementAt(idx, value); }
		}

		public void SetElementAt(int at, T o)
		{
			m_data[at] = o;
		}

		public T FirstElement()
		{
			if (m_used > 0)
			{
				return m_data[0];
			}
			else
			{
				return default(T);
			}
		}

		public T LastElement()
		{
			if (m_used > 0)
			{
				return m_data[m_used - 1];
			}
			else
			{
				return default(T);
			}
		}

		public void RemoveElement()
		{
			RemoveElementAt(0);
		}

		public void RemoveElement(T o)
		{
			int i;
			for (i = 0; (i < m_used); i++)
			{
				if (m_data[i].Equals(o))
				{
					RemoveElementAt(i);
					return;
				}
			}
		}

		public int IndexOf(T o)
		{
			int i;
			for (i = 0; (i < m_used); i++)
			{
				if (m_data[i].Equals(o))
				{
					return i;
				}
			}
			return -1;
		}

		public void RemoveElementAt(int at)
		{
			Debug.Assert(m_used > 0);

			int i;
			for (i = at; i < m_used; i++)
			{
				m_data[i] = m_data[i + 1];
			}
			m_used--;
		}

		public void InsertElementAt(T o, int at)
		{
			int i;
			Extend();
			for (i = m_used; (i > at); i--)
			{
				m_data[i] = m_data[i - 1];
			}
			m_data[at] = o;
			m_used++;
		}

		public void Copy(Vector<T> v)
		{
			Clear();
			for (int i = 0; i < v.m_used; i++)
			{
				AddElement(v.ElementAt(i));
			}
		}

		public void InsertBefore(T oNew, T oExisting)
		{
			int at = IndexOf(oExisting);
			if (at < 0)
			{
				throw new Exception("insertBefore can't find before");
				//addElement(oNew);
				//return;
			}
			InsertElementAt(oNew, at);
		}

		public bool IsEmpty()
		{
			return m_used == 0;
		}

		public T[] ToArray()
		{
			T[] ta = new T[Count];
			for (int x = 0; x < ta.Length; x++)
			{
				ta[x] = ElementAt(x);
			}
			return ta;
		}

		public List<T> ToList()
		{
			List<T> l = new List<T>();

			for (int x = 0; x < Count; x++)
			{
				l.Add(ElementAt(x));
			}

			return l;
		}

		public T[] Data()
		{
			return m_data;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return (IEnumerator)GetEnumerator();
		}

		public VectorEnum<T> GetEnumerator()
		{
			return new VectorEnum<T>(m_data, m_used);
		}

		public void Dispose()
		{
			m_data = null;
		}
	}

	public class VectorEnum<T> : IEnumerator
	{
		private T[] _data;
		private int _size; 

		// Enumerators are positioned before the first element
		// until the first MoveNext() call.
		int position = -1;

		public VectorEnum(T[] list, int size)
		{
			_data = list;
			_size = size;
		}

		public bool MoveNext()
		{
			if (position >= _size)
			{
				return false;
			}
			position++;
			return (position < _size);
		}

		public void Reset()
		{
			position = -1;
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
				try
				{
					return _data[position];
				}
				catch (IndexOutOfRangeException)
				{
					throw new InvalidOperationException();
				}
			}
		}
	}
}
