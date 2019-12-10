using System;
using System.Collections.Generic;
using System.Text;

namespace Producer.Domain.Commands
{
    public class CreateTransferCommand : TransferCommand
    {
        public CreateTransferCommand(string firstName, string surName)
        {
           FirstName = firstName;
           SurName = surName;
        }
    }
}
