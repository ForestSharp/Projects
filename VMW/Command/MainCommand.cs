using System;
using System.Windows.Input;

namespace VMW.Command
{
    internal class MainCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;
        readonly Action<object>? action;

        public MainCommand(Action<object>? action) 
        {
            this.action = action;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            action?.Invoke(parameter);
        }
    }
}
