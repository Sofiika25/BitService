using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BitService
{
    public partial class OperatorPage : Window
    {
        public OperatorPage()
        {
            InitializeComponent();
            LoadData();
            this.WindowState = WindowState.Maximized;
        }

        private void LoadData()
        {
            try
            {
                using (var context = new bitServiceEntities())
                {
                    var requests = context.request.ToList();
                    foreach (var request in requests)
                    {
                        orderNumberComboBox.Items.Add($"Заявка #{request.requestID}");
                    }
                    var masters = context.users
                    .Where(u => u.type.Any(t => t.id == 2)) 
                    .Select(u => new { u.userID, u.fio })
                    .ToList();
                    responsiblesComboBox.ItemsSource = masters;
                    responsiblesComboBox.DisplayMemberPath = "fio";
                    responsiblesComboBox.SelectedValuePath = "userID";
                    var statuses = context.status.Select(s => s.requestStatus).ToList();
                    foreach (var status in statuses)
                    {
                        statusComboBox.Items.Add(status);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void orderNumberComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (orderNumberComboBox.SelectedItem != null)
            {
                string selectedOrder = orderNumberComboBox.SelectedItem.ToString();
                try
                {
                    using (var context = new bitServiceEntities())
                    {
                        int requestID = int.Parse(selectedOrder.Split('#')[1]);
                        var selectedRequest = context.request.FirstOrDefault(r => r.requestID == requestID);
                        if (selectedRequest != null)
                        {
                            statusComboBox.SelectedItem = selectedRequest.status.requestStatus;
                            if (selectedRequest.requestClientMaster.Any())
                            {
                                var masterFIO = selectedRequest.requestClientMaster.First().users.fio;
                                responsiblesComboBox.SelectedItem = masterFIO;
                            }
                            else
                            {
                                MessageBox.Show("Мастер не найден");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Заявка не найдена");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string selectedOrder = orderNumberComboBox.SelectedItem?.ToString();
            int selectedMasterFIO = (int)responsiblesComboBox.SelectedValue;
            string selectedStatus = statusComboBox.SelectedItem?.ToString();

            if (selectedOrder == null || selectedMasterFIO == 0 || selectedStatus == null)
            {
                MessageBox.Show("Выберите значения всех полей");
                return;
            }
            try
            {
                using (var context = new bitServiceEntities())
                {
                    int requestID = int.Parse(selectedOrder.Split('#')[1]);
                    var selectedRequest = context.request.FirstOrDefault(r => r.requestID == requestID);
                    if (selectedRequest != null)
                    {
                        var selectedMaster = context.users.FirstOrDefault(u => u.userID == selectedMasterFIO);
                        if (selectedMaster != null)
                        {
                            var requestClientMaster = new requestClientMaster
                            {
                                requestID = requestID,
                                masterID = selectedMaster.userID
                            };
                            context.requestClientMaster.Add(requestClientMaster);
                            var selectedStatusEntity = context.status.FirstOrDefault(s => s.requestStatus == selectedStatus);
                            if (selectedStatusEntity != null)
                            {
                                selectedRequest.statusID = selectedStatusEntity.id;
                            }
                            context.SaveChanges();
                            MessageBox.Show("Заявка успешно зарегистрирована");
                        }
                        else
                        {
                            MessageBox.Show("Выбранный мастер не найден");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Такой заявки нет");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            RequestsOperator requestsOperator = new RequestsOperator();
            requestsOperator.Show();
            this.Close();
        }
    }
}
