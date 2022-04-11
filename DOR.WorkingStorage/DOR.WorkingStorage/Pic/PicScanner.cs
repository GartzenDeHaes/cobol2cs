using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using DOR.Core;

namespace DOR.WorkingStorage.Pic
{
	enum PicCharClass
	{
		Text,
		EOF
	}

	class PicScanner
	{
		private StringBuilder _buf = new StringBuilder();
		private string _txt;
		private int _pos;

		public string Lexum
		{
			get { return _buf.ToString(); }
		}

		public PicCharClass Token
		{
			get;
			private set;
		}

		public bool IsEof
		{
			get { return _pos >= _txt.Length; }
		}

		public PicScanner(string txt)
		{
			_txt = txt;
			Next();
		}

		public bool Next()
		{
			_buf.Clear();

			if (_pos >= _txt.Length)
			{
				Token = PicCharClass.EOF;
				return false;
			}

			if (_txt[_pos] == 'C' && _txt[_pos + 1] == 'R')
			{
				// CR, print two spaces for positive or "CR" for negative number
				_pos += 2;
				return Next();
			}

			switch (_txt[_pos])
			{
				case 'X':
				case 'x':
				case 'S':
				case 's':
				case '+':
				case '9':
				case 'A':
				case 'a':
				case 'B':
				case 'b':
				case 'V':
				case 'v':
				case '.':
					Read();
					break;
				case '/':
					ReadMask('/');
					break;
				case '-':
					ReadMask('-');
					break;
				case 'Z':
				case 'z':
					ReadMask('Z');
					break;
				default:
					throw new NotImplementedException("Unknown character class");
			}

			return true;
		}

		private void Read()
		{
			Token = PicCharClass.Text;
			char chMask = _txt[_pos];

			while (_txt[_pos] == chMask)
			{
				_buf.Append(Char.ToUpper(_txt[_pos++]));
				if (IsEof)
				{
					break;
				}
				if (_txt[_pos] == '(')
				{
					_buf.Length--;
					ReadLength(chMask);
				}
				if (IsEof)
				{
					break;
				}
			}
		}

		private void ReadLength(char repeat)
		{
			char ch = _txt[_pos++];
			Debug.Assert(ch == '(');

			StringBuilder buf = new StringBuilder();

			ch = _txt[_pos++];
			while (ch != ')')
			{
				buf.Append(ch);
				ch = _txt[_pos++];
			}
			
			_buf.Append(StringHelper.RepeatChar(repeat, Int32.Parse(buf.ToString())));
		}

		private void ReadMask(char maskType)
		{
			Token = PicCharClass.Text;
			
			while 
			(
				_pos < _txt.Length && 
				(
					_txt[_pos] == maskType || 
					_txt[_pos] == ',' || 
					_txt[_pos] == '/'
				)
			)
			{
				_buf.Append(_txt[_pos++]);
				if (IsEof)
				{
					break;
				}
				if (_txt[_pos] == '(')
				{
					_buf.Length--;
					ReadLength(maskType);
				}
			}
		}
	}
}
