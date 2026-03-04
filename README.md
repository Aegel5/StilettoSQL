# StilettoSQL
Ultra-minimalistic SQL library for C#

## Examples
`Query` for simple and static queries.
```csharp
await foreach (var rd in Query.ReadAllRows("SELECT * FROM table WHERE id=$1", 123)){
    var id = rd.Val<long>("id");
}
```
`QueryBuilder` for any complex or dynamic queries
```csharp
var deleted_cnt = await new QueryBuilder("DELETE from table")
    .WhereAndEq("val1", 10)
    .WhereAndEq("val2", 20)
    .ExecuteGetRowsTouched(); // SQL: DELETE from table WHERE (val1=$1) AND (val2=$2)
```
`Inserter` for basic insert queries
```csharp
var inserted_id = await new Inserter("table")
    .SkipOnConflict()
    .Value("id", 123)
    .ExecuteReturnField<long?>("id"); // SQL: INSERT into table(id) VALUES ($1) on conflict do nothing returning id
if (inserted_id != null) {
    Console.WriteLine("Was inserted!");
}
```
