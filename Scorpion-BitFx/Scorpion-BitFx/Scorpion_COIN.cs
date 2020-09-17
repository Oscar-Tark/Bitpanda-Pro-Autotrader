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
        Scorpion_LOG scl;

        enum signal_translations
        {
            neutral,
            sell,
            buy
        }

        public COIN(string symbol, int precision, EXCHANGE exchange, bool Autobuysell, string auth)
        {
            scl = new Scorpion_LOG();
            ex = exchange;
            Console.WriteLine("Starting coin {0} with precision {1}", symbol, precision);
            cs.buy_sell_type = Autobuysell;
            Console.WriteLine("Automatic buy sells: {0}", cs.buy_sell_type);

            if (!Autobuysell)
            {
                Console.WriteLine("Please enter your buy price preference: ");
                cs.buy_sell_type_buy_at = Convert.ToDouble(Console.ReadLine());
                Console.WriteLine("Please enter your sell price preference: ");
                cs.buy_sell_type_sell_at = Convert.ToDouble(Console.ReadLine());
            }
            cs.precision = precision;
            cs.symbol = symbol;
            cs.period = "1";
            cs.unit = "HOURS";
            cs.signal = 0;
            cs.key = auth;
            cs.current_balance = 0;
            cs.EUR_balance = 0;

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
            public double buy_sell_type_buy_at;
            public double buy_sell_type_sell_at;

            public bool failed;
            public string CANDLE_JSON;

            public string symbol;
            public int precision;
            public string unit;
            public string unit_day;
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

            public double high_mid_average;
            public double low_mid_average;
            //<--END

            public string key;
            public string account_id;
            public double current_balance;
            public double EUR_balance;
            public string current_balance_JSON;
            public int signal; //0=neutral 1=sell 2=buy

            JArray transactions;

            //Points to last transaction in 'transactions'
            int ptr_last_transaction;

            public double manual_buy;
            public double manual_sell;
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
                cs.high_average = coin_average(cs.highs);
                cs.low_average = coin_average(cs.lows);
                cs.low = cs.lows.Min();
                cs.high = cs.highs.Max();

                cs.high_mid_average = coin_average(new double[2] { cs.high, cs.high_average });
                cs.low_mid_average = coin_average(new double[2] { cs.low, cs.low_average });

                //Get Current price and check against averages to create signal
                //Code here

                get_signal();
                trade();

                Console.WriteLine("Decoded {0} on (Auto Buy/Sell: {6}) [Current price: {5}][High: {1}][Low: {2}][Average High: {3}][Average Low: {4}][Mid-Average High: {7}][Mid-Average Low: {8}]", cs.symbol, cs.high, cs.low, cs.high_average, cs.low_average, cs.current_price, cs.buy_sell_type, cs.high_mid_average, cs.low_mid_average);

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
                    kill();
                cs.failed = true;
            }
            return;
        }

        private double coin_average(double[] Averagable)
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

        // Manual Buy-sell
        private void get_signal()
        {
            //Create buy sell signal
            if (cs.buy_sell_type)
            {
                if (cs.current_price >= cs.high_mid_average)
                    cs.signal = 1;
                else if (cs.current_price <= cs.low_mid_average)
                    cs.signal = 2;
                else
                    cs.signal = 0;
            }
            else
            {
                if (cs.current_price >= cs.buy_sell_type_sell_at)
                    cs.signal = 2;
                else if (cs.current_price <= cs.buy_sell_type_buy_at)
                    cs.signal = 1;
                else cs.signal = 0;
            }
            Console.WriteLine("Signal for {0} set to {1}", cs.symbol, cs.signal);
            return;
        }

        private void trade()
        {
            //Do buysell operation if buy_sell_type not auto kill after transaction
            //Check and compare averages vs current price
            if (cs.signal == 2)
            {
                get_balance();
                buy();
            }
            else if (cs.signal == 1)
            {
                get_balance();
                sell();
            }
            else
                Console.WriteLine("Neutral on {0}", cs.symbol);

            if (cs.buy_sell_type == false)
                kill();
        }

        //Not the best way, but ok for now in a small scale
        private void get_balance()
        {
            cs.current_balance_JSON = ex.xwallet(cs.symbol, cs.key);
            //Console.WriteLine(cs.current_balance_JSON);
            JObject jobj = json.jsontoobject(ref cs.current_balance_JSON);
            cs.account_id = jobj.Value<string>("account_id");
            //Console.WriteLine("VAL: {0}", cs.account_id);


            //Console.WriteLine("Code: {0}", jobj["balances"][0]["currency_code"]);
            JArray jarr = (JArray)jobj["balances"];

            foreach(JObject j_inobjs in jarr)
            {
                if (j_inobjs.Value<string>("currency_code") == ex.bfx_url.PREFFERED_FIAT)
                {
                    cs.EUR_balance = j_inobjs.Value<double>("available");
                }
                else if (j_inobjs.Value<string>("currency_code") == cs.symbol)
                {
                    cs.current_balance = j_inobjs.Value<double>("available");
                    break;
                }
            }
            Console.WriteLine("{0} at: {1}", ex.bfx_url.PREFFERED_FIAT, cs.EUR_balance);
            Console.WriteLine("{0} at: {1}", cs.symbol, cs.current_balance);
            //JToken bals = jobj["balances"];

            /*foreach (JObject jobj in jarr)
            {
                //foreach(JObject in )
                //string code = (jobj.Value<string>("currency_code"));
                //Console.WriteLine(code);
            }*/
            return;
        }

        private void buy()
        {
            Console.WriteLine("Buying {0}", cs.symbol);
            if (cs.EUR_balance > 0)
            {
                //buy
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("Buying {0} {1} of {2}", cs.EUR_balance, ex.bfx_url.PREFFERED_FIAT, cs.symbol);
                Console.ForegroundColor = ConsoleColor.White;
                string json_buy = ex.xorder(ref cs.symbol, "BUY", "MARKET", (cs.current_price / cs.EUR_balance).ToString());

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(json_buy);
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("Unable to buy not enough {0}: {1}", ex.bfx_url.PREFFERED_FIAT, cs.EUR_balance);
                Console.ForegroundColor = ConsoleColor.White;
            }
            scl.write("Signal:" + cs.signal + " Symbol:" + cs.symbol + " Current price:" + cs.current_price + " High:" + cs.high + " Low:" + cs.low);
            return;
        }

        private void sell()
        {
            Console.WriteLine("Selling {0}", cs.symbol);
            if (cs.current_balance > 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Selling {0} of {1}", cs.current_balance, cs.symbol);
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("Unable to sell not enough {0} to sell at {1}", cs.symbol, cs.current_balance);
                Console.ForegroundColor = ConsoleColor.White;
            }
            scl.write("Signal:" + cs.signal + " Symbol:" + cs.symbol + " Current price:" + cs.current_price + " High:" + cs.high + " Low:" + cs.low + " Tax:" + calculate_tax());
            return;
        }

        private double calculate_tax()
        {
            return (cs.current_price / 100) * cs.current_price;
        }

        private void clear_all()
        {
            cs.highs = null;
            cs.lows = null;
            return;
        }
    }
}
