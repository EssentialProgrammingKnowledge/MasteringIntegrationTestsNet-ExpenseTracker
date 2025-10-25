using ExpenseTracker.UI.Models;
using ExpenseTracker.UI.Services;

namespace ExpenseTracker.IntegrationTests.Setup.Services
{
    public interface IObservableCategoryService : ICategoryService
    {
        event Func<List<CategoryDTO>, Task>? GetCategories;
        event Func<CategoryDTO?, Task>? GetCategory;
        event Func<CategoryDTO, Task>? CategoryAdded;
        event Func<CategoryDTO, Task>? CategoryUpdated;
        event Func<int, Task>? CategoryDeleted;

        Task<List<CategoryDTO>> WaitForGetAllAsync(CancellationToken cancellationToken = default);
        Task<CategoryDTO?> WaitForGetByIdAsync(CancellationToken cancellationToken = default);
        Task<CategoryDTO> WaitForAddAsync(CancellationToken cancellationToken = default);
        Task<CategoryDTO> WaitForUpdateAsync(CancellationToken cancellationToken = default);
        Task<int> WaitForDeleteAsync(CancellationToken cancellationToken = default);
    }

    public class ObservableCategoryService : IObservableCategoryService
    {
        private readonly ICategoryService _inner;

        private TaskCompletionSource<List<CategoryDTO>>? _getAllTcs;
        private TaskCompletionSource<CategoryDTO?>? _getByIdTcs;
        private TaskCompletionSource<CategoryDTO>? _addTcs;
        private TaskCompletionSource<CategoryDTO>? _updateTcs;
        private TaskCompletionSource<int>? _deleteTcs;

        public event Func<List<CategoryDTO>, Task>? GetCategories;
        public event Func<CategoryDTO?, Task>? GetCategory;
        public event Func<CategoryDTO, Task>? CategoryAdded;
        public event Func<CategoryDTO, Task>? CategoryUpdated;
        public event Func<int, Task>? CategoryDeleted;

        public ObservableCategoryService(ICategoryService inner)
        {
            _inner = inner;
        }

        public async Task<Result<List<CategoryDTO>>> GetAll()
        {
            var result = await _inner.GetAll();
            if (result.Valid)
            {
                if (_getAllTcs != null)
                {
                    _getAllTcs.TrySetResult(result.Data ?? []);
                    _getAllTcs = null;
                }
                if (GetCategories != null)
                    await GetCategories.Invoke(result.Data ?? []);
            }

            return result;
        }

        public async Task<Result<CategoryDTO?>> GetById(int id)
        {
            var result = await _inner.GetById(id);
            if (result.Valid)
            {
                if (_getByIdTcs != null)
                {
                    _getByIdTcs.TrySetResult(result.Data);
                    _getByIdTcs = null;
                }
                if (GetCategory != null)
                    await GetCategory.Invoke(result.Data);
            }

            return result;
        }


        public async Task<Result> Add(CategoryDTO product)
        {
            var result = await _inner.Add(product);
            if (result.Valid)
            {
                if (_addTcs != null)
                {
                    _addTcs.TrySetResult(product);
                    _addTcs = null;
                }
                if (CategoryAdded != null)
                    await CategoryAdded.Invoke(product);
            }
            return result;
        }

        public async Task<Result> Update(CategoryDTO product)
        {
            var result = await _inner.Update(product);
            if (result.Valid)
            {
                if (_updateTcs != null)
                {
                    _updateTcs.TrySetResult(product);
                    _updateTcs = null;
                }
                if (CategoryUpdated != null)
                    await CategoryUpdated.Invoke(product);
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
                if (CategoryDeleted != null)
                    await CategoryDeleted.Invoke(id);
            }
            return result;
        }

        public Task<List<CategoryDTO>> WaitForGetAllAsync(CancellationToken cancellationToken = default)
        {
            _getAllTcs = new TaskCompletionSource<List<CategoryDTO>>();
            cancellationToken.Register(() => _getAllTcs.TrySetCanceled());
            return _getAllTcs.Task;
        }

        public Task<CategoryDTO?> WaitForGetByIdAsync(CancellationToken cancellationToken = default)
        {
            _getByIdTcs = new TaskCompletionSource<CategoryDTO?>();
            cancellationToken.Register(() => _getByIdTcs.TrySetCanceled());
            return _getByIdTcs.Task;
        }

        public Task<CategoryDTO> WaitForAddAsync(CancellationToken cancellationToken = default)
        {
            _addTcs = new TaskCompletionSource<CategoryDTO>();
            cancellationToken.Register(() => _addTcs.TrySetCanceled());
            return _addTcs.Task;
        }

        public Task<CategoryDTO> WaitForUpdateAsync(CancellationToken cancellationToken = default)
        {
            _updateTcs = new TaskCompletionSource<CategoryDTO>();
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
