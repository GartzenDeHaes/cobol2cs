using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CobolParser
{
	public enum VerbType
	{
		Unknown = 0,
		AbortTrans,
		Accept,
		Add,
		BeginTrans,
		Call,
		Clear,
		Close,
		CompilerDirective,
		CompilerWarn,
		Compute,
		Continue,
		Copy,
		Delay,
		Delete,
		Display,
		Divide,
		EndTrans,
		Enter,
		Eval,
		ExecSql,
		Exit,
		GoTo,
		Fd,
		If,
		Init,
		Inspect,
		Move,
		Mult,
		Next,
		Open,
		Perform,
		PerformCall,
		PerformStmts,
		PerformOneOf,
		Period,
		Print,
		Read,
		Reset,
		Rewrite,
		Search,
		Send,
		Set,
		Start,
		Stop,
		StringVerb,
		Sub,
		Turn,
		Unlock,
		Write,
		Unstring
	}
}
