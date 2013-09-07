using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Orujin.Pipeline
{
    public class Loader
    {
        public static string Load()
        {
            SimplerAES aes = new SimplerAES();
            string line = "";
            try
            {
                using (StreamReader reader = new StreamReader("Content/SaveFile/Save.txt"))
                {
                    line = reader.ReadLine();
                }
                line = aes.Decrypt(line);
            }
            catch { }
            return line;
        }
    }
}
