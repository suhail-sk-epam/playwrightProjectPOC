namespace BsiPlaywrightPoc.Model.User
{
    public class UserAddressDetails
    {
        public string Address { get; set; }
        public string City { get; set; }
        public string Postcode { get; set; }
        public string Zipcode { get; set; }
        public string State { get; set; } // For US, Australia, etc.
        public string Region { get; set; } // For New Zealand, etc.
        public string Province { get; set; } // For Canada
        public string County { get; set; } // For UK
        public string Country { get; set; }
    }
}
