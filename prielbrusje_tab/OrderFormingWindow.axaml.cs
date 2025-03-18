using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using prielbrusje_tab.Models;
using System;
using System.Collections.ObjectModel;

namespace prielbrusje_tab;

public partial class OrderFormingWindow : Window
{
    private DispatcherTimer _LogOutTimer = new() { Interval = new TimeSpan(0, 0, 1) }; //������ � ����� � �������
    private User _LogUser;
    private DateTime _Time = new();
    private DateTime _LogTime = new();
    private Order _FormingOrder = new Order();
    private ClientInfo _FormingClientInfo = new ClientInfo();

    private ObservableCollection<Service> _AllServices = new ObservableCollection<Service>();
    private ObservableCollection<Service> _SelectedServices = new ObservableCollection<Service>();

    public OrderFormingWindow()
    {
        InitializeComponent();
    }
    public OrderFormingWindow(User user, DateTime time)
    {
        _LogUser = user;
        _Time = time;
        _LogTime = DateTime.Now.Add(new TimeSpan(0, _Time.Minute, _Time.Second));

        InitializeComponent();

        grid_formingOrder.DataContext = _LogUser;

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

    private void Button_ToLogWindow(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _LogOutTimer.Stop();
        LoginWindow loginWindow = new LoginWindow(_LogUser, _Time);
        loginWindow.Show();
        Close();
    }
}