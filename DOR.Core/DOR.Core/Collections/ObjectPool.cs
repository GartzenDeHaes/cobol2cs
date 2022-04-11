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
using System.Collections.Generic;
using System.Text;

namespace DOR.Core.Collections
{
	public class ObjectPool<T> where T : new()
	{
		private Stack<T> m_pool = new Stack<T>();

		public ObjectPool()
		{
			for (int x = 0; x < 5; x++)
			{
				m_pool.Push(new T());
			}
		}

		public T Get()
		{
			lock (m_pool)
			{
				if (m_pool.Count == 0)
				{
					return new T();
				}
				return m_pool.Pop();
			}
		}

		public void Release(T t)
		{
			lock (m_pool)
			{
				m_pool.Push(t);
			}
		}
	}
}
