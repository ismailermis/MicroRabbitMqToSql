using Consumer.Domain.Events;
using Domain.Core.Bus;
using Infra.DataLayer.Interface;
using Infra.DataLayer.Models;
using System.Threading.Tasks;

namespace Consumer.Domain.EventHandlers
{
    public class TransferEventHandler : IEventHandler<TransferCreatedEvent>
    {
        private readonly ITestRecRepository _consumerRepository;
        public TransferEventHandler(ITestRecRepository consumerRepository)
        {
            _consumerRepository = consumerRepository;
        }
        public Task Handle(TransferCreatedEvent @event)
        {
           
            _consumerRepository.Add(new TestTransfer(){ 
                FirstName = @event.FirstName,
                SurName = @event.SurName
                
                });
            return Task.CompletedTask;
        }
    }
}
