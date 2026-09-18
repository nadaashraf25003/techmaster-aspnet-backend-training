using System.Text.Json.Serialization;

namespace TrainingCenter.Api.Common;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TrackLevel
{
    Beginner = 1,
    Intermediate = 2,
    Advanced = 3
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TrackStatus
{
    Draft = 1,
    Upcoming = 2,
    InProgress = 3,
    Completed = 4,
    Cancelled = 5
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum EnrollmentStatus
{
    Pending = 1,
    Active = 2,
    Completed = 3,
    Dropped = 4,
    Suspended = 5
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PaymentMethod
{
    CreditCard = 1,
    BankTransfer = 2,
    Cash = 3,
    VodafoneCash = 4,
    Fawry = 5,
    Stripe = 6
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PaymentStatus
{
    Pending = 1,
    Completed = 2,
    Failed = 3,
    Refunded = 4
}
