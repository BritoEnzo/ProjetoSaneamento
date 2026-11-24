using System.Text.Json;
using System.Text.Json.Serialization;

namespace SaneaBrasil.Services;

/// <summary>Erro de comunicação com uma API externa, com mensagem amigável.</summary>
public class ApiException : Exception
{
    public ApiException(string mensagem, Exception? interna = null) : base(mensagem, interna) { }
}

/// <summary>
/// Classe base abstrata de todos os clientes de API REST do app.
/// Centraliza o HttpClient, a URL base e a desserialização de JSON (herança + reuso).
/// </summary>
public abstract class ApiServiceBase
{
    protected static readonly JsonSerializerOptions OpcoesJson = new()
    {
        PropertyNameCaseInsensitive = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString
    };

    protected ApiServiceBase(HttpClient http) => Http = http;

    protected HttpClient Http { get; }

    /// <summary>URL base da API (cada serviço define a sua).</summary>
    protected abstract string UrlBase { get; }

    /// <summary>Nome amigável da API, usado nas mensagens de erro.</summary>
    protected abstract string NomeApi { get; }

    protected async Task<T?> GetAsync<T>(string caminho, CancellationToken ct = default)
    {
        try
        {
            using var resposta = await Http.GetAsync(UrlBase + caminho, ct);
            if (!resposta.IsSuccessStatusCode)
                throw new ApiException($"{NomeApi} respondeu com erro {(int)resposta.StatusCode}.");

            await using var conteudo = await resposta.Content.ReadAsStreamAsync(ct);
            return await JsonSerializer.DeserializeAsync<T>(conteudo, OpcoesJson, ct);
        }
        catch (ApiException) { throw; }
        catch (OperationCanceledException) { throw; }
        catch (HttpRequestException ex)
        {
            throw new ApiException($"Não foi possível conectar à {NomeApi}. Verifique sua internet.", ex);
        }
        catch (JsonException ex)
        {
            throw new ApiException($"A {NomeApi} retornou dados num formato inesperado.", ex);
        }
    }
}
