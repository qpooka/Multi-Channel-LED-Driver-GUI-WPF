using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MVVM.viewmodel
{
    public class DelegateCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;// = delegate { };
        private readonly Action<object> _executeAction; //what is diff between Action<object> and Action??? it can accept a parameter?
        private readonly Func<bool> _canExecuteAction;


        public DelegateCommand(Action<object> executeAction)
        {
            _executeAction = executeAction;
        }

/*        public DelegateCommand(Action executeAction, Func<bool> canExecuteAction)
        {
            _executeAction = executeAction;
            _canExecuteAction = canExecuteAction;
        }
*/
        public DelegateCommand(Action<object> executeAction, Func<bool> canExecuteAction)
        {
            _executeAction = executeAction;
            _canExecuteAction = canExecuteAction;
        }

        public bool CanExecute(object parameter)
        {
            if(_canExecuteAction != null)
                return _canExecuteAction();

            if (_executeAction != null)
                return true;

            return false;
        }

        public void InvokeCanExecuteChanged()
        {
            if(CanExecuteChanged != null)
                CanExecuteChanged(this, EventArgs.Empty);
        }

        public void Execute(object parameter)
        {
            _executeAction(parameter);
        }
    }
}
