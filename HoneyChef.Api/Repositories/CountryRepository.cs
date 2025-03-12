using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HoneyChef.Api.Entities;
using HoneyChef.Api.Repositories.Interfaces;
using IOITCore.Persistence;
using IOITCore.Repositories.Bases;

namespace HoneyChef.Api.Repositories
{
    public class CountryRepository:AsyncGenericRepository<Country, long>,ICountryRepository
    {
        private readonly IOITDbContext _context;

        public CountryRepository(IOITDbContext context):base(context)
        {
            _context = context;
        }
    }
}