using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace IM_Destinations_Creator
{
	public class CommandSinkBinding : CommandBinding
	{
		ICommandSink _commandSink;

		public ICommandSink CommandSink
		{
			get { return _commandSink; }
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("...");
				}
				if (_commandSink != null)
				{
					throw new InvalidOperationException("...");
				}

				_commandSink = value;

				base.CanExecute += (s, e) =>
				{
					e.CanExecute = _commandSink.CanExecuteCommand(e.Command, e.Parameter, out bool handled);
					e.Handled = handled;
				};

				base.Executed += (s, e) =>
				{
					_commandSink.ExecuteCommand(e.Command, e.Parameter, out bool handled);
					e.Handled = handled;
				};
			}
		}

		public static ICommandSink GetCommandSink(DependencyObject obj)
		{
			return (ICommandSink)obj.GetValue(CommandSinkProperty);
		}

		public static void SetCommandSink(DependencyObject obj, ICommandSink value)
		{
			obj.SetValue(CommandSinkProperty, value);
		}

		public static readonly DependencyProperty CommandSinkProperty =
			DependencyProperty.RegisterAttached(
				"CommandSink",
				typeof(ICommandSink),
				typeof(CommandSinkBinding),
				new UIPropertyMetadata(null, OnCommandSinkChanged));

		static void OnCommandSinkChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
		{
			ICommandSink commandSink = e.NewValue as ICommandSink;
			if (!ConfigureDelayedProcessing(depObj, commandSink))
			{
				ProcessCommandSinkChanged(depObj, commandSink);
			}
		}

		// This method is necessary when the CommandSink attached property 
		// is set on an element in a template, or any other situation in 
		// which the element's CommandBindings have not yet had a chance to be 
		// created and added to its CommandBindings collection.
		static bool ConfigureDelayedProcessing(DependencyObject depObj, ICommandSink sink)
		{
			bool isDelayed = false;

			CommonElement elem = new CommonElement(depObj);
			if (elem.IsValid && !elem.IsLoaded)
			{
				RoutedEventHandler handler = null;
				handler = delegate
				{
					elem.Loaded -= handler;
					ProcessCommandSinkChanged(depObj, sink);
				};
				elem.Loaded += handler;
				isDelayed = true;
			}

			return isDelayed;
		}

		static void ProcessCommandSinkChanged(DependencyObject depObj, ICommandSink sink)
		{
			CommandBindingCollection cmdBindings = GetCommandBindings(depObj);
			if (cmdBindings == null)
				throw new ArgumentException("...");

			foreach (CommandBinding cmdBinding in cmdBindings)
			{
				CommandSinkBinding csb = cmdBinding as CommandSinkBinding;
				if (csb != null && csb.CommandSink == null)
					csb.CommandSink = sink;
			}
		}

		static CommandBindingCollection GetCommandBindings(DependencyObject depObj)
		{
			var elem = new CommonElement(depObj);
			return elem.IsValid ? elem.CommandBindings : null;
		}

		#region CommonElement [nested class]

		/// <summary>
		/// This class makes it easier to write code that works 
		/// with the common members of both the FrameworkElement
		/// and FrameworkContentElement classes.
		/// </summary>
		private class CommonElement
		{
			readonly FrameworkElement _fe;
			readonly FrameworkContentElement _fce;

			public readonly bool IsValid;

			public CommonElement(DependencyObject depObj)
			{
				_fe = depObj as FrameworkElement;
				_fce = depObj as FrameworkContentElement;

				IsValid = _fe != null || _fce != null;
			}

			public CommandBindingCollection CommandBindings
			{
				get
				{
					this.Verify();

					if (_fe != null)
						return _fe.CommandBindings;
					else
						return _fce.CommandBindings;
				}
			}

			public bool IsLoaded
			{
				get
				{
					this.Verify();

					if (_fe != null)
						return _fe.IsLoaded;
					else
						return _fce.IsLoaded;
				}
			}

			public event RoutedEventHandler Loaded
			{
				add
				{
					this.Verify();

					if (_fe != null)
						_fe.Loaded += value;
					else
						_fce.Loaded += value;
				}
				remove
				{
					this.Verify();

					if (_fe != null)
						_fe.Loaded -= value;
					else
						_fce.Loaded -= value;
				}
			}

			void Verify()
			{
				if (!this.IsValid)
					throw new InvalidOperationException("Cannot use an invalid CommonElement.");
			}
		}

		#endregion // CommonElement [nested class]
	}
}
