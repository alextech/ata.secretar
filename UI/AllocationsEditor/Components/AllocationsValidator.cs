using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Ata.Investment.Allocation.Cmd;
using Ata.Investment.Allocation.Domain.Composition;

namespace AllocationsEditor.Components
{
    public class AllocationsValidator : ComponentBase
    {
        [CascadingParameter] public EditContext CurrentEditContext { get; set; }

        [Parameter] public EventCallback<bool> OnValidationStateChanged { get; set; }

        private ValidationMessageStore _messages;

        protected override void OnInitialized()
        {
            _messages = new ValidationMessageStore(CurrentEditContext);
            CurrentEditContext.OnFieldChanged += (sender, args) => ValidateModel(CurrentEditContext, _messages);
        }

        private void ValidateModel(EditContext editContext, ValidationMessageStore messages)
        {
            IEnumerable<AllocationDTO> allocations = editContext.Model as IEnumerable<AllocationDTO>;
            bool isValid = Validate(allocations);


            editContext.NotifyValidationStateChanged();
            OnValidationStateChanged.InvokeAsync(isValid);
        }

        public static bool Validate(IEnumerable<AllocationDTO> allocationDTOs)
        {
            return allocationDTOs.All(allocationDTO =>
                allocationDTO.Options.All(optionDTO =>
                    100 == optionDTO.CompositionParts.Sum(c => c.Percent)
                )
            );
        }
    }
}