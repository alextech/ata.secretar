using System;
using SharedKernel;
using Ata.Investment.Profile.Domain.Points.Tree;

namespace Ata.Investment.Profile.Domain.Profile
{
    public class NetWorth : ValueObject<NetWorth>, ICloneable, IExpression
    {
        public NetWorth () : this(0, 0, 0) { }

        public NetWorth(int liquidAssets, int fixedAssets, int liabilities)
        {
            LiquidAssets = liquidAssets;
            FixedAssets = fixedAssets;
            Liabilities = liabilities;

            Notes = "";
        }

        public NetWorth(int liquidAssets, int fixedAssets, int liabilities, string notes)
        {
            LiquidAssets = liquidAssets;
            FixedAssets = fixedAssets;
            Liabilities = liabilities;
            
            Notes = notes;
        }
        public int LiquidAssets { get; set; }
        public int FixedAssets { get; set; }
        public int Liabilities { get; set; }

        public string Notes { get; set; }

        public int Total => LiquidAssets + FixedAssets - Liabilities;

        public object Clone()
        {
            return new NetWorth(LiquidAssets, FixedAssets, Liabilities, Notes);
        }
    }
}