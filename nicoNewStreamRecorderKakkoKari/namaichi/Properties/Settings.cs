using System.Configuration;

namespace namaichi.Properties
{
    internal sealed partial class Settings
    {
        [UserScopedSetting, SettingsSerializeAs(SettingsSerializeAs.String)]
        public SunokoLibrary.Application.CookieSourceInfo SelectedSourceInfo
        {
            get { return (SunokoLibrary.Application.CookieSourceInfo)this["SelectedSourceInfo"]; }
            set { this["SelectedSourceInfo"] = value; }
        }
    }
}