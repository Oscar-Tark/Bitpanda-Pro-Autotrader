using System;
using Newtonsoft.Json.Linq;

namespace ScorpionBitFx
{
    public partial class EXCHANGE
    {
        //Public functions for BITPANDAPRO
        public void xkey(string key)
        {
            bfx_settings.key = key;
            Console.WriteLine(">> Set API key to: {0}", key);
            return;
        }

        //Get wallet
        public string xwallet(string symbol, string auth)
        {
            Console.WriteLine(bfx_url.base_URL + bfx_url.balances);
            return json.JSON_get_auth(bfx_url.base_URL + bfx_url.balances, auth);
        }

        //Basic Bitpanda functions
        public void xcoins()
        {
            bfx_vars.currenciesJSON = json.JSON_get(bfx_url.public_URL + bfx_url.currencies);
            bool auto_by_sell = true; bool auto_buy_sell_all = false; string yna;
            //bool run_all = false;

            //Console.WriteLine("XRP is the only coin available in BETA mode");
            bfx_vars.instrumentsJSON = xinstruments();

            Console.WriteLine("\nAvailable COINS:");
            foreach (JObject jobj in json.jsontoarray(ref bfx_vars.currenciesJSON))
                //Run all: if (jobj.Value<string>("code") != "EUR" && jobj.Value<string>("code") != "CHF")
                //Run one only:
                Console.WriteLine(">> {0:s}", jobj.Value<string>("code"));
            Console.WriteLine("\nEnter the COIN symbol you would like to run: ");
            string Code = Console.ReadLine();
            foreach (JObject jobj in json.jsontoarray(ref bfx_vars.currenciesJSON))
            {
                if (jobj.Value<string>("code") == Code)
                {
                    Console.WriteLine(">> Runing COIN: {0}", jobj.Value<string>("code"));
                    auto_by_sell = true;
                    auto_buy_sell_all = true;
                    EXCHANGE_COINS_JSON.Add(jobj);
                    EXCHANGE_COINS_REF.Add(jobj);
                    EXCHANGE_COINS.Add(new COIN(jobj.Value<string>("code"), jobj.Value<int>("precision"), this, auto_by_sell, bfx_settings.key));
                }
            }
            foreach (COIN cs in EXCHANGE_COINS)
                cs.start();
            Console.WriteLine(">> Total available coins are: {0}", EXCHANGE_COINS_JSON.Count);
            return;
        }

        public void xfees()
        {
            bfx_vars.feesJSON = json.JSON_get(bfx_url.public_URL + bfx_url.fees);
            return;
        }

        public void xprefferedfiat()
        {
            //::*fiat
            //Not available as EUR is the only currency
            Console.WriteLine(">> Preffered FIAT currency automatically set to {0}", bfx_url.PREFFERED_FIAT);
            return;
        }

        public string xcandles(string symbol, string unit, string period, string date_from, string time_from, string date_to, string time_to)
        {
            //::currency, *unit, *period, *fromdate, *fromtime, *todate, *totime
            //Example bfxcandles::*BTC *HOURS *1 *2019-10-03 *00:00:00 *2019-10-04 *00:00:00
            return json.JSON_get(bfx_url.public_URL + bfx_url.candles + "/" + symbol + "_" + bfx_url.PREFFERED_FIAT + "?unit=" + unit + "&period=" + period + "&from=" + date_from + "T" + time_from + "Z&to=" + date_to + "T" + time_to + "Z");
        }

        public string xinstrumentticker(string symbol)
        {
            //Gets current market ticker values for Crypto symbol
            return json.JSON_get(bfx_url.public_URL + bfx_url.marketticker + symbol + "_" + bfx_url.PREFFERED_FIAT);
        }

        public string xinstruments()
        {
            return json.JSON_get(bfx_url.public_URL + bfx_url.instruments);
        }

        public void bfxtime(ref string[] command)
        {
            bfx_vars.timeJSON = json.JSON_get(bfx_url.public_URL + bfx_url.time);
            return;
        }

        public string xorder(ref string symbol, string side, string type, string amount, string id)
        {
            Scorpion_Write.write_notice("Amount to " + type + " is: " + amount);
            return json.JSON_post_auth(bfx_url.base_URL + bfx_url.orders, bfx_settings.key, new string[] { }, new string[] { }, true, "{\"instrument_code\" : \"" + symbol + "_EUR\", \"side\": \"" + side + "\",\"type\":\"" + type + "\", \"amount\":\"" + amount + "\"}");//, \"client_id\":\"" + id + "\"}");
        }

        private void flush_orders()
        {

            return;
        }
    }
}
