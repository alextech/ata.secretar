using System;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using AtaUiToolkit;

namespace KycViewer.Shared
{
    public class ObservableValidator : FluentValidationValidator, IDisposable
    {
        [CascadingParameter]
        public QuestionnaireLayout QLayout { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            CurrentEditContext.OnValidationStateChanged += UpdateNavigationPath;
        }


        private void UpdateNavigationPath(object? sender, ValidationStateChangedEventArgs validationStateChangedEventArgs)
        {
            if (NavigationPath == null) return;

            if (!((EditContext) sender).GetValidationMessages().Any())
            {
                QLayout.ValidationObserver.EnablePath(NavigationPath);
            }
            else
            {
                QLayout.ValidationObserver.DisablePath(NavigationPath);
            }
        }

        public void Dispose()
        {
            CurrentEditContext.OnValidationStateChanged -= UpdateNavigationPath;
        }
    }
}