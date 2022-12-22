using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace LocalizationResourceManager.Maui.ComponentModel;

/// <summary>
/// A base class for objects of which the properties must be observable.
/// </summary>
/// <remarks>
/// Uses WeakEventManager to avoid memory leaks!
/// </remarks>
public abstract class ObservableObject : INotifyPropertyChanged
{
    private readonly WeakEventManager weakEventManager = new WeakEventManager();

    public event PropertyChangedEventHandler? PropertyChanged
    {
        add => weakEventManager.AddEventHandler(value);
        remove => weakEventManager.RemoveEventHandler(value);
    }

    protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        weakEventManager.HandleEvent(this, e, nameof(INotifyPropertyChanged.PropertyChanged));
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetProperty<T>([NotNullIfNotNull("newValue")] ref T field, T newValue, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, newValue))
            return false;

        field = newValue;
        OnPropertyChanged(propertyName);

        return true;
    }
}