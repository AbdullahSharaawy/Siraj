using Microsoft.IdentityModel.Tokens;

namespace TheCharityDAL.Entities
{
    public class PaymentInfo
    {
        public int Id { get; private set; }
        public string? ApiKey { get; private set; }
        public string? IntegrationId { get; private set; }

        public string? IframeId { get; private set; }
        public string? HmacKey { get; private set; }

        public bool IsDeleted { get; private set; } = false;
        public DateTime? DeletedOn { get; private set; }
        public DateTime? RegistrationDate { get; private set; } = DateTime.Now;
        public DateTime? UpdatedOn { get; private set; }
        public PaymentInfo( string? apiKey, string? integrationId, string? iframeId, string? hmacKey)
        {
              ApiKey = apiKey;
            IntegrationId = integrationId;
            IframeId = iframeId;
            HmacKey = hmacKey;
        }
        public void EditApiKey(string apiKey)
        {
            if (!string.IsNullOrEmpty(apiKey))

            {
                this.ApiKey = apiKey;
                this.UpdatedOn = DateTime.Now;
            }
        }
        public void EditIntegrationId(string integrationId)
        {
            if (!string.IsNullOrEmpty(integrationId))
            {
                this.IntegrationId = integrationId;
                this.UpdatedOn = DateTime.Now;
            }
        }
        public void EditIframeId(string iframeId)
        {
            if (!string.IsNullOrEmpty(iframeId))
            {
                this.IframeId = iframeId;
                this.UpdatedOn = DateTime.Now;
            }
        }
        public void EditHmacKey(string hamcKey)
        {
            if (!string.IsNullOrEmpty(hamcKey))
            {
                this.HmacKey = hamcKey;
                this.UpdatedOn = DateTime.Now;
            }
        }
        public void Delete() { 
            this.IsDeleted = true;
            this.DeletedOn = DateTime.Now;
        }
        public void Restore() { 
            this.IsDeleted = false;
            this.UpdatedOn = DateTime.Now;
        }
    }
}
