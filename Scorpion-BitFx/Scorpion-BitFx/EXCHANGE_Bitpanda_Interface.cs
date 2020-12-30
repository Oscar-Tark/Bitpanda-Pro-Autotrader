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

            Console.WriteLine("XRP is the only coin available in BETA mode");
            bfx_vars.instrumentsJSON = xinstruments();
            foreach (JObject jobj in json.jsontoarray(ref bfx_vars.currenciesJSON))
            {
                //Run all: if (jobj.Value<string>("code") != "EUR" && jobj.Value<string>("code") != "CHF")
                //Run one only:
                if(jobj.Value<string>("code") == "XRP")
                {
                    Console.WriteLine(">> Run COIN: {0}? (Y/N)", jobj.Value<string>("code"));
                    if (Console.ReadLine().ToLower() == "y")
                    {
                        if (!auto_buy_sell_all)
                        {
                            Console.WriteLine(">> Automatic buy sells for {0}? (Y/N/ALL)", jobj.Value<string>("code"));
                            yna = Console.ReadLine().ToLower();
                            if (yna == "n")
                                auto_by_sell = false;
                            else if (yna == "all" || yna == "a")
                            {
                                auto_by_sell = true;
                                auto_buy_sell_all = true;
                            }
                        }

                        EXCHANGE_COINS_JSON.Add(jobj);
                        EXCHANGE_COINS_REF.Add(jobj);
                        EXCHANGE_COINS.Add(new COIN(jobj.Value<string>("code"), jobj.Value<int>("precision"), this, auto_by_sell, bfx_settings.key));
                    }
                }
            }

            foreach(COIN cs in EXCHANGE_COINS)
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

        //OLD
        /*public void xinstruments()
        {
            //::
            bfx_vars.instrumentsJSON = json.JSON_get(bfx_url.public_URL + bfx_url.instruments);
            return;
        }*/



        /*public void bfxorderbook(ref string[] command)
        {
            //::*instrument_code
            bfx_vars.orderbookJSON = json.JSON_get(bfx_url.public_URL + bfx_url.orderbook + command[1] + "_" + bfx_url.PREFFERED_FIAT);
            return;
        }*/

        /*public void xmarketticker(ref string[] command)
        {
            //::
            bfx_vars.markettickerJSON = json.JSON_get(bfx_url.public_URL + bfx_url.marketticker);
            return;
        }*/


        //PRIVATE
        /*public void xbalances()
        {
            if (bfx_settings.key == null)
                Console.WriteLine("No authorization API key available. Please use bfxkey::*key in order to set your API key");
            else
                bfx_vars.balancesJSON = json.JSON_get_auth(bfx_url.base_URL + bfx_url.balances, bfx_settings.key);
            return;
        }

        public void bfxdepositcrypto(ref string[] command)
        {
            bfx_vars.depositcryptoJSON = json.JSON_post_auth(bfx_url.base_URL + bfx_url.depositcrypto, bfx_settings.key);
            return;
        }

        public void bfxdepositcryptoaddress(ref string[] command)
        {
            bfx_vars.depositcryptoaddrJSON = json.JSON_get_auth(bfx_url.base_URL + bfx_url.depositcryptoaddr + command[1], bfx_settings.key);
            return;
        }

        public void bfxdepositfiat(ref string[] command)
        {
            bfx_vars.depositfiatJSON = json.JSON_get_auth(bfx_url.base_URL + bfx_url.deposit_fiat + bfx_url.PREFFERED_FIAT, bfx_settings.key);
            return;
        }

        public void bfxwithdrawcrypto(ref string[] command)
        {

            return;
        }*/


        //PARSE
        /*private void bfxjsontoobj(ref string[] command)
        {
            //bfx_vars.currencies = json.jsontoarray(ref bfx_vars.currenciesJSON);
            return;
        }

        private void bfxjarrayget(ref string[] command)
        {

            return;
        }*/

        /*public void bfxvars(ref string[] command)
        {
            Do_on.write_cui("Candles: " + bfx_vars.candlesJSON);
            Do_on.write_cui("Currencies: " + bfx_vars.currenciesJSON);
            Do_on.write_cui("Fees: " + bfx_vars.feesJSON);
            Do_on.write_cui("Instruments: " + bfx_vars.instrumentsJSON);
            Do_on.write_cui("Order book: " + bfx_vars.orderbookJSON);
            Do_on.write_cui("Market ticker: " + bfx_vars.markettickerJSON);
            Do_on.write_cui("Temporary price ticker: " + bfx_vars.ticker_tempJSON);
            Do_on.write_cui("Time: " + bfx_vars.timeJSON);
            Do_on.write_cui("Balances: " + bfx_vars.balancesJSON);
            Do_on.write_cui("Deposit crypto: " + bfx_vars.depositcryptoJSON);
            Do_on.write_cui("Deposit crypto addr: " + bfx_vars.depositcryptoaddrJSON);
            Do_on.write_cui("Deposit fiat addr: " + bfx_vars.depositfiatJSON);
            return;
        }*/
    }
}
