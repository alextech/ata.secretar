using System.Collections.Generic;
using System.Linq;

namespace SharedKernel
{
    public class RecommendationsVM
    {
        public List<OptionVM> Options { get; } = new List<OptionVM>();

        private readonly Stack<string> _availableColors = new Stack<string>(new []
        {
            "#7E7F73", "#009FC8", "#D43825", "#F1B51C", "#D05F27", "#99B035", // avoid crashing while looking for colors
            "#7E7F73", "#009FC8", "#D43825", "#F1B51C", "#D05F27", "#99B035"
        } );

        private readonly Dictionary<string, string> _assignedColors = new Dictionary<string, string>();

        public void AssignColors()
        {
            foreach (OptionVM optionVM in Options)
            {
                foreach (CompositionVM compositionVM in optionVM.Composition)
                {
                    if (!_assignedColors.ContainsKey(compositionVM.Portfolio))
                    {
                        _assignedColors[compositionVM.Portfolio] = _availableColors.Pop();
                    }

                    compositionVM.Color = _assignedColors[compositionVM.Portfolio];
                }
            }
        }

        public void Oder()
        {
            foreach (OptionVM optionVM in Options)
            {
                optionVM.Composition = optionVM.Composition.OrderBy(c => c.Percent).ToList();
            }
        }
    }

    public class OptionVM
    {
        public string AllocationName { get; set; }
        public string Name { get; set; }
        public int OptionId { get; set; }
        public string ChartId => $"option_chart_{OptionId}";
        public bool IsSelected { get; set; }
        public List<CompositionVM> Composition { get; set; }
    }

    public class CompositionVM
    {
        public int Percent { get; set; }
        public string Portfolio { get; set; }
        public string Color { get; set; }
    }


}