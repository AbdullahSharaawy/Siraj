using TheCharityDAL.Entities;
using TheCharityDAL.Enums;

namespace TheCharityDAL.Repositories.Abstraction
{
    public interface IOrganizationRepository
    {
        // ===== Organization CRUD Operations =====
        Task<IEnumerable<Organization>> GetAllOrganizationsAsync(bool includeDeleted = false);
        Task<Organization?> GetOrganizationByIdAsync(int id);
        Task<Organization> AddOrganizationAsync(Organization organization);
        Task<Organization> UpdateOrganizationAsync(Organization organization);
        Task DeleteOrganizationAsync(int id);
        Task RestoreOrganizationAsync(int id);

        // ===== Organization Filtering & Search =====
        Task<Organization?> GetOrganizationByNameAsync(string name);
        Task<Organization?> GetOrganizationByPaymentInfoIdAsync(int PaymentInfoId);
        Task<IEnumerable<Organization>> SearchOrganizationsAsync(string searchTerm);
        Task<IEnumerable<Organization>> GetDeletedOrganizationsAsync();
        Task<IEnumerable<Organization>> GetOrganizationsDropDownAsync();
        Task<IEnumerable<Organization>> GetOrganizationsByAddressAsync(string address);

        // ===== Organization Statistics =====
        Task<int> GetTotalOrganizationsCountAsync();
        Task<int> GetActiveOrganizationsCountAsync();

        // ===== Organization Contact Methods =====
        Task<IEnumerable<OrganizationContactMethod>> GetOrganizationContactMethodsAsync(int organizationId);
        Task<OrganizationContactMethod?> GetContactMethodByIdAsync(int contactMethodId);
        Task<OrganizationContactMethod> AddContactMethodAsync(OrganizationContactMethod contactMethod);
        Task<OrganizationContactMethod> UpdateContactMethodAsync(OrganizationContactMethod contactMethod);
        Task DeleteContactMethodAsync(int contactMethodId);
        Task RestoreContactMethodAsync(int contactMethodId);
        Task<IEnumerable<OrganizationContactMethod>> GetContactMethodsByTypeAsync(int organizationId, ContactType type);

        // ===== Payment Info Management =====
        Task<PaymentInfo?> GetPaymentInfoByOrganizationIdAsync(int organizationId);
        Task<PaymentInfo?> GetPaymentInfoByIdAsync(int paymentInfoId);
        Task<PaymentInfo> AddPaymentInfoAsync(PaymentInfo paymentInfo);
        Task<PaymentInfo> UpdatePaymentInfoAsync(PaymentInfo paymentInfo);
        Task DeletePaymentInfoAsync(int paymentInfoId);
        Task RestorePaymentInfoAsync(int paymentInfoId);
        Task<bool> HasPaymentInfoAsync(int organizationId);

        // ===== Organization Performance =====
        Task<IEnumerable<Organization>> GetOrganizationsByCampaignCountAsync(int minCampaigns = 1);

        // ===== Validation & Checks =====
        Task<bool> OrganizationExistsAsync(int id);
        Task<bool> OrganizationNameExistsAsync(string name);

        // ===== Eager Loading =====
        Task<Organization?> GetOrganizationWithDetailsAsync(int id);

        // ===== Dashboard & Reporting =====
        Task<IEnumerable<Organization>> GetRecentlyRegisteredOrganizationsAsync(int days = 30);

        // ===== Advanced Queries =====
        Task<IEnumerable<Organization>> GetOrganizationsWithoutCampaignsAsync();
        Task<IEnumerable<Organization>> GetOrganizationsWithoutPaymentInfoAsync();
        Task<IEnumerable<Organization>> GetOrganizationsWithActiveCampaignsAsync();
        Task<IEnumerable<Organization>> GetOrganizationsWithCompletedCampaignsAsync();

        // ===== Contact Method Utilities =====
        Task<bool> ContactMethodExistsAsync(int organizationId, ContactType type, string value);
        Task<int> GetContactMethodCountByTypeAsync(int organizationId, ContactType type);
        Task<IEnumerable<Organization>> GetOrganizationsByContactTypeAsync(ContactType type);

        // ===== Payment Info Utilities =====
        Task<bool> ValidatePaymentInfoAsync(int organizationId);
        Task<IEnumerable<Organization>> GetOrganizationsWithValidPaymentInfoAsync();
        Task<Dictionary<int, DateTime>> GetOrganizationLastPaymentUpdateAsync();
    }
}
