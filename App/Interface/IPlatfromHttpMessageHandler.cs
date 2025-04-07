using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Interface
{
    interface IPlatfromHttpMessageHandler
    {
        HttpMessageHandler GetHttpMessageHandler();
    }
}
