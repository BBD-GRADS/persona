using System;

namespace PersonaBackend.Utils
{
    public class Chronos
    {
        private static Chronos _instance;
        private string simulationStartDateString;
        private DateTime simulationStartDate;

        private Chronos()
        {
            simulationStartDateString = "00|01|01";
            simulationStartDate = DateTime.UtcNow;
        }

        public static Chronos Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Chronos();
                }
                return _instance;
            }
        }

        public int getAge(string date)
        {
            var currentDate = this.GetCurrentDateString();

            var dateParts = date.Split('|');
            var currentParts = currentDate.Split('|');

            int birthDay = int.Parse(dateParts[0]);
            int birthMonth = int.Parse(dateParts[1]);
            int birthYear = int.Parse(dateParts[2]);

            int currentDay = int.Parse(currentParts[0]);
            int currentMonth = int.Parse(currentParts[1]);
            int currentYear = int.Parse(currentParts[2]);

            // Calculate total days since birth date and current date
            int totalDaysBirth = birthYear * 360 + birthMonth * 30 + birthDay; // Assuming each year has 360 days and each month has 30 days
            int totalDaysCurrent = currentYear * 360 + currentMonth * 30 + currentDay;

            // Calculate age in days
            int ageInDays = totalDaysCurrent - totalDaysBirth;

            return ageInDays;
        }

        // Method to set the simulation start date from a string in "YY|MM|DD" format
        public void SetSimulationStartDate(string date)
        {
            simulationStartDate = DateTime.UtcNow;
            simulationStartDateString = date;
        }

        public string GetCurrentDateString()
        {
            TimeSpan elapsedTime = GetCurrentElapsedSimulationTime();
            int totalDays = (int)Math.Floor(elapsedTime.TotalMinutes / 2);

            // Parse the simulation start date
            string[] startDateParts = simulationStartDateString.Split('|');
            int startYears = int.Parse(startDateParts[0]);
            int startMonths = int.Parse(startDateParts[1]);
            int startDays = int.Parse(startDateParts[2]);

            // Calculate simulated date components
            int years = startYears;
            int months = startMonths - 1; // Adjust for zero-based index
            int days = startDays - 1; // Adjust for zero-based index

            // Add the total simulated days
            years += totalDays / 360; // Each 360 days = 1 simulated year
            int remainingDays = totalDays % 360;

            months += remainingDays / 30; // Each 30 days = 1 simulated month
            days += remainingDays % 30;

            // Adjust months and days to stay within valid ranges
            while (months >= 12)
            {
                years++;
                months -= 12;
            }

            while (days >= 30)
            {
                months++;
                days -= 30;
            }

            // Ensure months and days are within valid ranges
            if (months >= 12)
            {
                months = 11; // Cap at December
            }
            if (days >= 30)
            {
                days = 29; // Cap at 30 days max
            }

            // Create the simulated date string
            string dateString = $"{years:00}|{months + 1:00}|{days + 1:00}";
            return dateString;
        }

        private TimeSpan GetCurrentElapsedSimulationTime()
        {
            DateTime currentTimeUtc = DateTime.UtcNow;
            TimeSpan elapsedTime = currentTimeUtc - simulationStartDate;
            return elapsedTime;
        }

        // Method to get the current simulated date as DateTime
        public DateTime GetCurrentDate()
        {
            TimeSpan elapsedTime = GetCurrentElapsedSimulationTime();
            DateTime currentDate = simulationStartDate.Add(elapsedTime);
            return currentDate;
        }

        // Method to compare two simulated dates in "YY|MM|DD" format
        public int CompareDates(string date1, string date2)
        {
            DateTime simulatedDate1 = ConvertToDate(date1);
            DateTime simulatedDate2 = ConvertToDate(date2);

            return simulatedDate1.CompareTo(simulatedDate2);
        }

        // Helper method to convert "YY|MM|DD" string to DateTime
        private DateTime ConvertToDate(string dateString)
        {
            string[] parts = dateString.Split('|');
            if (parts.Length != 3)
            {
                throw new ArgumentException("Invalid date format. Expected YY|MM|DD");
            }

            int year = int.Parse(parts[0]);
            int month = int.Parse(parts[1]);
            int day = int.Parse(parts[2]);

            // Adjusting the year to 2000 + YY
            return new DateTime(2000 + year, month, day);
        }
    }
}