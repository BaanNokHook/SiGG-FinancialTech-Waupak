using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace GM.WaTuPak.Web.App_Start
{
    public class AuthorizeRolesAttribute: AuthorizeAttribute
    {
        public AuthorizeRolesAttribute(string roles) : base()
        {
            Roles = "Administrator"; // string.Join(",", roles);
        }
    }

}