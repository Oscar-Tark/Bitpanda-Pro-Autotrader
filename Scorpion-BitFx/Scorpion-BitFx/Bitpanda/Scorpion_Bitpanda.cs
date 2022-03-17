using System.Threading;
using Newtonsoft.Json.Linq;
using System.Linq;
using System;

namespace ScorpionBitFx
{
    public class COIN
    {
        Thread coin_thread;
        Timer market_thread;
        COIN_settings cs = new COIN_settings();
        Scorpion_DATETIME scdt = new Scorpion_DATETIME();
        Scorpion_CRYPTO_MIN crypto_min = new Scorpion_CRYPTO_MIN();
        EXCHANGE ex;
        ScorpionJSON json = new ScorpionJSON();
        Scorpion_LOG scl = new Scorpion_LOG();

        enum signal_translations
        {
            neutral,
            sell,
            buy
        }

        public COIN(string symbol, int precision, EXCHANGE exchange, bool Autobuysell, string auth)
        {
            //scl = new Scorpion_LOG();
            ex = exchange;
            Scorpion_Write.write_success("Starting coin " + symbol + " with precision " + precision);
            cs.buy_sell_type = Autobuysell;
            Scorpion_Write.write_success("Automatic buy/sells: " + cs.buy_sell_type);

            /*if (!Autobuysell)
            {
                Scorpion_Write.write_input("Please enter your buy price preference: ");
                cs.buy_sell_type_buy_at = Convert.ToDouble(Console.ReadLine());
                Scorpion_Write.write_input("Please enter your sell price preference: ");
                cs.buy_sell_type_sell_at = Convert.ToDouble(Console.ReadLine());
            }*/

            cs.precision = precision;
            cs.symbol = symbol;
            cs.period = "1";
            cs.unit = "HOURS";
            cs.signal = 0;
            cs.key = auth;
            cs.current_balance = 0;
            cs.EUR_balance = 0;

            string line = null;
            while (true)
            {
                line = Console.ReadLine();

                if (cs.buy_sell_type)
                {
                    if (line == "autotrade start")
                        start();
                    else if (line == "autotrade stop")
                        stop();
                    else if (line == "autotrade interval")
                        continue;
                    else if (line == "manual")
                    {
                        stop();
                        manual();
                    }

                    continue;
                }

                if(!cs.buy_sell_type)
                {
                    if (line == "trade")
                        market();
                    else if (line == "auto")
                        continue;

                    continue;
                }


                if (line == "candles")
                    continue;
                else if (line == "change coin")
                    continue;
                else if (line == "exit")
                    Environment.Exit(0);
                else
                    Scorpion_Write.write_notice("Commands available are:\v\r> trade: To manually run an autotrade\n> autorade start: To start the auto trade system at specified intervals\n> autotrade stop: Top stop the intervalled trading system");
            }
        }

        public void start()
        {
            Scorpion_Write.write_success("Starting intervalled autotrade");
            start_ticker();
            return;
        }

        public void stop()
        {
            if (market_thread == null)
                return;
            Scorpion_Write.write_success("Stopping intervalled autotrade");
            market_thread.Dispose();
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

            public double min;

            //One time use client order id's
            public string buy_id;
            public string sell_id;

            JArray transactions;

            //Points to last transaction in 'transactions'
            int ptr_last_transaction;

            public double manual_buy;
            public double manual_sell;
        };

        private void start_ticker()
        {
            const int interval = 60000;
            Scorpion_Write.write_success("Running coin ticker/trader " + cs.symbol + " every: " + (interval / 1000) + " seconds");
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

        private void manual()
        {
            cs.buy_sell_type = false;
            Scorpion_Write.write_notice("Note!: Autotrading is off, to run autotrade functions run the command 'auto'");
            Scorpion_Write.write_input("Please enter your buy price preference: ");
            cs.buy_sell_type_buy_at = Convert.ToDouble(Console.ReadLine());
            Scorpion_Write.write_input("Please enter your sell price preference: ");
            cs.buy_sell_type_sell_at = Convert.ToDouble(Console.ReadLine());
            return;
        }

        private void auto()
        {
            Console.WriteLine("Your previous manual thresholds will remain at:\v\r> Buy: [{0}]\n> Sell: [{1}]", cs.buy_sell_type_buy_at, cs.buy_sell_type_sell_at);
            cs.buy_sell_type = true;
            return;
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

                if (!cs.buy_sell_type)
                    Scorpion_Write.write_notice("Manual buy/sell active [Buy at: " + cs.buy_sell_type_buy_at + "] [Sell at: " + cs.buy_sell_type_sell_at + "]");

                Console.WriteLine("Decoded {0} on (Auto Buy/Sell: {6}) [Current price: {5}][High: {1}][Low: {2}][Average High: {3}][Average Low: {4}][Mid-Average High: {7}][Mid-Average Low: {8}]", cs.symbol, cs.high, cs.low, cs.high_average, cs.low_average, cs.current_price, cs.buy_sell_type, cs.high_mid_average, cs.low_mid_average);

                if (cs.failed == true)
                {
                    cs.failed = false;
                    Scorpion_Write.write_notice("Failure signals cleared");
                }
            }
            catch (Exception e)
            {
                Scorpion_Write.write_error("Unable to decode " + cs.symbol + ". Aborting on next failure. Error[" + e.Message + "])\n");
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
            Scorpion_Write.write_error("Aborting " + cs.symbol);
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
            scl.write("Signal for " + cs.symbol + " set to " + cs.signal);
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
            {
                Scorpion_Write.write_notice("Neutral on " + cs.symbol);
                scl.write("Neutral on " + cs.symbol);
            }
            return;
        }

        //Not the best way, but ok for now in a small scale
        private void get_balance()
        {
            cs.current_balance_JSON = ex.xwallet(cs.symbol, cs.key);
            JObject jobj = json.jsontoobject(ref cs.current_balance_JSON);
            cs.account_id = jobj.Value<string>("account_id");

            JArray jarr = (JArray)jobj["balances"];

            foreach (JObject j_inobjs in jarr)
            {
                if (j_inobjs.Value<string>("currency_code") == ex.bfx_url.PREFFERED_FIAT)
                    cs.EUR_balance = j_inobjs.Value<double>("available");
                else if (j_inobjs.Value<string>("currency_code") == cs.symbol)
                {
                    cs.current_balance = j_inobjs.Value<double>("available");
                    break;
                }
            }
            Console.WriteLine("{0} at: {1}", ex.bfx_url.PREFFERED_FIAT, cs.EUR_balance);
            Console.WriteLine("{0} at: {1}", cs.symbol, cs.current_balance);
            scl.write(ex.bfx_url.PREFFERED_FIAT + " at " + cs.EUR_balance);
            return;
        }

        private void buy()
        {
            Scorpion_Write.write("Buying " + cs.symbol);
            get_min();
            cs.buy_id = order_id();
            Scorpion_Write.write("Got new order id: " + cs.buy_id);
            scl.write("Got new order id: " + cs.buy_id);
            if (cs.EUR_balance > 0 && check_min_buy())
            {
                //buy
                Scorpion_Write.write_success("Buying " + cs.EUR_balance + " " + ex.bfx_url.PREFFERED_FIAT + " | ID: " + cs.buy_id);
                string json_buy = ex.xorder(ref cs.symbol, "BUY", "MARKET", convert_comma_dot((cs.EUR_balance / cs.current_price).ToString()), cs.buy_id);
                Scorpion_Write.write_success(json_buy);
                scl.write(json_buy);
            }
            else
            {
                Scorpion_Write.write_error("Unable to buy " + cs.symbol + " not enough FIAT " + ex.bfx_url.PREFFERED_FIAT + ": " + cs.EUR_balance + ", Required minimum " + ex.bfx_url.PREFFERED_FIAT + ": " + cs.min);
                scl.write("Unable to buy " + cs.symbol + " not enough FIAT " + ex.bfx_url.PREFFERED_FIAT + ": " + cs.EUR_balance + ", Required minimum " + ex.bfx_url.PREFFERED_FIAT + ": " + cs.min);
            }
            scl.write("Signal:" + cs.signal + " Symbol:" + cs.symbol + " Current price:" + cs.current_price + " High:" + cs.high + " Low:" + cs.low);
            return;
        }

        private void sell()
        {
            Scorpion_Write.write("Selling " + cs.symbol);
            cs.sell_id = order_id();
            if (cs.current_balance > 0)
            {
                Scorpion_Write.write_success("Selling " + cs.current_balance + " of " + cs.symbol + " | ID: " + cs.sell_id);
                string json_sell = ex.xorder(ref cs.symbol, "SELL", "MARKET", convert_comma_dot(cs.current_balance.ToString()), cs.sell_id);
                Scorpion_Write.write_success(json_sell);
                scl.write(json_sell);
            }
            else
            {
                Scorpion_Write.write_error("Unable to sell not enough " + cs.symbol + " to sell at " + cs.current_balance);
                scl.write("Unable to sell not enough " + cs.symbol + " to sell at " + cs.current_balance);
            }
            scl.write("Signal:" + cs.signal + " Symbol:" + cs.symbol + " Current price:" + cs.current_price + " High:" + cs.high + " Low:" + cs.low + " Tax:" + calculate_tax());
            return;
        }

        private string order_id()
        {
            return crypto_min.GetUniqueKey(8) + crypto_min.GetUniqueKey(4) + "-" + crypto_min.GetUniqueKey(4) + "-" + crypto_min.GetUniqueKey(4) + "-" + crypto_min.GetUniqueKey(12);
        }

        private bool check_min_buy()
        {
            if (cs.EUR_balance > cs.min)
                return true;
            return false;
        }

        //Gets minimum tradable amount
        private void get_min()
        {
            ex.bfx_vars.instrumentsJSON = ex.xinstruments();
            JArray jarr = json.jsontoarray(ref ex.bfx_vars.instrumentsJSON);
            JObject j_inner_base, j_inner_quote;
            foreach (JObject j_inobjs in jarr)
            {
                j_inner_base = j_inobjs.Value<JObject>("base");
                j_inner_quote = j_inobjs.Value<JObject>("quote");
                if (j_inner_base.Value<string>("code") == cs.symbol && j_inner_quote.Value<string>("code") == ex.bfx_url.PREFFERED_FIAT)
                {
                    cs.min = j_inobjs.Value<double>("min_size");
                    Console.WriteLine("Min size set to: {0}", cs.min);
                }
            }
            return;
        }

        private void calculate_wallet()
        {
            return;
        }

        private double calculate_tax()
        {
            return (cs.current_price / 100) * cs.current_price;
        }

        private string convert_comma_dot(string amount)
        {
            amount = amount.Remove(amount.IndexOf(",", StringComparison.CurrentCulture));
            return amount;
        }

        private void clear_all()
        {
            cs.highs = null;
            cs.lows = null;
            return;
        }
    }
}