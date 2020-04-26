using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.FileHelpers;
using FileUploadSample.Commands;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using Point = System.Windows.Point;
using Size = System.Windows.Size;
using Microsoft.Win32;
using System.Data;
using System.Configuration;

namespace FileUploadSample.ViewModels
{
    public class ExcelReportViewModel:ViewModelBase
    {
        public FileBase fileBase;
        ExcelFile excelFile;
        readonly string DEFAULT_COUNTRY = ConfigurationManager.AppSettings["DEFAULT_COUNTRY"];
        public RelayCommand SaveReportCommand { get; set; }
        public RelayCommand CboCountryChangedCommand { get; set; }
        private DataView processedDataView = new DataView();
        public DataView ProcessedDataView 
        {
            get 
            { 
                return processedDataView; 
            } 
            set 
            { 
                processedDataView = value;
                OnPropertyChanged(nameof(ProcessedDataView));
            } 
        }
        private IEnumerable<string> _countries = new List<String>();
        public IEnumerable<string> Countries 
        { 
            get 
            { 
                return _countries;
            }
            set 
            {
                _countries = value;
                OnPropertyChanged(nameof(Countries));
            } 
        }
        private string selectedCountry;
        public string SelectedCountry 
        {
            get 
            { 
                return selectedCountry;
            }
            set
            {
                selectedCountry = value;
                OnPropertyChanged(nameof(SelectedCountry));
            }
        }

        public ExcelReportViewModel(FileBase fileBase)
        {
            SelectedCountry = DEFAULT_COUNTRY;
            SaveReportCommand = new RelayCommand(SaveReport, CanSaveReport);
            CboCountryChangedCommand = new RelayCommand(CountrySelectionChanged, CanSaveReport);
            this.fileBase = fileBase;
            this.excelFile = (fileBase as ExcelFile);
            Countries = excelFile.GetCountries();

            if (Countries.Contains(DEFAULT_COUNTRY))
            {
                Task t = Task.Factory.StartNew(() => excelFile.ProcessExcel(DEFAULT_COUNTRY));
                t.ContinueWith((x) => ProcessedDataView = excelFile.ProcessedData.DefaultView);
            }
        }
        public void SaveReport(object parameter)
        {
            var PrintElement = parameter as StackPanel;
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Image file (*.jpg)|*.jpg";
            if (saveFileDialog.ShowDialog() == true)
            {

                var image = GetImage(PrintElement);
                string PATH = saveFileDialog.FileName;

                Stream stream = new FileStream(PATH, FileMode.OpenOrCreate);

                SaveAsPng(image, stream);
                MessageBox.Show("File saved successfully");
            }
        }
        public void CountrySelectionChanged(object parameter)
        {
            var country = parameter.ToString();
            excelFile.ProcessExcel(country);
            ProcessedDataView = excelFile.ProcessedData.DefaultView;
        }
        public bool CanSaveReport(object parameter)
        {
            return true;
        }
        public static RenderTargetBitmap GetImage(StackPanel view)
        {
            Size size = new Size(view.ActualWidth, view.ActualHeight);
            if (size.IsEmpty)
                return null;

            RenderTargetBitmap result = new RenderTargetBitmap((int)size.Width, (int)size.Height, 96, 96, PixelFormats.Pbgra32);

            DrawingVisual drawingvisual = new DrawingVisual();
            using (DrawingContext context = drawingvisual.RenderOpen())
            {
                context.DrawRectangle(new VisualBrush(view), null, new Rect(new Point(), size));
                context.Close();
            }

            result.Render(drawingvisual);
            return result;
        }
        public static void SaveAsPng(RenderTargetBitmap src, Stream outputStream)
        {
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(src));

            encoder.Save(outputStream);
        }
    }
}
