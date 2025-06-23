using System.Windows.Controls;
using testProject.ViewModels;

namespace testProject.Views
{
    public partial class AddRecordPage : Page
    {
        public AddRecordPage(string tableName, object record = null)
        {
            InitializeComponent();
            DataContext = new AddRecordViewModel(tableName, record);
        }
        private void BackButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
        }
    }
}
