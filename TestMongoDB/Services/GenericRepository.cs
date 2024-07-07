using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;
using TestMongoDB.Abstractions;
using TestMongoDB.Models;
using TestMongoDB.Models.Helpers;

namespace TestMongoDB.Services;

public class GenericRepository<T> : IRepository<T> where T : IEntityWithId
{
    private readonly IMongoCollection<T> _collection;
    public GenericRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<T>(CollectionsDict.CollectionNames[typeof(T)]);
    }

    public async Task<Result<ObjectId>> CreateAsync(T entity, IClientSessionHandle? clientSession = null)
    {
        if (entity == null)
        {
            return Result<ObjectId>.Failure(new Error("EntityIsNull", $"{typeof(T).Name} is null"));
        }

        try
        {
            var entityWithId = entity as IEntityWithId
                ?? throw new InvalidOperationException($"{typeof(T).Name} does not implement IEntityWithId");

            if (clientSession is null)
            {
                await _collection.InsertOneAsync(entity);
            }
            else
            {
                await _collection.InsertOneAsync(clientSession, entity);
            }

            return Result<ObjectId>.Success(entityWithId.Id);
        }
        catch (Exception ex)
        {
            return Result<ObjectId>.Failure(new Error("CreateEntityError", ex.Message));
        }
    }

    public async Task<Result<bool>> DeleteAsync(ObjectId id, IClientSessionHandle? clientSession = null)
    {
        try
        {
            var filter = Builders<T>.Filter.Eq(e => e.Id, id);
            DeleteResult result;

            if (clientSession is null)
            {
                result = await _collection.DeleteOneAsync(filter);
            }
            else
            {
                result = await _collection.DeleteOneAsync(clientSession, filter);
            }

            return Result<bool>.Success(result.DeletedCount > 0);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure(new Error("DeleteEntityError", ex.Message));
        }
    }

    public async Task<Result<List<T>>> GetAllAsync(int page = 1, int pageSize = 10, IClientSessionHandle? clientSession = null)
    {
        try
        {
            var filter = Builders<T>.Filter.Empty;
            IFindFluent<T, T> findFluent;

            if (clientSession is null)
            {
                findFluent = _collection.Find(filter);
            }
            else
            {
                findFluent = _collection.Find(clientSession, filter);
            }

            var entities = await findFluent
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();

            return Result<List<T>>.Success(entities);
        }
        catch (Exception ex)
        {
            return Result<List<T>>.Failure(new Error("GetAllEntitiesError", ex.Message));
        }
    }

    public async Task<Result<T>> GetByIdAsync(ObjectId id, IClientSessionHandle? clientSession = null)
    {
        try
        {
            var filter = Builders<T>.Filter.Eq(e => e.Id, id);
            T entity;

            if (clientSession is null)
            {
                entity = await _collection.Find(filter).FirstOrDefaultAsync();
            }
            else
            {
                entity = await _collection.Find(clientSession, filter).FirstOrDefaultAsync();
            }

            return Result<T>.Success(entity);
        }
        catch (Exception ex)
        {
            return Result<T>.Failure(new Error("GetEntityByIdError", ex.Message));
        }
    }

    public async Task<Result<bool>> UpdateAsync(T entity, IClientSessionHandle? clientSession = null)
    {
        try
        {
            var entityWithId = entity as IEntityWithId
                ?? throw new InvalidOperationException($"{typeof(T).Name} does not implement IEntityWithId");
            var filter = Builders<T>.Filter.Eq(e => e.Id, entityWithId.Id);
            ReplaceOneResult result;

            if (clientSession is null)
            {
                result = await _collection.ReplaceOneAsync(filter, entity);
            }
            else
            {
                result = await _collection.ReplaceOneAsync(clientSession, filter, entity);
            }

            return Result<bool>.Success(result.ModifiedCount > 0);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure(new Error("UpdateEntityError", ex.Message));
        }
    }

    public async Task<Result<bool>> AddSubdocumentAsync<TSub>(ObjectId parentId, Expression<Func<T, IEnumerable<TSub>>> subdocumentSelector, TSub subdocument, IClientSessionHandle? clientSession = null)
    {
        try
        {
            var update = Builders<T>.Update.Push(subdocumentSelector, subdocument);
            var filter = Builders<T>.Filter.Eq(e => e.Id, parentId);
            UpdateResult result;

            if (clientSession is null)
            {
                result = await _collection.UpdateOneAsync(filter, update);
            }
            else
            {
                result = await _collection.UpdateOneAsync(clientSession, filter, update);
            }

            return Result<bool>.Success(result.ModifiedCount > 0);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure(new Error("AddSubdocumentError", ex.Message));
        }
    }

    public async Task<Result<List<T>>> GetByPredicateAsync(Expression<Func<T, bool>> predicate, IClientSessionHandle? clientSession = null)
    {
        try
        {
            List<T> entities;

            if (clientSession is null)
            {
                entities = await _collection.Find(predicate).ToListAsync();
            }
            else
            {
                entities = await _collection.Find(clientSession, predicate).ToListAsync();
            }

            return Result<List<T>>.Success(entities);
        }
        catch (Exception ex)
        {
            return Result<List<T>>.Failure(new Error("GetByPredicateError", ex.Message));
        }
    }

    public async Task<Result<List<ObjectId>>> InsertManyAsync(List<T> entities, IClientSessionHandle? clientSession = null)
    {
        if (entities == null)
        {
            return Result<List<ObjectId>>.Failure(new Error("EntitiesIsNull", "Entities list is null"));
        }

        try
        {
            InsertManyOptions options = new()
            {
                IsOrdered = false
            };

            if (clientSession is null)
            {
                await _collection.InsertManyAsync(entities, options);
            }
            else
            {
                await _collection.InsertManyAsync(clientSession, entities, options);
            }

            var entityIds = entities.Select(e => (e as IEntityWithId)?.Id).Where(id => id != null).Cast<ObjectId>().ToList();
            return Result<List<ObjectId>>.Success(entityIds);
        }
        catch (Exception ex)
        {
            return Result<List<ObjectId>>.Failure(new Error("InsertManyEntitiesError", ex.Message));
        }
    }

}

