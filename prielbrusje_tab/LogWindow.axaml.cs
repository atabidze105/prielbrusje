using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Microsoft.EntityFrameworkCore;
using prielbrusje_tab.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using static prielbrusje_tab.Helper;

namespace prielbrusje_tab;

public partial class LogWindow : Window
{
    private DispatcherTimer _LogOutTimer = new() { Interval = new TimeSpan(0, 0, 1) }; //������ � ����� � �������
    private User _LogUser;
    private DateTime _Time = new();
    private DateTime _LogTime = new();
    private List<LoginHistory> _LoginHistory = DBContext.LoginHistories.Include(x => x.IdUsers).OrderBy(x => x.Id).ToList();

    public LogWindow()
    {
        InitializeComponent();

        lbox_loginHistory.ItemsSource = _LoginHistory.ToList();
    }
    public LogWindow(User user, DateTime dateTime)
    {
        _LogUser = user;
        _Time = dateTime;
        _LogTime = DateTime.Now.Add(new TimeSpan(0, _Time.Minute, _Time.Second));

        InitializeComponent();

        grid_logList.DataContext = _LogUser;

        lbox_loginHistory.ItemsSource = _LoginHistory.ToList();

        TimeSpan t = _LogTime - DateTime.Now;
        tblock_timer.Text = Convert.ToDateTime(t.ToString()).ToString("HH:mm:ss");

        _LogOutTimer.Tick += DispatcherTimer_LogOut;
        _LogOutTimer.Start();
    }

    private async void DispatcherTimer_LogOut(object? sender, EventArgs e) //����� ������ �� ���-� �� ��������� �������
    {
        try
        {
            TimeSpan ts = _LogTime - DateTime.Now; //�������� ����� ������� ��������� � ������� � 10 ����� � �������� �������
            if (new TimeSpan(0, ts.Minutes, ts.Seconds) == new TimeSpan(0, 4, 59)) //����� �� ��������� ������ �������� 5 �����
            {
                PopupWindow popupWindow = new PopupWindow("��������! ���� ������ �������������"); //���������� ���������
                await popupWindow.ShowDialog(this);
            }
            _Time = Convert.ToDateTime(ts.ToString()); //�������������� �������� ���� � DateTime ��� ������� ����������� � string
            tblock_timer.Text = _Time.ToString("HH:mm:ss"); //� ������ �� ����� ������� �������
        }
        catch //����� �������� ���������� ������������� ���������, ����������� ����������. � ����� ������� ��� ��� �������� ������
        {
            _LogOutTimer.Stop(); //��������� �������
            MainWindow mainWindow = new MainWindow(false); //����������� ��������� ���� � ������������ ���������, ���������� � ���������� �����
            mainWindow.Show();
            Close();
        }
    }

    private void Button_BackToLogin(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _LogOutTimer.Stop();
        LoginWindow loginWindow = new LoginWindow(_LogUser, _Time);
        loginWindow.Show();
        Close();
    }
}