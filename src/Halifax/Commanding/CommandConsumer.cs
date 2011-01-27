using System;
using Halifax.Storage.Aggregates;

namespace Halifax.Commanding
{
     /// <summary>
    /// Base class used for all command message consumers.
    /// </summary>
    public abstract class CommandConsumer
    {
        #region Nested type: For

        /// <summary>
        /// Base class used for distinguishing the type 
        /// of command message to consume.
        /// </summary>
        /// <typeparam name="TCOMMAND">Type of the command to consume</typeparam>
        public abstract class For<TCOMMAND>
            where TCOMMAND : Command
        {
            /// <summary>
            /// This will execute the current command against an aggregate within 
            /// a transaction session so that state changes can be pushed to all interested
            /// parties.
            /// </summary>
            /// <param name="session">Current unit of work session</param>
            /// <param name="theCommand">Current command to execute against the aggregate</param>
            /// <example>
            /// <![CDATA[
            /// public class UpdateProductPriceCommandConsumer
            ///   CommandConsumer.For<UpdatePriceCommand>
            /// {
            ///     private readonly IDomainRepository _repository;
            ///  
            ///     public UpdateProductPriceCommandConsumer(IDomainRepository repository)
            ///     {
            ///          _repository = repository;
            ///     }
            ///       
            ///     public override void Execute(IUnitOfWork session, UpdatePriceCommand theCommand)  
            ///     {
            ///           var product = _repository.Find<Product>(theCommand.Id);
            ///           
            ///           using(ITransactedSession txn = session.BeginTransaction(product))
            ///           {
            ///               product.UpdatePrice(theCommand.Price);
            ///           }
            ///     }
            /// }
            /// ]]>
            /// </example>
            public abstract void Execute(IUnitOfWork session, TCOMMAND theCommand);
        }

        #endregion
    }

  
}