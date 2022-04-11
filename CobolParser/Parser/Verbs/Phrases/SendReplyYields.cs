using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DOR.Core.Collections;

namespace CobolParser.Verbs.Phrases
{
	public class SendReplyYields
	{
		public List<Association<IExpr, string>> Cases
		{
			get;
			private set;
		}

		public SendReplyYields(Terminalize terms)
		{
			Cases = new List<Association<IExpr, string>>();

			terms.MatchOptional("REPLY");

			while (terms.CurrentEquals("CODE"))
			{
				terms.Match("CODE");
				terms.MatchOptional("FIELD");
				terms.MatchOptional("IS");
				IExpr code = IExpr.Parse(terms);
				terms.Match("YIELDS");
				string dataItem = terms.Current.Str;
				terms.Next();

				Cases.Add(new Association<IExpr, string>(code, dataItem));
			}
		}
	}
}
