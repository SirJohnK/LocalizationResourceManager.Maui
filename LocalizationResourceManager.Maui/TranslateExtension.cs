using System.Collections.ObjectModel;
using System.Globalization;

namespace LocalizationResourceManager.Maui;

/// <summary>
/// Markup extension (XAML) for handling and updating localized string by tracking current culture from current localization resource manager.
/// </summary>
/// <example>
/// <Button x:Name="CounterBtn" Text="{localization:Translate ClickMe}" />
/// <Button x:Name="CounterBtnOne" Text="{localization:Translate ClickedOneTime, X0={Binding Count}}" />
/// <Button x:Name="CounterBtnMany" Text="{localization:Translate ClickedManyTimes, X0={Binding Count}}" />
/// </example>
[ContentProperty(nameof(Text))]
public class TranslateExtension : BindableObject, IMarkupExtension<BindingBase>
{
    /// <summary>
    /// A localize string or a binding to a localize string.
    /// </summary>
    public static readonly BindableProperty TextProperty = BindableProperty.Create("Text", typeof(object), typeof(TranslateExtension), string.Empty);
    public object Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>
    /// Value or binding that will be used as argument {0} in the localized string
    /// </summary>
    public object? X0 { get; set; } = null;

    /// <summary>
    /// Value or binding that will be used as argument {1} in the localized string
    /// </summary>
    public object? X1 { get; set; } = null;

    /// <summary>
    /// Value or binding that will be used as argument {2} in the localized string
    /// </summary>
    public object? X2 { get; set; } = null;

    /// <summary>
    /// Value or binding that will be used as argument {3} in the localized string
    /// </summary>
    public object? X3 { get; set; } = null;

    /// <summary>
    /// Value or binding that will be used as argument {4} in the localized string
    /// </summary>
    public object? X4 { get; set; } = null;

    /// <summary>
    /// Value or binding that will be used as argument {5} in the localized string
    /// </summary>
    public object? X5 { get; set; } = null;

    /// <summary>
    /// Value or binding that will be used as argument {6} in the localized string
    /// </summary>
    public object? X6 { get; set; } = null;

    /// <summary>
    /// Value or binding that will be used as argument {7} in the localized string
    /// </summary>
    public object? X7 { get; set; } = null;

    /// <summary>
    /// Value or binding that will be used as argument {8} in the localized string
    /// </summary>
    public object? X8 { get; set; } = null;

    /// <summary>
    /// Value or binding that will be used as argument {9} in the localized string
    /// </summary>
    public object? X9 { get; set; } = null;

    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => ProvideValue(serviceProvider);

    /// <summary>
    /// A binding to a localized string resource.
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <returns>A binding to a localized string resource.</returns>
    public BindingBase ProvideValue(IServiceProvider serviceProvider)
    {
        object?[] values = { X0, X1, X2, X3, X4, X5, X6, X7, X8, X9 };
        int maxIndex = -1;
        for (int i = 0; i < values.Length; i++)
        {
            if (values[i] is not null)
            {
                maxIndex = i;
            }
        }

        return NewBinding(Text, values.Take(maxIndex + 1).ToArray());
    }

    /// <summary>
    /// Creates a BindingBase to a localize string with optional arguments.
    /// Used by SetTranslate extension method.
    /// </summary>
    /// <example>
    /// CounterBtn.SetBinding(Button.TextProperty, new TranslateExtension().NewBinding("ClickMe"));
    /// CounterBtnOne.SetBinding(Button.TextProperty, new TranslateExtension().NewBinding("ClickedOneTime", count));
    /// CounterBtnMany.SetBinding(Button.TextProperty, new TranslateExtension().NewBinding("ClickedManyTimes", count));
    /// </example>
    /// <param name="text">A localize string resource or a binding to a localize string resource</param>
    /// <param name="arguments">An array of arguments or binding to arguments</param>
    /// <returns></returns>
    public BindingBase NewBinding(object text, params object?[] arguments)
    {
        Collection<BindingBase> bindings = new Collection<BindingBase>
        {
            (text is BindingBase textBinding) ? textBinding : new Binding(".", source: text)
        };

        bindings.Add(new Binding("CurrentCulture", BindingMode.OneWay, null, null, null, LocalizationResourceManager.Current));

        foreach (var value in arguments)
        {
            bindings.Add((value is BindingBase binding) ? binding : new Binding(".", source: value));
        }

        return new MultiBinding()
        {
            Bindings = bindings,
            Converter = new TranslateExtensionConverter()
        };
    }


    internal sealed class TranslateExtensionConverter : IMultiValueConverter
    {
        public object? Convert(object?[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values is not null && values[0] is string text && values[1] is CultureInfo c)
            {
                if (LocalizationResourceManager.Current.IsNameWithDotsSupported)
                {
                    text = text.Replace(".", LocalizationResourceManager.Current.DotSubstitution);
                }

                object obj = LocalizationResourceManager.Current.GetValue(text);
                if (obj is null)
                {
                    return null;
                }

                if (values.Length > 2 && obj is string format)
                {
                    obj = string.Format(format, values.Skip(2).ToArray());
                }

                return $"{obj}";
            }

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
