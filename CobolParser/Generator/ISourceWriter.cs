using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CobolParser.Text
{
	public interface ISourceWriter : IDisposable
	{
		string LineEnding
		{
			get;
			set;
		}

		int IndentLevel
		{
			get;
			set;
		}

		IDictionary<string, string> Variables
		{
			get;
			set;
		}

		ISourceWriter MirrorWriter
		{
			get;
			set;
		}

		void Close();

		IDisposable NewVariableScope();

		IDisposable LockThis();

		IDisposable Indent();

		void IndentManual();

		void Unindent();

		void WriteLine();

		void WriteIndent();

		string WriteLine(string line);

		string Write(string str);

		char Write(char ch);

		string WriteUsing(string ns);

		void WriteDocumentation(string summary, string longDescription);

		void WriteInterfaceWarning();

		void WriteClassWarning();
	}
}
