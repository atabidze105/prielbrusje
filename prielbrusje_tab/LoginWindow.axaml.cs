using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using prielbrusje_tab.Models;
using System;

namespace prielbrusje_tab;

public partial class LoginWindow : Window
{
    private DispatcherTimer _LogOutTimer = new() { Interval = new TimeSpan(0, 0, 1) }; //Таймер с шагом в секунду
    private User _LogUser;
    private DateTime _LogTime = new(); //Время вхождения в аккаунт + 10 минут, ао истечении кот-х происходит выход
    private DateTime _Time = new();

    public LoginWindow()
    {
        _LogUser = new User() { Lastname = "иванов", Name = "Ivan", IdRole = 3 };

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
        _LogOutTimer.Stop(); //Остановка таймера
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