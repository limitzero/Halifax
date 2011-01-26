namespace Halifax.Tests.Samples.ATM.ReadModel
{
    /// <summary>
    /// View projection from the data storage for the balance of an account.
    /// </summary>
    public class AccountBalanceView
    {
        public string AccountNumber { get; set; }
        public decimal CurrentBalance { get; set; }
    }
}