using Messenger.Web.Helpers;
using Messenger.Web.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Messenger.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : ControllerBase
    {
        public ErrorController()
        {
        }

        [Route("error")]
        public ErrorRes Error()
        {
            var exception = HttpContext.Features.Get<IExceptionHandlerFeature>().Error;

            // logic exception
            if (exception is AppLogicException)
            {
                Response.StatusCode = 400;
                string message = exception.Message;
                return new ErrorRes(message);
            }
            // server exception
            else
            {
                Response.StatusCode = 500;
                string message = "Server error has occured";
                return new ErrorRes(message);
            }
        }
    }
}
