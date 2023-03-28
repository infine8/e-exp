using System.Numerics;
using Nethereum.Util;

namespace EthExplorer.Application.Common.Extensions;

public static class BitIntegerExtensions
{
    public static decimal FromWei(this BigInteger val, byte? decimals = null)
    {
        return decimals.HasValue 
            ? UnitConversion.Convert.FromWei(val, decimals.Value)
            : UnitConversion.Convert.FromWei(val);
    }

    public static string FromWeiToString(this BigInteger val, byte? decimals = null)
    {
        var bigDecimal = decimals.HasValue 
            ? UnitConversion.Convert.FromWeiToBigDecimal(val, decimals.Value)
            : UnitConversion.Convert.FromWeiToBigDecimal(val);

        return bigDecimal.ToString();
    }
}