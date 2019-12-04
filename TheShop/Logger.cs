using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheShop
{
    public class Logger
    {
        public void Info(string message)
        {
            Console.WriteLine("Info: " + message);
        }

        public void Error(string message)
        {
            Console.WriteLine("Error: " + message);
        }

        public void Debug(string message)
        {
            Console.WriteLine("Debug: " + message);
        }
    }
}
