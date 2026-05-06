namespace TheCharityBLL.DTOs.CampaignDTOs
{
    public class DonationBasicDto
    {
        public int Id { get; set; }
        public double Amount { get; set; }
        public DateTime DonationDate { get; set; }
        public string? DonorName { get; set; }
    }
}
