using System.Globalization;
using SaneaBrasil.Models;

namespace SaneaBrasil.Converters;

/// <summary>Converte o nível de saneamento numa cor (vermelho → verde).</summary>
public class NivelParaCorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is NivelSaneamento nivel ? Cor(nivel) : Colors.Gray;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();

    public static Color Cor(NivelSaneamento nivel) => nivel switch
    {
        NivelSaneamento.Critico => Color.FromArgb("#C62828"),
        NivelSaneamento.Baixo => Color.FromArgb("#EF6C00"),
        NivelSaneamento.Regular => Color.FromArgb("#F9A825"),
        NivelSaneamento.Bom => Color.FromArgb("#7CB342"),
        NivelSaneamento.Excelente => Color.FromArgb("#2E7D32"),
        _ => Colors.Gray
    };
}
