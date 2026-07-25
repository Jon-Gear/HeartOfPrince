namespace HeartOfPrince.Domain
{
    public readonly struct AvailabilityResult
    {
        public bool IsAvailable { get; }
        public string Reason { get; }

        private AvailabilityResult(bool isAvailable, string reason)
        {
            IsAvailable = isAvailable;
            Reason = reason;
        }

        public static AvailabilityResult Available()
        {
            return new AvailabilityResult(true, null);
        }

        public static AvailabilityResult Unavailable(string reason)
        {
            return new AvailabilityResult(
                false,
                string.IsNullOrWhiteSpace(reason)
                    ? "This activity is currently unavailable."
                    : reason);
        }
    }
}
