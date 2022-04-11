using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DOR.Core.Net.Command
{
	public interface ICommandFactory
	{
		ICommand GetCommand(CommandId id);
		void ReleaseCommand(ICommand cmd);
	}
}
