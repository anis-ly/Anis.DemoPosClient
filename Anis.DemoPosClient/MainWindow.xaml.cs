using System.Windows;

namespace Anis.DemoPosClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            var authenticationOptions = new AuthenticationOptions();
            var authenticationService = new AuthenticationService(authenticationOptions);
            authenticationService.AuthenticationStateChanged += (sender, args) =>
            {
                if (args.IsAuthenticated)
                {
                    MessageBox.Show("Login Succeed");
                }
                else
                {
                    MessageBox.Show("Login Failed");
                }
            };
            await authenticationService.LoginAsync();
        }
    }
}
