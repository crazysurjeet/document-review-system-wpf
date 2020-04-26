using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using BusinessLayer.OperationHelpers;
using BusinessLayer.PivotHelpers;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Spreadsheet;
using System.ComponentModel;
using Run = DocumentFormat.OpenXml.Wordprocessing.Run;
using Text = DocumentFormat.OpenXml.Wordprocessing.Text;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Configuration;

namespace BusinessLayer.FileHelpers
{
    public abstract class FileBase: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string FileName { get; set; }
        private double filesize = 0;
        public double FileSize { get { return filesize;  } set { filesize = value; OnPropertyChanged(nameof(FileSize)); } }
        private long spellCheckCount = 0;
        public long SpellCheckCount { get { return spellCheckCount; } set { spellCheckCount = value; OnPropertyChanged(nameof(SpellCheckCount)); } }

        private long spaceCheckCount = 0;
        public long SpaceCheckCount { get { return spaceCheckCount; } set { spaceCheckCount = value; OnPropertyChanged(nameof(SpaceCheckCount)); } }

        private long brandCheckCount = 0;
        public long BrandCheckCount { get { return brandCheckCount; } set { brandCheckCount = value; OnPropertyChanged(nameof(BrandCheckCount)); } }
        public string FilePath { get; set; }
        private StringBuilder content;
        public StringBuilder Content { get { return content; } 
            set 
            { 
                content = value;
                OnPropertyChanged(nameof(Content));
            } 
        }
        protected bool isParsingCompleted = false;
        public bool IsParsingCompleted { 
            get { return isParsingCompleted; } 
            set { 
                isParsingCompleted = value;
                OnPropertyChanged(nameof(IsParsingCompleted));
            }
        }
        public abstract void Parse();
        public void OnPropertyChanged(string PropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }
        public virtual bool Save(StringBuilder content)
        {
            //this is a base class which will be overriden by every sub to save the content back to the file.
            return false;
        }
    }

    public class FileElement : FileBase 
    {
        VisitorBase visitorBase;
        public FileElement(string FilePath)
        {
            this.FilePath = FilePath;
            this.FileName = Path.GetFileName(FilePath);
        }
        public void SetVisitor(VisitorBase visitorBase)
        {
            this.visitorBase = visitorBase;
        }
        public override void Parse()
        {

        }
    }
    public class ExcelFile : FileElement
    {
        private readonly string COUNTRY_COLUMN;
        private DataTable allData = new DataTable();
        public DataTable AllData
        { 
            get 
            { 
                return allData; 
            }
            set
            {
                allData = value;
            }
        }
        private DataTable processedData = new DataTable();
        public DataTable ProcessedData
        {
            get
            {
                return processedData;
            }
            set
            {
                processedData = value;
                OnPropertyChanged(nameof(ProcessedData));
            }
        }

        public ExcelFile(string FilePath):base(FilePath)
        {
            COUNTRY_COLUMN= ConfigurationManager.AppSettings["COUNTRY_COLUMN"];
        }
        public override void Parse()
        {
            SpellCheckCount = -1;
            SpaceCheckCount = -1;
            BrandCheckCount = -1;
            allData = ReadExcel();
        }
        public DataTable ReadExcel()
        {
            string conn = string.Empty;
            DataTable dtexcel = new DataTable();
            var fileExt = Path.GetExtension(this.FilePath);
            if (fileExt.CompareTo(".xls") == 0)
                conn = @"provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + this.FilePath + ";Extended Properties='Excel 8.0;HRD=Yes;IMEX=1';";
            else
                conn = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + this.FilePath + ";Extended Properties='Excel 12.0;HDR=Yes;IMEX=1';";
            using (OleDbConnection con = new OleDbConnection(conn))
            {
                try
                {
                    OleDbDataAdapter oleAdpt = new OleDbDataAdapter("select * from [Sheet1$]", con);
                    oleAdpt.Fill(dtexcel);
                }
                catch (Exception e)
                {
                    throw new Exception($"Exception occured while storing records to DataSet - {e.Message}");
                }
            }
            return dtexcel;
        }
        public IEnumerable<string> GetCountries()
        {
            return allData.AsEnumerable().Select(row => row.Field<String>(COUNTRY_COLUMN)).Distinct().OrderBy(x=>x);
        }
        public void ProcessExcel(string country)
        {
            try
            {
                var tempData = allData.AsEnumerable()
                                .Where(row => row.Field<String>(COUNTRY_COLUMN) == country)
                                .CopyToDataTable();

                string x = ConfigurationManager.AppSettings["PIVOT_COLUMN_X"];
                string y = ConfigurationManager.AppSettings["PIVOT_COLUMN_Y"];
                string z = ConfigurationManager.AppSettings["PIVOT_COLUMN_Z"];

                processedData = PivotBase.ProcessClientAssetData(tempData, x, y, z);
            }
            catch(Exception e)
            {
                throw new Exception($"Exception occured while applying filter to recordset - {e.Message}");
            }
        }
    }
    public class ImageFile : FileElement
    {
        public ImageFile(string FilePath) : base(FilePath)
        {

        }
        public override void Parse()
        {
            var Ocr = new IronOcr.AutoOcr();
            var Result = Ocr.Read(FilePath);
            this.Content = new StringBuilder(Result.Text);
            isParsingCompleted = true;
            OnPropertyChanged("IsParsingCompleted");
        }
    }
    public class WordFile : FileElement
    {
        public WordFile(string FilePath) : base(FilePath)
        {
        }
        public override void Parse()
        {
            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(FilePath, true))
            {
                Body body = wordDoc.MainDocumentPart.Document.Body;
                this.Content = new StringBuilder(body.InnerText);
                isParsingCompleted = true;
                OnPropertyChanged("IsParsingCompleted");
            }
        }
        public override bool Save(StringBuilder UpdatedContent)
        {
            try
            {
                string filename = "UpdatedDoc.docx";
                using (WordprocessingDocument wordDoc = WordprocessingDocument.Create(filename, DocumentFormat.OpenXml.WordprocessingDocumentType.Document))
                {
                    MainDocumentPart mainPart = wordDoc.AddMainDocumentPart();
                    mainPart.Document = new Document();
                    Body body = mainPart.Document.AppendChild(new Body());
                    Paragraph para = body.AppendChild(new Paragraph());
                    Run run = para.AppendChild(new Run());
                    run.AppendChild(new Text(UpdatedContent.ToString()));
                }
            }
            catch(Exception)
            {
                return false;
            }
            return true;
        }
    }


}
