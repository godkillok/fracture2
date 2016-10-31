using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;

namespace fracture
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
       
            System.Globalization.CultureInfo enUs = new System.Globalization.CultureInfo("zh-hans");
            System.Threading.Thread.CurrentThread.CurrentCulture = enUs;
            System.Threading.Thread.CurrentThread.CurrentUICulture = enUs;
            //DevExpress.Utils.LocalizationHelper.SetCurrentCulture(DataHelper.ApplicationArguments);
           // DevExpress.UserSkins.BonusSkins.Register();
            DevExpress.Utils.AppearanceObject.DefaultFont = new Font("Segoe UI", 8);
            DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle("Office 2013");
            SkinManager.EnableFormSkins();
           // EnumProcessingHelper.RegisterEnum<TaskStatus>();

          
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

           Application.Run(new RibbonForm1());
            //   Application.Run(new Predicttion());
       //    Application.Run(new frmreservoir());
            // Application.Run(new Plotting_Form2());
              //Application.Run(new ToAccess());
              //  Application.Run(new declinepred());
           //  Application.Run(new fractureparameter());
            // Application.Run(new wellmap());
          //   Application.Run(new WellHeadTemp());
            // Application.Run(new decline());
           //  Application.Run(new Pred());
        }
    }
}
