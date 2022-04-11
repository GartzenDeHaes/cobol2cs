using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CobolParser.SQL.Conds
{
	public class CondNoOp : ISqlCondToken
	{
		public override void GetParameters(List<CondParam> lst)
		{
		}
	}
}
