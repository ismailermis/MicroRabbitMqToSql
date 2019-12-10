using Infra.DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infra.DataLayer.Context
{
    public class DataBaseContext :DbContext
    {
        public DataBaseContext(DbContextOptions options):base(options)
        {

        }
       public DbSet<test_rec> test_rec { get;set;}
       public DbSet<TestTransfer> TestTransfer { get;set; }
    }
}
