using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BitService
{
    public partial class MasterPage : Window
    {
        private int selectedRequestID;
        private int masterID;
        public MasterPage(int masterID)
        {
            InitializeComponent();
            this.WindowState = WindowState.Maximized;
            LoadData();
            this.masterID = masterID;
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
                        orderNumberComboBox.Items.Add($"Order #{request.requestID}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            RequestsMaster requestsMaster = new RequestsMaster(masterID);
            requestsMaster.Show();
            this.Close();
        }

        private void Button_AddComment_Click(object sender, RoutedEventArgs e)
        {
            if (selectedRequestID == 0)
            {
                MessageBox.Show("Please select a request.");
                return;
            }

            string commentMessage = commentTextBox.Text;

            if (string.IsNullOrWhiteSpace(commentMessage))
            {
                MessageBox.Show("Please enter a comment message.");
                return;
            }

            try
            {
                using (var context = new bitServiceEntities())
                {
                    int newCommentID = context.comment.Any() ? context.comment.Max(c => c.commentID) + 1 : 1;
                    var newComment = new comment
                    {
                        commentID = newCommentID,
                        message = commentMessage
                    };
                    var commentMasterClient = new commentMasterClient
                    {
                        requestID = selectedRequestID,
                        commentID = newCommentID,
                        comment = newComment
                    };
                    context.comment.Add(newComment);
                    context.commentMasterClient.Add(commentMasterClient);
                    context.SaveChanges();
                    MessageBox.Show("Комментарий добавлен успешно");
                    commentTextBox.Text = string.Empty;
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
                selectedRequestID = int.Parse(selectedOrder.Split('#')[1]);
            }
        }
    }
}