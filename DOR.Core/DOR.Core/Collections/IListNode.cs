using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DOR.Core.Collections
{
	public interface IListNode<T>
	{
		T Next { get; set; }
		T Prev { get; set; }

		T Clone();
	}
}
