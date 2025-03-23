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
    private DispatcherTimer _LogOutTimer = new() { Interval = new TimeSpan(0, 0, 1) }; //Таймер с шагом в секунду
    private User _LogUser;
    private DateTime _Time = new();
    private DateTime _LogTime = new();
    private Order _FormingOrder = new Order();
    private ClientInfo _FormingClientInfo = new ClientInfo();
    private List<ClientInfo> _Clients = DBContext.ClientInfos.OrderBy(x => x.Id).ToList();
    private List<Status> _Statuses = DBContext.Statuses.OrderBy(x => x.Id).ToList();

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
        tblock_timer.Text = Convert.ToDateTime(t.ToString()).ToString("HH:mm");

        _LogOutTimer.Tick += DispatcherTimer_LogOut;
        _LogOutTimer.Start();

        lbox_allServices.ItemsSource = _AllServices;
        lbox_selectedServices.ItemsSource = _SelectedServices;
        cbox_orderStatus.ItemsSource = _Statuses;
    }



    private async void DispatcherTimer_LogOut(object? sender, EventArgs e) //Метод выхода из акк-а по истечению времени
    {
        try
        {
            TimeSpan ts = _LogTime - DateTime.Now; //РАЗНОСТЬ суммы времени вхождения в аккаунт и 10 минут И текущего времени
            if (new TimeSpan(0, ts.Minutes, ts.Seconds) == new TimeSpan(0, 4, 59)) //Когда до окончания сессии остается 5 минут
            {
                PopupWindow popupWindow = new PopupWindow("Внимание! Ваша сессия заканчивается"); //появляется сообщение
                await popupWindow.ShowDialog(this);
            }
            _Time = Convert.ToDateTime(ts.ToString()); //Преобразование РАЗНОСТИ выше в DateTime для удобной конвертации в string
            tblock_timer.Text = _Time.ToString("HH:mm"); //В тексте не будут указаны секунды
        }
        catch //Когда РАЗНОСТЬ становится отрицательным значением, срабатывает исключение. К этому времени как раз истекает сессия
        {
            _LogOutTimer.Stop(); //Остановка таймера
            MainWindow mainWindow = new MainWindow(false); //Отображение основного окна с передающимся значением, сообщающим о блокировке входа
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

            tbox_orderCode.Text = $"{_FormingClientInfo.Code}/{DateTime.Now.Day}.{DateTime.Now.Month}.{DateTime.Now.Year}";
            spanel_selectedUserInfo.DataContext = _FormingClientInfo;
        }
        catch
        {

        }
    }

    private void ComboBox_SelectionChangedClientInfo(object? sender, Avalonia.Controls.SelectionChangedEventArgs e)
    {
        _FormingClientInfo = cbox_clientInfos.SelectedItem as ClientInfo;

        tbox_orderCode.Text = $"{_FormingClientInfo.Code}/{new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).ToString("dd.MM.yyyy")}";
        spanel_selectedUserInfo.DataContext = _FormingClientInfo;
    }

    private void Button_AddToList(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var button = (sender as Button)!;
        _SelectedServices.Add(_AllServices.Where(x => x.Id == (int)button.Tag).First());
        _AllServices.Remove(_AllServices.Where(x => x.Id == (int)button.Tag).First());

        lbox_allServices.ItemsSource = _AllServices;
        lbox_selectedServices.ItemsSource = _SelectedServices;
    }

    private void Button_DelFromList(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var button = (sender as Button)!;
        _AllServices.Add(_SelectedServices.Where(x => x.Id == (int)button.Tag).First());
        _SelectedServices.Remove(_SelectedServices.Where(x => x.Id == (int)button.Tag).First());

        lbox_allServices.ItemsSource = _AllServices;
        lbox_selectedServices.ItemsSource = _SelectedServices;
    }

    private async void Button_AddOrder(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        try
        {
            int id = 0;

            if (_FormingClientInfo.Id == 0)
            {
                id = DBContext.ClientInfos.OrderByDescending(x => x.Id).First().Id + 1;
                DBContext.ClientInfos.Add(_FormingClientInfo);
            }
            else
                id = _FormingClientInfo.Id;

            foreach (Service service in _SelectedServices)
                _FormingOrder.IdServices.Add(service);

            _FormingOrder.IdClient = id;

            _FormingOrder.DateTimeOrder = DateTime.Now;
            _FormingOrder.RentTime = new TimeOnly(timepicker_rentTime.SelectedTime.Value.Hours, timepicker_rentTime.SelectedTime.Value.Minutes);

            DBContext.Orders.Add(_FormingOrder);
            DBContext.SaveChanges();

            _LogOutTimer.Stop();
            LoginWindow loginWindow = new LoginWindow(_LogUser, _Time);
            loginWindow.Show();
            Close();
        }
        catch
        {
            PopupWindow popupWindow = new PopupWindow("Внимание! Данные введены некорректно");
            await popupWindow.ShowDialog(this);
        }
    }
}