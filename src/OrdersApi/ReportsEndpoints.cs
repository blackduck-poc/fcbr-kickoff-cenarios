using System.Diagnostics;
using Microsoft.Data.Sqlite;

namespace OrdersApi;

public static class ReportsEndpoints
{
    // Hard-coded credential.
    private const string AdminPassword = "Adm1n!2024";

    public static void MapReports(this WebApplication app)
    {
        // OS command injection: the script name comes from the query string.
        app.MapGet("/reports/run", (HttpRequest request) =>
        {
            var script = request.Query["script"].ToString();
            var psi = new ProcessStartInfo("/bin/sh", "-c \"" + script + "\"")
            {
                RedirectStandardOutput = true
            };
            using var process = Process.Start(psi)!;
            return Results.Text(process.StandardOutput.ReadToEnd());
        });

        // SQL injection through the search term.
        app.MapGet("/reports/search", (HttpRequest request) =>
        {
            var term = request.Query["term"].ToString();
            using var connection = new SqliteConnection("Data Source=orders.db");
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT title FROM reports WHERE title LIKE '%" + term + "%'";
            return Results.Ok(command.ExecuteScalar()?.ToString());
        });

        app.MapGet("/reports/login", (HttpRequest request) =>
            request.Query["password"] == AdminPassword ? Results.Ok("ok") : Results.Unauthorized());
    }
}
