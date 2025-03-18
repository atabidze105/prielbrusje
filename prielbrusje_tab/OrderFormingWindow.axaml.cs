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
    private DispatcherTimer _LogOutTimer = new() { Interval = new TimeSpan(0, 0, 1) }; //Таймер с шагом в секунду
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
            tblock_timer.Text = _Time.ToString("HH:mm:ss"); //В тексте не будут указаны секунды
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
}