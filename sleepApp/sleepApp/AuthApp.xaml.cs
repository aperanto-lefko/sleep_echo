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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace sleepApp
{
    public partial class AuthApp : Window
    {
        public AuthApp()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e) //обработчик события кнопки войти
        {
            string login = LoginTextBox.Text; //логин из textBox
            string password = PasswordBox.Visibility == Visibility.Visible ? PasswordBox.Password : VisiblePasswordBox.Text; //пароль из PasswordBox

            if (login == "app_user" && password == "753214")
            {

                DashboardWindow dashBoardWindow = new DashboardWindow();
                dashBoardWindow.Show();
                this.Close();
            }
            else
            {
                HighlightIfError(LoginTextBox, login != "app_user");
                HighlightIfError(PasswordBox, password != "753214");
                HighlightIfError(VisiblePasswordBox, password != "753214");
            }
        }
        private void HighlightIfError(Control control, bool isError) //подсветка в случае ошибки
        {
            control.BorderBrush = isError ? Brushes.Red : SystemColors.ControlDarkBrush; //цвет рамки
            control.BorderThickness = isError ? new Thickness(2) : new Thickness(1); //толщина


        }

        private void ResetHighlight(object sender, RoutedEventArgs e) //обработчик чтобы сбросить красный цвет после ошибочного ввода
        {
            if (sender is Control control)
            {
                HighlightIfError(control, false);
            }
        }
        private void TogglePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            // Переключаем видимость пароля
            // Проверяем, видимо ли поле PasswordBox
            if (PasswordBox.Visibility == Visibility.Visible)
            {
                // Если PasswordBox видимо, значит, пароль сейчас скрыт.
                // Мы хотим показать пароль.
                // Копируем текст из PasswordBox в VisiblePasswordBox.
                // PasswordBox.Password содержит введённый пароль.
                VisiblePasswordBox.Text = PasswordBox.Password;
                // Делаем VisiblePasswordBox видимым, чтобы показать пароль.
                VisiblePasswordBox.Visibility = Visibility.Visible;
                // Скрываем PasswordBox, так как пароль теперь виден в VisiblePasswordBox
                PasswordBox.Visibility = Visibility.Collapsed;
            }
            else
            {
                // Если PasswordBox не видимо, значит, пароль сейчас показан в VisiblePasswordBox.
                // Мы хотим скрыть пароль.
                // Копируем текст из VisiblePasswordBox обратно в PasswordBox.
                // Это нужно, чтобы сохранить введённый пароль.
                PasswordBox.Password = VisiblePasswordBox.Text;
                // Делаем PasswordBox видимым, чтобы снова скрыть пароль.
                PasswordBox.Visibility = Visibility.Visible;
                // Скрываем VisiblePasswordBox, так как пароль теперь снова скрыт.
                VisiblePasswordBox.Visibility = Visibility.Collapsed;
            }
        }


    }
}
