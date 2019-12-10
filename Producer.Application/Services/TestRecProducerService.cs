using Domain.Core.Bus;
using Producer.Application.Interfaces;
using Producer.Application.Models;
using Producer.Domain.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Producer.Application.Services
{
    public class TestRecProducerService : ITestRecProducer
    {
        private readonly IEventBus _bus;
        public TestRecProducerService(IEventBus eventBus)
        {
            _bus= eventBus;
        }
        public void TransferToMQ(TestTransfer testRecModel)
        {
            var createTransferCommand= new CreateTransferCommand(
                testRecModel.FirstName,
                testRecModel.SurName
                );
            _bus.SendCommand(createTransferCommand);
        }
    }
}
