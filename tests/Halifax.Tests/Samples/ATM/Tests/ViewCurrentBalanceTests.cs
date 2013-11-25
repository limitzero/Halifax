using System;
using Halifax.Testing;
using Halifax.Tests.Samples.ATM.Domain.Accounts.DepositCash;
using Halifax.Tests.Samples.ATM.ReadModel;
using Machine.Specifications;

namespace Halifax.Tests.Samples.ATM.Tests
{
	public class when_a_deposit_is_made_on_the_account_and_request_is_made_to_view_the_balance
		: BaseEventConsumerTestFixture<CashDeposited>
	{
		private const decimal depositAmount = 150.00M;
		private static readonly string accountNumber = Guid.NewGuid().ToString();
		
		public override CashDeposited When()
		{
			return new CashDeposited(accountNumber, depositAmount);
		}

		It it_will_record_that_an_event_was_triggered_for_the_cash_being_deposited_to_the_account = () =>
		{
			PublishedEvents.Latest().WillBeOfType<CashDeposited>();
		};

		It it_will_display_the_correct_balance_for_the_account = () =>
		{
			var query = new AccountBalanceQuery(accountNumber);
			ExecuteQueryOverReadModel(query);
			depositAmount.ShouldEqual(query.Result);
		};
	}
}