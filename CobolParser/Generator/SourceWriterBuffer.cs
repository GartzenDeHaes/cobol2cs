using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CobolParser.Text;

namespace CobolParser.Text
{
	public class SourceWriterBuffer : SourceWriterBase
	{
		private StringBuilder _buf;

		public SourceWriterBuffer()
		{
			_buf = new StringBuilder();
		}

		public SourceWriterBuffer(StringBuilder buf)
		{
			_buf = buf;
		}

		public override void Close()
		{
		}

		public override string Write(string str)
		{
			if (str == null)
			{
				return str;
			}

			if (MirrorWriter != null)
			{
				MirrorWriter.Write(str);
			}

			_buf.Append(ReplaceVariables(str));

			return str;
		}

		public override string ToString()
		{
			string s = _buf.ToString();
			IndentLevel = 0;
			_buf.Clear();
			return s;
		}

		public override void Dispose()
		{
		}
	}
}
