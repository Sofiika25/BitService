using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;

namespace BitService
{
    public partial class RequestsClient : Window
    {
        int currentUserId;
        public RequestsClient(int currentUserId)
        {
            InitializeComponent();
            this.currentUserId = currentUserId;
            LoadData();
            this.WindowState = WindowState.Maximized;      
        }

        private void LoadData()
        {
            try
            {
                using (var context = new bitServiceEntities())
                {
                    var requests = context.requestClientMaster
                        .Where(rcm => rcm.clientID == this.currentUserId)
                        .Select(rcm => rcm.request)
                        .Include(r => r.homeTech)
                        .Include(r => r.status)
                        .ToList();
                    var requestData = requests.Select(r => new
                    {
                        r.requestID,
                        r.startDate,
                        HomeTechDescription = r.HomeTechDescription,
                        r.problemDescryption,
                        r.completionDate,
                        r.repairParts,
                        RequestStatus = r.status.requestStatus
                    }).ToList();
                    RequestsDataGrid.ItemsSource = requestData;
                    if (requests.Count == 0)
                    {
                        MessageBox.Show("Заявки отсутствуют");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}");
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AddRequest addRequest = new AddRequest(currentUserId);
                addRequest.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии формы добавления заявки: {ex.Message}");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Autorization autorization = new Autorization();
                autorization.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии формы авторизации: {ex.Message}");
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                OtzivQR otzivQR = new OtzivQR();
                otzivQR.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии формы отзывов: {ex.Message}");
            }
        }
    }
}
