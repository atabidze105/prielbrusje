using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using prielbrusje_tab.Models;

namespace prielbrusje_tab;

public partial class LoginWindow : Window
{
    private User _LogUser;
    public LoginWindow()
    {
        _LogUser = new User();
        InitializeComponent();
    }
    public LoginWindow(User user)
    {
        _LogUser = user;
        InitializeComponent();
    }
}