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
using System.Windows.Shapes;
using FirstFloor.ModernUI.Windows.Controls;
using BusinessLayer.OperationHelpers;
using BusinessLayer.FileHelpers;

namespace FileUploadSample.Views
{
    /// <summary>
    /// Interaction logic for DetailedView.xaml
    /// </summary>
    public partial class DetailedView : Window
    {
        public FileBase fileBase;
        FileList fileList;

        public DetailedView(FileBase fileBase, FileList fileList)
        {
            InitializeComponent();
            this.fileList = fileList;
            this.fileBase = fileBase;
            var result = fileList._mainWindow.DrsMediators[fileBase].visitors.Values.AsEnumerable();

            FileDetailItemsControl.ItemsSource = result;
        }

        private void btnCorrect_Click(object sender, RoutedEventArgs e)
        {
            var visitor = (VisitorBase)(sender as Button).DataContext;
            visitor.Correct(this.fileBase);
            MessageBox.Show("Content saved to test.docx");
        }
    }
}
