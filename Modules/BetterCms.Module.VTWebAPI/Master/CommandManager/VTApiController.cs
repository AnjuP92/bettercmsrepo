using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
namespace BetterCms.Module.VTWebAPI.Master
{
    public class VTApiController: ApiController
    {
        public virtual string GetCommand(string message)
        {
            return "Handled: " + message;
        }
    }
}