using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DOR.Core;
using System.Diagnostics;
using CobolParser.Expressions;

namespace CobolParser.Records
{
	public class ScreenField : OffsetAttributes
	{
		public string Level
		{
			get;
			private set;
		}

		public string Name
		{
			get;
			private set;
		}

		public int X
		{
			get;
			private set;
		}

		public int Y
		{
			get;
			private set;
		}

		public int OverlayWidth
		{
			get;
			private set;
		}

		public int OverlayHeight
		{
			get;
			private set;
		}

		public IExpr FromExpr
		{
			get;
			private set;
		}

		public IExpr ToExpr
		{
			get;
			private set;
		}

		public IExpr DependsOn
		{
			get;
			private set;
		}

		public IExpr UsingExpr
		{
			get;
			private set;
		}

		public int OccuresOnLines
		{
			get { return Occures == null ? 1 : Occures.Lines; }
		}

		public bool IsNormal
		{
			get;
			private set;
		}

		public bool IsDim
		{
			get;
			private set;
		}

		public bool IsDimReverse
		{
			get;
			private set;
		}

		public bool IsUnderline
		{
			get;
			private set;
		}

		public string IsUpshift
		{
			get;
			private set;
		}

		public bool IsAdvisory
		{
			get;
			private set;
		}

		public bool IsHidden
		{
			get;
			private set;
		}

		public bool IsReverse
		{
			get;
			private set;
		}

		public bool IsBlink
		{
			get;
			private set;
		}

		public string WhenFull
		{
			get;
			private set;
		}

		public string WhenAbsent
		{
			get;
			private set;
		}

		public string WhenBlank
		{
			get;
			private set;
		}

		public string FillValue
		{
			get;
			private set;
		}

		public IExpr ShadowedByField
		{
			get;
			private set;
		}

		public IExpr RequiredChoices
		{
			get;
			private set;
		}

		/// <summary>
		/// Modified Data Tag (can be ON or OFF)
		/// </summary>
		public string MDT
		{
			get;
			private set;
		}

		public IExpr RequiredLength
		{
			get;
			private set;
		}

		public bool IsProtected
		{
			get;
			private set;
		}

		public bool IsNoReverse
		{
			get;
			private set;
		}

		public int LineNumber
		{
			get;
			private set;
		}

		public Screen Screen
		{
			get;
			private set;
		}

		public bool IsArea
		{
			get;
			private set;
		}

		public ScreenField(Terminalize terms, Screen screen)
		{
			Screen = screen;
			LineNumber = terms.Current.LineNumber;
			Level = terms.Current.Str;
			if (!StringHelper.IsInt(Level))
			{
				throw new SyntaxError(terms.Current.FileName, terms.Current.LineNumber, "Expected 05");
			}

			terms.Next();

			Name = terms.Current.Str;
			terms.Match(StringNodeType.Word);

			if (terms.Current.Type == StringNodeType.Period)
			{
				terms.Match(StringNodeType.Period);
				return;
			}

			IsArea = terms.CurrentEquals("AREA");
			terms.MatchOptional("AREA");

			terms.Match("AT");
			Y = Int32.Parse(terms.Current.Str);
			terms.Next();
			terms.MatchOptional(",");
			X = Int32.Parse(terms.Current.Str);
			terms.Next();

			if (IsArea)
			{
				terms.MatchOptional("SIZE");
				OverlayHeight = Int32.Parse(terms.Current.Str);
				terms.Next();
				terms.MatchOptional(",");
				OverlayWidth = Int32.Parse(terms.Current.Str);
				terms.Next();
			}

			while (terms.Current.Type != StringNodeType.Period)
			{
				base.Parse(Name, terms);

				if (terms.CurrentEquals("FROM"))
				{
					terms.Match("FROM");
					FromExpr = IExpr.Parse(terms);
					CobolProgram.CurrentSymbolTable.AddScreenReference(((Expr)FromExpr).Terms[0], this);
				}
				else if (terms.CurrentEquals("USING"))
				{
					terms.Match("USING");
					UsingExpr = IExpr.Parse(terms);
					CobolProgram.CurrentSymbolTable.AddScreenReference(((Expr)UsingExpr).Terms[0], this);
				}
				else if (terms.CurrentEquals("DIM"))
				{
					terms.Match("DIM");
					IsDim = true;
				}
				else if (terms.CurrentEquals("HIDDEN"))
				{
					terms.Next();
					IsHidden = true;
				}
				else if (terms.CurrentEquals("UNDERLINE"))
				{
					terms.Next();
					IsUnderline = true;
				}
				else if (terms.CurrentEquals("ADVISORY"))
				{
					terms.Next();
					IsAdvisory = true;
				}
				else if (terms.CurrentEquals("REVERSE"))
				{
					terms.Next();
					IsReverse = true;
				}
				else if (terms.CurrentEquals("UPSHIFT"))
				{
					terms.Next();
					if 
					(
						terms.CurrentEquals("I") || 
						terms.CurrentEquals("O") ||
						terms.CurrentEquals("I-O") ||
						terms.CurrentEquals("INPUT-OUTPUT") ||
						terms.CurrentEquals("INPUT") || 
						terms.CurrentEquals("OUTPUT")
					)
					{
						IsUpshift = terms.Current.Str;
						terms.Next();
					}
				}
				else if (terms.CurrentEquals("TO"))
				{
					terms.Next();
					ToExpr = IExpr.Parse(terms);
					CobolProgram.CurrentSymbolTable.AddScreenReference(((Expr)ToExpr).Terms[0], this);
				}
				else if (terms.CurrentEquals("FULL"))
				{
					// WHEN is optional
					terms.Match("FULL");
					WhenFull = terms.Current.Str;
					terms.Next();
				}
				else if (terms.CurrentEquals("WHEN"))
				{
					terms.Next();

					if (terms.CurrentEquals("FULL"))
					{
						terms.Match("FULL");
						WhenFull = terms.Current.Str;
						terms.Next();
					}
					else if (terms.CurrentEquals("ABSENT"))
					{
						terms.Match("ABSENT");
						WhenAbsent = terms.Current.Str;
						terms.Next();
					}
					else if (terms.CurrentEquals("BLANK"))
					{
						terms.Next();
						WhenBlank = terms.Current.Str;
						terms.Next();
					}
					else
					{
						throw new SyntaxError(terms.Current.FileName, terms.Current.LineNumber, "Unhandled WHEN of " + terms.Current.Str);
					}
				}
				else if (terms.CurrentEquals("NORMAL"))
				{
					terms.Next();
					IsNormal = true;
				}
				else if (terms.CurrentEquals("BLINK"))
				{
					terms.Next();
					IsBlink = true;
				}
				else if (terms.CurrentEquals("DIM-REVERSE"))
				{
					terms.Next();
					IsDimReverse = true;
				}
				else if (terms.CurrentEquals("FILL"))
				{
					terms.Next();
					FillValue = terms.Current.Str;
					terms.Next();
				}
				else if (terms.CurrentEquals("SHADOWED"))
				{
					terms.Next();
					terms.MatchOptional("BY");
					ShadowedByField = IExpr.Parse(terms);
				}
				else if (terms.CurrentEquals("MUST"))
				{
					terms.Next();
					terms.MatchOptional("BE");
					RequiredChoices = IExpr.Parse(terms);
				}
				else if (terms.CurrentEquals("MDTON"))
				{
					terms.Next();
					MDT = "ON";
				}
				else if (terms.CurrentEquals("MDTOFF"))
				{
					terms.Next();
					MDT = "OFF";
				}
				else if (terms.CurrentEquals("PROTECTED"))
				{
					terms.Next();
					IsProtected = true;
				}
				else if (terms.CurrentEquals("NOREVERSE"))
				{
					terms.Next();
					IsNoReverse = true;
				}
				else if (terms.CurrentEquals("LENGTH"))
				{
					terms.Next();
					if (terms.CurrentEquals("MUST"))
					{
						terms.Match("MUST");
						terms.Match("BE");
						RequiredLength = IExpr.Parse(terms);
					}
					else
					{
						RequiredLength = IExpr.Parse(terms);
					}
				}
				else if (terms.Current.Type == StringNodeType.Comma)
				{
					// COBOL ignores stray commas, but parser errors will tend to hit this line
					terms.Next();
				}
				else
				{
					if (terms.Current.Type != StringNodeType.Period)
					{
						throw new SyntaxError(terms.Current.FileName, terms.Current.LineNumber, "Unknown screen field " + terms.Current.Str);
					}
				}
			}

			terms.Match(StringNodeType.Period);
		}

		public string TextAttributeString()
		{
			StringBuilder buf = new StringBuilder();
			if (IsNormal)
			{
				buf.Append("N|");
			}
			if (IsDim)
			{
				buf.Append("D|");
			}
			if (IsDimReverse)
			{
				buf.Append("DR|");
			}
			if (IsUnderline)
			{
				buf.Append("U|");
			}
			if (IsAdvisory)
			{
				buf.Append("A|");
			}
			if (IsHidden)
			{
				buf.Append("H|");
			}
			if (IsReverse)
			{
				buf.Append("R|");
			}
			if (IsBlink)
			{
				buf.Append("B|");
			}
			if (null != MDT)
			{
				if ("ON" == MDT)
				{
					buf.Append("MDTON|");
				}
				else
				{
					buf.Append("MDTOFF|");
				}
			}
			if (IsProtected)
			{
				buf.Append("P|");
			}
			if (IsNoReverse)
			{
				buf.Append("NR|");
			}

			if (buf.Length == 0)
			{
				return "";
			}

			return buf.ToString().Substring(0, buf.Length - 1);
		}
	}
}
