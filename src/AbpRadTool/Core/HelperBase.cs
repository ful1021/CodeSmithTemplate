using System;
using System.Windows;
using AbpRadTool.Core.Dto;

namespace AbpRadTool.Core
{
    public abstract class HelperBase
    {
        protected readonly IServiceProvider ServiceProvider;

        protected HelperBase(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public abstract bool CanExecute(ExecuteInput input);

        public abstract void Execute(ExecuteInput input);

        public static MessageBoxResult MessageBox(string message, MessageBoxButton button = MessageBoxButton.OK, MessageBoxImage icon = MessageBoxImage.Information, params object[] parameters)
        {
            return System.Windows.MessageBox.Show(string.Format(message, parameters), "AbpRadTool", button, icon);
        }
    }
}