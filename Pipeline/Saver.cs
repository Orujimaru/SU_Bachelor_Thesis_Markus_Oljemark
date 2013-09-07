using Orujin.Core.Renderer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Orujin.Pipeline
{
    public class Saver
    {
        public static void Save(string saveString)
        {
            SimplerAES aes = new SimplerAES();
            string s = aes.Encrypt(saveString);
            FileStream fs = new FileStream("Content/SaveFile/Save.txt", FileMode.Truncate);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine(s);
            sw.Close();
            fs.Close();
        }
    }
}
