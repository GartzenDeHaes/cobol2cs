using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

using DOR.Core.Collections;

namespace CobolParser
{
	public class Terminalize
	{
		private static readonly char[] _stopChars = new char[] { '\'', '"', '?', ':', ' ', '(', ')', ',', '/', '*', '=', '(', ')', '<', '>', '\r', '\n', '\t'};

		private SimpleList<StringNode> _words = new SimpleList<StringNode>();
		private GuardianPath _fileName;
		private int _lineNumber = 1;
		private List<SyntaxWarning> _warnings = new List<SyntaxWarning>();

		public bool SkipComments
		{
			get;
			set;
		}

		public GuardianPath FileName
		{
			get { return _fileName; }
		}

		public int LineCount
		{
			get { return _lineNumber; }
		}

		public IList<SyntaxWarning> Warnings
		{
			get { return _warnings; }
		}

		public Terminalize(GuardianPath fileName)
		{
			_fileName = fileName;
			SkipComments = true;
		}

		private char _prevChar = '\n';
		private char _curChar = '\n';
		private char[] _buf = new char[1];
		private int _charPos = -1;
		private int _charCount = 0;

		private int Read(TextReader reader)
		{
			int ret = reader.Read(_buf, 0, 1);
			_prevChar = _curChar;
			_curChar = _buf[0];

			if (ret > 0)
			{
				_charCount++;
			}

			return ret;
		}

		public Terminalize(GuardianPath fileName, TextReader reader)
		: this(fileName)
		{
			Parse(reader);
		}

		public Terminalize(GuardianPath fileName, string txt)
		: this(fileName)
		{
			Parse(new StringReader(txt));
		}

		public void Parse(TextReader reader)
		{
			char quoteChar = '"';
			StringBuilder wbuf = new StringBuilder();
			bool readingString = false;

			_charPos = 0;

			while (0 != Read(reader))
			{
				if ('\r' == _curChar)
				{
					continue;
				}
				if (readingString)
				{
					if (_curChar == '\n')
					{
						_lineNumber++;

						Read(reader);
						if (_curChar != '-')
						{
							throw new SyntaxError(_fileName, _lineNumber, "Expected line continuation for string");
						}

						while (_curChar != '"')
						{
							Read(reader);
						}
						Read(reader);
					}
					wbuf.Append(_curChar);
					if (quoteChar == _curChar)
					{
						AddWord(wbuf, _lineNumber);
						readingString = false;
					}
					continue;
				}
				if 
				(
					wbuf.Length == 0 && 
					_prevChar == '\n' &&
					(
						_curChar == '*' || 
						_curChar == '/' || 
						_curChar == '!'
					)
				)
				{
					while (_curChar != '\n' && _curChar != '\r')
					{
						wbuf.Append(_curChar);
						if (1 != Read(reader))
						{
							break;
						}
					}

					if (wbuf.Length == 2 && wbuf[1] == '.')
					{
						wbuf.Length = 1;
						AddWord(wbuf, _lineNumber, StringNodeType.Star);
						wbuf.Append('.');
						AddWord(wbuf, _lineNumber, StringNodeType.Period);
					}
					else
					{
						AddWord(wbuf, _lineNumber, StringNodeType.Comment);
					}

					if (_curChar != '\r')
					{
						_lineNumber++;
						if (_words.Count > 0)
						{
							_words.Last.AtEOL = true;
						}
					}
					continue;
				}

				if (_stopChars.Contains(_curChar))
				{
					if ('\n' == _curChar || ' ' == _curChar || '\t' == _curChar)
					{
						if (wbuf.Length > 0 && wbuf[wbuf.Length - 1] == '.')
						{
							wbuf.Length -= 1;
							if (wbuf.Length > 0)
							{
								AddWord(wbuf, _lineNumber);
							}
							wbuf.Append('.');
							AddWord(wbuf, _lineNumber);
							if ('\n' == _curChar)
							{
								_lineNumber++;
								if (_words.Count > 0)
								{
									_words.Last.AtEOL = true;
								}
							}
							continue;
						}
					}
					if (wbuf.Length > 0)
					{
						AddWord(wbuf, _lineNumber);
					}
					if ('\n' == _curChar)
					{
						_lineNumber++;
						if (_words.Count > 0)
						{
							_words.Last.AtEOL = true;
						}
					}
					else if ('"' == _curChar || '\'' == _curChar)
					{
						readingString = true;
						quoteChar = _curChar;
						wbuf.Append(_curChar);
					}
					else if (!Char.IsWhiteSpace(_curChar))
					{
						wbuf.Append(_curChar);
						AddWord(wbuf, _lineNumber);
					}
				}
				else
				{
					wbuf.Append(_curChar);
				}
			}
		}

		public bool Match(StringNodeType type)
		{
			while (SkipComments && Current.Type == StringNodeType.Comment)
			{
				if (!Next())
				{
					return false;
				}
			}

			if (Current.Type != type)
			{
				throw new SyntaxError(Current.FileName, Current.LineNumber, "Expected " + type.ToString());
			}
			return Next();
		}

		public bool Match(string lex)
		{
			while (SkipComments && Current.Type == StringNodeType.Comment)
			{
				if (!Next())
				{
					return false;
				}
			}

			if (!CurrentEquals(lex))
			{
				throw new SyntaxError(_fileName, Current.LineNumber, "Expected " + lex + ", found " + Current.Str);
			}
			return Next();
		}

		public bool MatchOptional(StringNodeType type, string warning = null)
		{
			while (SkipComments && Current.Type == StringNodeType.Comment)
			{
				if (!Next())
				{
					return false;
				}
			}

			if (Current.Type == type)
			{
				return Next();
			}
			else if (warning != null)
			{
				_warnings.Add(new SyntaxWarning(Current, warning));
			}

			return true;
		}

		public bool MatchOptional(string lex, string warning = null)
		{
			while (SkipComments && Current.Type == StringNodeType.Comment)
			{
				if (!Next())
				{
					return false;
				}
			}

			if (CurrentEquals(lex))
			{
				return Next();
			}
			else if (warning != null)
			{
				_warnings.Add(new SyntaxWarning(Current, warning));
			}

			return true;
		}

		private void AddWord(StringBuilder bword, int lineNumber)
		{
			string word = bword.ToString();
			if (word == "\"\"" && _words.Last.Str == "\"\"")
			{
				_words.Last.Str = "\"\"\"\"";
			}
			else
			{
				_words.Add(new StringNode(_fileName, lineNumber, _charPos, word));
				_charPos = _charCount;
			}

			bword.Length = 0;
		}

		private void AddWord(StringBuilder word, int lineNumber, StringNodeType type)
		{
			StringNode node = new StringNode(_fileName, lineNumber, _charPos, word.ToString(), type);
			_charPos = _charCount;
			_words.Add(node);
			word.Length = 0;
		}

		public void BeginIteration()
		{
			_words.BeginIteration();
		}

		public bool Next()
		{
			if (!_words.Next())
			{
				return false;
			}

			while (SkipComments && Current.Type == StringNodeType.Comment)
			{
				if (!_words.Next())
				{
					return false;
				}
			}

			while (HandleCompilerDirIf())
			{
				while (SkipComments && Current.Type == StringNodeType.Comment)
				{
					if (!_words.Next())
					{
						return false;
					}
				}
			}

			return ! _words.IsIterationComplete;
		}

		public bool Prev()
		{
			return _words.Prev();
		}

		public bool IsIterationComplete
		{
			get { return _words.IsIterationComplete; }
		}

		public StringNode Current
		{
			get { return _words.Current; }
		}

		public int CurrentLineNumber
		{
			get { return _words.Current.LineNumber; }
		}

		public StringNode Last
		{
			get { return _words.Last; }
		}

		public StringNode First
		{
			get { return _words.First; }
		}

		public bool CurrentEquals(string s)
		{
			return Current.Str.Equals(s, StringComparison.InvariantCultureIgnoreCase);
		}

		public bool CurrentNextEquals(int pos, string s)
		{
			if (null == CurrentNext(pos))
			{
				return false;
			}
			return CurrentNext(pos).Str.Equals(s, StringComparison.InvariantCultureIgnoreCase);
		}

		public bool CurrentNextEquals(string s)
		{
			return CurrentNextEquals(1, s);
		}

		public void Insert(StringNode node)
		{
			_words.Insert(node);
			_words.AssertListIntegrety();
		}

		public void Remove()
		{
			_words.Remove();
			_words.AssertListIntegrety();
		}

		public void InsertAfterCurrent(StringNode node)
		{
			_words.InsertAfterCurrent(node);
		}

		public void InjectAfterCurrent(Terminalize terms)
		{
			_words.InjectAfterCurrent(terms._words);
		}

		public void Add(StringNode node)
		{
			_words.Add(node);
		}

		public StringNode CurrentNext(int pos)
		{
			return _words.ElementAtFromNode(Current, pos);
		}

		public void ReadSentence(Vector<StringNode> lst)
		{
			lst.Clear();

			while (Current.Type != StringNodeType.Period)
			{
				lst.Add(Current);
				if (!Next())
				{
					return;
				}
			}

			Next();
		}

		protected bool HandleCompilerDirIf()
		{
			if (Current.Type != StringNodeType.QuestionMark)
			{
				return false;
			}

			if
			(
				!CurrentNext(1).Str.Equals("if", StringComparison.InvariantCultureIgnoreCase) &&
				!CurrentNext(1).Str.Equals("ifnot", StringComparison.InvariantCultureIgnoreCase) &&
				!CurrentNext(1).Str.Equals("endif", StringComparison.InvariantCultureIgnoreCase)
			)
			{
				return false;
			}

			Match(StringNodeType.QuestionMark);

			if (Current.Str.Equals("endif", StringComparison.InvariantCultureIgnoreCase))
			{
				Match("endif");
				_words.Next();
				return true;
			}

			Debug.Assert(!Current.Str.Equals("endif", StringComparison.InvariantCultureIgnoreCase));

			if (Current.Str.Equals("if", StringComparison.InvariantCultureIgnoreCase))
			{
				Match("if");
				if (Current.Str != "3")
				{
					int startLine = Current.LineNumber;

					// IBM screen compilation
					while (!Current.Str.Equals("endif", StringComparison.InvariantCultureIgnoreCase))
					{
						if (!_words.Next())
						{
							throw new SyntaxError(Current.FileName, startLine, "Unterminated ?if");
						}
					}
					Match("endif");
					Next();

					return true;
				}
				else
				{
					Next();
					return true;
				}
			}
			else if (Current.Str.Equals("ifnot", StringComparison.InvariantCultureIgnoreCase))
			{
				Match("ifnot");
				if (Current.Str != "3")
				{
					Next();
					return true;
				}
				else
				{
					int startLine = Current.LineNumber;

					// IBM screen compilation
					while (!Current.Str.Equals("endif", StringComparison.InvariantCultureIgnoreCase))
					{
						if (!Next())
						{
							throw new SyntaxError(Current.FileName, startLine, "Unterminated ?if");
						}
					}
					Match("endif");
					Next();

					return true;
				}
			}
			else
			{
				throw new SyntaxError(Current.FileName, Current.LineNumber, "Unknown compiler directive " + Current.Str);
			}
		}
	}
}
