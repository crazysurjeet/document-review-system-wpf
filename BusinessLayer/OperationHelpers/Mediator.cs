using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.FileHelpers;
using System.IO;

namespace BusinessLayer.OperationHelpers
{
    public class Mediator
    {
        public Dictionary<Type, VisitorBase> visitors = new Dictionary<Type, VisitorBase>();
        public void Initialize()
        {
            VisitorBase spellCheckVisitor = new SpellCheckVisitor();
            VisitorBase doubleSpaceVisitor = new DoubleSpaceCheckVisitor();
            VisitorBase brandnamecheckvisitor = new BrandNameCheckVisitor();

            visitors.Add(typeof(BrandNameCheckVisitor), brandnamecheckvisitor);
            visitors.Add(typeof(SpellCheckVisitor), spellCheckVisitor);
            visitors.Add(typeof(DoubleSpaceCheckVisitor), doubleSpaceVisitor);
        }
        public void Process(FileBase _file)
        {
            Task t = Task.Factory.StartNew(()=> _file.Parse());
            t= t.ContinueWith((x)=>
            {
                visitors[typeof(SpellCheckVisitor)].Visit(_file);
            });
            t = t.ContinueWith((x) =>
            {
                visitors[typeof(DoubleSpaceCheckVisitor)].Visit(_file);
            });
            t = t.ContinueWith((x) =>
            {
                visitors[typeof(BrandNameCheckVisitor)].Visit(_file);
            });
            t = t.ContinueWith((x) =>
            {
                _file.IsParsingCompleted = true;
            });
        }
    }
}
