﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gesobrowser
{
    class InifileUtils : IDisposable
    {
        /// <summary>
        /// iniファイルのパスを保持
        /// </summary>
        private String FilePath { get; set; }

        public void Dispose()
        {//base.Dispose();
        }
        /// <summary>
        /// コンストラクタ(デフォルト)
        /// </summary>
        //       public InifileUtils()
        //       {
        //           this.filePath = AppDomain.CurrentDomain.BaseDirectory + "hogehoge.ini";
        //        }

        /// <summary>
        /// コンストラクタ(fileパスを指定する場合)
        /// </summary>
        /// <param name="filePath">iniファイルパス</param>
        public InifileUtils(String filePath)
        {
            this.FilePath = filePath;
        }

        /// <summary>
        /// iniファイル中のセクションのキーを指定して、文字列を返す
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <returns></returns>
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

        /// <summary>
        /// iniファイル中のセクションのキーを指定して、整数値を返す
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public int GetValueInt(String section, String key)
        {
            return (int)WinApi.GetPrivateProfileInt(section, key, 0, FilePath);
        }

        /// <summary>
        /// 指定したセクション、キーに数値を書き込む
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public void SetValue(String section, String key, int val)
        {
            SetValue(section, key, val.ToString());
        }

        /// <summary>
        /// 指定したセクション、キーに文字列を書き込む
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public void SetValue(String section, String key, String val)
        {
            WinApi.WritePrivateProfileString(section, key, val, FilePath);
        }
    }
}