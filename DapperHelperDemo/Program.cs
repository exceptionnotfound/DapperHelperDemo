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

            Console.WriteLine("--------------Update SQL-------------------");

            DapperHelper updateHelper = new DapperHelper("UpdateTableName", "WHERE ID = @id");
            updateHelper.AddCondition("id", 5);
            updateHelper.Add("DateTimeValue", DateTime.Now);
            updateHelper.Add("LongValue", 5827453L);
            
            //Uncomment the below line to see an exception thrown by DapperHelper
            //updateHelper.Add("DateTimeValue", DateTime.Now.AddDays(-7)); //This will be ignored

            var updateSql = updateHelper.UpdateSql;
            var updateParameters = updateHelper.Parameters;

            Console.WriteLine(updateSql);

            foreach (var parameterName in updateParameters.ParameterNames)
            {
                var value = updateParameters.Get<dynamic>(parameterName);
                Console.WriteLine(parameterName + ": " + value);
            }
        }
    }
}
