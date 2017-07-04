﻿using System;
using System.Collections.Generic;
using System.Text;
using BuildIt.States;
using System.Windows.Input;
using BuildIt.States.Interfaces;

namespace BuildIt.forms.Sample.Core.ViewModels
{
    public enum SampleStates
    {
        Base,
        Show,
        Hide

    }

    public class MainViewModel:IHasStates
    {
        private ICommand pressedCommand;
        public ICommand PressedCommand => pressedCommand ?? (pressedCommand = new Command(SwitchStates));

        public IStateManager StateManager { get; } = new StateManager();

        public MainViewModel()
        {
            StateManager
                .Group<SampleStates>()
                .DefineAllStates();
        }

        private bool visible = true;
        public void SwitchStates()
        {
         visible = !visible;
            StateManager.GoToState(visible ? SampleStates.Show : SampleStates.Hide);
        }
    }




    public class Command : ICommand
    {
        private readonly Func<object, bool> _canExecute;

        private readonly Action<object> _execute;

        public event EventHandler CanExecuteChanged;

        public Command(Action<object> execute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }
            this._execute = execute;
        }

        public Command(Action execute) : this(delegate (object o)
        {
            execute();
        })
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }
        }

        public Command(Action<object> execute, Func<object, bool> canExecute) : this(execute)
        {
            if (canExecute == null)
            {
                throw new ArgumentNullException("canExecute");
            }
            this._canExecute = canExecute;
        }

        public Command(Action execute, Func<bool> canExecute) : this(delegate (object o)
        {
            execute();
        }, (object o) => canExecute())
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }
            if (canExecute == null)
            {
                throw new ArgumentNullException("canExecute");
            }
        }

        /// <param name="parameter">An <see cref="T:System.Object" /> used as parameter to determine if the Command can be executed.</param>
        /// <summary>Returns a <see cref="T:System.Boolean" /> indicating if the Command can be exectued with the given parameter.</summary>
        /// <returns>
        ///     <see langword="true" /> if the Command can be executed, <see langword="false" /> otherwise.</returns>
        /// <remarks>
        ///     <para>If no canExecute parameter was passed to the Command constructor, this method always returns <see langword="true" />.</para>
        ///     <para>If the Command was created with non-generic execute parameter, the parameter of this method is ignored.</para>
        /// </remarks>
        public bool CanExecute(object parameter)
        {
            return this._canExecute == null || this._canExecute(parameter);
        }

        /// <param name="parameter">An <see cref="T:System.Object" /> used as parameter for the execute Action.</param>
        /// <summary>Invokes the execute Action</summary>
        /// <remarks>
        ///     <para>If the Command was created with non-generic execute parameter, the parameter of this method is ignored.</para>
        /// </remarks>
        public void Execute(object parameter)
        {
            this._execute(parameter);
        }

        /// <summary>Send a <see cref="E:System.Windows.Input.ICommand.CanExecuteChanged" /></summary>
        /// <remarks />
        public void ChangeCanExecute()
        {
            EventHandler expr_06 = this.CanExecuteChanged;
            if (expr_06 == null)
            {
                return;
            }
            expr_06(this, EventArgs.Empty);
        }
    }
}
