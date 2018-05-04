using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IM_Destinations_Creator
{
	public interface ICommandSink
	{
		bool CanExecuteCommand(ICommand command, object parameter, out bool handled);
		void ExecuteCommand(ICommand command, object parameter, out bool handled);
	}
}
