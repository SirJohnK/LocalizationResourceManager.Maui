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
public class TranslateExtension : IMarkupExtension<BindingBase>
{
    /// <summary>
    /// A localize string or a binding to a localize string.
    /// </summary>
    public object? Text { get; set; } = string.Empty;

    /// <summary>
    /// A string format to apply to the text string.
    /// </summary>
    public string? StringFormat { get; set; }

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
        #region Required work-around to prevent linker from removing the implementation
        if (DateTime.Now.Ticks < 0 && Text is string text)
            _ = LocalizationResourceManager.Current[text];
        #endregion Required work-around to prevent linker from removing the implementation

        List<object?> values = new List<object?>() { X0, X1, X2, X3, X4, X5, X6, X7, X8, X9 };
        while (values.Count > 0 && values[values.Count - 1] is null)
        {
            values.RemoveAt(values.Count - 1);
        }
        return NewBinding(Text, StringFormat, values.ToArray());
    }

    /// <summary>
    /// Creates a BindingBase to a localize string with optional arguments.
    /// Used by SetTranslate extension method.
    /// </summary>
    /// <example>
    /// CounterBtn.SetBinding(Button.TextProperty, TranslateExtension.NewBinding("ClickMe"));
    /// CounterBtnOne.SetBinding(Button.TextProperty, TranslateExtension.NewBinding("ClickedOneTime", null, count));
    /// CounterBtnMany.SetBinding(Button.TextProperty, TranslateExtension.NewBinding("ClickedManyTimes", null, count));
    /// </example>
    /// <param name="text">A localize string resource or a binding to a localize string resource</param>
    /// <param name="stringFormat">A string format to apply to the text string</param>
    /// <param name="arguments">An array of arguments or binding to arguments</param>
    /// <returns></returns>
    public static BindingBase NewBinding(object? text, string? stringFormat = null, params object?[] arguments)
    {
        if (text is string value && arguments.Length == 0)
        {
            if (LocalizationResourceManager.Current.IsNameWithDotsSupported)
            {
                value = value.Replace(".", LocalizationResourceManager.Current.DotSubstitution);
            }

            var binding = new Binding
            {
                Mode = BindingMode.OneWay,
                Path = $"[{value}]",
                Source = LocalizationResourceManager.Current,
                StringFormat = stringFormat
            };
            return binding;
        }

        Collection<BindingBase> bindings = new Collection<BindingBase>
        {
            (text is BindingBase textBinding) ? textBinding : new Binding(".", source: text),
            new Binding("CurrentCulture", source: LocalizationResourceManager.Current)
        };

        foreach (var arg in arguments)
        {
            bindings.Add((arg is BindingBase binding) ? binding : new Binding(".", source: arg));
        }

        return new MultiBinding()
        {
            Mode = BindingMode.OneWay,
            Bindings = bindings,
            StringFormat = stringFormat,
            Converter = new TranslateExtensionConverter()
        };
    }
}
