using CoreLibrary;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.DependencyInjection;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;



// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace SqlLibrary
{
    public class SqlController : Controller
    {
        [HttpGet,Route("sql")]
        public IActionResult Index()
        {
            return View("SqlView" , new SqlCommandModel { });
        }



        private SqlCommand createCommand(string connectionStringKey,string sqlCommand)
        {
            IConfigProvider  provider = Context.ApplicationServices.GetRequiredService<IConfigProvider>();

            return new SqlCommand(sqlCommand, new SqlConnection(provider.GetString(connectionStringKey)));
        }


        public async Task<IActionResult> ExecuteReader(SqlCommandModel model)
        {
            SqlCommand command = createCommand("System.QueryBuilder.MsSql\\Connection", model.Command);

            await command.Connection.OpenAsync();

            return null;
        }


        private async Task<SqlCommandModel> appendSqlValues(StringBuilder insertPattern, SqlCommandModel model, SqlDataReader dataReader)
        {
            StringBuilder sqlBuffer = new StringBuilder();

            while (await dataReader.ReadAsync())
            {
                sqlBuffer.Length = 0;
                sqlBuffer.Append(insertPattern);

                for (int i = 0; i < dataReader.FieldCount; i++)
                {
                    if (i > 0)
                    {
                        sqlBuffer.Append(',');
                    }

                    sqlBuffer.AppendConstant(dataReader.GetFieldType(i), dataReader.GetValue(i));
                }

                sqlBuffer.Append(')');

                model.ResultSet.Add(sqlBuffer.ToString());

            }

            return model;

        }




        [HttpPost, Route("sql/execute")]
        public async Task<IActionResult> ExecuteNonQuery(SqlCommandModel model)
        {
            SqlCommand command = createCommand("System.QueryBuilder.MsSql\\Connection", model.Command);

            await command.Connection.OpenAsync();

            SqlDataReader dataReader = await command.ExecuteReaderAsync();

            StringBuilder insertPattern = new StringBuilder("insert into ").Append(model.GetTableName()).Append('(');
            StringBuilder valuesPattern = new StringBuilder("values(");

            SqlCommand remoteInsertCommand = createCommand("System.QueryBuilder.MsSql\\RemoteConnection" , string.Empty);

            for (int i = 0; i < dataReader.FieldCount; i++)
            {
                if (i > 0)
                {
                    insertPattern.Append(',');
                    valuesPattern.Append(',');
                }

             
                string fieldName = dataReader.GetName(i);

                remoteInsertCommand.Parameters.Add(fieldName, dataReader.GetFieldType(i).GetDbType());

                insertPattern.Append('[').Append(fieldName).Append(']');
                valuesPattern.Append('@').Append(fieldName);
            }


            remoteInsertCommand.CommandText = new StringBuilder().Append(insertPattern).Append(')').Append(valuesPattern).Append(')').ToString();


            await remoteInsertCommand.Connection.OpenAsync();

            while (await dataReader.ReadAsync())
            {

                for (int i = 0; i < dataReader.FieldCount; i++)
                {
                    remoteInsertCommand.Parameters[i].Value = dataReader.GetValue(i);
                }

                await remoteInsertCommand.ExecuteNonQueryAsync();
            }

            return View("SqlView", model);

            return View("SqlView", await appendSqlValues(insertPattern.Append(")values("), model, dataReader));
        }
    }
}
