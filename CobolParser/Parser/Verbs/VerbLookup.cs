using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

using CobolParser.Verbs;

namespace CobolParser
{
	public class VerbInfo
	{
		private static char[] _spaceArray = new char[] { ' ' };
		private string[] _words;

		public string Name;
		public VerbType VerbType;
		public DivisionType Div;

		public string FirstWord
		{
			get { return _words[0]; }
		}

		public int WordCount
		{
			get { return _words.Length; }
		}

		internal VerbInfo(string name, VerbType type, DivisionType div)
		{
			Name = name;
			VerbType = type;
			Div = div;

			_words = name.Split(_spaceArray);
		}

		public string WordAt(int pos)
		{
			Debug.Assert(pos >= 0);

			if (pos >= _words.Length)
			{
				return null;
			}

			return _words[pos];
		}
	}

	public static class VerbLookup
	{
		private static VerbInfo[] _verbInfos = {
			new VerbInfo(CompilerDirective.Lexum, VerbType.CompilerDirective, DivisionType.Any),
			new VerbInfo(PeriodVerb.Lexum, VerbType.Period, DivisionType.Any),
			new VerbInfo(CopyVerb.Lexum, VerbType.Copy, DivisionType.Any),
			new VerbInfo(ExecSql.Lexum, VerbType.ExecSql, DivisionType.Any),
			new VerbInfo(Send.Lexum, VerbType.Send, DivisionType.Procedure),
			new VerbInfo(Read.Lexum, VerbType.Read, DivisionType.Procedure),
			new VerbInfo(Continue.Lexum, VerbType.Continue, DivisionType.Procedure),
			new VerbInfo(Move.Lexum, VerbType.Move, DivisionType.Procedure),
			new VerbInfo(Perform.Lexum, VerbType.Perform, DivisionType.Procedure),
			new VerbInfo(Stop.Lexum, VerbType.Stop, DivisionType.Procedure),
			new VerbInfo(Open.Lexum, VerbType.Open, DivisionType.Procedure),
			new VerbInfo(Enter.Lexum, VerbType.Enter, DivisionType.Procedure),
			new VerbInfo(If.Lexum, VerbType.If, DivisionType.Procedure),
			new VerbInfo(Add.Lexum, VerbType.Add, DivisionType.Procedure),
			new VerbInfo(Write.Lexum, VerbType.Write, DivisionType.Procedure),
			new VerbInfo(Evaluate.Lexum, VerbType.Eval, DivisionType.Procedure),
			new VerbInfo(Set.Lexum, VerbType.Set, DivisionType.Procedure),
			new VerbInfo(Initialize.Lexum, VerbType.Init, DivisionType.Procedure),
			new VerbInfo(Compute.Lexum, VerbType.Compute, DivisionType.Procedure),
			new VerbInfo(Display.Lexum, VerbType.Display, DivisionType.Procedure),
			new VerbInfo(Close.Lexum, VerbType.Close, DivisionType.Procedure),
			new VerbInfo(Next.Lexum, VerbType.Next, DivisionType.Procedure),
			new VerbInfo(Accept.Lexum, VerbType.Accept, DivisionType.Procedure),
			new VerbInfo(Inspect.Lexum, VerbType.Inspect, DivisionType.Procedure),
			new VerbInfo(Multiply.Lexum, VerbType.Mult, DivisionType.Procedure),
			new VerbInfo(Divide.Lexum, VerbType.Divide, DivisionType.Procedure),
			new VerbInfo(StringVerb.Lexum, VerbType.StringVerb, DivisionType.Procedure),
			new VerbInfo("UNSTRING", VerbType.StringVerb, DivisionType.Procedure),
			new VerbInfo(Exit.Lexum, VerbType.Exit, DivisionType.Procedure),
			new VerbInfo(GoTo.Lexum, VerbType.GoTo, DivisionType.Procedure),
			new VerbInfo(Subtract.Lexum, VerbType.Sub, DivisionType.Procedure),
			new VerbInfo(Delete.Lexum, VerbType.Delete, DivisionType.Procedure),
			new VerbInfo(Search.Lexum, VerbType.Search, DivisionType.Procedure),
			new VerbInfo(Call.Lexum, VerbType.Call, DivisionType.Procedure),
			new VerbInfo(ReWrite.Lexum, VerbType.Rewrite, DivisionType.Procedure),
			new VerbInfo(Start.Lexum, VerbType.Start, DivisionType.Procedure),
			new VerbInfo(UnLockRecord.Lexum, VerbType.Unlock, DivisionType.Procedure),
			new VerbInfo(Turn.Lexum, VerbType.Turn, DivisionType.Procedure),
			new VerbInfo(AbortTransaction.Lexum, VerbType.AbortTrans, DivisionType.Procedure),
			new VerbInfo(Delay.Lexum, VerbType.Delay, DivisionType.Procedure),
			new VerbInfo(Print.Lexum, VerbType.Print, DivisionType.Procedure),
			new VerbInfo(Reset.Lexum, VerbType.Reset, DivisionType.Procedure),
			new VerbInfo(EndTransaction.Lexum, VerbType.EndTrans, DivisionType.Procedure),
			new VerbInfo(BeginTransaction.Lexum, VerbType.BeginTrans, DivisionType.Procedure),
			new VerbInfo(Clear.Lexum, VerbType.Clear, DivisionType.Procedure),
			new VerbInfo(Fd.Lexum, VerbType.Fd, DivisionType.Data)
		};

		private static Dictionary<string, List<VerbInfo>> _idx;

		public static bool CanCreate(StringNode term, DivisionType div)
		{
			CheckLoadIdx();
			return _idx.ContainsKey(term.Str);
		}

		private static void CheckLoadIdx()
		{
			if (null == _idx)
			{
				_idx = new Dictionary<string, List<VerbInfo>>();

				for (int x = 0; x < _verbInfos.Length; x++)
				{
					if (!_idx.ContainsKey(_verbInfos[x].FirstWord))
					{
						_idx.Add(_verbInfos[x].FirstWord, new List<VerbInfo>());
					}
					_idx[_verbInfos[x].FirstWord].Add(_verbInfos[x]);
				}
			}
		}

		public static IVerb Create(Terminalize terms, DivisionType div)
		{
			while (terms.Current.Type == StringNodeType.Comment)
			{
				if (!terms.Next())
				{
					return null;
				}
			}

			CheckLoadIdx();

			string verbName = terms.Current.Str;
			if (!_idx.ContainsKey(verbName))
			{
				verbName = verbName.ToUpper();

				if (!_idx.ContainsKey(verbName))
				{
					return null;
				}
			}

			List<VerbInfo> lvi = _idx[verbName];
			foreach (VerbInfo vi in lvi)
			{
				if ((vi.Div & div) == 0)
				{
					continue;
				}
				for (int x = 0; x < vi.WordCount; x++)
				{
					if (!vi.WordAt(x).Equals(terms.CurrentNext(x).Str, StringComparison.InvariantCultureIgnoreCase))
					{
						continue;
					}
				}

				switch (vi.VerbType)
				{
					case VerbType.CompilerDirective:
						return new Verbs.CompilerDirective(terms);
					case VerbType.Period:
						return new Verbs.PeriodVerb(terms);
					case VerbType.Copy:
						return new Verbs.CopyVerb(terms);
					case VerbType.ExecSql:
						return new Verbs.ExecSql(terms);
					case VerbType.Send:
						return new Verbs.Send(terms);
					case VerbType.Read:
						return new Verbs.Read(terms);
					case VerbType.Continue:
						return new Verbs.Continue(terms);
					case VerbType.Move:
						return new Verbs.Move(terms);
					case VerbType.Perform:
						return new Verbs.Perform(terms);
					case VerbType.Stop:
						return new Verbs.Stop(terms);
					case VerbType.Open:
						return new Verbs.Open(terms);
					case VerbType.Enter:
						return new Verbs.Enter(terms);
					case VerbType.If:
						return new Verbs.If(terms);
					case VerbType.Add:
						return new Verbs.Add(terms);
					case VerbType.Write:
						return new Verbs.Write(terms);
					case VerbType.Eval:
						return new Verbs.Evaluate(terms);
					case VerbType.Set:
						return new Verbs.Set(terms);
					case VerbType.Init:
						return new Verbs.Initialize(terms);
					case VerbType.Compute:
						return new Verbs.Compute(terms);
					case VerbType.Display:
						return new Verbs.Display(terms);
					case VerbType.Close:
						return new Verbs.Close(terms);
					case VerbType.Next:
						return new Verbs.Next(terms);
					case VerbType.Accept:
						return new Verbs.Accept(terms);
					case VerbType.Inspect:
						return new Verbs.Inspect(terms);
					case VerbType.Mult:
						return new Verbs.Multiply(terms);
					case VerbType.Divide:
						return new Verbs.Divide(terms);
					case VerbType.StringVerb:
						return new Verbs.StringVerb(terms);
					case VerbType.Exit:
						return new Verbs.Exit(terms);
					case VerbType.GoTo:
						return new Verbs.GoTo(terms);
					case VerbType.Sub:
						return new Verbs.Subtract(terms);
					case VerbType.Delete:
						return new Verbs.Delete(terms);
					case VerbType.Search:
						return new Verbs.Search(terms);
					case VerbType.Call:
						return new Verbs.Call(terms);
					case VerbType.Rewrite:
						return new Verbs.ReWrite(terms);
					case VerbType.Start:
						return new Verbs.Start(terms);
					case VerbType.Unlock:
						return new Verbs.UnLockRecord(terms);
					case VerbType.Turn:
						return new Verbs.Turn(terms);
					case VerbType.AbortTrans:
						return new Verbs.AbortTransaction(terms);
					case VerbType.Delay:
						return new Verbs.Delay(terms);
					case VerbType.Print:
						return new Verbs.Print(terms);
					case VerbType.Reset:
						return new Verbs.Reset(terms);
					case VerbType.EndTrans:
						return new Verbs.EndTransaction(terms);
					case VerbType.BeginTrans:
						return new Verbs.BeginTransaction(terms);
					case VerbType.Clear:
						return new Verbs.Clear(terms);
					case VerbType.Fd:
						return new Verbs.Fd(terms);
					default:
						return null;
				}
			}

			return null;
		}
	}
}
