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
            Console.WriteLine("------------Insert Parameters--------------");
            foreach (var parameterName in parameters.ParameterNames)
            {
                var value = parameters.Get<dynamic>(parameterName);
                Console.WriteLine(parameterName + ": " + value);
            }

            Console.WriteLine("--------------Update SQL-------------------");

            //Remove the WHERE clause to see an exception thrown by DapperHelper.
            DapperHelper updateHelper = new DapperHelper("UpdateTableName", "WHERE ID = @id");
            updateHelper.AddCondition("id", 5);
            updateHelper.Add("DateTimeValue", DateTime.Now);
            updateHelper.Add("LongValue", 5827453L);
            //Uncomment the below line to see an exception thrown by DapperHelper
            //updateHelper.Add("DateTimeValue", DateTime.Now.AddDays(-7));

            var updateSql = updateHelper.UpdateSql;
            var updateParameters = updateHelper.Parameters;

            Console.WriteLine(updateSql);

            Console.WriteLine("-----------Update Parameters---------------");

            foreach (var parameterName in updateParameters.ParameterNames)
            {
                var value = updateParameters.Get<dynamic>(parameterName);
                Console.WriteLine(parameterName + ": " + value);
            }


            string connectionString = "Your Connection String";
            DatabaseService myDB = new DatabaseService(connectionString);

            //The lines below this point will throw errors at runtime,
            //since the connection string above doesn't do anything.
            //Comment them out to run the program.

            DapperHelper insertHelper = new DapperHelper("Users"); //"Users" is the SQL table name

            insertHelper.Add("FirstName", "John");
            insertHelper.Add("LastName", "Smith");
            insertHelper.Add("DateOfBirth", new DateTime(1970, 1, 1));

            myDB = new DatabaseService("ConnectionString");
            int userID = myDB.ExecuteInsert<int>(helper);

            updateHelper = new DapperHelper("Users", "WHERE ID = @ID");
            updateHelper.AddCondition("ID", 5);
            updateHelper.Add("FirstName", "Josephina");
            updateHelper.Add("LastName", "Ezekiel-Smith");

            myDB = new DatabaseService("ConnectionString");
            myDB.ExecuteUpdate(updateHelper);

            //Comment out the lines to this point to run the sample

            Console.ReadLine();
        }
    }
}
