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
        NivelSaneamento.Critico => Color.FromArgb("#FF3B30"),
        NivelSaneamento.Baixo => Color.FromArgb("#FF9500"),
        NivelSaneamento.Regular => Color.FromArgb("#FFCC00"),
        NivelSaneamento.Bom => Color.FromArgb("#34C759"),
        NivelSaneamento.Excelente => Color.FromArgb("#248A3D"),
        _ => Color.FromArgb("#8E8E93")
    };
}
