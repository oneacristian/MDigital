namespace MDigital.Utils
{
    public static class Helpers
    {
        public enum PaymentStatus{
            Undefined = 0, 

            Pending = 1, 
            AttachedToTransaction = 2,
            Processed = 3, 
            Rejected = 4
        }
    }
}
