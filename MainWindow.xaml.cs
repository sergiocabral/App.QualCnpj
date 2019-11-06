using System.Windows;

namespace QualCnpj
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TextBoxOutput.Text = WebEngine.GetCnpjForAll(TextBoxSource.Text);
        }
    }
}
