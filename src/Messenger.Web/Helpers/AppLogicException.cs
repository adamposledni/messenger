using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Messenger.Web.Helpers
{
    public class AppLogicException : Exception
    {
        public AppLogicException(string message) : base(message)
        {
        }
    }
}
