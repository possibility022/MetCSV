using System.Globalization;
using System.Text.RegularExpressions;

namespace METCSV.Common
{
    public class DateTimeParser
    {
        private readonly string dateTimeRegexPattern;
        private readonly string dateTimeFormat1;
        private readonly string dateTimeFormat2;

        private readonly CultureInfo provider = CultureInfo.CreateSpecificCulture("en-US");


        public DateTimeParser(string dateTimeRegexPattern, string dateTimeFormat1, string dateTimeFormat2)
        {
            this.dateTimeRegexPattern = dateTimeRegexPattern ?? throw new ArgumentNullException(nameof(dateTimeRegexPattern));
            this.dateTimeFormat2 = dateTimeFormat2 ?? throw new ArgumentNullException(nameof(dateTimeFormat2));
            this.dateTimeFormat1 = dateTimeFormat1 ?? throw new ArgumentNullException(nameof(dateTimeFormat1));
        }

        public DateTime ParseDateTime(string input)
        {
            Regex rgx = new Regex(dateTimeRegexPattern, RegexOptions.IgnoreCase);
            MatchCollection matches = rgx.Matches(input);

            //20 Feb 2017 08:28


            DateTime dateTime;

            try
            {
                dateTime = DateTime.ParseExact(matches[0].Value, dateTimeFormat1, provider);
                return dateTime;
            }
            catch (FormatException)
            { //to moglo sie zdazyc
            }

            dateTime = DateTime.ParseExact(matches[0].Value, dateTimeFormat2, provider);
            return dateTime;
        }
    }
}
