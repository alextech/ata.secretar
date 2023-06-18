using System;
using System.Globalization;
using Newtonsoft.Json;

namespace Ata.Investment.Allocation.Domain
{
    public class VersionDraft
    {
        [JsonProperty]
        public int Version { get; private set; }

        [JsonProperty]
        public DateTimeOffset Date { get; private set; }
        
        public string Draft { get; set; }

        public string Description { get; set; }
        
        public string Notes { get; set; }

        [JsonConstructor]
        public VersionDraft(int version)
        {
            Version = version;
            CreateDateFromVersion();
        }

        private void CreateDateFromVersion()
        {
            string format = CalculateFormat();
            Date = DateTimeOffset.ParseExact(Version.ToString(), format, CultureInfo.InvariantCulture);
        }

        private string CalculateFormat()
        {
            
            if (Version <= 9999) // year month 1803
            {
                return "yyMM"; 
            }

            if (Version <= 999999) // year month day
            {
                return "yyMMdd";
            }

            if (Version <= 99999999) // year month day hour
            {
                return "yyMMddHH";
            }

            return "yyMMddHHmm";

//            throw new Exception("Out of range version");
        }
    }
}