using System;
using Newtonsoft.Json;
using SharedKernel;

namespace Ata.Investment.Allocation.Domain
{
    public class HistoryDay : ValueObject<HistoryDay>
    {
        private DateTime _i;
        private decimal _v;

        public DateTime i
        {
            set => _i = value;
        }

        public decimal v
        {
            set => _v = value;
        }

        public DateTime Day => _i;
        public decimal Value => _v;
        [JsonIgnore]
        public string FundCode { get; set; }
    }
}