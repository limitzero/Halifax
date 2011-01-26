namespace Halifax.Tests.Samples.ATM.Domain.Accounts
{
    public enum  DepositTypes
    {
        /// <summary>
        /// Deposit to the account made in cash
        /// </summary>
        Cash, 

        /// <summary>
        /// Deposit made to the account via check submission 
        /// </summary>
        Check,

        /// <summary>
        /// Deposit made to the account from inter-bank account deposits
        /// </summary>
        Transfer,

        /// <summary>
        /// Deposit made to the account from outside entity account deposits
        /// </summary>
        Electronic
    }
}