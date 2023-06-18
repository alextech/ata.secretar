using System;
using System.Collections.Generic;
using System.Text;

namespace Ata.Investment.Allocation.Data
{
    public class FundSyncDTO
    {
        public string FundCode { get; set; }
        public string MsUrl { get; set; }
        public string MsCode { get; set; }
        public DateTime LastSync { get; set; }
    }
}
