using StilettoSQL.Profile;
using StilettoSQL.Query;

//StGlobal.DefaultProfile = new StProfile { CreateConnection = () => ... };

////await foreach (var rd in Query.ReadAllRows("SELECT * FROM table WHERE id=$1", 123)) {
////    var id = rd.Val<long>("id");
////}

//var res = await Query.ExecuteGetRowsTouched("Select ' OR''OR ??' from table where a=??", 10);

//var deleted_cnt = await new QueryBuilder("DELETE from table")
//    .WhereAndEq("col1", 10)
//    .WhereAnd("col1 * col2 > ??", 20)
//    .ExecuteGetRowsTouched();

////var inserted_id = await new Inserter("table")
////    .SkipOnConflict()
////    .Value("id", 123)
////    .ExecuteReturnField<long?>("id");
////if (inserted_id != null) {
////    Console.WriteLine("Was inserted!");
////}




