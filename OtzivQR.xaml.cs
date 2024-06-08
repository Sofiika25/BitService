using QRCoder;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;


namespace BitService
{
    public partial class OtzivQR : Window
    {
        public OtzivQR()
        {
            InitializeComponent();
            LoadQR();
        }
        private void LoadQR()
        {
            string url = "https://docs.google.com/forms/d/e/1FAIpQLSdhZcExx6LSIXxk0ub55mSu-WIh23WYdGG9HY5EZhLDo7P8eA/viewform?usp=sf_link";
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            BitmapImage bitmapImage = new BitmapImage();
            using (MemoryStream memory = new MemoryStream())
            {
                qrCodeImage.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
            }
            QRCodeImage.Source = bitmapImage;
        }
    }
}
