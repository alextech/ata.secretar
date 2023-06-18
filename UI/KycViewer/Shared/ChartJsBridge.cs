using System.Collections.Immutable;
using System.Linq;
using KycViewer.Components;
using KycViewer.Pages;
using Microsoft.JSInterop;
using SharedKernel;
using Ata.Investment.Profile.Domain.Profile;

namespace KycViewer.Shared
{
    public class ChartJsBridge
    {
        private readonly Profile _profile;
        private readonly RecommendationsVM _recommendationsVM;
        private readonly DocumentValidationObserver _validationObserver;

        public ChartJsBridge(Profile profile, RecommendationsVM recommendationsVM, DocumentValidationObserver validationObserver)
        {
            _profile = profile;
            _recommendationsVM = recommendationsVM;
            _validationObserver = validationObserver;
        }

        [JSInvokable]
        // ReSharper disable once UnusedMember.Global
        public void SetRecommendation(int allocationOptionId)
        {
            OptionVM selectedOption = _recommendationsVM
                .Options
                .Single(o => o.OptionId == allocationOptionId);

            _profile.Recommendation = new Recommendation(
                selectedOption.AllocationName,
                selectedOption.Name,
                selectedOption.Composition.ToDictionary(
                    o => o.Portfolio,
                    o => o.Percent
                ),
                selectedOption.OptionId
            );

            _validationObserver.EnablePath(
                $"{(_profile.IsJoint ? "" : $"/client/{_profile.PrimaryClient.Guid}")}/profile/{_profile.Guid}/results"
            );
        }
    }
}