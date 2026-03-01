using StilettoSQL;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

async void Test() {
    await foreach (var rd in new Reader("select * from pg_catalog.pg_tables")
        .ReadAll()) {

        rd.LoadOut("test", out string val);

    }
}




