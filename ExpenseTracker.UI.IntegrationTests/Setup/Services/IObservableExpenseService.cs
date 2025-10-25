using ExpenseTracker.UI.Models;
using ExpenseTracker.UI.Services;

namespace ExpenseTracker.IntegrationTests.Setup.Services
{
    public interface IObservableExpenseService : IExpenseService
    {
        event Func<List<ExpenseDTO>, Task>? GetExpenses;
        event Func<ExpenseDetailsDTO?, Task>? GetExpense;
        event Func<ExpenseDTO, Task>? ExpenseAdded;
        event Func<ExpenseDTO, Task>? ExpenseUpdated;
        event Func<int, Task>? ExpenseDeleted;

        Task<List<ExpenseDTO>> WaitForGetAllAsync(CancellationToken cancellationToken = default);
        Task<ExpenseDetailsDTO?> WaitForGetByIdAsync(CancellationToken cancellationToken = default);
        Task<ExpenseDTO> WaitForAddAsync(CancellationToken cancellationToken = default);
        Task<ExpenseDTO> WaitForUpdateAsync(CancellationToken cancellationToken = default);
        Task<int> WaitForDeleteAsync(CancellationToken cancellationToken = default);
    }

    public class ObservableExpenseService : IObservableExpenseService
    {
        private readonly IExpenseService _inner;

        private TaskCompletionSource<List<ExpenseDTO>>? _getAllTcs;
        private TaskCompletionSource<ExpenseDetailsDTO?>? _getByIdTcs;
        private TaskCompletionSource<ExpenseDTO>? _addTcs;
        private TaskCompletionSource<ExpenseDTO>? _updateTcs;
        private TaskCompletionSource<int>? _deleteTcs;

        public event Func<List<ExpenseDTO>, Task>? GetExpenses;
        public event Func<ExpenseDetailsDTO?, Task>? GetExpense;
        public event Func<ExpenseDTO, Task>? ExpenseAdded;
        public event Func<ExpenseDTO, Task>? ExpenseUpdated;
        public event Func<int, Task>? ExpenseDeleted;

        public ObservableExpenseService(IExpenseService inner)
        {
            _inner = inner;
        }

        public async Task<Result<List<ExpenseDTO>>> GetAll()
        {
            var result = await _inner.GetAll();
            if (result.Valid)
            {
                if (_getAllTcs != null)
                {
                    _getAllTcs.TrySetResult(result.Data ?? []);
                    _getAllTcs = null;
                }
                if (GetExpenses != null)
                    await GetExpenses.Invoke(result.Data ?? []);
            }

            return result;
        }

        public async Task<Result<ExpenseDetailsDTO?>> GetById(int id)
        {
            var result = await _inner.GetById(id);
            if (result.Valid)
            {
                if (_getByIdTcs != null)
                {
                    _getByIdTcs.TrySetResult(result.Data);
                    _getByIdTcs = null;
                }
                if (GetExpense != null)
                    await GetExpense.Invoke(result.Data);
            }

            return result;
        }

        public async Task<Result> Add(ExpenseDTO product)
        {
            var result = await _inner.Add(product);
            if (result.Valid)
            {
                if (_addTcs != null)
                {
                    _addTcs.TrySetResult(product);
                    _addTcs = null;
                }
                if (ExpenseAdded != null)
                    await ExpenseAdded.Invoke(product);
            }
            return result;
        }

        public async Task<Result> Update(ExpenseDTO product)
        {
            var result = await _inner.Update(product);
            if (result.Valid)
            {
                if (_updateTcs != null)
                {
                    _updateTcs.TrySetResult(product);
                    _updateTcs = null;
                }
                if (ExpenseUpdated != null)
                    await ExpenseUpdated.Invoke(product);
            }
            return result;
        }

        public async Task<Result> Delete(int id)
        {
            var result = await _inner.Delete(id);
            if (result.Valid)
            {
                if (_deleteTcs != null)
                {
                    _deleteTcs.TrySetResult(id);
                    _deleteTcs = null;
                }
                if (ExpenseDeleted != null)
                    await ExpenseDeleted.Invoke(id);
            }
            return result;
        }

        public Task<List<ExpenseDTO>> WaitForGetAllAsync(CancellationToken cancellationToken = default)
        {
            _getAllTcs = new TaskCompletionSource<List<ExpenseDTO>>();
            cancellationToken.Register(() => _getAllTcs.TrySetCanceled());
            return _getAllTcs.Task;
        }

        public Task<ExpenseDetailsDTO?> WaitForGetByIdAsync(CancellationToken cancellationToken = default)
        {
            _getByIdTcs = new TaskCompletionSource<ExpenseDetailsDTO?>();
            cancellationToken.Register(() => _getByIdTcs.TrySetCanceled());
            return _getByIdTcs.Task;
        }

        public Task<ExpenseDTO> WaitForAddAsync(CancellationToken cancellationToken = default)
        {
            _addTcs = new TaskCompletionSource<ExpenseDTO>();
            cancellationToken.Register(() => _addTcs.TrySetCanceled());
            return _addTcs.Task;
        }

        public Task<ExpenseDTO> WaitForUpdateAsync(CancellationToken cancellationToken = default)
        {
            _updateTcs = new TaskCompletionSource<ExpenseDTO>();
            cancellationToken.Register(() => _updateTcs.TrySetCanceled());
            return _updateTcs.Task;
        }

        public Task<int> WaitForDeleteAsync(CancellationToken cancellationToken = default)
        {
            _deleteTcs = new TaskCompletionSource<int>();
            cancellationToken.Register(() => _deleteTcs.TrySetCanceled());
            return _deleteTcs.Task;
        }
    }
}
