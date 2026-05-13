using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheCharityBLL.DTOs.DonationDTOs;

namespace TheCharityBLL.Services.Abstraction.MoneyDonation
{
    public interface IDonationService
    {
        // ===== CRUD =====
        Task<IEnumerable<DonationResponseDto>> GetAllDonationsAsync(bool includeDeleted = false);
        Task<DonationResponseDto?> GetDonationByIdAsync(int id);
        Task<DonationResponseDto> CreateDonationAsync(CreateDonationDto dto);
        Task<DonationResponseDto?> UpdateDonationAsync(int id, UpdateDonationDto dto);
        Task<bool> DeleteDonationAsync(int id);
        Task<bool> RestoreDonationAsync(int id);

        // ===== Filtering & Search =====
        Task<IEnumerable<DonationResponseDto>> GetDonationsByUserAsync(string userId);
        Task<IEnumerable<DonationResponseDto>> GetDonationsByCampaignAsync(int campaignId);
        Task<IEnumerable<DonationResponseDto>> GetDonationsByAmountRangeAsync(double minAmount, double maxAmount);
        Task<IEnumerable<DonationResponseDto>> GetDonationsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<DonationResponseDto>> GetRecentDonationsAsync(int days = 30);
        Task<IEnumerable<DonationResponseDto>> GetDeletedDonationsAsync();

        // ===== Statistics =====
        Task<double> GetTotalDonationsAmountAsync();
        Task<double> GetTotalDonationsAmountByUserAsync(string userId);
        Task<double> GetTotalDonationsAmountByCampaignAsync(int campaignId);
        Task<int> GetTotalDonationsCountAsync();
        Task<int> GetDonationsCountByUserAsync(string userId);
        Task<int> GetDonationsCountByCampaignAsync(int campaignId);

        // ===== Advanced Analytics =====
        Task<double> GetAverageDonationAmountAsync();
        Task<double> GetAverageDonationAmountByUserAsync(string userId);
        Task<double> GetAverageDonationAmountByCampaignAsync(int campaignId);
        Task<Dictionary<string, double>> GetTopDonorsByAmountAsync(int limit = 10);
        Task<Dictionary<int, double>> GetTopCampaignsByDonationsAsync(int limit = 10);
        Task<Dictionary<DateTime, double>> GetDonationsTrendAsync(int days = 30);
        Task<Dictionary<string, int>> GetDonationFrequencyByUserAsync();

        // ===== Campaign-Specific =====
        Task<double> GetCampaignTotalRaisedAsync(int campaignId);
        Task<double> GetCampaignProgressPercentageAsync(int campaignId);
        Task<IEnumerable<DonationResponseDto>> GetUsersDonationsOfACampaignAsync(int campaignId);

        // ===== User-Specific =====
        Task<IEnumerable<DonationResponseDto>> GetUserDonationHistoryAsync(string userId);
        Task<DateTime?> GetUserLastDonationDateAsync(string userId);
        Task<IEnumerable<int>> GetCampaignsDonatedByUserAsync(string userId);

        // ===== Bulk Operations =====
        Task<int> TransferDonationsToCampaignAsync(int fromCampaignId, int toCampaignId);
        Task<int> DeleteOldDonationsAsync(int daysOld = 365);

        // ===== Validation & Checks =====
        Task<bool> DonationExistsAsync(int id);
        Task<bool> HasUserDonatedToCampaignAsync(string userId, int campaignId);

        // ===== Eager Loading =====
        Task<DonationResponseDto?> GetDonationWithDetailsAsync(int id);

        // ===== Dashboard & Reporting =====
        Task<IEnumerable<DonationResponseDto>> GetLatestDonationsAsync(int limit = 10);
        Task<IEnumerable<DonationResponseDto>> GetLargestDonationsAsync(int limit = 10);
        Task<Dictionary<int, int>> GetDonationsPerCampaignCountAsync();
        Task<Dictionary<string, int>> GetDonationsPerUserCountAsync();
        Task<double> GetTodayDonationsTotalAsync();
        Task<double> GetThisWeekDonationsTotalAsync();
        Task<double> GetThisMonthDonationsTotalAsync();

        // ===== Financial Reporting =====
        Task<Dictionary<string, double>> GetMonthlyDonationsReportAsync(int year);
        Task<Dictionary<string, double>> GetQuarterlyDonationsReportAsync(int year);
        Task<Dictionary<string, double>> GetYearlyDonationsReportAsync(int yearsBack = 5);
        Task<Dictionary<string, double>> GetDonationsByTimeOfDayAsync();
        Task<Dictionary<string, double>> GetDonationsByDayOfWeekAsync();

        // ===== Campaign Performance =====
        Task<Dictionary<DateTime, double>> GetCampaignDonationTimelineAsync(int campaignId);

        // ===== User Engagement =====
        Task<IEnumerable<DonationResponseDto>> GetRecurringDonorsAsync(int minDonations = 3);
        Task<IEnumerable<DonationResponseDto>> GetFirstTimeDonorsAsync(DateTime startDate, DateTime endDate);
        Task<Dictionary<string, double>> GetUserLifetimeValueAsync();
        Task<IEnumerable<string>> GetLoyalDonorsAsync(double minTotalAmount = 1000, int minDonations = 5);

        // ===== Search & Filter Combinations =====
        Task<IEnumerable<DonationResponseDto>> SearchDonationsByUserAndCampaignAsync(string userId, int campaignId);
        Task<IEnumerable<DonationResponseDto>> GetDonationsByMultipleUsersAsync(IEnumerable<string> userIds);
        Task<IEnumerable<DonationResponseDto>> GetDonationsByMultipleCampaignsAsync(IEnumerable<int> campaignIds);
        Task<IEnumerable<DonationResponseDto>> GetDonationsByAmountAndDateAsync(double minAmount, DateTime startDate);

        // ===== Audit =====
        Task<IEnumerable<DonationResponseDto>> GetSuspiciousDonationsAsync(double amountThreshold = 10000);

        // ===== Export =====
        Task<int> GetDonationRecordCountForPeriodAsync(DateTime startDate, DateTime endDate);
    }
}
