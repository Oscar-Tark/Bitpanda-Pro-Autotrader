using System;
using System.IO;

namespace ScorpionBitFx
{
    public class Scorpion_LOG
    {
        string dir;
        const string log_file = "scorpion_bitfx.log";
        public Scorpion_LOG(string log_dir)
        {
            dir = log_dir;
            dir = dir + log_file;
            Console.WriteLine("Log file path is: {0}", dir);
        }

        public void write(string to_write)
        {
            to_write = "\n[" + DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString() + "] :" + to_write;
            File.AppendAllText(dir, to_write);
            return;
        }
    }
}
