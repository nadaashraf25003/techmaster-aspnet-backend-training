namespace TrainingCenter.Api.Common;

public enum TrackStatus
{
    Planned = 1,
    Active = 2,
    Completed = 3,
    Cancelled = 4
}

public enum EnrollmentStatus
{
    Active = 1,
    Completed = 2,
    Cancelled = 3,
    Suspended = 4
}

public enum PaymentStatus
{
    Pending = 1,
    Paid = 2,
    Refunded = 3,
    Failed = 4
}

public enum PaymentMethod
{
    Cash = 1,
    CreditCard = 2,
    BankTransfer = 3,
    VodafoneCash = 4,
    InstaPay = 5
}
