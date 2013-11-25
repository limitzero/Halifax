using Halifax.Testing;
using Halifax.Tests.Samples.ATM.Domain.Accounts;
using Halifax.Tests.Samples.ATM.Domain.Accounts.CreateAccount;
using Machine.Specifications;

namespace Halifax.Tests.Samples.ATM.Tests
{
    public class when_an_account_is_created_in_the_bank_for_the_customer_and_the_complete_information_is_provided
		: BaseAggregateRootTestFixture<Account, CreateAccountCommand>
    {
        private static readonly string firstName = "Joe";
		private static readonly string lastName = "Shmoe";
		private static readonly decimal initialAmount = 100.00M;

		public override CreateAccountCommand When()
        {
            // the complete information is supplied for creating the account:
            return new CreateAccountCommand(firstName, lastName, initialAmount);
        }

		It will_publish_the_event_noting_that_the_account_was_created = () =>
	   {
		   PublishedEvents.Latest().WillBeOfType<AccountCreated>();
	   };

		It it_will_have_an_physical_account_number_created = () =>
	   {
		   PublishedEvents.Latest<AccountCreated>().AccountNumber.WillNotBeNullOrEmpty();
	   };

		It it_will_have_the_correct_customer_attached_to_the_account = () =>
		{
			PublishedEvents.Latest<AccountCreated>().FirstName.WillBe(firstName);
			PublishedEvents.Latest<AccountCreated>().LastName.WillBe(lastName);
			PublishedEvents.Latest<AccountCreated>().InitialAmount.WillBe(initialAmount);
		};
		
    }
}