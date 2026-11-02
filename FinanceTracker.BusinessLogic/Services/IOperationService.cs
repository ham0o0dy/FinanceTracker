using FinanceTracker.BusinessLogic.DTOs.Common;
using FinanceTracker.BusinessLogic.DTOs.Operations;

namespace FinanceTracker.BusinessLogic.Services;

public interface IOperationService
{
    Task<PagedResult<OperationDto>> GetAllAsync(Guid userId, OperationQueryParameters query);
    Task<OperationDto> GetByIdAsync(Guid userId, Guid operationId);
    Task<OperationDto> CreateAsync(Guid userId, CreateOperationRequest request);
    Task<OperationDto> UpdateAsync(Guid userId, Guid operationId, UpdateOperationRequest request);
    Task DeleteAsync(Guid userId, Guid operationId);
}
