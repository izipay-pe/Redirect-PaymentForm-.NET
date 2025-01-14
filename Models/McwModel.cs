namespace Redirect_PaymentForm_.NET.Models
{
    public class PaymentRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string OrderId { get; set; }
        public double Amount { get; set; }
        public string Currency { get; set; }
        public string IdentityCode { get; set; }
    }

    public class PaymentResponse
    {
        public Dictionary<string, string> Parameters { get; set; }
        public string PrettyJson { get; set; }
    }
}