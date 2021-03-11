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
        public String GetValueString(String section, String key, String b)
        {
            section=GetValueString(section, key);
            if (section == "")
                return b;
            return section;
        }
        public int GetValueString(String section, String key,int b=0)
        {
            section = GetValueString(section, key);
            if (section == "")
                return b;
            return int.Parse(section);
        }
        public double GetValueString(String section, String key, double b = 0)
        {
            section = GetValueString(section, key);
            if (section == "")
                return b;
            return double.Parse(section);
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
