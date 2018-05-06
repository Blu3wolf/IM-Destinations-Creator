﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IM_Destinations_Creator
{
	/// <summary>
	/// This implementation of ICommandSink can serve as a base
	/// class for a ViewModel or as an object embedded in a ViewModel.
	/// It provides a means of registering commands and their callback 
	/// methods, and will invoke those callbacks upon request.
	/// </summary>
	public class CommandSink : ICommandSink
	{
		#region Data
		readonly Dictionary<ICommand, CommandCallbacks>
		   _commandToCallbacksMap = new Dictionary<ICommand, CommandCallbacks>();
		#endregion // Data

		#region Command Registration
		public void RegisterCommand(
		  ICommand command, Predicate<object> canExecute, Action<object> execute)
		{
			VerifyArgument(command, "command");
			VerifyArgument(canExecute, "canExecute");
			VerifyArgument(execute, "execute");

			_commandToCallbacksMap[command] =
				new CommandCallbacks(canExecute, execute);
		}

		public void UnregisterCommand(ICommand command)
		{
			VerifyArgument(command, "command");

			if (_commandToCallbacksMap.ContainsKey(command))
				_commandToCallbacksMap.Remove(command);
		}
		#endregion // Command Registration

		#region ICommandSink Members
		public virtual bool CanExecuteCommand(
		  ICommand command, object parameter, out bool handled)
		{
			VerifyArgument(command, "command");
			if (_commandToCallbacksMap.ContainsKey(command))
			{
				handled = true;
				return _commandToCallbacksMap[command].CanExecute(parameter);
			}
			else
			{
				return (handled = false);
			}
		}

		public virtual void ExecuteCommand(
		  ICommand command, object parameter, out bool handled)
		{
			VerifyArgument(command, "command");
			if (_commandToCallbacksMap.ContainsKey(command))
			{
				handled = true;
				_commandToCallbacksMap[command].Execute(parameter);
			}
			else
			{
				handled = false;
			}
		}
		#endregion // ICommandSink Members

		#region VerifyArgument
		static void VerifyArgument(object arg, string argName)
		{
			if (arg == null)
				throw new ArgumentNullException(argName);
		}
		#endregion // VerifyArgument

		#region CommandCallbacks [nested struct]
		private struct CommandCallbacks
		{
			public readonly Predicate<object> CanExecute;
			public readonly Action<object> Execute;

			public CommandCallbacks(Predicate<object> canExecute,
									Action<object> execute)
			{
				this.CanExecute = canExecute;
				this.Execute = execute;
			}
		}
		#endregion // CommandCallbacks [nested struct]
	}
}
