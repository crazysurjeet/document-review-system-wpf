using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FileUploadSample.Commands
{
    public class RelayCommand : ICommand
    {
        Action<object> ExecuteMethod;
        Func<object, bool> CanExecuteMethod;

        public RelayCommand(Action<object> ExecuteMethod, Func<object, bool> CanExecuteMethod)
        {
            this.ExecuteMethod = ExecuteMethod;
            this.CanExecuteMethod = CanExecuteMethod;
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        public bool CanExecute(object parameter)
        {
            return CanExecuteMethod(parameter);
        }

        public void Execute(object parameter)
        {
            ExecuteMethod(parameter);
        }
    }
}
