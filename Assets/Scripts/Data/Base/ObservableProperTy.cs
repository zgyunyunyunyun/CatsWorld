using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class ObservableProperty<T>
{
    private T value;
    public event Action<T, T> OnValueChanged;

    public ObservableProperty(T initialValue = default)
    {
        value = initialValue;
    }

    public T Value
    {
        get => value;
        set
        {
            if (!EqualityComparer<T>.Default.Equals(this.value, value))
            {
                var oldValue = this.value;
                this.value = value;
                OnValueChanged?.Invoke(this.value, oldValue);
            }
        }
    }
}
