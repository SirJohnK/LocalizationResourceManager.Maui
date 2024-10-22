namespace LocalizationResourceManager.Maui;

/// <summary>
/// Provides the SetTranslate extension method.
/// </summary>
public static class TranslateExtensionMethod
{
    /// <summary>
    /// .NET MAUI SetTranslate extension method that enable VisualElement objects to respond to localization changes.
    /// </summary>
    /// <param name="bindable">The BindableObject</param>
    /// <param name="targetProperty">The BindableProperty on which to set a localized string</param>
    /// <param name="text">A localized string or a binding to a localize string</param>
    /// <param name="args">A collection of arguments or a binding to arguments</param>
    /// <example>
    /// // count as constant arguments
    /// if (count == 0)
    ///     CounterBtn.SetTranslate(Button.TextProperty, "ClickMe");
    /// else if (count == 1)
    ///     CounterBtn.SetTranslate(Button.TextProperty, "ClickedOneTime", count);
    /// else
    ///     CounterBtn.SetTranslate(Button.TextProperty, "ClickedManyTimes", count);
    /// // count as a binding
    /// CounterBtn.SetTranslate(Button.TextProperty, "ClickedManyTimes", new Binding(nameof(Count), source: this));
    /// </example>
    public static void SetTranslate(this BindableObject bindable, BindableProperty targetProperty, object? text, params object?[] args)
        => bindable.SetBinding(targetProperty, TranslateExtension.NewBinding(text, null, args));
}
