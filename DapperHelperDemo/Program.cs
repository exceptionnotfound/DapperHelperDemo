using System;

namespace DapperHelperDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            DapperHelper helper = new DapperHelper("TableName");
            helper.Add("IntValue", 5);
            helper.Add("DateTimeValue", DateTime.Now);
            helper.Add("StringValue", "This is a string");

            var insertSql = helper.InsertSql;
            var parameters = helper.Parameters;

            Console.WriteLine("---------------Insert SQL------------------");
            Console.WriteLine(insertSql);
            Console.WriteLine("---------------Parameters------------------");
            foreach(var parameterName in parameters.ParameterNames)
            {
                var value = parameters.Get<dynamic>(parameterName);
                Console.WriteLine(parameterName + ": " + value);
            }
        }
    }
}
