using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using BusinessLayer.OperationHelpers;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Spreadsheet;
using System.ComponentModel;
using Run = DocumentFormat.OpenXml.Wordprocessing.Run;
using Text = DocumentFormat.OpenXml.Wordprocessing.Text;

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
        public ExcelFile(string FilePath):base(FilePath)
        {

        }
        public override void Parse()
        {
            using (SpreadsheetDocument doc = SpreadsheetDocument.Open(FilePath, true))
            {
                WorkbookPart workbookPart = doc.WorkbookPart;
                WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
                SheetData sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();
                string text= string.Empty;
                foreach (Row r in sheetData.Elements<Row>())
                {
                    foreach (Cell c in r.Elements<Cell>())
                    {
                        text += c.CellValue?.Text +" ";
                    }
                }
                this.Content = new StringBuilder(text);

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
                string filename = "test.docx";
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
