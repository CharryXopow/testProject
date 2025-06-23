using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Input;
using testProject.Helpers;
using testProject.Models;
using testProject.Views;

namespace testProject.ViewModels
{

    public class AddRecordViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<FieldInfo> Fields { get; set; } = new ObservableCollection<FieldInfo>();
        private string _tableName;
        public bool IsEditMode { get; set; }

        public ICommand SaveCommand { get; }

        public AddRecordViewModel(string tableName, object record = null)
        {
            _tableName = tableName;
            IsEditMode = record != null;

            LoadTableStructure(record);
            SaveCommand = new RelayCommand(OnSave);
        }
        //comment
        private void LoadTableStructure(object record)
        {
            using (var context = new AppDbContext())
            using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
            using (var cmd = new SqlCommand($"SELECT TOP 1 * FROM {_tableName}", conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly))
                {
                    var schemaTable = reader.GetSchemaTable();
                    foreach (DataRow row in schemaTable.Rows)
                    {
                        Fields.Add(new FieldInfo
                        {
                            Name = row["ColumnName"].ToString(),
                            Value = record == null ? "" : ((DataRowView)record).Row[row["ColumnName"].ToString()]
                        });
                    }
                }
            }
        }

        private void OnSave(object obj)
        {
            using (var context = new AppDbContext())
            using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
            using (var cmd = new SqlCommand())
            {
                conn.Open();
                cmd.Connection = conn;

                if (IsEditMode) // Если редактирование
                {
                    // Предполагаем, что первый столбец — это первичный ключ
                    var keyColumn = Fields[0].Name;
                    var keyValue = Fields[0].Value;

                    // Формируем SET часть запроса
                    var setClause = string.Join(", ", Fields.Skip(1).Select(f => $"{f.Name} = @{f.Name}"));

                    cmd.CommandText = $"UPDATE {_tableName} SET {setClause} WHERE {keyColumn} = @Id";

                    // Добавляем параметры
                    foreach (var field in Fields.Skip(1)) // Пропускаем ключ
                    {
                        cmd.Parameters.AddWithValue("@" + field.Name, field.Value ?? DBNull.Value);
                    }

                    cmd.Parameters.AddWithValue("@Id", keyValue);
                }
                else // Если добавление новой записи
                {
                    var columns = string.Join(", ", Fields.Select(f => f.Name));
                    var parameters = string.Join(", ", Fields.Select(f => "@" + f.Name));

                    cmd.CommandText = $"INSERT INTO {_tableName} ({columns}) VALUES ({parameters})";

                    foreach (var field in Fields)
                    {
                        cmd.Parameters.AddWithValue("@" + field.Name, field.Value ?? DBNull.Value);
                    }
                }

                cmd.ExecuteNonQuery();
            }

            NavigationHelper.GoBack();
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}