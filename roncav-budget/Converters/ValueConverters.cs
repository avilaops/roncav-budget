using System.Globalization;

namespace roncav_budget.Converters;

public class SaldoToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
{
  if (value is decimal saldo)
        {
    return saldo >= 0 ? Color.FromArgb("#4CAF50") : Color.FromArgb("#F44336");
        }
     return Color.FromArgb("#9E9E9E");
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
    throw new NotImplementedException();
  }
}

public class TipoToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
        if (value is string tipo)
  {
       return tipo switch
            {
 "Receita" => Color.FromArgb("#4CAF50"),
  "Despesa" => Color.FromArgb("#F44336"),
     "Transferência" => Color.FromArgb("#2196F3"),
_ => Color.FromArgb("#9E9E9E")
        };
        }
     return Color.FromArgb("#9E9E9E");
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
   throw new NotImplementedException();
    }
}

public class BoolToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool efetivada)
   {
            return efetivada ? Color.FromArgb("#4CAF50") : Color.FromArgb("#FF9800");
   }
        return Color.FromArgb("#9E9E9E");
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
  throw new NotImplementedException();
    }
}

public class PercentToProgressConverter : IValueConverter
{
public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
     if (value is decimal percent)
  {
   return (double)(percent / 100);
}
    return 0.0;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class PercentToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
 if (value is decimal percent)
        {
     return percent switch
         {
     <= 50 => Color.FromArgb("#4CAF50"),
         <= 80 => Color.FromArgb("#FFC107"),
   <= 100 => Color.FromArgb("#FF9800"),
       _ => Color.FromArgb("#F44336")
     };
      }
  return Color.FromArgb("#9E9E9E");
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
