using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CobolParser.Expressions;
using CobolParser.Verbs.Phrases;

namespace CobolParser.Verbs
{
	public class Inspect : IVerb
	{
		public const string Lexum = "INSPECT";

		public ITerm Target
		{
			get;
			private set;
		}

		public List<IInspectOperator> Operations
		{
			get;
			private set;
		}

		public Inspect(Terminalize terms)
		: base(terms.Current)
		{
			Type = VerbType.Inspect;

			Operations = new List<IInspectOperator>();

			terms.Match(Lexum);
			Target = ITerm.Parse(terms);

			while (true)
			{
				if (terms.CurrentEquals("REPLACING"))
				{
					Operations.Add(new InspectReplacing(terms));
				}
				else if (terms.CurrentEquals("TALLYING"))
				{
					Operations.Add(new InspectTallying(terms));
				}
				else if (terms.CurrentEquals("CONVERTING"))
				{
					Operations.Add(new InspectConverting(terms));
				}
				else
				{
					break;
				}
			}
		}
	}
}
