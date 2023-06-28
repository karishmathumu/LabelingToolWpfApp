using System;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;


namespace UIDesignWPFApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>


    public partial class MainWindow : Window
    {
        private List<string> imageFiles;
        private int currentImageIndex;

        public MainWindow()
        {
            InitializeComponent();
        }



        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
            dialog.Multiselect = false;

            if (dialog.ShowDialog() == true)
            {
                string folderPath = System.IO.Path.GetDirectoryName(dialog.FileName);
                LoadImagesFromFolder(folderPath);
                currentImageIndex = 0;
                DisplayCurrentImage();
                UpdateImageCount();
            }
        }
        private void LoadImagesFromFolder(string folderPath)
        {
            string[] supportedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
            imageFiles = Directory.GetFiles(folderPath)
                .Where(file => supportedExtensions.Contains(System.IO.Path.GetExtension(file), StringComparer.OrdinalIgnoreCase))
                .ToList();
        }

        private void UpdateImageCount()
        {
            int totalImages = imageFiles.Count;
            int currentImageCount = currentImageIndex + 1;
            imageCount.Text = $"{currentImageCount}/{totalImages}";
        }

        private void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            currentImageIndex--;
            if (currentImageIndex < 0)
            {
                currentImageIndex = imageFiles.Count - 1;
            }
            DisplayCurrentImage();
            UpdateImageCount();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            currentImageIndex++;
            if (currentImageIndex >= imageFiles.Count)
            {
                currentImageIndex = 0;
            }
            DisplayCurrentImage();
            UpdateImageCount();
        }

        private void DisplayCurrentImage()
        {
            string imagePath = imageFiles[currentImageIndex];
            BitmapImage bitmapImage = new BitmapImage(new Uri(imagePath));
            imageControl.Source = bitmapImage;
        }


    }
}