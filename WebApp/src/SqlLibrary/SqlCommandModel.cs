using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SqlLibrary
{
    public class SqlCommandModel
    {
        public string       Command   { get; set; }
        public List<string> ResultSet { get; set; }

        public SqlCommandModel()
        {
            Command = "";
            ResultSet = new List<string>();
        }



        public string GetTableName()
        {

            bool hasFrom = false;

            foreach (string sqlToken in Command.Split(' ', '\r', '\n'))
            {
                if (hasFrom)
                {
                    return sqlToken;
                }

                hasFrom = string.Compare(sqlToken, "FROM", true) == 0;

            }
            return string.Empty;

        }

    }
}