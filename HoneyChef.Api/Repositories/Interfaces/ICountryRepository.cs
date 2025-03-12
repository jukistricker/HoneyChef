using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HoneyChef.Api.Entities;
using IOITCore.Repositories.Interfaces.Bases;

namespace HoneyChef.Api.Repositories.Interfaces
{
    public interface ICountryRepository:IAsyncGenericRepository<Country, long>
    {
        
    }
}