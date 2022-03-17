using System;
namespace ScorpionBitFx
{
    public partial class EXCHANGE
    {
        public EXCHANGE_URL EXCHANGE_BITPANDA(ref EXCHANGE_URL bfx_url)
        {
            bfx_url.public_URL = "https://api.exchange.bitpanda.com/public/v1/";
            bfx_url.base_URL = "https://api.exchange.bitpanda.com/public/v1/account/";
            bfx_url.trades = "trades";
            bfx_url.currencies = "currencies";
            bfx_url.candles = "candlesticks";
            bfx_url.balances = "balances";
            bfx_url.deposit = "deposit";
            bfx_url.instruments = "instruments";
            bfx_url.orderbook = "order-book/";
            bfx_url.marketticker = "market-ticker/";
            bfx_url.price_ticker = "price-ticks/";
            bfx_url.time = "time";
            bfx_url.fees = "fees";
            bfx_url.depositcrypto = "deposit/crypto";
            bfx_url.depositcryptoaddr = "deposit/crypto/";
            bfx_url.deposit_fiat = "deposit/fiat/";
            bfx_url.PREFFERED_FIAT = "EUR";
            bfx_url.FIAT = new string[1];
            bfx_url.FIAT[0] = "EUR";
            bfx_url.orders = "orders";
            return bfx_url;
        }
    }
}
