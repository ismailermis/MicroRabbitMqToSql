using Infra.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Infra.DataLayer.Interface
{
    public interface ITestRecRepository
    {
        IList<test_rec> GetTest_recList(Expression<Func<test_rec, bool>> filter = null);
        IEnumerable<test_rec> Filter(
           Expression<Func<test_rec, bool>> filter = null,
           Func<IQueryable<test_rec>, IOrderedQueryable<test_rec>> orderBy = null,
           int? page = null,
           int? pageSize = null);
        void Add(TestTransfer transfer);
    }
}
