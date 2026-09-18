namespace TrainingCenter.Api.Common;

public enum EnrollmentStatus
{
    Pending = 1,
    Active = 2,
    Completed = 3,
    Cancelled = 4,
    Suspended = 5
}

public enum PaymentStatus
{
    Pending = 1,
    Completed = 2,
    Failed = 3,
    Refunded = 4
}

public enum PaymentMethod
{
    CreditCard = 1,
    BankTransfer = 2,
    Cash = 3,
    Fawry = 4,
    VodafoneCash = 5
}

public enum TrackStatus
{
    Draft = 1,
    Upcoming = 2,
    InProgress = 3,
    Completed = 4,
    Archived = 5,
    Closed = 6
}
