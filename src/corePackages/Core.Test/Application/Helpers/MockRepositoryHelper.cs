﻿using System.Linq.Expressions;
using Core.Persistence.Paging;
using Core.Persistence.Repositories;
using Microsoft.EntityFrameworkCore.Query;
using Moq;

namespace Core.Test.Application.Helpers;

public static class MockRepositoryHelper
{
    public static Mock<TRepository> GetRepository<TRepository, TEntity, TEntityId>(List<TEntity> list)
        where TEntity : Entity<TEntityId>, new()
        where TRepository : class, IAsyncRepository<TEntity, TEntityId>, IRepository<TEntity, TEntityId>
    {
        var mockRepo = new Mock<TRepository>();

        Build<TRepository, TEntity, TEntityId>(mockRepo, list);
        return mockRepo;
    }

    private static void Build<TRepository, TEntity, TEntityId>(Mock<TRepository> mockRepo, List<TEntity> entityList)
        where TEntity : Entity<TEntityId>, new()
        where TRepository : class, IAsyncRepository<TEntity, TEntityId>, IRepository<TEntity, TEntityId>
    {
        SetupGetListAsync<TRepository, TEntity, TEntityId>(mockRepo, entityList);
        SetupGetAsync<TRepository, TEntity, TEntityId>(mockRepo, entityList);
        SetupAddAsync<TRepository, TEntity, TEntityId>(mockRepo, entityList);
        SetupUpdateAsync<TRepository, TEntity, TEntityId>(mockRepo, entityList);
        SetupDeleteAsync<TRepository, TEntity, TEntityId>(mockRepo, entityList);
    }

    private static void SetupGetListAsync<TRepository, TEntity, TEntityId>(Mock<TRepository> mockRepo, List<TEntity> entityList)
        where TEntity : Entity<TEntityId>, new()
        where TRepository : class, IAsyncRepository<TEntity, TEntityId>, IRepository<TEntity, TEntityId>
    {
        mockRepo
            .Setup(
                s =>
                    s.GetListAsync(
                        It.IsAny<Expression<Func<TEntity, bool>>>(),
                        It.IsAny<Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>>(),
                        It.IsAny<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>>(),
                        It.IsAny<int>(),
                        It.IsAny<int>(),
                        It.IsAny<bool>(),
                        It.IsAny<CancellationToken>()
                    )
            )
            .ReturnsAsync(
                (
                    Expression<Func<TEntity, bool>> expression,
                    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
                    Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include,
                    int index,
                    int size,
                    bool enableTracking,
                    CancellationToken cancellationToken
                ) =>
                {
                    IList<TEntity> list = new List<TEntity>();

                    if (expression == null)
                        list = entityList;
                    else
                        list = entityList.Where(expression.Compile()).ToList();

                    Paginate<TEntity> paginateList = new() { Items = list };
                    return paginateList;
                }
            );
    }

    private static void SetupGetAsync<TRepository, TEntity, TEntityId>(Mock<TRepository> mockRepo, List<TEntity> entityList)
        where TEntity : Entity<TEntityId>, new()
        where TRepository : class, IAsyncRepository<TEntity, TEntityId>, IRepository<TEntity, TEntityId>
    {
        mockRepo
            .Setup(
                s =>
                    s.GetAsync(
                        It.IsAny<Expression<Func<TEntity, bool>>>(),
                        It.IsAny<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>>(),
                        It.IsAny<bool>(),
                        It.IsAny<CancellationToken>()
                    )
            )
            .ReturnsAsync(
                (
                    Expression<Func<TEntity, bool>> expression,
                    Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include,
                    bool enableTracking,
                    CancellationToken cancellationToken
                ) =>
                {
                    TEntity? result = entityList.FirstOrDefault(predicate: expression.Compile());
                    return result;
                }
            );
    }

    private static void SetupAddAsync<TRepository, TEntity, TEntityId>(Mock<TRepository> mockRepo, List<TEntity> entityList)
        where TEntity : Entity<TEntityId>, new()
        where TRepository : class, IAsyncRepository<TEntity, TEntityId>, IRepository<TEntity, TEntityId>
    {
        mockRepo
            .Setup(r => r.AddAsync(It.IsAny<TEntity>()))
            .ReturnsAsync(
                (TEntity entity) =>
                {
                    entityList.Add(entity);
                    return entity;
                }
            );
    }

    private static void SetupUpdateAsync<TRepository, TEntity, TEntityId2>(Mock<TRepository> mockRepo, List<TEntity> entityList)
        where TEntity : Entity<TEntityId2>, new()
        where TRepository : class, IAsyncRepository<TEntity, TEntityId2>, IRepository<TEntity, TEntityId2>
    {
        mockRepo
            .Setup(r => r.UpdateAsync(It.IsAny<TEntity>()))
            .ReturnsAsync(
                (TEntity entity) =>
                {
                    TEntity? result = entityList.FirstOrDefault(x => x.Id.Equals(entity.Id));
                    if (result != null)
                        result = entity;
                    return result;
                }
            );
    }

    private static void SetupDeleteAsync<TRepository, TEntity, TEntityId>(Mock<TRepository> mockRepo, List<TEntity> entityList)
        where TEntity : Entity<TEntityId>, new()
        where TRepository : class, IAsyncRepository<TEntity, TEntityId>, IRepository<TEntity, TEntityId>
    {
        mockRepo
            .Setup(r => r.DeleteAsync(It.IsAny<TEntity>()))
            .ReturnsAsync(
                (TEntity entity) =>
                {
                    entityList.Remove(entity);
                    return entity;
                }
            );
    }
}
