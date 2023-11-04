using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsClientTests
{
    public class BaseTest
    {
        public BaseTest()
        {
            TestDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../TestData");
            TempDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp");
            if (!Directory.Exists(TempDir))
            {
                Directory.CreateDirectory(TempDir);
            }
            Environment.CurrentDirectory = TempDir;
        }

        ~BaseTest() 
        {
            foreach (string file in Directory.GetFiles(TempDir))
            {
                File.Delete(file);
            }
        }

        public string TestDir { get; set; }
        public string TempDir { get; set; }
    }
}
