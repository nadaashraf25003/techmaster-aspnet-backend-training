namespace TechMaster.Phase03.Task02.Common;

public enum TrackLevel
{
    Beginner = 1,
    Intermediate = 2,
    Advanced = 3
}

public enum TrackStatus
{
    Draft = 1,
    Upcoming = 2,
    InProgress = 3,
    Completed = 4,
    Cancelled = 5
}

public enum EnrollmentStatus
{
    Pending = 1,
    Active = 2,
    Completed = 3,
    Dropped = 4,
    Suspended = 5
}

public enum PaymentMethod
{
    CreditCard = 1,
    BankTransfer = 2,
    Cash = 3,
    VodafoneCash = 4,
    Fawry = 5,
    Stripe = 6
}

public enum PaymentStatus
{
    Pending = 1,
    Completed = 2,
    Failed = 3,
    Refunded = 4
}

public enum AttendanceStatus
{
    Present = 1,
    Absent = 2,
    Late = 3,
    Excused = 4
}
