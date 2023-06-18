using Ata.Investment.Profile.Domain.Points.Tree.Operation;

namespace Ata.Investment.Profile.Domain.Points.Tree
{
    public abstract class Visitor
    {
        public abstract int Visit<T>(T expression);
        // public abstract int Visit(Sum expression);
        // public abstract int Visit(IncomeAmount incomeAmount);
        // public abstract int Visit(SingleValue value);
    }
}