namespace AbpRadTool.ToolWindows
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Controls;
    using AbpRadTool.Core;
    using AbpRadTool.Core.Dto;

    /// <summary>
    /// Interaction logic for AbpRadToolWindowControl.
    /// </summary>
    public partial class AbpRadToolWindowControl : UserControl
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbpRadToolWindowControl"/> class.
        /// </summary>
        public AbpRadToolWindowControl(IServiceProvider serviceProvider)
        {
            this.InitializeComponent();
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                string.Format(System.Globalization.CultureInfo.CurrentUICulture, "Invoked '{0}'", this.ToString()),
                "AbpRadToolWindow");
        }

        private void Generate_OnClick(object sender, RoutedEventArgs e)
        {
            var helper = new AddNewServiceMethodHelper(_serviceProvider);
            var names = Regex.Split(txtNames.Text, @"\r\n");

            if (HelperBase.MessageBox("{0} method{1} will be generated. OK?", MessageBoxButton.OKCancel, MessageBoxImage.Question, names.Length, (names.Length > 1 ? "s" : string.Empty)) == MessageBoxResult.Cancel)
            {
                return;
            }
            var input = new ExecuteInput
            {
                MethodNames = names,
                IsAsync = chkAsync.IsChecked ?? true
            };
            if (helper.CanExecute(input))
            {
                helper.Execute(input);
            }
        }
    }
}