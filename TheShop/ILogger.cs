﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheShop
{
    interface ILogger
    {
        void Info(string message);

        void Error(string message);

        void Debug(string message);
    }
}
