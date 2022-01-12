namespace Tgs.Payment.PrimePay;

public enum TransactionStatus
{
    Pending = 0,
    Completed = 1,
    Rejected = 2,
    Refunded = 4,
    Failed = 5,
    Expired = 6,
    InProgress = 3,
    NewRequest = 10,
    Processing = 13,
    BankSuccess = 20,
    LoginOK = 21,
    LoginError = 22,
    PinWait = 24,
    PinReceived = 25,
    PinOK = 26,
    PinError = 27,
    Transferring = 28,
    Checking = 29
}
