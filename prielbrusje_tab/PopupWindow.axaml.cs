using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace prielbrusje_tab;

public partial class PopupWindow : Window
{
    public PopupWindow()
    {
        InitializeComponent();
    }
    public PopupWindow(string message)
    {
        InitializeComponent();
        tblock_message.Text = message;
    }
}