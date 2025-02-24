using System.Globalization;

namespace IOITCore.Extensions
{
    public static class DateTimeExtension
    {
        public static DateTime? StringToDateTime(
            string dateTime,
            string format)
        {
            DateTime resultDate;

            var isValid = !DateTime.TryParseExact(dateTime, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out resultDate);

            if (isValid == true)
            {
                return resultDate;
            }

            return null;
        }
    }
}
