using Avalonia.Controls;
using Microsoft.EntityFrameworkCore;
using prielbrusje_tab.Models;
using System.Collections.Generic;
using System.Linq;
using static prielbrusje_tab.Helper;

namespace prielbrusje_tab
{
    public partial class MainWindow : Window
    {
        private List<User> _Users = DBContext.Users.Include(x => x.IdClientInfoNavigation).Include(x => x.IdRoleNavigation).Include(x => x.IdLogins).ToList();
        public MainWindow()
        {
            InitializeComponent();

        }

        private void Button_Login(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            try
            {
                foreach (User user in _Users.Where(x => x.Login == tbox_login.Text)) 
                {
                    if (user.Password == tbox_password.Text)
                    {
                        LoginWindow loginWindow = new LoginWindow(user);
                        loginWindow.Show();
                        Close();
                    }
                }
            }
            catch { }
        }

        private void Button_MaskOff(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            tbox_password.PasswordChar = '\0';
        }

    }
}