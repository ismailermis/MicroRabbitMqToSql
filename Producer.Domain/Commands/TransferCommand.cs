using Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Producer.Domain.Commands
{
    public abstract class TransferCommand : Command
    {
        public string FirstName { get; set; }
        public string SurName { get; set; }
        public bool IsEndPublish { get; set; }
    }
}
