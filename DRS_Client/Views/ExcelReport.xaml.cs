using BusinessLayer.FileHelpers;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
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
using FileUploadSample.ViewModels;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace FileUploadSample.Views
{
    /// <summary>
    /// Interaction logic for ExcelReport.xaml
    /// </summary>
    public partial class ExcelReport : Window
    {
        ExcelReportViewModel excelReportViewModel;
        public ExcelReport(FileBase fileBase)
        {
            this.excelReportViewModel = new ExcelReportViewModel(fileBase);
            InitializeComponent();
            this.DataContext = excelReportViewModel;
        }
    }
}
