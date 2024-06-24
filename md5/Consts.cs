using System.Globalization;

namespace md5;

internal class Consts
{
    public static CultureInfo Culture { get; } = CreateCulture(CultureInfo.InvariantCulture);

    private static CultureInfo CreateCulture(CultureInfo BaseCulture)
    {
        var culture = (CultureInfo)BaseCulture.Clone();

        var number_format = culture.NumberFormat;
        number_format.NumberGroupSeparator = "'";


        return culture;
    }
}
