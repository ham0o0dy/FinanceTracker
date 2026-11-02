using FinanceTracker.BusinessLogic.DTOs.Accounts;

namespace FinanceTracker.BusinessLogic.Services;

public interface IAccountService
{
    Task<List<AccountDto>> GetAllAsync(Guid userId, bool includeArchived);
    Task<AccountDto> GetByIdAsync(Guid userId, Guid accountId);
    Task<AccountDto> CreateAsync(Guid userId, CreateAccountRequest request);
    Task<AccountDto> UpdateAsync(Guid userId, Guid accountId, UpdateAccountRequest request);
    Task<AccountDto> ArchiveAsync(Guid userId, Guid accountId, ArchiveAccountRequest request);
    Task DeleteAsync(Guid userId, Guid accountId);
}
