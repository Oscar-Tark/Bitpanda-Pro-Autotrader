using System.Threading;
using Newtonsoft.Json.Linq;
using System.Linq;
using System;

namespace ScorpionBitFx
{
    public class COIN
    {
        Thread coin_thread; Timer market_thread;
        COIN_settings cs = new COIN_settings();
        Scorpion_DATETIME scdt = new Scorpion_DATETIME();
        EXCHANGE ex; 
        ScorpionJSON json = new ScorpionJSON();

        public COIN(string symbol, int precision, EXCHANGE exchange, bool Autobuysell)
        {
            ex = exchange;
            Console.WriteLine("Starting coin {0} with precision {1}", symbol, precision);
            cs.buy_sell_type = Autobuysell;
            Console.WriteLine("Automatic buy sells: {0}", cs.buy_sell_type);

            cs.precision = precision;
            cs.symbol = symbol;
            cs.period = "1";
            cs.unit = "HOURS";
            cs.signal = 0;

            //start_ticker();
            return;
        }

        public void start()
        {
            start_ticker();
            return;
        }

        struct COIN_settings
        {
            public bool buy_sell_type; //true = auto / false = manual
            public double buy_sell_type_buy;
            public double buy_sell_type_sell;

            public bool failed;
            public string CANDLE_JSON;

            public string symbol;
            public int precision;
            public string unit;
            public string period;
            public string[] time_interval;

            public double current_price;

            //Week based averages-->
            public double[] lows;
            public double[] highs;

            public double high;
            public double low;
            public double high_average;
            public double low_average;
            //<--END

            string wallet_address;
            public int signal; //0=neutral -1=sell 1=buy

            JArray transactions;

            //Points to last transaction in 'transactions'
            int ptr_last_transaction;
        };

        private void start_ticker()
        {
            const int interval = 30000;
            Console.WriteLine("Running coin ticker/trader {0} every: {1} seconds", cs.symbol, (interval/1000));
            market_thread = new Timer(coin_thread_start);
            market_thread.Change(0, interval);
            return;
        }

        private void coin_thread_start(object state)
        {
            ThreadStart coin_threads = new ThreadStart(market);
            coin_thread = new Thread(coin_threads);
            coin_thread.Priority = ThreadPriority.AboveNormal;
            coin_thread.Start();
        }

        private void market()
        {
            clear_all();
            cs.time_interval = scdt.process_datetime();
            cs.CANDLE_JSON = ex.xcandles(cs.symbol, cs.unit, ex.bfx_url.period, cs.time_interval[0], cs.time_interval[1], cs.time_interval[2], cs.time_interval[3]);

            //Get current price
            string tickJSON = ex.xinstrumentticker(cs.symbol);
            JObject tickarr = json.jsontoobject(ref tickJSON);
            cs.current_price = tickarr.Value<double>("last_price");

            //Get Week averages
            try
            {
                JArray jarr = json.jsontoarray(ref cs.CANDLE_JSON);

                //Create arrays on the fly in order to mitigat unwanted values
                cs.highs = new double[jarr.Count];
                cs.lows = new double[jarr.Count];

                int ndx = 0;
                foreach (JObject jobj in jarr)
                {
                    cs.highs[ndx] = (jobj.Value<double>("high"));
                    cs.lows[ndx] = (jobj.Value<double>("low"));
                    ndx++;
                }
                cs.high_average = coin_average(ref cs.highs);
                cs.low_average = coin_average(ref cs.lows);
                cs.low = cs.lows.Min();
                cs.high = cs.highs.Max();

                //Get Current price and check against averages to create signal
                //Code here

                Console.WriteLine("Decoded {0} on (Auto Buy/Sell: {6}) [Current price: {5}][High: {1}][Low: {2}][Average High: {3}][Average Low: {4}]", cs.symbol, cs.high, cs.low, cs.high_average, cs.low_average, cs.current_price, cs.buy_sell_type);

                if (cs.failed == true)
                {
                    cs.failed = false;
                    Console.WriteLine("Failure signals cleared");
                }
            }
            catch
            {
                Console.WriteLine("Unable to decode {0}. Aborting on next failure", cs.symbol);
                if (cs.failed)
                {
                    kill();
                }
                cs.failed = true;
            }
            return;
        }

        private double coin_average(ref double[] Averagable)
        {
            return Queryable.Average(Averagable.AsQueryable());
        }

        private void kill()
        {
            Console.WriteLine("Aborting {0}", cs.symbol);
            market_thread.Dispose();
            coin_thread.Abort();
            return;
        }

        //Buy-sell
        private void trade()
        {
            //Do buysell operation if buy_sell_type not auto kill after transaction

            if (cs.buy_sell_type == false)
                kill();
        }

        private void buy()
        {

        }

        private void sell()
        {

        }

        private void clear_all()
        {
            cs.highs = null;
            cs.lows = null;
            return;
        }
    }
}
