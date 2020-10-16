using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DapperHelperDemo
{
    /// <summary>
    /// The DapperHelper manages the base validation of input values and allows for the easy generation of well formed Insert and Update statements
    /// </summary>
    public class DapperHelper
    {
        private readonly SortedDictionary<string, object> _collection = new SortedDictionary<string, object>();

        private readonly string _tableName;

        public DapperHelper(string tableName)
        {
            _tableName = tableName;
        }

        /// <summary>
        /// Adds the supplied parameterName to value mapping to a collection
        /// of items used in the insert and update values to be modified.
        /// This method does not check for existing values
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        public void Add(string parameterName, object value)
        {
            if (_collection.ContainsKey(parameterName))
                throw new DuplicateNameException("This field was already declared");

            _collection.Add(parameterName, value);
        }


        /// <summary>
        /// Creates an insert statement for a single record that returns the bigint 
        /// identity of the record inserted
        /// Throws an exception if the collection is empty
        /// </summary>
        public string InsertSql
        {
            get
            {
                if (_collection.Keys.Count == 0)
                    throw new Exception("Attempted to perform an insert without any input parameters.  DapperHelper.InsertSql");

                var fields = string.Join(", ", _collection.Keys);
                var values = string.Join(", @", _collection.Keys);
                return "DECLARE @output table(ID bigint); " +
                       $"INSERT INTO {_tableName}({fields}) " +
                        "OUTPUT INSERTED.[ID] " +
                        "INTO @output " +
                        $"VALUES(@{values}) " +
                        "SELECT * FROM @output;";
            }
        }

        /// <summary>
        /// Generates a DynamicParameters instance based on the key value pairs
        /// that have been added
        /// </summary>
        public DynamicParameters Parameters
        {
            get
            {
                var parms = new DynamicParameters();
                foreach (var parameterName in _collection.Keys)
                {
                    parms.Add(parameterName, _collection[parameterName]);
                }
                return parms;
            }
        }
    }
}
