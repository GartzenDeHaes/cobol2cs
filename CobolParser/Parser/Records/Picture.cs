using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using DOR.Core;
using DOR.WorkingStorage;
using DOR.WorkingStorage.Pic;

namespace CobolParser.Records
{
	public class Picture
	{
		public StringNode LexicalPosition
		{
			get;
			private set;
		}

		public PicFormat PicFormat
		{
			get;
			private set;
		}

		public int Length
		{
			get;
			private set;
		}

		public bool IsCR
		{
			get;
			private set;
		}

		public bool IsDB
		{
			get;
			private set;
		}

		public bool IsBlankWhenZero
		{
			get;
			private set;
		}

		public bool IsDisplaySignIsLeading
		{
			get;
			private set;
		}

		public bool IsNativeBinary
		{
			get;
			private set;
		}

		public bool IsComp
		{
			get;
			private set;
		}

		public string Justification
		{
			get;
			private set;
		}

		public bool IsSignLeadingSeperate
		{
			get;
			set;
		}

		public Picture(Terminalize terms)
		{
			LexicalPosition = terms.Current;

			if (terms.CurrentEquals("NATIVE-2"))
			{
				terms.Match("NATIVE-2");
				IsNativeBinary = true;
				Length = 2;
				return;
			}
			if (terms.CurrentEquals("NATIVE-4"))
			{
				terms.Match("NATIVE-4");
				IsNativeBinary = true;
				Length = 4;
				return;
			}
			if (terms.CurrentEquals("BINARY"))
			{
				terms.Match("BINARY");
				Length = Int32.Parse(terms.Current.Str) / 8;
				terms.Next();
				PicFormat = PicFormat.Parse(StringHelper.RepeatChar('9', Length));
				return;
			}

			if
			(
				!terms.CurrentEquals("PIC") &&
				!terms.CurrentEquals("PICTURE")
			)
			{
				throw new Exception("Internal error");
			}
			
			terms.Next();

			StringBuilder buf = new StringBuilder();

			while 
			(
				terms.Current.Type == StringNodeType.Comma ||
				terms.Current.Type == StringNodeType.Number ||
				terms.Current.Type == StringNodeType.LPar ||
				terms.Current.Type == StringNodeType.RPar ||
				terms.Current.Type == StringNodeType.Period ||
				terms.Current.Type == StringNodeType.Slash ||
				terms.CurrentEquals("BLANK") ||
				(
					terms.Current.Str.StartsWith("V") && 
					!terms.CurrentEquals("VALUE") &&
					!terms.CurrentEquals("VALUES")
				) ||
				(
					terms.Current.Str.StartsWith("B") &&
					!terms.CurrentEquals("BLINK")
				) ||
				(
					terms.Current.Str.StartsWith("S") &&
					!terms.CurrentEquals("SIGN")
				) ||
				terms.Current.Str.StartsWith("Z") ||
				terms.Current.Str.StartsWith("X") ||
				(
					terms.Current.Str.StartsWith("v") &&
					!terms.CurrentEquals("VALUE") &&
					!terms.CurrentEquals("VALUES")
				) ||
				terms.Current.Str.StartsWith("b") ||
				(
					terms.Current.Str.StartsWith("s") &&
					!terms.CurrentEquals("SIGN")
				) ||
				terms.Current.Str.StartsWith("z") ||
				terms.Current.Str.StartsWith("x") ||
				terms.Current.Str.StartsWith("+") ||
				terms.Current.Str.StartsWith("-") ||
				terms.Current.Str.StartsWith("9")
			)
			{
				if (terms.CurrentEquals("BLANK"))
				{
					buf.Append("B");
				}
				else if (terms.Current.Type == StringNodeType.Period)
				{
					if 
					(
						terms.CurrentNext(1) == null || 
						terms.Current.AtEOL ||
						terms.CurrentNext(1).Str[0] != '9'
					)
					{
						break;
					}
				}
				else
				{
					buf.Append(terms.Current.Str);
				}
				terms.Next();
			}

			Debug.Assert(buf.Length > 0);

			PicFormat = PicFormat.Parse(buf.ToString());
			Length = PicFormat.Length;

			terms.MatchOptional("USAGE");
			terms.MatchOptional("IS");

			if (terms.CurrentEquals("COMP"))
			{
				IsComp = true;
				terms.Match("COMP");
			}

			terms.MatchOptional("COMP-3");
			terms.MatchOptional("SYNC");
			terms.MatchOptional("DISPLAY");

			if (terms.Current.Str.Equals("SIGN", StringComparison.InvariantCultureIgnoreCase))
			{
				terms.Match("SIGN");
				terms.MatchOptional("IS");
				terms.MatchOptional("LEADING");
				terms.MatchOptional("TRAILING");
				terms.MatchOptional("SEPARATE");
				IsSignLeadingSeperate = true;
			}
		}

		public string SqlTypeName()
		{
			if (IsNativeBinary)
			{
				if (Length == 4)
				{
					return "INT";
				}
				if (Length == 2)
				{
					return "SMALLINT";
				}
				return "CHAR";
			}
			if (PicFormat.IsFloat)
			{
				return "DECIMAL";
			}
			if (PicFormat.IsNumeric)
			{
				if (IsComp)
				{
					return "INT";
				}
				return "DECIMAL";
			}
			return "CHAR";
		}

		public string CsClrTypeName()
		{
			if (IsNativeBinary)
			{
				if (Length == 4)
				{
					return "int";
				}
				if (Length == 2)
				{
					return "short";
				}
				return "byte[]";
			}
			if (PicFormat.IsFloat)
			{
				return "decimal";
			}
			if (PicFormat.IsNumeric)
			{
				return "long";
			}
			return "string";
		}

		public string CsConversionMethod()
		{
			if (PicFormat.IsFloat)
			{
				return ".ToDecimal()";
			}
			if (PicFormat.IsNumeric)
			{
				return ".ToInt()";
			}
			return ".ToString()";
		}

		public string CConversionMethod()
		{
			if (IsNativeBinary)
			{
				if (Length == 4)
				{
					return ".ToInt32()";
				}
				if (Length == 2)
				{
					return ".ToInt16()";
				}
				return ".ToString()";
			}
			if (PicFormat.Decimals == 0)
			{
				if (PicFormat.Length < 5)
				{
					return ".ToInt16()";
				}
				if (PicFormat.Length < 11)
				{
					return ".ToInt32()";
				}
				return ".ToString()";
			}
			return ".ToDouble()";
		}

		public string CTypeName()
		{
			if (IsNativeBinary)
			{
				if (Length == 4)
				{
					return "int";
				}
				if (Length == 2)
				{
					return "short";
				}
				return "string";
			}
			if (PicFormat.Decimals == 0)
			{
				if (PicFormat.Length < 5)
				{
					return "short";
				}
				if (PicFormat.Length < 11)
				{
					return "int";
				}
				return "string";
			}
			return "double";
		}
		
		public static bool IsPictureKeyword(Terminalize terms)
		{
			return terms.Current.Str.Equals("PIC", StringComparison.InvariantCultureIgnoreCase) ||
				terms.Current.Str.Equals("PICTURE", StringComparison.InvariantCultureIgnoreCase) ||
				terms.Current.Str.Equals("NATIVE-4", StringComparison.InvariantCultureIgnoreCase) ||
				terms.Current.Str.Equals("NATIVE-2", StringComparison.InvariantCultureIgnoreCase) ||
				terms.Current.Str.Equals("BINARY", StringComparison.InvariantCultureIgnoreCase);
		}
	}
}
