using System;
using System.Collections;

namespace ScorpionBitFx
{
    public static class scmain
    {
        public static void Main(string[] args)
        {
            Scorpion sc = new Scorpion();
        }
    }

    public class Scorpion
    {
        public Scorpion_BITFX bfx;
        public ArrayList al_out = new ArrayList();

        public Scorpion()
        {
            //Start TCP
            write_cui("Scorpion Bitpanda Pro AutoTrader [0_0]_/ [Cryptocurrency trading] hub v0.1a GNU GPL 2020+ <Oscar Arjun Singh Tark>\n\n--------------------------------------------");
            write_cui("Please enter your API key:");
            string api = Console.ReadLine();
            write_cui("Please enter your preferred exchange:");
            string exchange = Console.ReadLine();
            bfx = new Scorpion_BITFX(this);
            bfx.start(exchange, null, api);
            //while (true)
            //    Console.ReadLine();
        }

        private void wasted_space(object state)
        {
            //A dirty work around to not make the console shutdown
            state = null;
            return;
        }

        public void write_cui(string STR_)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("{0}", STR_);
            Console.ForegroundColor = ConsoleColor.White;
            return;
        }

        public void write_error(string STR_)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("{0}", STR_);
            Console.ForegroundColor = ConsoleColor.White;
            return;
        }

        public void var_stringarray_dispose(ref string[] array)
        {
            for (int i = 0; i < array.Length; i++)
                array[i] = null;

            array = null;
            GC.Collect();
            return;
        }
    }

    class EngineFunctions
    {
        public string replace_fakes(string Scorp_Line)
        {
            return Scorp_Line.Replace("{&var}", "*").Replace("{&quot}", "'");
        }

        public string replace_telnet(string Scorp_Line)
        {
            return Scorp_Line.Replace("\n", "").Replace("\r", "").Replace("959;1R", "");
        }
    }
}
