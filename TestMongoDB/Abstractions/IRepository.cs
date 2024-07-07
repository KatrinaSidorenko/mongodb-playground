using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;
using TestMongoDB.Models;
using TestMongoDB.Models.Helpers;

namespace TestMongoDB.Abstractions;

public interface IRepository<T> where T : IEntityWithId
{
    Task<Result<bool>> AddSubdocumentAsync<TSub>(ObjectId parentId, Expression<Func<T, IEnumerable<TSub>>> subdocumentSelector, TSub subdocument, IClientSessionHandle? clientSession = null);
    Task<Result<ObjectId>> CreateAsync(T entity, IClientSessionHandle? clientSession = null);
    Task<Result<bool>> DeleteAsync(ObjectId id, IClientSessionHandle? clientSession = null);
    Task<Result<List<T>>> GetAllAsync(int page = 1, int pageSize = 10, IClientSessionHandle? clientSession = null);
    Task<Result<T>> GetByIdAsync(ObjectId id, IClientSessionHandle? clientSession = null);
    Task<Result<List<T>>> GetByPredicateAsync(Expression<Func<T, bool>> predicate, IClientSessionHandle? clientSession = null);
    Task<Result<List<ObjectId>>> InsertManyAsync(List<T> entities, IClientSessionHandle? clientSession = null);
    Task<Result<bool>> UpdateAsync(T entity, IClientSessionHandle? clientSession = null);
}
