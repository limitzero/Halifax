using Halifax.Domain;

namespace Halifax.Commands
{
     /// <summary>
    /// Base class used for all command message consumers.
    /// </summary>
    public abstract class CommandConsumer
    {
        #region Nested type: For

		public abstract class For
		{
			
		}

        /// <summary>
        /// Base class used for distinguishing the type 
        /// of command message to consume.
        /// </summary>
        /// <typeparam name="T">Type of the command to consume</typeparam>
        public abstract class For<T> : For
            where T : Command
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
            ///     public override AggregateRoot Execute(IUnitOfWork session, UpdatePriceCommand theCommand)  
            ///     {
            ///           var product = _repository.Find<Product>(theCommand.Id);
			///           product.UpdatePrice(theCommand.Price);
			///			return product;
            /// }
            /// ]]>
            /// </example>
            public abstract AggregateRoot Execute(T theCommand);
        }

        #endregion
    }

  
}