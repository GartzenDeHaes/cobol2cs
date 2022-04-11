using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using CobolParser.Expressions;

namespace CobolParser.Verbs
{
	public class Turn : IVerb
	{
		public static string Lexum = "TURN";

		public bool IsTemp
		{
			get;
			private set;
		}

		public string[] Attributes
		{
			get;
			private set;
		}

		public ValueList Fields
		{
			get;
			private set;
		}

		public bool IsShadowed
		{
			get;
			private set;
		}

		public bool IsReverse
		{
			get;
			private set;
		}

		public bool IsProtected
		{
			get;
			private set;
		}

		public bool IsUnprotected
		{
			get;
			private set;
		}

		public bool IsHidden
		{
			get;
			private set;
		}

		public ITerm DependingOn
		{
			get;
			private set;
		}

		public Turn(Terminalize terms)
		: base(terms.Current)
		{
			Type = VerbType.Turn;

			terms.Match("TURN");

			if (terms.CurrentEquals("TEMP"))
			{
				IsTemp = true;
				terms.Next();
			}

			Attributes = terms.Current.Str.Split(new char[] { '-' }); ;
			terms.Next();

			for (int x = 0; x < Attributes.Length; x++)
			{
				Debug.Assert(Attributes[x].IndexOf('-') < 0);

				if (Attributes[x].Equals("PROTECTED", StringComparison.InvariantCultureIgnoreCase))
				{
					IsProtected = true;
				}
				else if (Attributes[x].Equals("UNPROTECTED", StringComparison.InvariantCultureIgnoreCase))
				{
					IsUnprotected = true;
				}
				else if (Attributes[x].Equals("REVERSE", StringComparison.InvariantCultureIgnoreCase))
				{
					IsReverse = true;
				}
				else if (Attributes[x].Equals("HIDDEN", StringComparison.InvariantCultureIgnoreCase))
				{
					IsHidden = true;
				}
				else if 
				(
					Attributes[x].Equals("NOUNDERLINE", StringComparison.InvariantCultureIgnoreCase) ||
					Attributes[x].Equals("UNDERLINE", StringComparison.InvariantCultureIgnoreCase) ||
					Attributes[x].Equals("NORMAL", StringComparison.InvariantCultureIgnoreCase) ||
					Attributes[x].Equals("DIM", StringComparison.InvariantCultureIgnoreCase) ||
					Attributes[x].Equals("BLINK", StringComparison.InvariantCultureIgnoreCase)
				)
				{
					// don't care
				}
				else
				{
					throw new NotImplementedException();
				}
			}

			terms.MatchOptional("IN");

			Fields = new ValueList(terms);

			if (terms.CurrentEquals("DEPENDING"))
			{
				terms.Next();
				terms.Match("ON");
				DependingOn = ITerm.Parse(terms);
			}

			if (terms.CurrentEquals("SHADOWED"))
			{
				IsShadowed = true;
				terms.Next();
			}
		}

		public bool HasAttibute(string attr)
		{
			for (int x = 0; x < Attributes.Length; x++)
			{
				if (Attributes[x].Equals(attr, StringComparison.InvariantCultureIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}
	}
}
