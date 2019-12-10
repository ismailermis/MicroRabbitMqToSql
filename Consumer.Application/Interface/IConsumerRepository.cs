using Infra.DataLayer.Models;

namespace Consumer.Application.Interface
{
    public interface IConsumerRepository
    {
        void Add(TestTransfer transfer);
    }
}
