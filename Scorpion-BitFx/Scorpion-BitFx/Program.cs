using System;

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
        Scorpion_TCP tcp;
        public Scorpion_BITFX_BITPANDA bfx;

        public Scorpion()
        {
            //Start TCP
            write_cui("Scorpion BITFX [Cryptocurrency trading] hub v0.1b GNU GPL 2020+ <Oscar Arjun Singh Tark>");

            Console.WriteLine("Run as networked instance? (Y/N)");

            if (Console.ReadLine().ToLower() == "y")
            { 
                write_cui("Please enter a valid URL and PORT to bind to:");
                try
                {
                    tcp = new Scorpion_TCP(Console.ReadLine(), Convert.ToInt32(Console.ReadLine()), this);
                }
                catch (Exception erty) { write_error("Unable to establish a network server due to: " + erty.Message); }
            }
            bfx = new Scorpion_BITFX_BITPANDA(this);
            while (true)
                execute_command(Console.ReadLine());
        }

        public void execute_command(string command)
        {
            //bfx::function::mongocommand
            Console.ForegroundColor = ConsoleColor.White;
            string[] command_vars = split_command(ref command);
            bfx.do_bitfx(ref command_vars);
        }

        static string[] unwanted = { "::", "*" };
        private static string[] split_command(ref string command)
        {
            return command.Replace("\n", "").Replace(" ", "").Replace(",", "").Split(unwanted, StringSplitOptions.RemoveEmptyEntries);
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
}
