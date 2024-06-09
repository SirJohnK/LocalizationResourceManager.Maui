namespace LocalizationResourceManager.Maui;

/// <summary>
/// Markup extension (XAML) for handling and updating localized string by tracking current culture from current localization resource manager.
/// </summary>
[ContentProperty(nameof(Text))]
public class TranslateExtension : IMarkupExtension<BindingBase>
{
    public string Text { get; set; } = string.Empty;

    public string? StringFormat { get; set; }

    public IValueConverter? Converter { get; set; }

    public object? ConverterParameter { get; set; }

    public string? ResourceManager { get; set; }

    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => ProvideValue(serviceProvider);

    [Microsoft.Maui.Controls.Internals.Preserve(Conditional = true)]
    public BindingBase ProvideValue(IServiceProvider serviceProvider)
    {
        //Init
        ILocalizationResourceManager? resources = null;

        //Check if specific resource manager is specified!
        ResourceManager ??= (serviceProvider.GetService<IRootObjectProvider>()?.RootObject as ISpecificResourceManager)?.ResourceManager;

        //Get Localization Resource Manager
        if (ResourceManager is null)
            resources = serviceProvider.GetRequiredService<ILocalizationResourceManager>();
        else
            resources = serviceProvider.GetRequiredKeyedService<ILocalizationResourceManager>(ResourceManager);

        //Check if name with dots is supported and adjust Text!
        Text = resources.IsNameWithDotsSupported ? Text.Replace(".", resources.DotSubstitution) : Text;

        //Create Binding
        var binding = new Binding
        {
            Mode = BindingMode.OneWay,
            Path = $"[{Text}]",
            Source = resources,
            StringFormat = StringFormat,
            Converter = Converter,
            ConverterParameter = ConverterParameter
        };

        //Return Binding
        return binding;
    }
}