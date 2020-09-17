using System.Collections;

namespace ScorpionBitFx
{
    public partial class EXCHANGE
    {
        //BITPANDA EXCHANGE
        public EXCHANGE_settings bfx_settings;
        public EXCHANGE_JSON bfx_vars;
        public EXCHANGE_URL bfx_url;

        public ArrayList EXCHANGE_COINS_JSON = new ArrayList();
        public ArrayList EXCHANGE_COINS_REF = new ArrayList();
        public ArrayList EXCHANGE_COINS = new ArrayList();

        public struct EXCHANGE_settings
        {
            public string key;
        };

        public struct EXCHANGE_JSON
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
            public string depositfiatJSON;

            //Contains all tickers
            public System.Collections.ArrayList tickers_ref;
            public System.Collections.ArrayList tickers;
        };

        public struct EXCHANGE_URL
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
            public string period;
            public string[] FIAT;// = { "EUR", "USD" };
            public string orders;
        }
    }
}
