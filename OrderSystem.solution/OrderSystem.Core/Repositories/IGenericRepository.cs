﻿using OrderSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderSystem.Core.Entities;
using OrderSystem.Core.Specifications;

namespace OrderSystem.Core.Repositories
{
    public interface IGenericRepository<T> where T : BaseEntity
    {

        Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecifications<T> specifications);
        Task<T> GetEntityWithSpecAsync(ISpecifications<T> specifications);

        Task<int> GetCountWithSpecAsync(ISpecifications<T> specifications);

        Task Add(T item);
        void Delete(T item);
        void Update(T item);
        void SaveChanges();

    }
}
