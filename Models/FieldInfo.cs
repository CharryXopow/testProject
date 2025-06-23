using System;
using System.ComponentModel;

public class FieldModel : INotifyPropertyChanged, IDataErrorInfo
{
    public string Name { get; set; }

    private object _value;
    public object Value
    {
        get => _value;
        set
        {
            _value = value;
            OnPropertyChanged(nameof(Value));
        }
    }

    public string DataType { get; set; } // Например: "int", "datetime", "nvarchar"

    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    // IDataErrorInfo реализация
    public string this[string columnName]
    {
        get
        {
            if (columnName == nameof(Value))
            {
                // Пример: проверяем на пустое значение
                if (Value == null || string.IsNullOrWhiteSpace(Value.ToString()))
                    return "Поле не может быть пустым.";

                // Пример: проверяем числовое поле
                if (DataType == "int" && !int.TryParse(Value.ToString(), out _))
                    return "Введите корректное целое число.";

                // Пример: проверяем дату
                if (DataType == "datetime" && !DateTime.TryParse(Value.ToString(), out _))
                    return "Введите корректную дату.";
            }
            return null;
        }
    }

    public string Error => null;
}
