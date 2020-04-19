using Microsoft.Win32;
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

namespace WPF_CustomFileControl
{
    public class FileUploadCustomControl : Control
    {
        TextBox txtFileName = null;
        Button btnBrowse = null;
        public string[] FileNames
        {
            get { return (string[])GetValue(FileNamesProperty); }
            set { SetValue(FileNamesProperty, value); }
        }

        public event RoutedEventHandler FileNameChanged
        {
            add { AddHandler(FileNameChangedEvent, value); }
            remove { RemoveHandler(FileNameChangedEvent, value); }
        }

        public static readonly RoutedEvent FileNameChangedEvent =
            EventManager.RegisterRoutedEvent("FileNameChanged",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(FileUploadCustomControl));


        // Using a DependencyProperty as the backing store for FileName. This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FileNamesProperty =
            DependencyProperty.Register("FileNames", typeof(string[]), typeof(FileUploadCustomControl),
                new FrameworkPropertyMetadata(new string[] { }, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        static FileUploadCustomControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FileUploadCustomControl), new FrameworkPropertyMetadata(typeof(FileUploadCustomControl)));
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            txtFileName = this.Template.FindName("TXT_FILE_NAME", this) as TextBox;

            btnBrowse = this.Template.FindName("BTN_BROWSE_FILE", this) as Button;
            btnBrowse.Click += new RoutedEventHandler(btnBrowse_Click);
            txtFileName.TextChanged += new TextChangedEventHandler(txtFileName_TextChanged);
        }
        void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDIalog = new OpenFileDialog();
            fileDIalog.Filter = "All files(*.doc, *.docx, *.png,*.jpg,*.bmp,*.jpeg,*.gif,*.xlsx,*.xls)|*.doc; *.docx; *.png; *.jpg; *.bmp; *.jpeg; *.gif;*.xlsx; *.xls |Word files(*.doc, *.docx)| *.doc; *.docx|Image files (*.png,*.jpg,*.bmp,*.jpeg,*.gif)|*.png;*.jpg;*.bmp;*.jpeg;*.gif|Excel Files (*.xlsx;*.xls)|*.xlsx;*.xls";
            fileDIalog.AddExtension = true;
            fileDIalog.Multiselect = true;
            if (fileDIalog.ShowDialog() == true)
            {
                FileNames = fileDIalog.FileNames;
                txtFileName.Text = (FileNames.Count()>0) ? "Multiple files selected": FileNames[0];
            }
        }

        void txtFileName_TextChanged(object sender, TextChangedEventArgs e)
        {
            e.Handled = true;
            base.RaiseEvent(new RoutedEventArgs(FileNameChangedEvent));
        }

    }
}
