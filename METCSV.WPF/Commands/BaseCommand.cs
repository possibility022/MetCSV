using System;
using System.Windows.Input;

namespace METCSV.WPF.Commands
{
    public class BaseCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        readonly Action action;

        public BaseCommand(Action action)
        {
            this.action = action;
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            action.Invoke();
        }
    }
}
