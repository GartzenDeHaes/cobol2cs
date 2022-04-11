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
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DOR.Core.IO
{
	public class DelimitedFile
	{
		protected string m_filename;
		protected string[] m_lines;
		protected List<List<string>> m_rows;
		protected char m_delim;
		protected int m_numColumns;

		public string Filename
		{
			get { return m_filename; }
		}

		public char Delimiter
		{
			get { return m_delim; }
		}

		public int ColumnCount
		{
			get { return m_numColumns; }
		}

		private void Load(string[] lines, char delim)
		{
			m_lines = lines;
			m_delim = delim;
			m_rows = new List<List<string>>();

			for (int lineno = 0; lineno < m_lines.Length; lineno++)
			{
				LoadLine(lines[lineno], lineno);
			}
			if (m_rows.Count > 0)
			{
				m_numColumns = m_rows[0].Count;
			}
		}

		public DelimitedFile(char delim, List<string> lines)
		{
			m_delim = delim;
			m_lines = new string[lines.Count];
			m_rows = new List<List<string>>();

			int lineno = 0;
			foreach (string line in lines)
			{
				m_lines[lineno] = line;
				LoadLine(line, lineno++);
			}
			if (m_rows.Count > 0)
			{
				m_numColumns = m_rows[0].Count;
			}
		}

		public DelimitedFile(char delim, int numColumns)
		{
			m_delim = delim;
			m_numColumns = numColumns;
			m_rows = new List<List<string>>();
		}

		public DelimitedFile(char delim, string[] lines)
		{
			Load(lines, delim);
		}

		public DelimitedFile(char delim, string filename)
		{
			m_filename = filename;
			Load(File.ReadAllLines(filename), delim);
		}

		private void LoadLine(string line, int lineno)
		{
			if (String.IsNullOrWhiteSpace(line))
			{
				return;
			}

			m_rows.Add(new List<string>());
			Debug.Assert(m_rows[lineno] != null);

			int pos = 0;
			int delimpos = 0;
			while (pos < line.Length)
			{
				string data;
				if (line[pos] == '"')
				{
					delimpos = line.IndexOf('"', pos + 1);
					data = StringHelper.MidStr(line, pos + 1, delimpos);
					pos = delimpos + 1;
					if (line[pos] == m_delim)
					{
						pos++;
					}
				}
				else
				{
					if (-1 == (delimpos = line.IndexOf(m_delim, pos)))
					{
						data = line.Substring(pos);
						pos = line.Length;
					}
					else
					{
						data = StringHelper.MidStr(line, pos, delimpos).Trim();
						pos = delimpos + 1;
					}
				}
				m_rows[lineno].Add(data);
			}

			if (line[line.Length - 1] == m_delim)
			{
				m_rows[lineno].Add("");
			}
		}

		public List<List<string>> Rows
		{
			get { return m_rows; }
		}

		public void AddRow()
		{
			List<string> row = new List<string>();
			for (int c = 0; c < m_numColumns; c++)
			{
				row.Add("");
			}
			m_rows.Add(row);
		}

		public void AddRow(List<string> row)
		{
			List<string> nrow = new List<string>();
			for (int c = 0; c < row.Count; c++)
			{
				nrow.Add(row[c]);
			}
			m_rows.Add(nrow);
		}

		public void InsertRowAt(int idx)
		{
			if (idx < 0 || idx >= m_rows.Count)
			{
				throw new IndexOutOfRangeException();
			}
			List<string> row = new List<string>();
			for (int c = 0; c < m_numColumns; c++)
			{
				row.Add("");
			}
			m_rows.Insert(idx, row);
		}

		public int Count
		{
			get { return m_rows.Count; }
		}

		public void Write(string filename)
		{
			StreamWriter sw = new StreamWriter(File.OpenWrite(filename));
			for (int r = 0; r < m_rows.Count; r++)
			{
				for (int c = 0; c < m_rows[r].Count; c++)
				{
					if (c > 0)
					{
						sw.Write(m_delim);
					}
					string coldata = m_rows[r][c];
					if (coldata.IndexOf(m_delim) > -1)
					{
						sw.Write('"');
						sw.Write(coldata);
						sw.Write('"');
					}
					else
					{
						sw.Write(coldata);
					}
				}
				sw.WriteLine();
			}
			sw.Close();
		}

		public static DelimitedFile Parse(char delim, string txt)
		{
			StringBuilder buf = new StringBuilder();
			List<string> lines = new List<string>();

			for (int x = 0; x < txt.Length; x++)
			{
				char ch = txt[x];
				if (ch == '\r')
				{
					continue;
				}
				if (ch == '\n')
				{
					lines.Add(buf.ToString());
					buf.Clear();
				}
				else
				{
					buf.Append(ch);
				}
			}

			return new DelimitedFile(delim, lines);
		}
	}
}
