﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Data.SqlClient;
using WpfMyShop.model;
using WpfMyShop.pages;

namespace WpfMyShop
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Window
    {
        public Dashboard()
        {
            InitializeComponent();
            MainScreen.Content = new DashBoardPage();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
        private bool IsMaximized=false;
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if(IsMaximized)
                {
                    this.WindowState = WindowState.Normal;
                    this.Width = 1080;
                    this.Height=720;
                    IsMaximized = false;
                }
                else
                {
                    this.WindowState = WindowState.Normal;
                    IsMaximized = true;
                }
            }
        }

        
        private void ProductBtn_Click(object sender, RoutedEventArgs e)
        {
            MainScreen.Content = new ProductPage();
        }
        

        private void btn_Genre_Click(object sender, RoutedEventArgs e)
        {
            MainScreen.Content = new GenrePage();
        }

        private void btn_Dashboard_Click(object sender, RoutedEventArgs e)
        {
            MainScreen.Content = new DashBoardPage();
        }
    }
}
