using System.Text.Json;
using FluentAssertions;

namespace PostService.IntegrationTests.Helpers;

public static class HttpResponseMessageExtensions
{
    public static async Task EnsureSuccessWithBodyAsync(this HttpResponseMessage response)
    {
        var body = await response.Content.ReadAsStringAsync();
        response.IsSuccessStatusCode.Should().BeTrue(
            $"se esperaba una respuesta exitosa y se recibio {(int)response.StatusCode}. Body: {body}");
    }

    public static async Task<T> ReadRequiredJsonAsync<T>(this HttpResponseMessage response, JsonSerializerOptions jsonOptions)
    {
        var body = await response.Content.ReadAsStringAsync();
        response.IsSuccessStatusCode.Should().BeTrue(
            $"se esperaba una respuesta exitosa y se recibio {(int)response.StatusCode}. Body: {body}");

        var value = JsonSerializer.Deserialize<T>(body, jsonOptions);
        return value ?? throw new InvalidOperationException(
            $"No se pudo deserializar la respuesta al tipo '{typeof(T).Name}'. Body: {body}");
    }
}
