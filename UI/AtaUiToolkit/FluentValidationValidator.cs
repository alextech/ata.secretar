using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace AtaUiToolkit
{
    #nullable enable
    // ReSharper disable once ClassNeverInstantiated.Global
    public class FluentValidationValidator : ComponentBase
    {
        // ReSharper disable MemberCanBePrivate.Global
        // ReSharper disable UnusedAutoPropertyAccessor.Global

        [CascadingParameter]
        public EditContext CurrentEditContext { get; set; }

        [Parameter]
        public string? NavigationPath { get; set; }

        [Parameter]
        public Type ValidatorType { get; set; }

        [Parameter]
        public EventCallback<bool> OnValidationStateChanged { get; set; }

        [Parameter]
        public object[] ConstructorParams { get; set; }

        [Parameter]
        public bool RealtimeValidation { get; set; } = true;

        private IValidator _validatorInstance;

        private ValidationMessageStore _messages;

        protected override void OnInitialized()
        {
            if (CurrentEditContext == null)
            {
                throw new InvalidOperationException($"{nameof(FluentValidationValidator)} requires a cascading " +
                                                    $"parameter of type {nameof(EditContext)}. For example, you can use {nameof(FluentValidationValidator)} " +
                                                    $"inside an {nameof(EditForm)}.");
            }

            _messages = new ValidationMessageStore(CurrentEditContext);

            if (RealtimeValidation)
            {
                CurrentEditContext.OnFieldChanged += (sender, args) => ValidateModel(CurrentEditContext, _messages);
            }
            else
            {
                CurrentEditContext.OnValidationRequested += (sender, args) => ValidateModel(CurrentEditContext, _messages);
            }

            CurrentEditContext.OnValidationStateChanged += TriggerValidationStateChangedListeners;

        }

        protected override void OnParametersSet()
        {
            if (ValidatorType == null)
            {
                throw new Exception("Tried to initialize validator component without specifying what type validator to use.");
            }
            _validatorInstance = (IValidator) Activator.CreateInstance(ValidatorType, args:ConstructorParams)!;
        }

        private void TriggerValidationStateChangedListeners(object? sender,
            ValidationStateChangedEventArgs validationStateChangedEventArgs)
        {
            OnValidationStateChanged.InvokeAsync(!((EditContext) sender).GetValidationMessages().Any());
        }

        private bool ValidateModel(EditContext editContext, ValidationMessageStore messages)
        {
            ValidationResult validationResults = _validatorInstance.Validate(editContext.Model);

            messages.Clear();
            foreach (ValidationFailure validationResult in validationResults.Errors)
            {
                messages.Add(editContext.Field(validationResult.PropertyName), validationResult.ErrorMessage);
            }

            editContext.NotifyValidationStateChanged();

            return validationResults.IsValid;
        }

        public void DisplayErrors(Dictionary<string,List<string>> errors)
        {
            _messages.Clear();
            foreach (KeyValuePair<string,List<string>> error in errors)
            {
                _messages.Add(CurrentEditContext.Field(error.Key), error.Value);
            }
        }
    }
}