using System.Windows;
using testProject.Helpers;
using testProject.Views;

namespace testProject
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            NavigationHelper.MainFrame = MainFrame;
            MainFrame.Navigate(new MainPage());
        }
    }
}