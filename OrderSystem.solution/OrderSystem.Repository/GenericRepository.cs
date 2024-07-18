using Microsoft.EntityFrameworkCore;
using OrderSystem.Core.Entities;
using OrderSystem.Core.Repositories;
using OrderSystem.Core.Specifications;
using OrderSystem.Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderSystem.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly OrderManagementDbContext _dbcontext;

        public GenericRepository(OrderManagementDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }
        public async Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecifications<T> specifications)
        {
            return await ApplySpec(specifications).ToListAsync();
        }
        public async Task<T> GetEntityWithSpecAsync(ISpecifications<T> specifications)
        {
            return await ApplySpec(specifications).FirstOrDefaultAsync();
        }
        private IQueryable<T> ApplySpec(ISpecifications<T> specifications)
        {
            return SpecificationsEvaluator<T>.GetQuery(_dbcontext.Set<T>(), specifications);
        }

        public async Task<int> GetCountWithSpecAsync(ISpecifications<T> specifications)
        {
            return await ApplySpec(specifications).CountAsync();
        }

        public async Task Add(T item)
        {
            await _dbcontext.Set<T>().AddAsync(item);
        }

        public void Delete(T item)
        {
            _dbcontext.Set<T>().Remove(item);
        }

        public void Update(T item)
        {
            _dbcontext.Set<T>().Update(item);
        }

        public void SaveChanges()
        {
            _dbcontext.SaveChanges();
        }
    }
}
