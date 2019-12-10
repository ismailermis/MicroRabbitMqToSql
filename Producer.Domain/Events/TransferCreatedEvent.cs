using Domain.Core.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Producer.Domain.Events
{
    public class TransferCreatedEvent : Event
    {
        public string FirstName { get; private set; }
        public string SurName { get; private set; }
        public TransferCreatedEvent(string  firstName, string surName)
        {
            FirstName = firstName;
            SurName = surName;

        }
    }
}
