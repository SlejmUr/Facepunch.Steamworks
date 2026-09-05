using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace Steamworks
{
	public static partial class Utility
    {
	    public static readonly Encoding Utf8NoBom = new UTF8Encoding( false, false );

        static internal T ToType<T>( this IntPtr ptr )
        {
            if ( ptr == IntPtr.Zero )
                return default;

            return Marshal.PtrToStructure<T>( ptr);
        }

        static internal object ToType( this IntPtr ptr, System.Type t )
        {
            if ( ptr == IntPtr.Zero )
                return default;

            return Marshal.PtrToStructure( ptr, t );
        }

        static internal uint Swap( uint x )
        {
            return ((x & 0x000000ff) << 24) +
                   ((x & 0x0000ff00) << 8) +
                   ((x & 0x00ff0000) >> 8) +
                   ((x & 0xff000000) >> 24);
        }

        static public uint IpToInt32( this IPAddress ipAddress )
        {
            return Swap( (uint) ipAddress.Address );
        }

        static public IPAddress Int32ToIp( uint ipAddress )
        {
            return new IPAddress( Swap( ipAddress ) );
        }

		public static string FormatPrice(string currency, double price)
        {
			var decimaled = price.ToString("0.00");

			return currency switch
			{
				"AED" => $"{decimaled}د.إ",
				"ARS" => $"${decimaled} ARS",
				"AUD" => $"A${decimaled}",
				"BRL" => $"R${decimaled}",
				"CAD" => $"C${decimaled}",
				"CHF" => $"Fr. {decimaled}",
				"CLP" => $"${decimaled} CLP",
				"CNY" => $"{decimaled}元",
				"COP" => $"COL$ {decimaled}",
				"CRC" => $"₡{decimaled}",
				"EUR" => $"€{decimaled}",
				"SEK" => $"{decimaled}kr",
				"GBP" => $"£{decimaled}",
				"HKD" => $"HK${decimaled}",
				"ILS" => $"₪{decimaled}",
				"IDR" => $"Rp{decimaled}",
				"INR" => $"₹{decimaled}",
				"JPY" => $"¥{decimaled}",
				"KRW" => $"₩{decimaled}",
				"KWD" => $"KD {decimaled}",
				"KZT" => $"{decimaled}₸",
				"MXN" => $"Mex${decimaled}",
				"MYR" => $"RM {decimaled}",
				"NOK" => $"{decimaled} kr",
				"NZD" => $"${decimaled} NZD",
				"PEN" => $"S/. {decimaled}",
				"PHP" => $"₱{decimaled}",
				"PLN" => $"{decimaled}zł",
				"QAR" => $"QR {decimaled}",
				"RUB" => $"{decimaled}₽",
				"SAR" => $"SR {decimaled}",
				"SGD" => $"S${decimaled}",
				"THB" => $"฿{decimaled}",
				"TRY" => $"₺{decimaled}",
				"TWD" => $"NT$ {decimaled}",
				"UAH" => $"₴{decimaled}",
				"USD" => $"${decimaled}",
				"UYU" => $"$U {decimaled}",// yes the U goes after $
				"VND" => $"₫{decimaled}",
				"ZAR" => $"R {decimaled}",
				// TODO - check all of them https://partner.steamgames.com/doc/store/pricing/currencies
				_ => $"{decimaled} {currency}",
			};
		}

		static readonly byte[] readBuffer = new byte[1024 * 8];

		public static string ReadNullTerminatedUTF8String( this BinaryReader br )
		{
			lock ( readBuffer )
			{
				byte chr;
				int i = 0;
				while ( (chr = br.ReadByte()) != 0 && i < readBuffer.Length )
				{
					readBuffer[i] = chr;
					i++;
				}

				return Utf8NoBom.GetString( readBuffer, 0, i );
			}
		}
	}
}
