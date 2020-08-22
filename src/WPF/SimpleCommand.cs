﻿using System;
using System.Windows.Input;

namespace Groundbeef.WPF
{
    [System.Runtime.InteropServices.ComVisible(true)]
    public class SimpleCommand : ICommand
    {
        public SimpleCommand() { }
        public SimpleCommand(Func<object?, bool>? canExecute = null, Action<object?>? execute = null)
        {
            CanExecuteDelegate = canExecute;
            ExecuteDelegate = execute;
        }

        public Func<object?, bool>? CanExecuteDelegate { get; set; }

        public Action<object?>? ExecuteDelegate { get; set; }

        public bool CanExecute(object? parameter)
        {
            var canExecute = CanExecuteDelegate;
            return canExecute == null || canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public void Execute(object? parameter)
        {
            ExecuteDelegate?.Invoke(parameter);
        }
    }
}