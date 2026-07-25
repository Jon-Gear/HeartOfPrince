using System;

namespace HeartOfPrince.Domain
{
    [Serializable]
    public sealed class WorldClockState
    {
        public const int MinutesPerDay = 24 * 60;

        public int Day = 1;
        public int MinuteOfDay = 8 * 60;

        public int NormalizedMinuteOfDay
        {
            get
            {
                int value = MinuteOfDay % MinutesPerDay;
                return value < 0 ? value + MinutesPerDay : value;
            }
        }

        public void BeginDay(int day, int wakeMinute)
        {
            Day = Math.Max(1, day);
            MinuteOfDay = Math.Max(0, wakeMinute);
        }

        public void Advance(int minutes)
        {
            MinuteOfDay += Math.Max(0, minutes);
        }
    }
}
