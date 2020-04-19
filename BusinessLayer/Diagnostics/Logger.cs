using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Diagnostics
{
    public interface ILogger
    {
        void Log(string text);
    }
    public class Logger : ILogger
    {
        private Logger _logger;
        private Logger()
        {

        }
        public ILogger GetInstance()
        {
            return _logger==null ?_logger = new Logger() : _logger;
        }
        
        public void Log(string text)
        {
            //TODO : This will be used across the project 
            throw new NotImplementedException();
        }
    }

}
