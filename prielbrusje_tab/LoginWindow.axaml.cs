using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using prielbrusje_tab.Models;
using System;

namespace prielbrusje_tab;

public partial class LoginWindow : Window
{
    private DispatcherTimer _LogOutTimer = new() { Interval = new TimeSpan(0, 0, 1) }; //������ � ����� � �������
    private User _LogUser;
    private DateTime _LogTime = new(); //����� ��������� � ������� + 10 �����, �� ��������� ���-� ���������� �����
    private DateTime _Time = new();

    public LoginWindow()
    {
        _LogUser = new User() { Lastname = "������", Name = "Ivan", IdRole = 3 };

        InitializeComponent();

        grid_login.DataContext = _LogUser;
        RoleEnability(_LogUser.IdRole);
    }
    public LoginWindow(User user, DateTime dateTime)
    {
        _LogUser = user;
        _Time = dateTime;
        _LogTime = DateTime.Now.Add(new TimeSpan(0, _Time.Minute, _Time.Second));

        InitializeComponent();

        grid_login.DataContext = _LogUser;
        RoleEnability(_LogUser.IdRole);

        TimeSpan t = _LogTime - DateTime.Now;
        tblock_timer.Text = Convert.ToDateTime(t.ToString()).ToString("HH:mm");

        _LogOutTimer.Tick += DispatcherTimer_LogOut;
        _LogOutTimer.Start();
    }

    private void RoleEnability(int roleId)
    {
        switch (roleId)
        {
            case 1:
                {
                    btn_toOrderForming.IsEnabled = true;
                    btn_tovar.IsEnabled = true;
                    btn_toServicesList.IsEnabled = true;
                    btn_toLogList.IsEnabled = true;
                }
                break;
            case 2:
                {
                    btn_toOrderForming.IsEnabled = true;
                    btn_tovar.IsEnabled = true;
                }
                break;
            case 3:
                {
                    btn_toOrderForming.IsEnabled = true;
                }
                break;
            default:
                break;
        }
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
            tblock_timer.Text = _Time.ToString("HH:mm"); //� ������ �� ����� ������� �������
        }
        catch //����� �������� ���������� ������������� ���������, ����������� ����������. � ����� ������� ��� ��� �������� ������
        {
            _LogOutTimer.Stop(); //��������� �������
            MainWindow mainWindow = new MainWindow(false); //����������� ��������� ���� � ������������ ���������, ���������� � ���������� �����
            mainWindow.Show();
            Close();
        }
    }

    private void Button_ToServicesList(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _LogOutTimer.Stop();
        ServiceWindow serviceWindow = new ServiceWindow(_LogUser, _Time);
        serviceWindow.Show();
        Close();
    }

    private void Button_toLogList(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _LogOutTimer.Stop();
        LogWindow logWindow = new LogWindow(_LogUser, _Time);
        logWindow.Show();
        Close();
    }

    private void Button_LogOut(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _LogOutTimer.Stop(); //��������� �������
        MainWindow mainWindow = new MainWindow();
        mainWindow.Show();
        Close();
    }

    private void Button_ToOrderForming(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _LogOutTimer.Stop();
        OrderFormingWindow orderFormingWindow = new OrderFormingWindow(_LogUser, _Time);
        orderFormingWindow.Show();
        Close();
    }
}