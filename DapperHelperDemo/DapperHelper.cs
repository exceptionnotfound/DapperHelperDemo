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
        private readonly Dictionary<string, object> _parameters = new Dictionary<string, object>();
        private readonly SortedDictionary<string, object> _conditionalParameters = new SortedDictionary<string, object>();

        private readonly string _tableName;
        private readonly string _whereSql;

        public DapperHelper(string tableName, string whereSql = null)
        {
            _tableName = tableName;
            _whereSql = whereSql;
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
            if (_parameters.ContainsKey(parameterName))
                throw new DuplicateNameException("This field was already declared");

            _parameters.Add(parameterName, value);
        }

        /// <summary>
        /// Method to add variables that will not be affected by insert or update
        /// operations but are necessary for use in the where clause
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        public void AddCondition(string parameterName, object value)
        {
            if (_parameters.ContainsKey(parameterName) || _conditionalParameters.ContainsKey(parameterName))
                throw new DuplicateNameException("This field was already declared");

            _conditionalParameters.Add(parameterName, value);
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
                if (_parameters.Keys.Count == 0)
                    throw new Exception("Attempted to perform an insert without any input parameters.");

                var fields = string.Join(", ", _parameters.Keys);
                var values = string.Join(", @", _parameters.Keys);
                return "DECLARE @output table(ID bigint); " +
                        $"INSERT INTO {_tableName}({fields}) " +
                        "OUTPUT INSERTED.[ID] " +
                        "INTO @output " +
                        $"VALUES(@{values}) " +
                        "SELECT * FROM @output;";
            }
        }

        /// <summary>
        /// Creates an update statement for a single record based on the key value pairs
        /// that have been added 
        /// Throws an exception if the collection is empty
        /// </summary>
        public string UpdateSql
        {
            get
            {
                if (string.IsNullOrEmpty(_whereSql))
                    throw new Exception("Attempted to perform an update without providing a where clause.  DapperHelper.UpdateSql");

                if (_parameters.Keys.Count == 0)
                    throw new Exception("Attempted to perform an update without any input parameters.  DapperHelper.UpdateSql");

                var sb = new StringBuilder();
                foreach (var parameterName in _parameters.Keys)
                {
                    sb.Append($"{parameterName} = @{parameterName}, ");
                }

                return $"UPDATE {_tableName} SET {sb.ToString().Substring(0, sb.Length - 2)} {_whereSql};";
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
                foreach (var parameterName in _parameters.Keys)
                {
                    parms.Add(parameterName, _parameters[parameterName]);
                }
                foreach (var parameterName in _conditionalParameters.Keys)
                {
                    parms.Add(parameterName, _conditionalParameters[parameterName]);
                }
                return parms;
            }
        }
    }
}
