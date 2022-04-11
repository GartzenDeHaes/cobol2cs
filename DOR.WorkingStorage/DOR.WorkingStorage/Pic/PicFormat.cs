using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using DOR.Core;
using DOR.Core.Collections;

namespace DOR.WorkingStorage.Pic
{
	public class PicFormat
	{
		private Vector<ICharacterClass> _charClasses = new Vector<ICharacterClass>();

		public string PictureClause
		{
			get;
			private set;
		}

		public int Length
		{
			get;
			private set;
		}

		public int DisplayLength
		{
			get;
			private set;
		}

		public int CharClassCount
		{
			get { return _charClasses.Count; }
		}

		public bool IsNumeric
		{
			get;
			private set;
		}

		public bool IsSigned
		{
			get;
			private set;
		}

		public bool IsFloat
		{
			get;
			private set;
		}

		public int Decimals
		{
			get;
			private set;
		}

		public bool IsVirtualDecimal
		{
			get;
			private set;
		}

		ICharacterClass LastCc
		{
			get { return _charClasses.Count > 0 ? _charClasses.LastElement() : null; }
		}

		private PicFormat()
		{
		}

		void Add(ICharacterClass cls)
		{
			_charClasses.Add(cls);
			Length += cls.Length;
			DisplayLength += cls.Mask.Length;
			Debug.Assert(cls.Mask.IndexOf("(") < 0);
		}

		void InsertAtHead(ICharacterClass cls)
		{
			_charClasses.InsertElementAt(cls, 0);
			Length += cls.Length;
		}

		public string Format(string raw)
		{
			int pos = raw.Length - 1;
			StringBuilder buf = new StringBuilder();

			for (int x = _charClasses.Count - 1; x > -1; x--)
			{
				_charClasses[x].Format(buf, raw, ref pos);
			}

			string ret = StringHelper.Reverse(buf.ToString());

			if
			(
				ret[0] != ' ' &&
				_charClasses.Count > 1 &&
				_charClasses[1] is CcMask &&
				(
					_charClasses[0] is CcPlusSigned || 
					_charClasses[0] is CcSigned
				)
			)
			{
				int spaceCount = StringHelper.CountOccurancesOf(ret, ' ');
				if (spaceCount > 0)
				{
					ret = StringHelper.RepeatChar(' ', spaceCount) + ret.Replace(" ", "");
				}
			}
			else if (ret[0] == ' ')
			{
				if (StringHelper.CountOccurancesOf(ret, ' ') == ret.Length - 1)
				{
					return ret.Replace('.', ' ');
				}
				else if (ret.Trim() == ".0")
				{
					return ret.Replace(".0", "  ");
				}
			}
			return ret;
		}

		public string ToRawString(decimal d)
		{
			return ToRawString(d.ToString());
		}

		public string ToRawString(long i)
		{
			return ToRawString(i.ToString());
		}

		public string ToRawString(int i)
		{
			return ToRawString((long)i);
		}

		public string ToRawString(NumericTemp n)
		{
			if (n.IsInt)
			{
				return ToRawString(n.ToInt());
			}
			return ToRawString(n.ToDecimal());
		}
	
		public string ToRawString(string s)
		{
			if (s == null)
			{
				s = "";
			}
			StringBuilder buf = new StringBuilder();
			int pos = 0;

			for (int x = 0; x < _charClasses.Count; x++)
			{
				buf.Append(_charClasses[x].ToRawString(s, ref pos));
			}

			return buf.ToString();
		}

		public static PicFormat Parse(string txt)
		{
			PicFormat pic = new PicFormat();
			pic.PictureClause = txt;

			if (txt.StartsWith("BINARY-", StringComparison.InvariantCultureIgnoreCase))
			{
				pic.Add(new CcBinary(Int32.Parse(txt.Substring(7))));
				return pic;
			}

			PicScanner lex = new PicScanner(txt);

			while (lex.Token != PicCharClass.EOF)
			{
				Debug.Assert(lex.Token == PicCharClass.Text);

				string lexum = lex.Lexum;
				lex.Next();

				if 
				(
					pic._charClasses.Count == 0 && 
					((lexum[0] != 'S' && 
					lexum[0] != '-' && 
					lexum[0] != '+' &&
					!lex.IsEof &&
					lex.Lexum[0] == '9')
					|| lexum[0] == '9')
				)
				{
					pic.Add(new CcNoSign());
				}

				switch (lexum[0])
				{
					case 'S':
						Debug.Assert(lexum.Length == 1);
						pic.Add(new CcSigned());
						pic.IsSigned = true;
						break;
					case '/':
						pic.Add(new CcLit(lexum));
						break;
					case '-':
						if 
						(
							pic.CharClassCount > 0 && 
							lex.IsEof && 
							lexum.Length == 1 && 
							!pic.IsSigned && 
							!(pic.LastCc is CcDot || pic.LastCc is CcVDot)
						)
						{
							pic.IsSigned = true;
							pic.Add(new CcSignedTrailing());
						}
						else
						{
							bool isLeft = pic.LastCc is CcDot || pic.LastCc is CcVDot;
							pic.Add(new CcMask(lexum));
							((CcMask)pic.LastCc).IsPadLeft = isLeft;
						}
						break;
					case '+':
						pic.IsSigned = true;
						pic.Add(new CcPlusSigned(lexum));
						break;
					case 'Z':
						pic.Add(new CcMask(lexum));
						break;
					case '9':
						CcNum ccn = new CcNum(lexum);
						pic.Add(ccn);
						pic.IsNumeric = true;
						if (pic.IsFloat)
						{
							pic.Decimals = ccn.Length;
							ccn.IsPadLeft = true;
						}
						break;
					case 'A':
						pic.Add(new CcAlpha(lexum));
						break;
					case 'X':
						pic.Add(new CcAlphaNum(lexum));
						break;
					case 'B':
						pic.Add(new CcSpace(lexum));
						break;
					case 'V':
						pic.Add(new CcVDot());
						pic.IsFloat = true;
						pic.IsNumeric = true;
						pic.IsVirtualDecimal = true;
						break;
					case '.':
						pic.Add(new CcDot());
						pic.IsFloat = true;
						pic.IsNumeric = true;
						break;
					default:
						throw new NotImplementedException("Unknown character class");
				}
			}
			
			return pic;
		}

		public override bool Equals(object obj)
		{
			PicFormat p2 = obj as PicFormat;
			if (p2 == null)
			{
				return false;
			}

			if (_charClasses.Count != p2._charClasses.Count)
			{
				return false;
			}

			for (int x = 0; x < _charClasses.Count; x++)
			{
				if (! _charClasses[x].Equals(p2._charClasses[x]))
				{
					return false;
				}
			}

			return true;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
