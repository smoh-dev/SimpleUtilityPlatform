using System.Data;
using System.Net.Http.Headers;
using System.Text.Json;
using Sup.Mm.BackgroundClient.Services;
using Sup.Mm.Common.DTO;

namespace Sup.Mm.BackgroundClient;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly AuthService _authService;
    public Worker(ILogger<Worker> logger, AuthService authService)
    {
        _logger = logger;
        _authService = authService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // if (_logger.IsEnabled(LogLevel.Information))
            // {
            //     _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            // }

            var notes = await GetNotesAsync();
            _logger.LogInformation("Get {note_count} notes.", notes.Count);

            await Task.Delay(1000 * 30, stoppingToken);
        }
    }

    private async Task<List<NoteTagDto>> GetNotesAsync()
    {
        var requestUrl = "http://localhost:5224/Note/notes";
        using var client = new HttpClient();
        try
        {
            var token = await _authService.GetAccessTokenAsync();
            if (string.IsNullOrEmpty(token))
                throw new Exception("Failed to get access token.");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync(requestUrl);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var res = JsonSerializer.Deserialize<List<NoteTagDto>>(json);
            if (res == null)
                throw new NoNullAllowedException("Deserialization failed.");
            return res;
        }
        catch (Exception ex)
        {
            return new List<NoteTagDto>();            
        }
    }
}