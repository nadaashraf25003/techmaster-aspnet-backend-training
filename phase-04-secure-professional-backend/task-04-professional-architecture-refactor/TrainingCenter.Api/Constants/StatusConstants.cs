namespace TrainingCenter.Api.Constants;

public static class StatusConstants
{
    public static class Track
    {
        public const string Draft = "Draft";
        public const string Upcoming = "Upcoming";
        public const string InProgress = "InProgress";
        public const string Completed = "Completed";
        public const string Closed = "Closed";
        public const string Archived = "Archived";
    }

    public static class Enrollment
    {
        public const string Pending = "Pending";
        public const string Active = "Active";
        public const string Completed = "Completed";
        public const string Cancelled = "Cancelled";
        public const string Suspended = "Suspended";
    }

    public static class Payment
    {
        public const string Pending = "Pending";
        public const string Completed = "Completed";
        public const string Failed = "Failed";
        public const string Refunded = "Refunded";
    }
}
