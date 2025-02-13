namespace DiegoG.Finance.Internal;

public interface IFinanciallyAnnotated
{
    public decimal Amount { get; set; }
    public ExpenseCategory Category { get; }
}
