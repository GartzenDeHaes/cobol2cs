using System;
using System.Collections.Generic;
using System.IO;

using DOR.Core;

namespace CobolParser.Text
{
    public class SourceWriter : SourceWriterBase
    {
        private TextWriter _textWriter;

		public SourceWriter(string filename)
		: base()
		{
			_textWriter = new StreamWriter(File.OpenWrite(filename));
		}

        public SourceWriter(TextWriter textWriter)
		: base()
		{
            _textWriter = textWriter;
        }

		public override void Close()
		{
			_textWriter.Close();
			if (MirrorWriter != null)
			{
				MirrorWriter.Close();
			}
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

			_textWriter.Write(ReplaceVariables(str));

			return str;
		}

		public override void Dispose()
		{
			_textWriter.Close();
			_variables.Clear();
			_textWriter.Dispose();
		}
    }
}
