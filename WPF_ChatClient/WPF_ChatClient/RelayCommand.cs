using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WPF_ChatRoom
{
    internal class RelayCommand : ICommand
    {
        readonly Func<bool> _canExecute;//判断命令是否能够执行
        readonly Action _execute;//命令需要执行的方法
        public RelayCommand(Action action, Func<bool> canExecute)
        {
            _canExecute = canExecute;
            _execute = action;
        }
        public bool CanExecute(object parameter)
        {
            if (_canExecute != null)
            {
                return true;
            }
            return _canExecute();
        }
        public void Execute(object parameter)
        {
            _execute();
        }
        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (_canExecute != null)
                {
                    CommandManager.RequerySuggested += value;
                }
            }
            remove
            {
                if (_canExecute != null)
                {
                    CommandManager.RequerySuggested -= value;
                }
            }
        }
    }
}
