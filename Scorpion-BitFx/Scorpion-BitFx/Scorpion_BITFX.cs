using System;
using Newtonsoft.Json.Linq;

namespace ScorpionBitFx
{
    public class Scorpion_BITFX_BITPANDA
    {
        Scorpion Do_on;
        ScorpionJSON json;
        public Scorpion_BITFX_objects bfx_objects;

        public Scorpion_BITFX_BITPANDA(Scorpion fm1)
        {
            Do_on = fm1;
            json = new ScorpionJSON(this);
            bfx_objects = new Scorpion_BITFX_objects();
            return;
        }

        //Does not support var_get. *var will take actual value as string *var results as 'var'. [2] is where vars start
        public void do_bitfx(ref string[] command)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Do_on.write_cui(">>> Executing: " + command[0]);
            this.GetType().GetMethod(command[0], System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).Invoke(this, new object[] { command });

            Do_on.var_stringarray_dispose(ref command);
            return;
        }

        //Public functions for BITPANDAPRO
        public void bfxkey(ref string[] command)
        {
            //::*bfxkey
            bfx_objects.bfx_settings.key = command[1];
            Do_on.write_cui("Set API key to: " + command[1]);
            return;
        }

        public void bfxcurrencies(ref string[] command)
        {
            //::
            bfx_objects.bfx_vars.currenciesJSON = json.JSON_get(bfx_objects.bfx_url.public_URL + bfx_objects.bfx_url.currencies);
            return;
        }

        public void bfxcandles(ref string[] command)
        {
            //::currency, *unit, *period, *fromdate, *fromtime, *todate, *totime
            //Example bfxcandles::*BTC *HOURS *1 *2019-10-03 *00:00:00 *2019-10-04 *00:00:00
            bfx_objects.bfx_vars.candlesJSON = json.JSON_get(bfx_objects.bfx_url.public_URL + bfx_objects.bfx_url.candles + "/" + command[1] + "_" + bfx_objects.bfx_url.PREFFERED_FIAT + "?unit=" + command[2] + "&period=" + command[3] + "&from=" + command[4] + "T" + command[5] + "Z&to=" + command[6] + "T" + command[7] + "Z");
            return;
        }

        public void bfxfees(ref string[] command)
        {
            //::
            bfx_objects.bfx_vars.feesJSON = json.JSON_get(bfx_objects.bfx_url.public_URL + bfx_objects.bfx_url.fees);
            return;
        }

        public void bfxprefferedfiat(ref string[] command)
        {
            //::*fiat
            //Not available as EUR is the only currency
            Do_on.write_error("Preffered fiat currencies are not available at the moment. All trading is done using EUR");
            return;
        }

        public void bfxinstruments(ref string[] command)
        {
            //::
            bfx_objects.bfx_vars.instrumentsJSON = json.JSON_get(bfx_objects.bfx_url.public_URL + bfx_objects.bfx_url.instruments);
            return;
        }

        public void bfxorderbook(ref string[] command)
        {
            //::*instrument_code
            bfx_objects.bfx_vars.orderbookJSON = json.JSON_get(bfx_objects.bfx_url.public_URL + bfx_objects.bfx_url.orderbook + command[1] + "_" + bfx_objects.bfx_url.PREFFERED_FIAT);
            return;
        }

        public void bfxmarketticker(ref string[] command)
        {
            //::
            bfx_objects.bfx_vars.markettickerJSON = json.JSON_get(bfx_objects.bfx_url.public_URL + bfx_objects.bfx_url.marketticker);
            return;
        }

        public void bfxticker(ref string[] command)
        {
            //::*instrument_code
            bfx_objects.bfx_vars.ticker_tempJSON = json.JSON_get(bfx_objects.bfx_url.public_URL + bfx_objects.bfx_url.price_ticker + command[1]);
            //JObject jo = json.jsontoarray(ref bfx_objects.bfx_vars.ticker_tempJSON);
            //Console.WriteLine(jo[0]);
            return;
        }

        public void bfxtime(ref string [] command)
        {
            bfx_objects.bfx_vars.timeJSON = json.JSON_get(bfx_objects.bfx_url.public_URL + bfx_objects.bfx_url.time);
            return;
        }

        //PRIVATE
        public void bfxbalances(ref string[] command)
        {
            if (bfx_objects.bfx_settings.key == null)
                Do_on.write_error("No authorization API key available. Please use bfxkey::*key in order to set your API key");
            else
                bfx_objects.bfx_vars.balancesJSON = json.JSON_get_auth(bfx_objects.bfx_url.base_URL + bfx_objects.bfx_url.balances, bfx_objects.bfx_settings.key);
            return;
        }

        public void bfxdepositcrypto(ref string[] command)
        {
            bfx_objects.bfx_vars.depositcryptoJSON = json.JSON_post_auth(bfx_objects.bfx_url.base_URL + bfx_objects.bfx_url.depositcrypto, bfx_objects.bfx_settings.key);
            return;
        }

        public void bfxdepositcryptoaddress(ref string[] command)
        {
            bfx_objects.bfx_vars.depositcryptoaddrJSON = json.JSON_get_auth(bfx_objects.bfx_url.base_URL + bfx_objects.bfx_url.depositcryptoaddr + command[1], bfx_objects.bfx_settings.key);
            return;
        }

        public void bfxdepositfiat(ref string[] command)
        {
            bfx_objects.bfx_vars.depositfiatJSON = json.JSON_get_auth(bfx_objects.bfx_url.base_URL + bfx_objects.bfx_url.deposit_fiat + bfx_objects.bfx_url.PREFFERED_FIAT, bfx_objects.bfx_settings.key);
            return;
        }

        public void bfxwithdrawcrypto(ref string[] command)
        {

            return;
        }


        //PARSE
        public void bfxjsontoobj(ref string[] command)
        {
            bfx_objects.bfx_vars.currencies = json.jsontoarray(ref bfx_objects.bfx_vars.currenciesJSON);
            return;
        }

        public void bfxjarrayget(ref string[] command)
        {

            return;
        }

        public void bfxvars(ref string[] command)
        {
            Do_on.write_cui("Candles: " + bfx_objects.bfx_vars.candlesJSON);
            Do_on.write_cui("Currencies: " + bfx_objects.bfx_vars.currenciesJSON);
            Do_on.write_cui("Fees: " + bfx_objects.bfx_vars.feesJSON);
            Do_on.write_cui("Instruments: " + bfx_objects.bfx_vars.instrumentsJSON);
            Do_on.write_cui("Order book: " + bfx_objects.bfx_vars.orderbookJSON);
            Do_on.write_cui("Market ticker: " + bfx_objects.bfx_vars.markettickerJSON);
            Do_on.write_cui("Temporary price ticker: " + bfx_objects.bfx_vars.ticker_tempJSON);
            Do_on.write_cui("Time: " + bfx_objects.bfx_vars.timeJSON);
            Do_on.write_cui("Balances: " + bfx_objects.bfx_vars.balancesJSON);
            Do_on.write_cui("Deposit crypto: " + bfx_objects.bfx_vars.depositcryptoJSON);
            Do_on.write_cui("Deposit crypto addr: " + bfx_objects.bfx_vars.depositcryptoaddrJSON);
            Do_on.write_cui("Deposit fiat addr: " + bfx_objects.bfx_vars.depositfiatJSON);
            return;
        }
    }
}
