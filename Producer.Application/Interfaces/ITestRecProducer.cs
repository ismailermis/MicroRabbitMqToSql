using Producer.Application.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Producer.Application.Interfaces
{
    public interface ITestRecProducer
    {
        void TransferToMQ(TestTransfer testRecModel);
    }
}
