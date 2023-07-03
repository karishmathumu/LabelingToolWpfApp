using System;
using System.Collections.Generic;
using System.Linq;

using System.Windows;
using System.Windows.Controls;

using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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

        private Point startPoint;
        private Rectangle currentRectangle;

        private List<Rect> boundingBoxes = new List<Rect>();

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
         
            SaveBoundingBoxes();
     

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
            
            SaveBoundingBoxes();
     

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


        private void canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(canvas);

            currentRectangle = new Rectangle
            {
                Stroke = Brushes.Red,
                StrokeThickness = 2,
                Width = 0,
                Height = 0
            };

            Canvas.SetLeft(currentRectangle, startPoint.X);
            Canvas.SetTop(currentRectangle, startPoint.Y);

            canvas.Children.Add(currentRectangle);
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && currentRectangle != null)
            {
                Point currentPoint = e.GetPosition(canvas);

                double width = Math.Abs(currentPoint.X - startPoint.X);
                double height = Math.Abs(currentPoint.Y - startPoint.Y);

                Canvas.SetLeft(currentRectangle, Math.Min(startPoint.X, currentPoint.X));
                Canvas.SetTop(currentRectangle, Math.Min(startPoint.Y, currentPoint.Y));

                currentRectangle.Width = width;
                currentRectangle.Height = height;
            }
        }

        private void canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (currentRectangle != null)
            {
                // Save the coordinates of the bounding box here
                double x = Canvas.GetLeft(currentRectangle);
                double y = Canvas.GetTop(currentRectangle);
                double width = currentRectangle.Width;
                double height = currentRectangle.Height;
                Rect boundingBox = new Rect(x, y, width, height);
                boundingBoxes.Add(boundingBox);

                // Save the coordinates to a file or perform any other desired operation

                currentRectangle = null;
            }
        }

        

        private void ClearCanvas()
        {
            canvas.Children.Clear();
            boundingBoxes.Clear();
        }

        private void SaveBoundingBoxes()
        {
            string imagePath = imageFiles[currentImageIndex];
            string fileName = System.IO.Path.GetFileNameWithoutExtension(imagePath) + ".txt";
            string filePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(imagePath), fileName);

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (Rect boundingBox in boundingBoxes)
                {
                    // Convert the bounding box coordinates to YOLO format
                    double x = boundingBox.X / imageControl.ActualWidth;
                    double y = boundingBox.Y / imageControl.ActualHeight;
                    double width = boundingBox.Width / imageControl.ActualWidth;
                    double height = boundingBox.Height / imageControl.ActualHeight;

                    writer.WriteLine($"{x} {y} {width} {height}");
                }
            }
        }
    }
}