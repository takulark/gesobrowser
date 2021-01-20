using System;
using System.Text;

namespace gesobrowser
{
    class InifileUtils : IDisposable
    {
        private String FilePath { get; set; }

        public void Dispose()
        {
        }
        public InifileUtils(String filePath)
        {
            this.FilePath = filePath;
        }
        public String GetValueString(String section, String key)
        {
            StringBuilder sb = new StringBuilder(1024);

            WinApi.GetPrivateProfileString(
                section,
                key,
                "",
                sb,
                Convert.ToUInt32(sb.Capacity),
                FilePath);

            return sb.ToString();
        }
        public int GetValueInt(String section, String key)
        {
            return (int)WinApi.GetPrivateProfileInt(section, key, 0, FilePath);
        }
        public void SetValue(String section, String key, int val)
        {
            SetValue(section, key, val.ToString());
        }
        public void SetValue(String section, String key, String val)
        {
            WinApi.WritePrivateProfileString(section, key, val, FilePath);
        }
    }
}
