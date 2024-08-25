using System;

namespace Dhaba_Delicious.Utilities
{
    public class DateTimeCalculator
    {
        public static string CalculateISTDate(int days = 0)
        {
                            // Get the current UTC time
                DateTime utcNow = DateTime.UtcNow;

                // Define the IST time zone (UTC + 5:30)
                TimeZoneInfo istTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

                // Convert UTC time to IST
                DateTime istTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, istTimeZone);

                istTime = istTime.AddDays(days);

                // Format the IST date to YYYY-MM-DD
                string formattedDate = istTime.ToString("yyyy-MM-dd");

            return formattedDate;
            
        }
    }
}
