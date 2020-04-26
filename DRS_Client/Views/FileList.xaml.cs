using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;
using BusinessLayer.FileHelpers;
using BusinessLayer.OperationHelpers;
using FileUploadSample.ViewModels;

namespace FileUploadSample.Views
{
    /// <summary>
    /// Interaction logic for FileList.xaml
    /// </summary>
    public partial class FileList : Window
    {
        public HashSet<FileBase> collection;
        public MainWindow _mainWindow;
        public FileList(HashSet<FileBase> collection, MainWindow _mainWindow)
        {
            this.collection = collection;
            this._mainWindow = _mainWindow;
            InitializeComponent();
            FilesItemControl.ItemsSource = new FileViewModel(this).files;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var senderObj = (sender as Button).DataContext;
            if (senderObj is ExcelFile)
            {
                ExcelReport excelReport = new ExcelReport((FileBase)senderObj);
                excelReport.Show();
            }
            else
            {
                DetailedView detailedView = new DetailedView((FileBase)senderObj, this);
                detailedView.Show();
            }
        }
        private void FileListWindow_Closed(object sender, EventArgs e)
        {
            this.Close();
            Application.Current.Shutdown();
        }
    }
}
