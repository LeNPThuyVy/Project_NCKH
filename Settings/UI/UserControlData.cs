using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SettingsUI.UI
{
    internal class UserControlData : BaseSettingControl
    {
        private void btnBack_Click(object sender, EventArgs e)
        {
            RaiseBackClicked();
        }
    }
}
