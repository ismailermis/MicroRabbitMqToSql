using StackExchange.Redis;
using System;

namespace RedisTest
{
    class Program
    {
        private readonly IDatabase _db;
        private readonly ConnectionMultiplexer _redis;
        static void Main(string[] args)
        {

            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost:6379,password=adminadmin");
            IDatabase db = redis.GetDatabase();
            if (db.StringSet("testKey", "testValue"))
            {
                var val = db.StringGet("testKey");
                Console.WriteLine(val);
            }
        }
    }
}
