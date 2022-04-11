using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using DOR.Core;
using DOR.Core.Collections;

namespace CobolParser.Expressions.Terms
{
	public struct SubRange
	{
		public ITerm From;
		public ITerm To;
	}

	public class Range : ITerm
	{
		public Vector<SubRange> Ranges = new Vector<SubRange>();

		public Range(Terminalize terms)
		{
			do
			{
				SubRange sr;

				terms.MatchOptional(",");

				if (terms.Current.Type == StringNodeType.Word)
				{
					sr.From = new Id(terms);
				}
				else if (terms.Current.Type == StringNodeType.Quoted)
				{
					sr.From = new StringLit(terms);
				}
				else
				{
					sr.From = new Number(terms);
				}
				terms.Next();

				terms.MatchOptional("THROUGH");
				terms.MatchOptional("THRU");

				if (terms.Current.Type != StringNodeType.RPar)
				{
					sr.To = ITerm.Parse(terms);
				}
				else
				{
					sr.To = null;
				}
				Ranges.Add(sr);
			} while (terms.Current.Type == StringNodeType.Comma);
		}

		public override string ToDocumentationString()
		{
			return ToString();
		}

		public override string ToCListStringList()
		{
			return ToString(true);
		}

		public override string ToString()
		{
			return ToString(false);
		}

		public string ToString(bool forceQuotes)
		{
			StringBuilder buf = new StringBuilder();
			bool isString = Ranges[0].From is StringLit;
			string sfrom = Ranges[0].From.ToString();
			int len = sfrom.Length - (isString ? 2 : 0);
			bool ischar = false;
			int start;
			int end;
			string sfromnoquote = StringHelper.StripQuotes(sfrom);

			if (!StringHelper.IsInt(sfromnoquote))
			{
				Debug.Assert(sfromnoquote.Length == 1);
				ischar = true;
				start = (int)sfromnoquote[0];
				end = (int)StringHelper.StripQuotes(Ranges[0].To.ToString())[0];
			}
			else
			{
				start = Int32.Parse(sfromnoquote);
				end = Int32.Parse(StringHelper.StripQuotes(Ranges[0].To.ToString()));
			}

			for (int x = start; x <= end; x++)
			{
				if (x != start)
				{
					buf.Append(", ");
				}

				if (isString || ischar || forceQuotes)
				{
					buf.Append("\"");
					if (ischar)
					{
						buf.Append((char)x);
					}
					else
					{
						buf.Append(StringHelper.PadLeft(x.ToString(), len, '0'));
					}
					buf.Append("\"");
				}
				else
				{
					buf.Append(x.ToString());
				}
			}

			return buf.ToString();
		}
	}
}
