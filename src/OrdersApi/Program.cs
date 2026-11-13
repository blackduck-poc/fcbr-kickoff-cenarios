using Microsoft.Data.Sqlite;
using OrdersApi;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Orders API - demo application");

// SQL injection: the customer parameter is concatenated into the query.
app.MapGet("/orders", (string customer) =>
{
    using var connection = new SqliteConnection("Data Source=orders.db");
    connection.Open();
    using var command = connection.CreateCommand();
    command.CommandText = "SELECT id, customer, total FROM orders WHERE customer = '" + customer + "'";
    using var reader = command.ExecuteReader();
    var result = new List<string>();
    while (reader.Read()) result.Add($"{reader.GetInt32(0)};{reader.GetString(1)};{reader.GetDecimal(2)}");
    return Results.Ok(result);
});

// Cross-site scripting: user input is written into HTML without encoding.
app.MapGet("/greeting", (string name) =>
    Results.Content($"<html><body><h1>Hello, {name}</h1></body></html>", "text/html"));

// Path traversal: the file name comes straight from the request.
app.MapGet("/invoices", (string file) =>
    Results.Text(File.ReadAllText(Path.Combine("invoices", file))));

// Open redirect.
app.MapGet("/redirect", (string url) => Results.Redirect(url));

// Insecure deserialization of request body.
app.MapPost("/orders/import", async (HttpRequest request) =>
{
    using var reader = new StreamReader(request.Body);
    var body = await reader.ReadToEndAsync();
    var order = Serialization.Deserialize(body);
    return Results.Ok(order?.ToString());
});

app.MapGet("/hash", (string value) => Results.Text(Crypto.Md5(value)));

app.MapReports();

app.Run();
