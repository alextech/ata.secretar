using Newtonsoft.Json;
using SharedKernel;

namespace Ata.Investment.Allocation.Domain.Composition
{
    public class CompositionPart : ValueObject<CompositionPart>
    {
        // TODO maybe convert to DTO
        public int Percent { get; set; }
        public string FundCode { get; set; }

        [JsonIgnore]
        public Fund Fund { get; set; }
        
        public CompositionPart() {}

        public CompositionPart(string fundCode, int percent)
        {
            Percent = percent;
            FundCode = fundCode;
        }
    }
}
