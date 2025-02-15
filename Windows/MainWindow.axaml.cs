using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using avalonia_application.Entities;
using Windows;

namespace avalonia_application;

public partial class MainWindow : Window
{
    private AvaloniaContext context = new AvaloniaContext();
    public MainWindow()
    {
        InitializeComponent();
    }

    public void login_Click(object sender, RoutedEventArgs e) {
        User u = context.Users.FirstOrDefault(c =>
            c.Username == username_textBox.Text &&
            c.Password == password_textBox.Text);
        if(u != null) {
            ProductsListWindow plw = new ProductsListWindow();
            plw.Closed += (sender, e) => {
                this.Show();
            };
            this.Hide();
            plw.Show();
        }
    }
}
