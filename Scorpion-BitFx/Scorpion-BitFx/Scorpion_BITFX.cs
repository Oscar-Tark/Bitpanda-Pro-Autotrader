using System;
using System.Collections;

namespace ScorpionBitFx
{
    public class Scorpion_BITFX
    {
        Scorpion Do_on;

        EXCHANGE[] exchange_instance = new EXCHANGE[10];
        string[] exchange_ref = new string[10];
        int instance_count = 0;
        const int max_instances = 10;

        public Scorpion_BITFX(Scorpion fm1)
        {
            Do_on = fm1;
            return;
        }

        public void start(string exchangename, string name, string key)
        {
            //old
            //::*typeofexchangeint, *name
            //new
            //::*typeofexchangeint, *name, *autotrade, *coin, *coin, *coin
            int this_instance = 0;
            try
            {
                if (instance_count == max_instances)
                    return;

                this_instance = instance_count++;
                exchange_instance[this_instance] = new EXCHANGE(ref exchangename, ref key);
                exchange_instance[this_instance].__start();
                exchange_ref[this_instance] = name;
            }
            catch (Exception e) { Console.WriteLine(e.Message); }
            return;
        }
    }

    partial class EXCHANGE
    {
        ScorpionJSON json;
        public EXCHANGE(ref string type, ref string key)
        {
            bfx_url = new EXCHANGE_URL();
            bfx_vars = new EXCHANGE_JSON();
            bfx_settings = new EXCHANGE_settings();

            //CHANGE WITH SOMETHING BETTER
            if (type.ToLower() == "bitpanda")
            {
                bfx_url = EXCHANGE_BITPANDA(ref bfx_url);
                xkey(key);
            }

            bfx_vars.tickers_ref = new ArrayList();
            bfx_vars.tickers = new ArrayList();
            json = new ScorpionJSON();
            return;
        }

        public void __start()
        {
            start_engine();
            return;
        }

        //Trading Engine
        private void start_engine()
        {
            engine(null);
            return;
        }

        private void engine(object state)
        {
            bfx_url.period = "1";

            //Get basic data
            xcoins();
            xfees();
            xprefferedfiat();
            return;
        }
    }
}
