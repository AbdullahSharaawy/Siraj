namespace TheCharityBLL.DTOs.CampaignDTOs
{
    public class DonationBasicDto
    {
        public int Id { get; set; }
        public double? Amount { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string? UserName { get; set; }
    }
}
