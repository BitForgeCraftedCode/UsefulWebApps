namespace UsefulWebApps.Helpers.Weather
{
    public class UnitConversions
    {
        public static DateTime UnixTimeStampToDateTime(long unixTimeStamp, long timeZoneShift)
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp + timeZoneShift);
            return dateTime;
        }

        public static double MetersToMiles(ushort meters)
        {
            return meters / 1000 * 0.6213711922;
        }

        public static double mmToInch(float mm)
        {
            return mm * 0.393701 / 10;
        }

        public static string WindDegToDir(float windDeg)
        {
            int deg = (int)Math.Round(windDeg, 0);
            switch (deg)
            {
                case >= 349 and <= 360:
                case >= 0 and <= 11:
                    return "N";
                case >= 12 and <= 34:
                    return "NNE";
                case >= 35 and <= 56:
                    return "NE";
                case >= 57 and <= 79:
                    return "ENE";
                case >= 80 and <= 101:
                    return "E";
                case >= 102 and <= 124:
                    return "ESE";
                case >= 125 and <= 146:
                    return "SE";
                case >= 147 and <= 169:
                    return "SSE";
                case >= 170 and <= 191:
                    return "S";
                case >= 192 and <= 214:
                    return "SSW";
                case >= 215 and <= 236:
                    return "SW";
                case >= 237 and <= 259:
                    return "WSW";
                case >= 260 and <= 281:
                    return "W";
                case >= 282 and <= 304:
                    return "WNW";
                case >= 305 and <= 326:
                    return "NW";
                //case >= 327 and <= 348 //-- any remaining values will be in this range
                default:
                    return "NNW";
            }
        }
    }
}
