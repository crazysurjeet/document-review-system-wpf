using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.FileHelpers;
using BusinessLayer.Dictionary;
using System.ComponentModel;
using System.Configuration;

namespace BusinessLayer.OperationHelpers
{
    public abstract class VisitorBase:INotifyPropertyChanged
    {
        public string TypeName { get; set; }
        public StringBuilder UpdatedContent { get; set; }
        public abstract void Visit(FileBase fileBase);
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private long count = 0;
        public long Count { get { return count; } set { count = value; OnPropertyChanged(nameof(Count)); } }
        private bool isEnabled = false;
        public bool IsEnabled { get { return isEnabled; } set { isEnabled = value; OnPropertyChanged(nameof(IsEnabled)); } }
        public readonly string ERROR_COLOR= ConfigurationManager.AppSettings["ERROR_COLOR"];
        public virtual void Correct(FileBase fileBase)
        {
            //TODO: this will be overridden by all subclasses
        }
    }
    public class SpellCheckVisitor : VisitorBase
    {
        public IEnumerable<StringBuilder> MissSpelledWords = new List<StringBuilder>();
        public override void Visit(FileBase fileBase)
        {
            WordDictionary dictionary = new WordDictionary();
            dictionary.Initialize();
            TypeName = "Spelling Mistake Errors Count ";
            UpdatedContent = dictionary.MisspelledWords(fileBase.Content, out long count);
            this.Count = count;
            fileBase.SpellCheckCount = count;
            OnPropertyChanged(nameof(UpdatedContent));
        }
    }
    public class DoubleSpaceCheckVisitor : VisitorBase
    {
        public override void Visit(FileBase fileBase)
        {
            TypeName = "Double Space Errors Count ";
            UpdatedContent = CheckDoubleSpaces(fileBase, out long count);
            this.Count = count;
            fileBase.SpaceCheckCount = count;
            OnPropertyChanged(nameof(UpdatedContent));
        }
        public StringBuilder CheckDoubleSpaces(FileBase fileBase, out long count)
        {
            var temp = new StringBuilder(fileBase.Content.ToString());
            count = 0;
            int index = fileBase.Content.ToString().IndexOf("  ",0);
            if(index!=-1)
                while(temp.ToString().IndexOf("  ", index) != -1)
                {
                    index = temp.ToString().IndexOf("  ", index);
                    temp = temp.Insert(index+2, "■■[/color][/b][/url][/size]");
                    temp = temp.Remove(index,2);
                    temp = temp.Insert(index, $"[size=20][url=][b][color={ERROR_COLOR}]");
                    count++;
                    index += 20;
                }
            return temp;
        }
    }

    public class BrandNameCheckVisitor : VisitorBase
    {
        public override void Visit(FileBase fileBase)
        {
            TypeName = "Brand Naming Errors Count ";
            UpdatedContent = CheckBrandName(fileBase, out long count);
            this.Count = count;
            if(this.Count>0)
                this.IsEnabled = true;
            fileBase.BrandCheckCount = count;
            OnPropertyChanged(nameof(UpdatedContent));
        }
        public override void Correct(FileBase fileBase)
        {
            UpdatedContent = CorrectBrandName(fileBase, out long count);
            if (this.Count == count)
                this.Count = 0;
            this.IsEnabled = false;
            fileBase.BrandCheckCount = count;
            Task.Factory.StartNew(() => fileBase.Save(UpdatedContent));
            OnPropertyChanged(nameof(UpdatedContent));
        }
        public StringBuilder CheckBrandName(FileBase fileBase, out long count)
        {
            count = 0;
            var temp = new StringBuilder(fileBase.Content.ToString());
            int index = 0;
            int pointer = 1;
            char[] brand = "JPMorgan Chase & Co. ".ToCharArray();
            int start = 0;
            while (index >= 0)
            {
                index = temp.ToString().IndexOf("j", index, StringComparison.OrdinalIgnoreCase);

                if (index != -1)
                {
                    start = index;
                    index++;
                }
                while (index >= 0)
                {
                    if (pointer == brand.Length - 1)
                    {
                        if ((index - start) > pointer)
                        {
                            temp = temp.Insert(index, "[/color][/b][/url][/size]");
                            temp = temp.Insert(start, $"[size=20][url=][b][color={ERROR_COLOR}]");
                            index += 22;
                            count++;
                        }
                        pointer = 1;
                        break;
                    }
                    if (char.ToLowerInvariant(temp[index]) == char.ToLowerInvariant(brand[pointer]))
                    {
                        index++;
                        pointer++;
                        continue;
                    }
                    if (char.ToLowerInvariant(temp[index]) == ' ' || char.ToLowerInvariant(temp[index]) == '.')
                    {
                        index++;
                        continue;
                    }

                    if (pointer >= 7)
                    {
                        temp = temp.Insert(index, "[/color][/b][/url][/size]");
                        temp = temp.Insert(start, $"[size=20][url=][b][color={ERROR_COLOR}]");
                        index += 22;
                        count++;
                    }
                    pointer = 1;
                    break;
                }
            }
            return temp;
        }
        public StringBuilder CorrectBrandName(FileBase fileBase, out long count)
        {
            count = 0;
            var temp = fileBase.Content;
            int index=0;
            int pointer = 1;
            char[] brand = ConfigurationManager.AppSettings["BRAND_NAME"].ToCharArray();
            int start = 0;
            while (index >= 0)
            {
                index = temp.ToString().IndexOf("j", index, StringComparison.OrdinalIgnoreCase);

                if (index != -1)
                {
                    start = index;
                    index++;
                }
                while (index >= 0)
                {
                    if (pointer == brand.Length - 1)
                    {
                        if((index - start) > pointer)
                        {
                            temp = temp.Remove(start, index - start);
                            temp = temp.Insert(start, string.Concat(" ",new string(brand)));
                            count++;
                        }
                        pointer = 1;
                        break;
                    }
                    if (char.ToLowerInvariant(temp[index]) == char.ToLowerInvariant(brand[pointer]))
                    {
                        index++;
                        pointer++;
                        continue;
                    }
                    if (char.ToLowerInvariant(temp[index]) == ' ' || char.ToLowerInvariant(temp[index]) == '.')
                    {
                        index++;
                        continue;
                    }

                    if (pointer >= 7)
                    {
                        temp = temp.Remove(start, index - start);
                        temp = temp.Insert(start, string.Concat(" ", new string(brand)));
                        count++;
                    }
                    pointer = 1;
                    break;
                }
            }
            return temp;
        }
    }

    public class EnglishUKCheckVisitor : VisitorBase
    {
        public override void Visit(FileBase fileBase)
        {

        }
    }
}
