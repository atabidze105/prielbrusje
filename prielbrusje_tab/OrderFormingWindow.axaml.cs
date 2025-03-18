using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using prielbrusje_tab.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using static prielbrusje_tab.Helper;

namespace prielbrusje_tab;

public partial class OrderFormingWindow : Window
{
    private DispatcherTimer _LogOutTimer = new() { Interval = new TimeSpan(0, 0, 1) }; //������ � ����� � �������
    private User _LogUser;
    private DateTime _Time = new();
    private DateTime _LogTime = new();
    private Order _FormingOrder = new Order();
    private ClientInfo _FormingClientInfo = new ClientInfo();
    private List<ClientInfo> _Clients = DBContext.ClientInfos.OrderBy(x => x.Id).ToList();

    private ObservableCollection<Service> _AllServices = new ObservableCollection<Service>( DBContext.Services.OrderBy(x => x.Id) );
    private ObservableCollection<Service> _SelectedServices = new ObservableCollection<Service>();

    public OrderFormingWindow()
    {
        InitializeComponent();

        lbox_allServices.ItemsSource = _AllServices;
        lbox_selectedServices.ItemsSource = _SelectedServices;
    }
    public OrderFormingWindow(User user, DateTime time)
    {
        _LogUser = user;
        _Time = time;
        _LogTime = DateTime.Now.Add(new TimeSpan(0, _Time.Minute, _Time.Second));

        InitializeComponent();

        cbox_clientInfos.ItemsSource = _Clients.ToList();
        grid_formingOrder.DataContext = _LogUser;
        panel_order.DataContext = _FormingOrder;
        spanel_selectedUserInfo.DataContext = _FormingClientInfo;

        TimeSpan t = _LogTime - DateTime.Now;
        tblock_timer.Text = Convert.ToDateTime(t.ToString()).ToString("HH:mm:ss");

        _LogOutTimer.Tick += DispatcherTimer_LogOut;
        _LogOutTimer.Start();

        lbox_allServices.ItemsSource = _AllServices;
        lbox_selectedServices.ItemsSource = _SelectedServices;
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

    private async void Button_AddClient(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        try
        {
            _FormingClientInfo = new ClientInfo();
            AddClientInfoWindow addClientInfoWindow = new AddClientInfoWindow(_FormingClientInfo);
            await addClientInfoWindow.ShowDialog(this);
            if (_FormingClientInfo.PassportSerie.Contains("_") || _FormingClientInfo.PassportCode.Contains("_") ||
                _FormingClientInfo.Address == "" || _FormingClientInfo.Address == null)
            {
                _FormingClientInfo = new ClientInfo();
                return;
            }

            spanel_selectedUserInfo.DataContext = _FormingClientInfo;
        }
        catch
        {

        }
    }

    private void ComboBox_SelectionChangedClientInfo(object? sender, Avalonia.Controls.SelectionChangedEventArgs e)
    {
        _FormingClientInfo = cbox_clientInfos.SelectedItem as ClientInfo;
        spanel_selectedUserInfo.DataContext = _FormingClientInfo;
    }
}