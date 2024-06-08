using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace BitService
{
    public partial class Autorization : Window
    {
        int currentUserId;
        public Autorization()
        {
            InitializeComponent();
            this.WindowState = WindowState.Maximized;        
        }

        private void Label_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Registration registration = new Registration();
            registration.Show();
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text;
            string password = PasswordTextBox.Text;
            string selectedRole = GetSelectedRole();

            try
            {
                using (var context = new bitServiceEntities())
                {
                    var user = context.autoriz
                        .Include("users")
                        .FirstOrDefault(u => u.login == login && u.password == password);

                    if (user != null)
                    {
                        currentUserId = user.userID;
                        var userType = user.users.type.FirstOrDefault()?.type1;

                        if (selectedRole == userType)
                        {
                            switch (selectedRole)
                            {
                                case "Заказчик":
                                    var requestsWindow = new RequestsClient(currentUserId);
                                    requestsWindow.Show();
                                    break;
                                case "Мастер":
                                    var masterWindow = new RequestsMaster(currentUserId);
                                    masterWindow.Show();
                                    break;
                                case "Менеджер":
                                    var managerWindow = new Requests();
                                    managerWindow.Show();
                                    break;
                                case "Оператор":
                                    var operatorWindow = new RequestsOperator();
                                    operatorWindow.Show();
                                    break;
                            }
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Роль пользователя не совпадает с выбранной ролью.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Неправильный логин или пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при попытке входа: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public string GetSelectedRole()
        {
            if (CustomerRadioButton.IsChecked == true) return "Заказчик";
            if (MasterRadioButton.IsChecked == true) return "Мастер";
            if (ManagerRadioButton.IsChecked == true) return "Менеджер";
            if (OperatorRadioButton.IsChecked == true) return "Оператор";
            return null;
        }
    }
}
