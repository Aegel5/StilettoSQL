using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StilettoSQL;

public interface IDbProvider {
    public Task<DbDataReader> ExecuteReader(string sql, Dictionary<string, DataToDb> parms);
    public Task<object?> ExecuteScalar(string sql, Dictionary<string, DataToDb> parms);
    public Task<int> ExecuteNonQuery(string sql, Dictionary<string, DataToDb> parms);

}

