using System;
namespace ScorpionBitFx
{
    public class Scorpion_DATETIME
    {
        public string[] process_datetime()
        {
            DateTime start = new DateTime();
            DateTime end = new DateTime();

            start = DateTime.Now;
            end = DateTime.Now;

            start = start.Subtract(new TimeSpan(7, 0, 0, 0, 0));

            return new string[4] { start.ToString("yyyy-MM-dd"), start.ToShortTimeString(), end.ToString("yyyy-MM-dd"), end.ToShortTimeString() };
        }
    }
}
