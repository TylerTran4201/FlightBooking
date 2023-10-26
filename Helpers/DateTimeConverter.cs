namespace FlightBooking.Helpers
{
    public class DateTimeConverter
    {
        public static DateTime GetDateTimeFromString(string str)
        {
            return DateTime.Parse(str);
        }
    }
}