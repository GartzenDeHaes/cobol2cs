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
using System.IO;
using System.Text;

using DOR.Core.Collections;

namespace DOR.Core.IO
{
	public class DelimitedFileReader
	{
		private TextReader m_in;
		private TextWriter m_out;
		private Vector<StringBuilder> m_cols;
		private char m_delim;
		private int m_rowColCount;

		public int ColumnCount
		{
			get { return m_cols.Count; }
		}

		public int CurrentRowColumnCount
		{
			get { return m_rowColCount; }
		}

		public DelimitedFileReader(char delim, int numColumns, TextReader ins, TextWriter outs)
		{
			m_delim = delim;
			m_in = ins;
			m_out = outs;
			m_cols = new Vector<StringBuilder>();
			
			for ( int c = 0; c < numColumns; c++ )
			{
				m_cols.Add( new StringBuilder() );
			}
		}
		
		public bool Next()
		{
			int c;
			for ( c = 0; c < m_cols.Count; c++ )
			{
				if (null != m_out)
				{
					m_out.Write(m_cols.ElementAt(c));
					m_out.Write(m_delim);
				}
				m_cols.ElementAt(c).Length = 0;
			}
			if (null != m_out)
			{
				m_out.WriteLine();
			}

			State state = State.CHARS;
			m_rowColCount = 1;
			c = 0;
			int ch;
			while ( (ch = m_in.Read()) > 0 )
			{
				if (ch == '\r')
				{
					continue;
				}
				if (ch == '\n')
				{
					return true;
				}
				if ( state == State.CHARS )
				{
					if ( ch == m_delim )
					{
						m_rowColCount++;
						c++;
						continue;
					}
					if ( ch == '"' )
					{
						state = State.QUOTE;
						continue;
					}
				}
				else if ( state == State.QUOTE )
				{
					if ( ch == '"' )
					{
						state = State.CHARS;
						continue;
					}
				}
				if ( c >= m_cols.Count )
				{
					m_cols.Add( new StringBuilder() );
				}
				m_cols.ElementAt(c).Append((char)ch);
			}
			return c != 0;
		}
				
		public StringBuilder Column(int idx)
		{
			while ( idx >= m_cols.Count )
			{
				m_cols.Add( new StringBuilder() );
			}
			return m_cols.ElementAt(idx);
		}

		public bool RowHadData
		{
			get
			{
				for (int x = 0; x < m_cols.Count; x++)
				{
					if (m_cols.ElementAt(x).Length > 0)
					{
						return true;
					}
				}
				return false;
			}
		}

		private enum State
		{
			CHARS,
			QUOTE
		}
	}
}
