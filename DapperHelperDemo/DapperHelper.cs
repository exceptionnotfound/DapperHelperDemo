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
        private readonly SortedDictionary<string, object> _conditions = new SortedDictionary<string, object>();

        private readonly string _tableName;

        public DapperHelper(string tableName)
        {
            _tableName = tableName;
        }

        public bool HasChanges => _collection.Keys.Count > 0;

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
        /// Method to add variables that will not be affected by insert or update
        /// operations but are necessary for use in the where clause
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        public void AddCondition(string parameterName, object value)
        {
            if (_collection.ContainsKey(parameterName))
                throw new DuplicateNameException("This field was already declared");

            _conditions.Add(parameterName, value);
        }

        /// <summary>
        /// Checks if the supplied bool exists prior to adding it to the collection of parameters
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        /// <returns>True if Added</returns>
        public bool SafeAdd(string parameterName, bool? value)
        {
            if (string.IsNullOrEmpty(value.ToString())) return false;

            Add(parameterName, value);
            return true;
        }

        public bool SafeAdd<TEnum>(string parameterName, string value) where TEnum : struct
        {
            if (Enum.TryParse<TEnum>(value, out var enumValue))
            {
                Add(parameterName, enumValue);
                return true;
            }

            return false;
        }

        public bool SafeAdd(string parameterName, int? value)
        {
            if (!value.HasValue) return false;

            Add(parameterName, value);
            return true;
        }

        public bool SafeAdd(string parameterName, long? value)
        {
            if (!value.HasValue) return false;

            Add(parameterName, value);
            return true;
        }

        /// <summary>
        /// Checks if the supplied DateTime exists prior to adding it to the collection of parameters
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        /// <returns>True if Added</returns>
        public bool SafeAdd(string parameterName, DateTime? value)
        {
            if (value > Convert.ToDateTime("01/01/1900"))
            {
                Add(parameterName, value);
                return true;
            }

            return false;
        }

        public bool SafeAdd(string parameterName, decimal? value)
        {
            if (!value.HasValue) return false;

            Add(parameterName, value);
            return true;
        }

        /// <summary>
        /// Checks if the supplied string is not null or empty prior to adding it to the collection of parameters
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        /// <returns>True if Added</returns>
        public bool SafeAdd(string parameterName, string value)
        {
            if (string.IsNullOrEmpty(value)) return false;

            Add(parameterName, value);
            return true;
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

                foreach (var parameterName in _conditions.Keys)
                {
                    parms.Add(parameterName, _conditions[parameterName]);
                }
                return parms;
            }
        }
    }
}
