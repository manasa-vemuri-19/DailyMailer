using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Helper
{
    public class UserIdentity
    {
        public static string GetCurrentUser() {
            string userid = HttpContext.Current.User.Identity.Name;
            string[] userids = userid.Split('\\');
            if (userids.Length == 2)
            {
                userid = userids[1];
            }
            //return "Srinivas_Kolipakkam";
            return userid;
        }
    }
}