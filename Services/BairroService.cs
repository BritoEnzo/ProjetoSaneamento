using System.Globalization;
using System.Text;
using SaneaBrasil.Models;

namespace SaneaBrasil.Services;

public class BairroService : IBairroService
{
    private static readonly IReadOnlyList<Bairro> Bairros =
    [
        new() { Nome = "A.E. Carvalho" },
        new() { Nome = "Aricanduva" },
        new() { Nome = "Artur Alvim" },
        new() { Nome = "Belém" },
        new() { Nome = "Brás" },
        new() { Nome = "Cachoeira" },
        new() { Nome = "Cangaíba" },
        new() { Nome = "Carrão" },
        new() { Nome = "Cidade Líder" },
        new() { Nome = "Cidade Patriarca" },
        new() { Nome = "Cidade Tiradentes" },
        new() { Nome = "Cohab Juscelino Kubitschek" },
        new() { Nome = "Ermelino Matarazzo" },
        new() { Nome = "Itaim Paulista" },
        new() { Nome = "Itaquera" },
        new() { Nome = "Jardim Helena" },
        new() { Nome = "Jardim Iguatemi" },
        new() { Nome = "José Bonifácio" },
        new() { Nome = "Lageado" },
        new() { Nome = "Mooca" },
        new() { Nome = "Parque do Carmo" },
        new() { Nome = "Penha" },
        new() { Nome = "Ponte Rasa" },
        new() { Nome = "São Mateus" },
        new() { Nome = "São Miguel Paulista" },
        new() { Nome = "Sapopemba" },
        new() { Nome = "Tatuapé" },
        new() { Nome = "Vila Curuçá" },
        new() { Nome = "Vila Formosa" },
        new() { Nome = "Vila Matilde" },
        new() { Nome = "Vila Prudente" }
    ];

    public IReadOnlyList<Bairro> Buscar(string nome)
    {
        var termo = RemoverAcentos(nome).Trim();
        if (termo.Length == 0) return Bairros;

        return Bairros
            .Where(bairro => RemoverAcentos(bairro.Nome).Contains(termo, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    private static string RemoverAcentos(string texto)
    {
        var normalizado = texto.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder(normalizado.Length);

        foreach (var caractere in normalizado)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(caractere) != UnicodeCategory.NonSpacingMark)
                sb.Append(caractere);
        }

        return sb.ToString().Normalize(NormalizationForm.FormC);
    }
}
