using System;
using System.IO;

namespace ScorpionBitFx
{
    public class Scorpion_LOG
    {
        string dir;
        private string get_name()
        {
            return System.Security.Principal.WindowsIdentity.GetCurrent().Name;
        }

        const string log_file = "scorpion_bitfx.log";
        public Scorpion_LOG()
        {
            string name = get_name();
            dir = "/home/" + name + "/";
            dir = dir + log_file;
            Console.WriteLine("Log file path is: {0}", dir);
        }

        public void write(string to_write)
        {
            to_write = "\n[" + DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString() + "] :" + to_write;
            File.AppendAllText(dir, to_write);
            return;
        }

        public void test()
        {
            Console.WriteLine("WORKING");
        }
    }
}
