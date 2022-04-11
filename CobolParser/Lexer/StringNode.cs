using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using DOR.Core;
using DOR.Core.Collections;

namespace CobolParser
{
	public enum StringNodeType
	{
		Quoted,
		Number,
		Word,
		Comma,
		Period,
		Gt,
		Lt,
		Eq,
		LPar,
		RPar,
		Colon,
		Plus,
		Min,
		Slash,
		Star,
		QuestionMark,
		Comment
	}

	public class StringNode : IListNode<StringNode>
	{
		public GuardianPath FileName { get; private set; }
		public int LineNumber { get; private set; }
		public int CharPos { get; private set; }
		public string Str { get; set; }

		public StringNodeType Type { get; private set; }

		public StringNode Next { get; set; }
		public StringNode Prev { get; set; }

		public bool AtEOL { get; set; }

		public StringNode
		(
			GuardianPath fileName, 
			int lineNumber, 
			int charPos,
			string str, 
			StringNodeType type
		)
		{
			FileName = fileName;
			Str = str;
			LineNumber = lineNumber;
			CharPos = charPos;
			Type = type;
		}

		public StringNode(GuardianPath fileName, int lineNumber, int charPos, string str)
		{
			FileName = fileName;
			Str = str;
			LineNumber = lineNumber;
			CharPos = charPos;

			if (str[0] == '"')
			{
				Type = StringNodeType.Quoted;
			}
			else if (str.Length == 1)
			{
				switch (str[0])
				{
					case '?':
						Type = StringNodeType.QuestionMark;
						break;
					case ',':
						Type = StringNodeType.Comma;
						break;
					case ':':
						Type = StringNodeType.Colon;
						break;
					case '>':
						Type = StringNodeType.Gt;
						break;
					case '<':
						Type = StringNodeType.Lt;
						break;
					case '=':
						Type = StringNodeType.Eq;
						break;
					case '(':
						Type = StringNodeType.LPar;
						break;
					case ')':
						Type = StringNodeType.RPar;
						break;
					case '+':
						Type = StringNodeType.Plus;
						break;
					case '-':
						Type = StringNodeType.Min;
						break;
					case '/':
						Type = StringNodeType.Slash;
						break;
					case '*':
						Type = StringNodeType.Star;
						break;
					case '.':
						Type = StringNodeType.Period;
						break;
					default:
						Type = StringNodeType.Word;
						break;
				}
			}
			else if (str.Length > 1)
			{
				Type = StringNodeType.Word;
			}
			if (StringHelper.IsNumeric(str))
			{
				Type = StringNodeType.Number;
			}
		}

		public StringNode()
		{
		}

		public bool StrEquals(string s)
		{
			return Str.Equals(s, StringComparison.InvariantCultureIgnoreCase);
		}

		public StringNode Clone()
		{
			return new StringNode(FileName, LineNumber, CharPos, Str, Type);
		}

		public string Capture(int startCharPos)
		{
			StringBuilder buf = new StringBuilder(CharPos - startCharPos + 2);
			FileStream fs = File.OpenRead(FileName.WindowsFileName());
			fs.Seek(startCharPos, SeekOrigin.Begin);

			for (int pos = startCharPos; pos < CharPos; pos++)
			{
				buf.Append((char)fs.ReadByte());
			}
			fs.Close();

			return buf.ToString();
		}

		public static implicit operator string(StringNode s)
		{
			return s.Str;
		}
	}
}
