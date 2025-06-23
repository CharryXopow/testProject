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
    public class MainViewModel : INotifyPropertyChanged
    {

        public ObservableCollection<string> TableNames { get; set; }
        public DataView Records { get; set; }
        private string _selectedTable;

        public string SelectedTable
        {
            get => _selectedTable;
            set
            {
                _selectedTable = value;
                LoadTableData();
                OnPropertyChanged("SelectedTable");
            }
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        public MainViewModel()
        {
            TableNames = new ObservableCollection<string>(GetTableNames());
            AddCommand = new RelayCommand(OnAdd);
            EditCommand = new RelayCommand(OnEdit);
            DeleteCommand = new RelayCommand(OnDelete);
        }

        private void LoadTableData()
        {
            using (var context = new AppDbContext())
            using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
            using (var cmd = new SqlCommand($"SELECT * FROM {_selectedTable}", conn))
            using (var adapter = new SqlDataAdapter(cmd))
            {
                conn.Open();
                var dt = new DataTable();
                adapter.Fill(dt);
                Records = dt.DefaultView;
                OnPropertyChanged("Records");
            }
        }
        private void OnAdd(object obj)
        {
            if (string.IsNullOrEmpty(SelectedTable))
            {
                System.Windows.MessageBox.Show("Пожалуйста, выберите таблицу перед добавлением записи.", "Ошибка", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return;
            }

            var addPage = new AddRecordPage(SelectedTable, null);
            NavigationHelper.Navigate(addPage);
        }


        private void OnEdit(object obj)
        {
            NavigationHelper.Navigate(new AddRecordPage(_selectedTable, obj));
        }

        private void OnDelete(object obj)
        {
            var rowView = obj as DataRowView;
            if (rowView == null) return;

            using (var context = new AppDbContext())
            using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
            {
                conn.Open();
                var keyColumn = rowView.Row.Table.Columns[0].ColumnName;
                var keyValue = rowView.Row[0];

                using (var cmd = new SqlCommand($"DELETE FROM {_selectedTable} WHERE {keyColumn} = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", keyValue);
                    cmd.ExecuteNonQuery();
                }
            }

            LoadTableData();
        }

        private List<string> GetTableNames()
        {
            using (var context = new AppDbContext())
            using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
            {
                conn.Open();
                var schema = conn.GetSchema("Tables");
                return (from DataRow row in schema.Rows select $"{row["TABLE_SCHEMA"]}.{row["TABLE_NAME"]}").ToList();
            }
        }
        public void ReloadTableData()
        {
            LoadTableData();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}