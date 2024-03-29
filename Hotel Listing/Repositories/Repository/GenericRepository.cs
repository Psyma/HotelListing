﻿using Hotel_Listing.Data;
using Hotel_Listing.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel_Listing.Repositories.Repository {
    public class GenericRepository<T> : IGenericRepository<T> where T : class {
        private readonly DatabaseContext _context;
        private readonly DbSet<T> _db;

        public GenericRepository(DatabaseContext context) {
            _context = context;
            _db = _context.Set<T>();
        }


        public async Task Delete(int id) {
            var entity = await _db.FindAsync(id);
            
            if(entity != null) {
                _db.Remove(entity);
            }
        }

        public void DeleteRange(IEnumerable<T> entities) {
            if (entities != null) {
                _db.RemoveRange(entities);
            }
        }

        public async Task<T> Get(System.Linq.Expressions.Expression<Func<T, bool>> expression, List<string> includes = null) {
            IQueryable<T> query = _db;

            if (includes != null) {
                foreach (var includeProperty in includes) {
                    query = query.Include(includeProperty);
                }
            }
           
            return await query.AsNoTracking<T>().FirstOrDefaultAsync(expression);
        }

        public async Task<IList<T>> GetAll(System.Linq.Expressions.Expression<Func<T, bool>> expression = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, List<string> includes = null) {
            IQueryable<T> query = _db;

            if(expression != null) {
                query = query.Where(expression);
            }
            if (includes != null) {
                foreach (var includeProperty in includes) {
                    query = query.Include(includeProperty);
                }
            }
            if(orderBy != null) {
                query = orderBy(query);
            }

            return await query.AsNoTracking<T>().ToListAsync();
        }

        public async Task InserRange(IEnumerable<T> entities) {
            await _db.AddRangeAsync(entities);
        }

        public async Task Insert(T entity) {
            await _db.AddAsync(entity);
        }

        public void Update(T entity) {
            _db.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }
    }
}
