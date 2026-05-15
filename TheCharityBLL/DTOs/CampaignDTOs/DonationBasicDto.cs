namespace TheCharityBLL.DTOs.CampaignDTOs
{
    public class DonationBasicDto
    {
        /// <example>101</example>
        public int Id { get; set; }
        /// <example>100.5</example>
        public double? Amount { get; set; }
        /// <example>2024-02-18T10:30:00</example>
        public DateTime RegistrationDate { get; set; }
        /// <example>john_doe</example>
        public string? UserName { get; set; }
    }
}
