namespace MoneyBee.Shared.Application.Caching.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public  class CachingAttribute : Attribute
    {
        public int DurationInSeconds { get; }
        public string Prefix { get; set; }
        public string[] VaryByProperties { get; set; }

        public CachingAttribute(int durationInSeconds)
        {
            if (durationInSeconds <= 0)
                throw new ArgumentOutOfRangeException(nameof(durationInSeconds), "Duration must be greater than zero.");

            DurationInSeconds = durationInSeconds;
        }
    }
}