using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DM.DB;

namespace DM.DMControl
{
    
    public static class LoginControl
    {
#if DEBUG
        public static LoginResult loginResult = LoginResult.ADMIN;
#else
        public static LoginResult loginResult = LoginResult.ERROR;
#endif
//        public static Forms.DMLogin dlg = new DM.Forms.DMLogin();
//        public static bool Login()
//        {
//#if DEBUG
//            //loginResult=LoginResult.ADMIN;
//#endif
//             if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK)
//                return false;


//            return true;
//        }
   }
}
