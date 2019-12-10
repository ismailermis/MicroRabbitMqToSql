using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace RedisControle
{
    public static class RedisHelper
    {
        public static void SetLastValue(string value)
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("redis,password=adminadmin");
            IDatabase db = redis.GetDatabase();
            db.StringSet("lastId", value);
        }
        public static int GetLastValue()
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("redis,password=adminadmin");
            IDatabase db = redis.GetDatabase();
            int ret = Convert.ToInt32(db.StringGet("lastId"));
            return ret;
        }
    }
}

