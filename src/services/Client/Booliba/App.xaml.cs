// This code is under Copyright (C) 2021 of Arkia Consulting SAS all right reserved

using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.PlatformConfiguration.WindowsSpecific;
using Microsoft.Maui.Essentials;
using Application = Microsoft.Maui.Controls.Application;

namespace Booliba
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }
    }
}
