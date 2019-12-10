using Domain.Core.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Consumer.Domain.Events
{
    public class TransferCreatedEvent: Event
    {
        public string FirstName { get;private set; }
        public string SurName { get;private set; }
        public TransferCreatedEvent(string firstname,string surname)
        {
            FirstName = firstname;
            SurName = surname;
        }
    }
}
