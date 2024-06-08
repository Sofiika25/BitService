using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Windows;


namespace BitService
{
    public partial class AddRequest : Window
    {
        private int currentUserId;
        public AddRequest(int currentUserId)
        {
            InitializeComponent();
            this.currentUserId = currentUserId;
            InitializeFields();
        }
        private bool ValidateFields()
        {
            if (string.IsNullOrWhiteSpace(txtEquipmentType.Text) || string.IsNullOrWhiteSpace(txtModel.Text))
            {
                MessageBox.Show("Поля 'Вид техники' и 'Модель' должны быть заполнены.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (txtEquipmentType.Text.Length > 50 || txtModel.Text.Length > 50)
            {
                MessageBox.Show("Максимальная длина поля 'Вид техники' и 'Модель' - 50 символов.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!txtEquipmentType.Text.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)))
            {
                MessageBox.Show("Полe 'Вид техники' может содержать только буквы и пробелы.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }
        private void InitializeFields()
        {
            try
            {
                using (var context = new bitServiceEntities())
                {
                    dpDate.SelectedDate = DateTime.Now;
                    var user = context.users.FirstOrDefault(u => u.userID == currentUserId);
                    if (user != null)
                    {
                        txtCustomerName.Text = user.fio;
                        txtPhoneNumber.Text = user.phone;
                    }
                    var lastRequest = context.request.OrderByDescending(r => r.requestID).FirstOrDefault();
                    int newRequestId = (lastRequest != null) ? lastRequest.requestID + 1 : 1;
                    txtTicketNumber.Text = newRequestId.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка инициализации полей: {ex.Message}");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateFields())
            {
                return;
            }
            try
            {
                using (var context = new bitServiceEntities())
                {
                    var newStatus = context.status.FirstOrDefault(s => s.requestStatus == "Новая заявка");
                    if (newStatus == null)
                    {
                        MessageBox.Show("Статус 'Новая заявка' не найден в базе данных.");
                        return;
                    }
                    var newRequest = new request
                    {
                        requestID = int.Parse(txtTicketNumber.Text),
                        startDate = dpDate.SelectedDate.Value,
                        problemDescryption = txtDescription.Text,
                        statusID = newStatus.id,
                        repairParts = null
                    };
                    var lastHomeTech = context.homeTech.OrderByDescending(ht => ht.id).FirstOrDefault();
                    int newHomeTechId = (lastHomeTech != null) ? lastHomeTech.id + 1 : 1;
                    var homeTech = new homeTech
                    {
                        id = newHomeTechId,
                        homeTechType = txtEquipmentType.Text,
                        homeTechModel = txtModel.Text
                    };
                    context.homeTech.Add(homeTech);
                    newRequest.homeTech.Add(homeTech);
                    var requestClientMaster = new requestClientMaster
                    {
                        clientID = currentUserId,
                        request = newRequest
                    };
                    context.requestClientMaster.Add(requestClientMaster);
                    context.SaveChanges();
                    MessageBox.Show("Заявка успешно добавлена!");
                    RequestsClient requestsClient = new RequestsClient(currentUserId);
                    requestsClient.Show();
                    this.Close();
                }
            }
            catch (DbUpdateException ex)
            {
                var sb = new StringBuilder();
                foreach (var entry in ex.Entries)
                {
                    sb.AppendLine($"Данная сущность {entry.Entity.GetType().Name} в состоянии {entry.State} не может быть обновлена");
                }
                sb.AppendLine(ex.InnerException?.Message);
                sb.AppendLine(ex.InnerException?.StackTrace);
                MessageBox.Show($"Ошибка при добавлении заявки: {ex.Message}\n{sb.ToString()}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении заявки: {ex.Message}");
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            RequestsClient requestsClient = new RequestsClient(currentUserId);
            requestsClient.Show();
            this.Close();
        }
    }
}