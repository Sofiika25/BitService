using System;
using System.Linq;
using System.Windows;

namespace BitService
{
    public partial class RequestsMaster : Window
    {
        private int masterID;
        public RequestsMaster(int masterID)
        {
            InitializeComponent();
            this.masterID = masterID;
            LoadData();
            this.WindowState = WindowState.Maximized;
        }

        private void LoadData()
        {
            try
            {
                using (var context = new bitServiceEntities())
                {
                    // Загружаем все связанные данные сразу
                    var requests = context.request
                        .Include("homeTech")
                        .Include("status")
                        .Include("requestClientMaster.users")
                        .Include("commentMasterClient.comment")
                        .Where(r => r.requestClientMaster.Any(rcm => rcm.masterID == masterID))
                        .ToList();
                    var requestData = requests.Select(r => new
                    {
                        r.requestID,
                        r.startDate,
                        HomeTechDescription = r.HomeTechDescription,
                        r.problemDescryption,
                        r.completionDate,
                        r.repairParts,
                        RequestStatus = r.status.requestStatus,
                        Comments = string.Join("; ", r.commentMasterClient.Select(c => c.comment.message))
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
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            MasterPage masterPage = new MasterPage(masterID); 
            masterPage.Show();
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Autorization autorization = new Autorization();
            autorization.Show();
            this.Close();
        }
    }
}