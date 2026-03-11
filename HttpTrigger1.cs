using System;
using System.IO;
using System.Text.Json;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;

public class HttpTrigger1
{
    private readonly ILogger _logger;

    public HttpTrigger1(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<HttpTrigger1>();
    }

    [Function("insert-reading")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
    {
        _logger.LogInformation("Processing request.");

        var body = await new StreamReader(req.Body).ReadToEndAsync();
        var data = JsonSerializer.Deserialize<JsonElement>(body);

        double voltage = data.GetProperty("voltage").GetDouble();
        int error = data.GetProperty("error").GetInt32();

        string connString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING");

        using SqlConnection conn = new SqlConnection(connString);using System;
using System.IO;
using System.Text.Json;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using Microsoft.Extensions.Logging;

public class HttpTrigger1
{
    private readonly ILogger _logger;

    public HttpTrigger1(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<HttpTrigger1>();
    }

    [Function("insert-reading")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
    {
        var response = req.CreateResponse();
        try
        {
            _logger.LogInformation("Processing request.");

            var body = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonSerializer.Deserialize<JsonElement>(body);

            double voltage = data.GetProperty("voltage").GetDouble();
            int error = data.GetProperty("error").GetInt32();

            string connString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING");

            using SqlConnection conn = new SqlConnection(connString);
            await conn.OpenAsync();

            using SqlCommand cmd = new SqlCommand("InsertReadingProc", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            // Add parameters
            cmd.Parameters.AddWithValue("@ReadingTime", DateTime.UtcNow);
            cmd.Parameters.AddWithValue("@ReadingVoltage", voltage);
            cmd.Parameters.AddWithValue("@ReadingError", error);

            await cmd.ExecuteNonQueryAsync();

            await response.WriteStringAsync("Inserted successfully via stored procedure");
            response.StatusCode = HttpStatusCode.OK;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            response.StatusCode = HttpStatusCode.InternalServerError;
            await response.WriteStringAsync($"Error: {ex.Message}");
        }

        return response;
    }
}
        await conn.OpenAsync();

        string query = "INSERT INTO Readings (ReadingTime, ReadingVoltage, ReadingError) VALUES (@time,@voltage,@error)";
        using SqlCommand cmd = new SqlCommand(query, conn);

        cmd.Parameters.AddWithValue("@time", DateTime.UtcNow);
        cmd.Parameters.AddWithValue("@voltage", voltage);
        cmd.Parameters.AddWithValue("@error", error);

        await cmd.ExecuteNonQueryAsync();

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteStringAsync("Inserted successfully");

        return response;
    }
}