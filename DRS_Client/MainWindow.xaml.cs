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
using System.IO;
using BusinessLayer.FileHelpers;
using BusinessLayer.OperationHelpers;
using FileUploadSample.Views;
using FirstFloor.ModernUI.Windows.Controls;

namespace FileUploadSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Dictionary<FileBase,Mediator> DrsMediators;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void fileUpload_FileNameChanged(object sender, RoutedEventArgs e)
        {
            var files = fileUpload.FileNames;
            DrsMediators = new Dictionary<FileBase, Mediator>();
            HashSet<FileBase> filesCollection = new HashSet<FileBase>();
            var path = Directory.GetCurrentDirectory()+@"\documents";
            
            if(Directory.Exists(path))
                Directory.Delete(path,true);
            Directory.CreateDirectory(path);

            

            foreach (var source in files)
            {
                var destination = Path.Combine(path, Path.GetFileName(source));
                File.Copy(source,  destination);
                FileBase temp=null;
                var extension = Path.GetExtension(destination);
                switch(extension.ToLower())
                {
                    case ".doc": case ".docx":
                        temp = new WordFile(destination); break;
                    case ".jpg": case ".bmp": case ".png": case ".jpeg": case ".gif":
                        temp = new ImageFile(destination); break;
                    case ".xls": case ".xlsx":
                        temp = new ExcelFile(destination); break;
                }
                filesCollection.Add(temp);
                var currentMediator = new Mediator();
                currentMediator.Initialize();
                currentMediator.Process(temp);

                DrsMediators.Add(temp,currentMediator);
            }
            FileList fileListWindow = new FileList(filesCollection, this);
            this.Hide();
            fileListWindow.Show();
            //var extensions = fileUpload.FileNames.Select(x => Path.GetExtension(x));

        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            this.Close();
            Application.Current.Shutdown();
        }
    }
}
