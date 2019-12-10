using Common.IoC;
using Infra.DataLayer.Interface;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Producer.Application.Interfaces;
using Producer.Application.Models;
using RedisControle;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Produser.App
{
    class Program
    {

        static void Main(string[] args)
        {
            var provider = DependencyContainer.Registerservices();
            var service = provider.GetRequiredService<ITestRecProducer>();
            var _testRecRepository = provider.GetRequiredService<ITestRecRepository>();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            bool flag = true;
          
            while (flag)
            {
                int lastId = RedisHelper.GetLastValue();
                lastId++;
                RedisHelper.SetLastValue(lastId.ToString());

                var test = _testRecRepository.Filter(null, q => q.OrderBy(s => s.id), lastId, 10);
               

               
                if (lastId > 10)
                {
                    Console.WriteLine("Aktatrım bitti.");
                    break;
                }
                foreach (var item in test)
                {
                    try
                    {
                        service.TransferToMQ(new TestTransfer()
                        {
                            FirstName = item.FirstName,
                            SurName = item.SurName
                        });
                        Console.WriteLine("devam");
                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine(ex.Message);
                    }
                }
            }
            stopwatch.Stop();
            Console.WriteLine("Time elapsed: {0:hh\\:mm\\:ss}", stopwatch.Elapsed);
        }

    }
}
