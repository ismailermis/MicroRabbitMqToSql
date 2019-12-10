using Domain.Core.Bus;
using MediatR;
using Producer.Domain.Commands;
using Producer.Domain.Events;
using System.Threading;
using System.Threading.Tasks;

namespace Producer.Domain.CommandHandlers
{
    public class TransferCommandHandler : IRequestHandler<CreateTransferCommand, bool>
    {
        private readonly IEventBus _bus;
         public TransferCommandHandler(IEventBus eventBus)
        {
            _bus = eventBus;
        }

        public Task<bool> Handle(CreateTransferCommand request, CancellationToken cancellationToken)
        {
            // publkish evet to RabbitMQ
             
            _bus.Publish(new TransferCreatedEvent(request.FirstName, request.SurName));
            return Task.FromResult(true);
        }
    }
}
