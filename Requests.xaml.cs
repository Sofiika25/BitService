using System;
using System.Linq;
using System.Windows;

namespace BitService
{
    public partial class Requests : Window
    {
        public Requests()
        {
            InitializeComponent();
            this.WindowState = WindowState.Maximized;
            LoadData();
        }

        public void LoadData()
        {
            try
            {
                using (var context = new bitServiceEntities())
                {
                    var requests = context.request
                        .Include("homeTech")
                        .Include("status")
                        .Include("requestClientMaster.users")
                        .ToList();
                    var requestData = requests.Select(r => new
                    {
                        r.requestID,
                        r.startDate,
                        HomeTechDescription = r.HomeTechDescription,
                        r.problemDescryption,
                        r.completionDate,
                        r.repairParts,
                        FIO = r.requestClientMaster.FirstOrDefault()?.users.fio,
                        PhoneNumber = r.requestClientMaster.FirstOrDefault()?.users.phone,
                        RequestStatus = r.status.requestStatus
                    }).ToList();

                    RequestsDataGrid.ItemsSource = requestData;

                    if (requests.Count == 0)
                    {
                        MessageBox.Show("Заявки не найдены");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}");
            }
        }

        public void Button_Click(object sender, RoutedEventArgs e)
        {
            string searchQuery = SearchTextBox.Text;
            if (string.IsNullOrEmpty(searchQuery))
            {
                MessageBox.Show("Введите номер заявки для поиска.");
                return;
            }
            try
            {
                using (var context = new bitServiceEntities())
                {
                    var requests = context.request
                        .Include("homeTech")
                        .Include("status")
                        .Include("requestClientMaster.users")
                        .Where(r => r.requestID.ToString() == searchQuery)
                        .ToList();
                    if (requests.Count == 0)
                    {
                        MessageBox.Show($"Заявка с номером {searchQuery} не найдена.");
                    }
                    else
                    {
                        var requestData = requests.Select(r => new
                        {
                            r.requestID,
                            r.startDate,
                            HomeTechDescription = r.HomeTechDescription,
                            r.problemDescryption,
                            r.completionDate,
                            r.repairParts,
                            FIO = r.requestClientMaster.FirstOrDefault()?.users.fio,
                            PhoneNumber = r.requestClientMaster.FirstOrDefault()?.users.phone,
                            RequestStatus = r.status.requestStatus
                        }).ToList();

                        RequestsDataGrid.ItemsSource = requestData;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выполнении поиска: {ex.Message}");
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Autorization autorization = new Autorization();
            autorization.Show();
            this.Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            LoadData();
            SearchTextBox.Text = null;
        }
    }
}
