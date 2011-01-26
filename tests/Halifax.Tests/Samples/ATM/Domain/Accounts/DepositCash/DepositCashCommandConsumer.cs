using Halifax;
using Halifax.Commanding;
using Halifax.Storage.Aggregates;
using Halifax.Tests.Samples.ATM.Domain.Accounts;

namespace Halifax.Tests.Samples.ATM.Domain.Accounts.DepositCash
{
    public class DepositCashCommandConsumer : 
        CommandConsumer.For<DepositCashCommand>
    {
        private readonly IDomainRepository _repository;

        public DepositCashCommandConsumer(IDomainRepository repository)
        {
            _repository = repository;
        }

        public override void Execute(IUnitOfWorkSession session, DepositCashCommand command)
        {
            var account = _repository.Find<Account>(command.Id);
            account.MakeCashDeposit(command);
            session.Accept(account);
        }
    }
}