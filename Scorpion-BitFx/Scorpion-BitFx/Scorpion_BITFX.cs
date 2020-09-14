using System;
using System.Collections;
using System.Threading;
using System.Diagnostics;

namespace ScorpionBitFx
{
    public partial class Scorpion_BITFX_BITPANDA
    {
        Scorpion Do_on;
        ScorpionJSON json;

        EXCHANGE[] exchange_instance = new EXCHANGE[10];
        string[] exchange_ref = new string[10];
        int instance_count = 0;
        const int max_instances = 10;

        public Scorpion_BITFX_BITPANDA(Scorpion fm1)
        {
            Do_on = fm1;
            Do_on.write_cui("Use the start function in order to initialize an exchange:\n\nstart::*exchange *name\n***************************************************\n");

        }

        //Does not support var_get. *var will take actual value as string *var results as 'var'. [2] is where vars start
        public void do_bitfx(ref string[] command)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;

            try
            {
                Do_on.write_cui(">> Executing: " + command[0]);
                this.GetType().GetMethod(command[0], System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).Invoke(this, new object[] { command });
            }
            catch(Exception e) { Do_on.write_error("An error occured: " + e.Message); }
            Do_on.var_stringarray_dispose(ref command);
            return;
        }

        public void start(ref string[] commands)
        {
            //::*typeofexchangeint, *name
            int this_instance = 0;
            try
            {
                if (instance_count == max_instances)
                    return;

                this_instance = instance_count++;
                exchange_instance[this_instance] = new EXCHANGE(ref commands[1]);
                exchange_instance[this_instance].__start();
                exchange_ref[this_instance] = commands[2];
            }
            catch (Exception e) { Console.WriteLine(e.Message); }
            return;
        }
    }

    partial class EXCHANGE
    {
        ScorpionJSON json;
        Thread th_engine;

        public EXCHANGE(ref string type)
        {
            bfx_url = new EXCHANGE_URL();
            bfx_vars = new EXCHANGE_JSON();
            bfx_settings = new EXCHANGE_settings();

            //CHANGE WITH SOMETHING BETTER
            if (type == "bitpanda")
                bfx_url = EXCHANGE_BITPANDA(ref bfx_url);
            else
                return;

            bfx_vars.tickers_ref = new ArrayList();
            bfx_vars.tickers = new ArrayList();
            json = new ScorpionJSON();
            return;
        }

        public void __start()
        {
            //Starts the exchange
            //Start the Trading engine
            //ThreadStart ths_engine = new ThreadStart(start_engine);
            //th_engine = new Thread(ths_engine);
            //th_engine.Priority = ThreadPriority.AboveNormal;
            //th_engine.Start();
            start_engine();
            return;
        }

        //Trading Engine
        private void start_engine()
        {
            //interval = interval in seconds
            //Console.WriteLine("Please enter a time interval for the Trading engine to trade on:");
            //int interval = Convert.ToInt32(Console.ReadLine());
            engine(null);
            return;
        }

        private void engine(object state)
        {
            //Get API key
            Console.WriteLine("Enter your Exchange key:");
            xkey(Console.ReadLine());
            bfx_url.period = "1";
            //Get basic data
            xcoins();
            xfees();
            xprefferedfiat();
            //xinstruments();
            xbalances();
            return;
        }
    }
}
