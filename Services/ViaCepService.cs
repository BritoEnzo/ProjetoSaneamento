using System.Text.Json;
using SaneaBrasil.Models;

namespace SaneaBrasil.Services;

/// <summary>
/// Cliente da API ViaCEP. A resposta já traz o código IBGE do município,
/// que é usado para consultar o saneamento.
/// Docs: https://viacep.com.br
/// </summary>
public class ViaCepService : ApiServiceBase, ICepService
{
    public ViaCepService(HttpClient http) : base(http) { }

    protected override string UrlBase => "https://viacep.com.br/ws/";
    protected override string NomeApi => "API ViaCEP";

    public static string ApenasDigitos(string texto) => new(texto.Where(char.IsDigit).ToArray());

    public async Task<Municipio?> BuscarMunicipioPorCepAsync(string cep, CancellationToken ct = default)
    {
        var digitos = ApenasDigitos(cep);
        if (digitos.Length != 8)
            throw new ArgumentException("O CEP precisa ter 8 dígitos.", nameof(cep));

        var json = await GetAsync<JsonElement>($"{digitos}/json/", ct);

        // CEP inexistente: o ViaCEP responde { "erro": true } (ou "true").
        if (json.ValueKind != JsonValueKind.Object || json.TryGetProperty("erro", out _))
            return null;

        var ibge = json.TryGetProperty("ibge", out var p) ? p.GetString() : null;
        if (!int.TryParse(ibge, out var codigo)) return null;

        return new Municipio
        {
            Id = codigo,
            Nome = json.TryGetProperty("localidade", out var l) ? l.GetString() ?? "" : "",
            Uf = json.TryGetProperty("uf", out var u) ? u.GetString() ?? "" : ""
        };
    }
}
