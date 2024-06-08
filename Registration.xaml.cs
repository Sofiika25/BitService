using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace BitService
{
    public partial class Registration : Window
    {
        public Registration()
        {
            InitializeComponent();
            this.WindowState = WindowState.Maximized;
        }
        public void Label_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Autorization autorization = new Autorization();
            autorization.Show();
            this.Close();
        }
        public bool IsNameValid(string name)
        {
            return !string.IsNullOrWhiteSpace(name) && name.Length <= 30 && name.All(c => char.IsLetter(c) || char.IsWhiteSpace(c));
        }
        public bool IsPhoneNumberValid(string phoneNumber)
        {
            return !string.IsNullOrWhiteSpace(phoneNumber) && phoneNumber.Length == 11 && phoneNumber.All(char.IsDigit);
        }
        public bool IsLoginValid(string login)
        {
            return !string.IsNullOrWhiteSpace(login) && login.Length <= 20 && login.All(char.IsLetterOrDigit);
        }

        public bool IsPasswordValid(string password)
        {
            return !string.IsNullOrWhiteSpace(password) && password.Length <= 20 && password.Any(char.IsUpper) && password.Any(char.IsLower) && password.Any(char.IsDigit) && password.Any(ch => !char.IsLetterOrDigit(ch));
        }
        public bool AddUser(users user)
        {
            using (var db = new bitServiceEntities())
            {
                db.users.Add(user);
                db.SaveChanges();
                return true;
            }
        }
        public void Button_Click(object sender, RoutedEventArgs e)
        {
            string fio = NameTextBox.Text;
            string phone = PhoneTextBox.Text;
            string login = LogTextBox.Text;
            string password = PassTextBox.Text;
            string selectedRole = GetSelectedRole();

            if (string.IsNullOrEmpty(fio) || string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(selectedRole))
            {
                MessageBox.Show("Все поля должны быть заполнены.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                using (var context = new bitServiceEntities())
                {
                    if (context.autoriz.Any(u => u.login == login))
                    {
                        MessageBox.Show("Логин уже существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (!IsNameValid(fio))
                    {
                        MessageBox.Show("Некорректное ФИО. Пожалуйста, введите только буквы и пробелы, не более 100 символов.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (!IsPhoneNumberValid(phone))
                    {
                        MessageBox.Show("Некорректный номер телефона. Пожалуйста, введите 11 цифр без пробелов и других символов.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (!IsLoginValid(login))
                    {
                        MessageBox.Show("Некорректный логин. Пожалуйста, используйте только буквы и цифры, не более 30 символов.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (!IsPasswordValid(password))
                    {
                        MessageBox.Show("Некорректный пароль. Пожалуйста, убедитесь, что пароль содержит как минимум одну заглавную и одну строчную букву, одну цифру и один специальный символ, не более 20 символов.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var lastUser = context.users.OrderByDescending(u => u.userID).FirstOrDefault();
                    int newUserId = (lastUser != null) ? lastUser.userID + 1 : 1;
                    var newUser = new users
                    {
                        userID = newUserId,
                        fio = fio,
                        phone = phone
                    };

                    var newAuth = new autoriz
                    {
                        login = login,
                        password = password,
                        userID = newUserId
                    };

                    var roleType = context.type.FirstOrDefault(t => t.type1 == selectedRole);
                    if (roleType != null)
                    {
                        newUser.type.Add(roleType);
                    }

                    context.users.Add(newUser);
                    context.autoriz.Add(newAuth);
                    context.SaveChanges();
                    MessageBox.Show("Регистрация прошла успешно!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    Autorization authorization = new Autorization();
                    authorization.Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при регистрации: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GetSelectedRole()
        {
            if (CustomerRadioButton.IsChecked == true) return "Заказчик";
            if (MasterRadioButton.IsChecked == true) return "Мастер";
            if (ManagerRadioButton.IsChecked == true) return "Менеджер";
            if (OperatorRadioButton.IsChecked == true) return "Оператор";
            return null;
        }
    }
}
