using FinanceTracker.BusinessLogic.DTOs.Common;
using FinanceTracker.BusinessLogic.DTOs.Transfers;

namespace FinanceTracker.BusinessLogic.Services;

public interface ITransferService
{
    Task<PagedResult<TransferDto>> GetAllAsync(Guid userId, TransferQueryParameters query);
    Task<TransferDto> GetByIdAsync(Guid userId, Guid transferId);
    Task<TransferDto> CreateAsync(Guid userId, CreateTransferRequest request);
    Task DeleteAsync(Guid userId, Guid transferId);
}
