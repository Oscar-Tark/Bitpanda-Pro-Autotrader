using System;
using Newtonsoft.Json.Linq;

namespace ScorpionBitFx
{
    public class Scorpion_BITFX_objects
    {
        public BFX_settings bfx_settings;
        public BFX_VARS bfx_vars;
        public BFX_URL bfx_url;

        public Scorpion_BITFX_objects()
        {
            bfx_url = new BFX_URL();
            bfx_vars = new BFX_VARS();
            bfx_settings = new BFX_settings();

            bfx_url.public_URL = "https://api.exchange.bitpanda.com/public/v1/";
            bfx_url.base_URL = "https://api.exchange.bitpanda.com/public/v1/account/";
            bfx_url.trades = "trades";
            bfx_url.currencies = "currencies";
            bfx_url.candles = "candlesticks";
            bfx_url.balances = "balances";
            bfx_url.deposit = "deposit";
            bfx_url.instruments = "instruments";
            bfx_url.orderbook = "order-book/";
            bfx_url.marketticker = "market-ticker";
            bfx_url.price_ticker = "price-ticks/";
            bfx_url.time = "time";
            bfx_url.fees = "fees";
            bfx_url.depositcrypto = "deposit/crypto";
            bfx_url.depositcryptoaddr = "deposit/crypto/";
            bfx_url.deposit_fiat = "deposit/fiat/";
            bfx_url.PREFFERED_FIAT = "EUR";
            bfx_url.FIAT = new string[1];
            bfx_url.FIAT[0] = "EUR";

            bfx_vars.currencies = new JArray();
            bfx_vars.tickers_ref = new System.Collections.ArrayList();
            bfx_vars.tickers = new System.Collections.ArrayList();
            return;
        }

        public struct BFX_settings
        {
            public string key;
            string uname;
            string pwd;
        };

        public struct BFX_VARS
        {
            public string currenciesJSON;
            public string candlesJSON;
            public string feesJSON;
            public string instrumentsJSON;
            public string orderbookJSON;
            public string markettickerJSON;
            public string ticker_tempJSON;
            public string timeJSON;
            public string balancesJSON;
            public string depositcryptoJSON;
            public string depositcryptoaddrJSON;

            //Contains all tickers
            public System.Collections.ArrayList tickers_ref;
            public System.Collections.ArrayList tickers;

            public JArray currencies;
        };

        public struct BFX_URL
        {
            public string public_URL;
            public string currencies;
            public string base_URL;
            public string trades;
            public string balances;
            public string deposit;
            public string fees;
            public string instruments;
            public string orderbook;
            public string marketticker;
            public string price_ticker;
            public string time;
            public string deposit_fiat;
            public string depositcrypto;
            public string depositcryptoaddr;
            public string API_KY;
            public string candles;
            public string PREFFERED_FIAT;
            public string[] FIAT;// = { "EUR", "USD" };
        }
    }
}
