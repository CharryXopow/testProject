using System.Windows.Controls;

namespace testProject.Helpers
{
    public static class NavigationHelper
    {
        public static Frame MainFrame { get; set; }

        public static void Navigate(Page page)
        {
            MainFrame.Navigate(page);
        }

        public static void GoBack()
        {
            if (MainFrame.CanGoBack)
                MainFrame.GoBack();
        }
    }
}