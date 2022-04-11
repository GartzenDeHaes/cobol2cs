using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DOR.Core.Collections;
using CobolParser.Sections;
using CobolParser.Divisions;
using CobolParser.Expressions;
using CobolParser.Verbs.Phrases;
using CobolParser.Verbs;
using CobolParser.Parser;
using System.Threading.Tasks;

namespace CobolParser.Division
{
	public class ProcedureDiv : IDivision
	{
		public Declaratives DeclareSection
		{
			get;
			private set;
		}

		public Vector<SubRoutine> SubRoutines
		{
			get;
			private set;
		}

		public ValueList UsingArguments
		{
			get;
			private set;
		}

		public ProcedureDiv()
		: base(DivisionType.Procedure)
		{
			SubRoutines = new Vector<SubRoutine>();
		}

		public string MainSubName()
		{
			return SubRoutines[0].Name;
		}

		public override void Parse(Terminalize terms)
		{
			terms.Match("PROCEDURE");
			terms.Match("DIVISION");

			if (terms.CurrentEquals("USING"))
			{
				terms.Next();
				UsingArguments = new ValueList(terms);
			}

			terms.Match(StringNodeType.Period);

			if (terms.CurrentEquals("DECLARATIVES"))
			{
				DeclareSection = new Declaratives(terms);
			}

			while (!terms.IsIterationComplete)
			{
				SubRoutines.Add(new SubRoutine(terms));
			}
		}

		public void ListTables(List<string> lst)
		{
			foreach (var sub in SubRoutines)
			{
				sub.ListTables(lst);
			}
		}

		private List<Turn> _turns;

		public List<Turn> ListTurns()
		{
			if (_turns != null)
			{
				return _turns;
			}

			_turns = ListVerbs<Turn>();

			return _turns;
		}

		public List<T> ListVerbs<T>() where T : IVerb
		{
			List<T> verbs = new List<T>();

			Vector<IVerb> vlist = new Vector<IVerb>();

			foreach (var sub in SubRoutines)
			{
				vlist.AddRange(sub.Verbs);
				while (vlist.Count > 0)
				{
					var vrb = vlist.Pop();

					if (vrb is T)
					{
						verbs.Add((T)vrb);
					}
					else if (vrb is If)
					{
						vlist.AddRange(((If)vrb).Stmts.Stmts);
						if (((If)vrb).ElseStmts != null)
						{
							vlist.AddRange(((If)vrb).ElseStmts.Stmts);
						}
					}
					else if (vrb is Evaluate)
					{
						foreach (var when in ((Evaluate)vrb).Whens)
						{
							vlist.AddRange(when.Stmts.Stmts);
						}
					}
				}
			}

			return verbs;
		}

		public SubRoutine FindSub(string name)
		{
			for (int x = 0; x < SubRoutines.Count; x++)
			{
				if (SubRoutines[x].Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
				{
					return SubRoutines[x];
				}
			}

			return null;
		}

		private void PruneUnReferencedFunctionsInner(Vector<IVerb> verbs, Stack<SubRoutine> subStk)
		{
			foreach (var v in verbs)
			{
				if (v.Type == VerbType.PerformCall)
				{
					if (((PerformCall)((Perform)v).PerformInner).SubRoutine != null)
					{
						subStk.Push(FindSub(((PerformCall)((Perform)v).PerformInner).SubRoutine.ToString()));
					}
					if (((PerformCall)((Perform)v).PerformInner).Stmts != null)
					{
						PruneUnReferencedFunctionsInner(((PerformCall)((Perform)v).PerformInner).Stmts.Stmts, subStk);
					}
				}
				else if (v.Type == VerbType.PerformOneOf)
				{
					foreach (var t in ((PerformOneOf)((Perform)v).PerformInner).Options.Items)
					{
						subStk.Push(FindSub(t.ToString()));
					}
				}
				else if (v.Type == VerbType.If)
				{
					PruneUnReferencedFunctionsInner(((If)v).Stmts.Stmts, subStk);

					if (((If)v).ElseStmts != null)
					{
						PruneUnReferencedFunctionsInner(((If)v).ElseStmts.Stmts, subStk);
					}
				}
				else if (v.Type == VerbType.Eval)
				{
					foreach (var w in ((Evaluate)v).Whens)
					{
						PruneUnReferencedFunctionsInner(w.Stmts.Stmts, subStk);
					}
				}
			}
		}

		public void PruneUnReferencedFunctions()
		{
			Stack<SubRoutine> subStk = new Stack<SubRoutine>();
			subStk.Push(SubRoutines[0]);
			
			// special case?  Procedure writer patches up this sub call from main.
			if 
			(
				SubRoutines[0].Verbs.Count > 1 && 
				SubRoutines[0].Verbs[SubRoutines[0].Verbs.Count - 2] is CopyVerb
			)
			{
				subStk.Push(SubRoutines[1]);
			}

			while (subStk.Count > 0)
			{
				SubRoutine sub = subStk.Pop();
				if (sub.IsReferenced)
				{
					continue;
				}
				sub.IsReferenced = true;
				
				PruneUnReferencedFunctionsInner(sub.Verbs, subStk);
			}

			foreach (var s in SubRoutines)
			{
				if (!s.IsReferenced)
				{
					Console.WriteLine("Pruning {0}", s.Name);
				}
			}
		}
	}
}
