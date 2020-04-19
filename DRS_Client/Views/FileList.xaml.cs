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
        BackgroundWorker worker;
        public HashSet<FileBase> collection;
        public MainWindow _mainWindow;
        public FileList(HashSet<FileBase> collection, MainWindow _mainWindow)
        {
            this.collection = collection;
            this._mainWindow = _mainWindow;

            InitializeComponent();
            FilesItemControl.ItemsSource = new FileViewModel(this).files;
            //worker = new BackgroundWorker();
            //worker.WorkerReportsProgress = true;
            //worker.DoWork += worker_DoWork;
            //worker.ProgressChanged += worker_ProgressChanged;
            //worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            //worker.RunWorkerAsync();
        }
        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var count = collection.Count();
            Dispatcher.Invoke(() =>
                {
                    bool flag = true;
                    while (flag)
                    {
                        for (int i = 1; i <= count; i++)
                        {
                            if (collection.ElementAt(i - 1).IsParsingCompleted) { flag = false; (sender as BackgroundWorker).ReportProgress(i / count * 100); }
                        }
                    }
                }
            );

		}

		void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{ 
            //CalculationInProgress.Value = e.ProgressPercentage;	
		}

		void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
            MessageBox.Show("Files processed successfully");
		}

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            DetailedView detailedView = new DetailedView((FileBase)(sender as Button).DataContext,this);
            detailedView.Show();
            //this.Hide();
        }
    }
}
