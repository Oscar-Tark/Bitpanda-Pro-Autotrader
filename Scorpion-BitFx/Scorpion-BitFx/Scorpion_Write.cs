using System;

namespace ScorpionBitFx
{
    public static class Scorpion_Write
    {
        private static readonly string pre_symbol = ">>";
        private static readonly string pre_symbol_input = "<<";

        public static void write(string to_write)
        {
            Console.WriteLine("{1} OUT: {0}", to_write, pre_symbol);
            return;
        }

        public static void write_error(string to_write)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("{1} OUT: {0}", to_write, pre_symbol);
            Console.ForegroundColor = ConsoleColor.White;
            return;
        }

        public static void write_success(string to_write)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("{1} OUT: {0}", to_write, pre_symbol);
            Console.ForegroundColor = ConsoleColor.White;
            return;
        }

        public static void write_notice(string to_write)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("{1} OUT: {0}", to_write, pre_symbol);
            Console.ForegroundColor = ConsoleColor.White;
            return;
        }

        public static void write_input(string to_write)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("{1} IN: {0}", to_write, pre_symbol_input);
            Console.ForegroundColor = ConsoleColor.White;
            return;
        }
    }
}
