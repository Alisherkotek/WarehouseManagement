namespace WarehouseManagement.Blazor.Services;

using System.Net.Http.Json;
using System.Text.Json;

public class ApiClient : IApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public ApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<T?> GetAsync<T>(string endpoint)
    {
        try
        {
            var response = await _httpClient.GetAsync(endpoint);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"API Error: {response.StatusCode} for {endpoint}");
                return default;
            }

            var content = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(content))
            {
                return default;
            }

            if (content.TrimStart().StartsWith("<"))
            {
                Console.WriteLine(
                    $"Received HTML instead of JSON from {endpoint}. Check if API is running and URL is correct.");
                return default;
            }

            return JsonSerializer.Deserialize<T>(content, _jsonOptions);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"HTTP Request Error: {ex.Message}");
            return default;
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"JSON Parse Error: {ex.Message}");
            return default;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected Error: {ex.Message}");
            return default;
        }
    }

    public async Task<T?> PostAsync<T>(string endpoint, object data)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(endpoint, data);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"API Error: {response.StatusCode} for {endpoint}");
                return default;
            }

            var content = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(content))
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(content, _jsonOptions);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in POST request: {ex.Message}");
            return default;
        }
    }

    public async Task<T?> PutAsync<T>(string endpoint, object data)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync(endpoint, data);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"API Error: {response.StatusCode} for {endpoint}");
                return default;
            }

            var content = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(content))
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(content, _jsonOptions);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in PUT request: {ex.Message}");
            return default;
        }
    }

    public async Task<bool> DeleteAsync(string endpoint)
    {
        try
        {
            var response = await _httpClient.DeleteAsync(endpoint);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in DELETE request: {ex.Message}");
            return false;
        }
    }
}