using System;
using Halifax.Commanding;

namespace Halifax.Tests.Samples.OnlineOrdering.Domain.CreateCart
{
    [Serializable]
    public class CreateCartCommand : Command
    {
        public string Username { get; set; }

        public CreateCartCommand()
            : this(string.Empty)
        {
            
        }

        public CreateCartCommand(string username)
        {
            Username = username;
        }

        
    }
}