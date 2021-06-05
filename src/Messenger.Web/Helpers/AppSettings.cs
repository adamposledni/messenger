using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Messenger.Web.Helpers
{
    public class AppSettings
    {
            public string Secret { get; set; }
            public string UserCollectionName { get; set; }
            public string ChatCollectionName { get; set; }
            public string ConnectionString { get; set; }
            public string DatabaseName { get; set; }
    }
}
