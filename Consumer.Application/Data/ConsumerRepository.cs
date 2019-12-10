using Consumer.Application.Interface;
using Infra.DataLayer.Context;
using Infra.DataLayer.Models;

namespace Consumer.Application.Data
{
    public class ConsumerRepository : IConsumerRepository
    {
        private readonly DataBaseContext _ctx;
        public ConsumerRepository(DataBaseContext dataBaseContext)
        {
            _ctx = dataBaseContext;
        }
        public void Add(TestTransfer transfer)
        {
            _ctx.TestTransfer.Add(transfer);
            _ctx.SaveChanges();

        }
    }
}
