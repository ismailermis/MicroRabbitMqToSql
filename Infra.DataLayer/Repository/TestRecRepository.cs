using Infra.DataLayer.Context;
using Infra.DataLayer.Interface;
using Infra.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Infra.DataLayer.Repository
{
    public class TestRecRepository : ITestRecRepository
    {
        private readonly DataBaseContext _ctx;
        public TestRecRepository(DataBaseContext context)
        {
            _ctx = context;
        }
        public void Add(TestTransfer transfer)
        {
            _ctx.TestTransfer.Add(transfer);
            _ctx.SaveChanges();
        }

        public IEnumerable<test_rec> Filter(Expression<Func<test_rec, bool>> filter = null, Func<IQueryable<test_rec>, IOrderedQueryable<test_rec>> orderBy = null, int? page = null, int? pageSize = null)
        {
            IQueryable<test_rec> query = _ctx.Set<test_rec>();
            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (page != null && pageSize != null)
            {
                query = query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
            }

            return query.ToList();
        }

        public IList<test_rec> GetTest_recList(Expression<Func<test_rec, bool>> filter = null)
        {
           return _ctx.test_rec.Where(filter).ToList();
        }
        
    }
}
