using Avalonia.Controls;
using Avalonia.Threading;
using Microsoft.EntityFrameworkCore;
using prielbrusje_tab.Models;
using System.Collections.Generic;
using System.Linq;
using System;
using static prielbrusje_tab.Helper;

namespace prielbrusje_tab
{
    public partial class MainWindow : Window
    {
        private List<User> _Users = DBContext.Users.Include(x => x.IdClientInfoNavigation).Include(x => x.IdRoleNavigation).Include(x => x.IdLogins).OrderBy(x => x.Id).ToList();
        private DispatcherTimer _BlockTimer = new DispatcherTimer() { Interval = new TimeSpan(0,3,0) };
        public MainWindow()
        {
            InitializeComponent();

        }
        public MainWindow(bool block)
        {
            InitializeComponent();
            btn_login.IsEnabled = block;
            _BlockTimer.Tick += DispatcherTimer_Tick;
            _BlockTimer.Start();
        }

        private void DispatcherTimer_Tick(object? sender, EventArgs e)
        {
            btn_login.IsEnabled = true;
            _BlockTimer.Stop();
        }

        private void Button_Login(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            try
            {
                foreach (User user in _Users.Where(x => x.Login == tbox_login.Text)) 
                {
                    if (user.Password == tbox_password.Text)
                    {
                        user.IdLogins.Add(new LoginHistory() { LoginDateTime = DateTime.Now, IsValid = true });
                        //DBContext.SaveChanges();
                        LoginWindow loginWindow = new LoginWindow(user, new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 10, 0));
                        loginWindow.Show();
                        Close();
                    }
                    else
                    {
                        user.IdLogins.Add(new LoginHistory() { LoginDateTime = DateTime.Now, IsValid = false });
                        DBContext.SaveChanges();
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