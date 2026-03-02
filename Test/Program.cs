using StilettoSQL;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var rd = await QueryBase.Create("select * from table where id=$$")
    .Add("id", 100)
    .ReadOneRow();
rd.LoadOut("id", out long id);

async void Test() {
    await foreach (var rd in new QueryBase("select * from pg_catalog.pg_tables")
        .ReadAllRows()) {

        rd.LoadOut("test", out string val);

    }
}




