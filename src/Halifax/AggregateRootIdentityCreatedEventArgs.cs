using System;

namespace Axiom
{
    public class AggregateRootIdentityCreatedEventArgs :EventArgs
    {
        public Guid Id { get; private set; }

        public AggregateRootIdentityCreatedEventArgs(Guid id)
        {
            Id = id;
        }
    }
}