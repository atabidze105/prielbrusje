using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using prielbrusje_tab.Models;
using System;
using System.Threading.Tasks;
using static prielbrusje_tab.Helper;

namespace prielbrusje_tab;

public partial class AddClientInfoWindow : Window
{
    private ClientInfo _Client;
    private Random _Random = new Random();
    public AddClientInfoWindow()
    {
        InitializeComponent();

        panel_clientInfo.DataContext = _Client;

    }
    public AddClientInfoWindow(ClientInfo clientInfo)
    {
        _Client = clientInfo;

        InitializeComponent();

        while (_Client.Code == null || _Client.Code.Length < 8)
        {
            _Client.Code += $"{ _Random.Next(0,9) }";
        };

        panel_clientInfo.DataContext = _Client;


    }

    private void Button_Cancel(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _Client = new();

        this.Close();
    }

    private async void Button_Confirm(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        try
        {
            if (_Client.PassportSerie == null || _Client.PassportCode == null ||
                _Client.PassportSerie.Contains("_") || _Client.PassportCode.Contains("_") ||
                clndrdatepicker_birthday.SelectedDate == null || _Client.Address == "" || _Client.Address == null)
            {
                PopupWindow popupWindow = new PopupWindow("Ошибка! Данные введены некорректно.");
                await popupWindow.ShowDialog(this);
                return;
            }

            _Client.DateOfBirth = new DateOnly(clndrdatepicker_birthday.SelectedDate.Value.Year, clndrdatepicker_birthday.SelectedDate.Value.Month, clndrdatepicker_birthday.SelectedDate.Value.Day);

            DBContext.ClientInfos.Add(_Client);
            DBContext.SaveChanges();
            this.Close();
        }
        catch 
        {
            PopupWindow popupWindow = new PopupWindow("Ошибка! Данные введены некорректно.");
            await popupWindow.ShowDialog(this);
        }
    }
}