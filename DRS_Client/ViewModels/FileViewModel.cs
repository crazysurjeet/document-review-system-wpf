using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.FileHelpers;
using BusinessLayer.OperationHelpers;
using FileUploadSample.Views;

namespace FileUploadSample.ViewModels
{
    public class FileViewModel:ViewModelBase
    {
        public IEnumerable<FileBase> files = new List<FileBase>();
        FileList fileList; 
        public FileViewModel(FileList fileList)
        {
            this.fileList = fileList;
            files = fileList.collection;
        }
    }
}
