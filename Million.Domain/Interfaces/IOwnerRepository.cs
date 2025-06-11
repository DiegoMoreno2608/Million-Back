using Microsoft.AspNetCore.Http;
using Million.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Million.Domain.   Interfaces
{
    public interface IOwnerRepository
    {
        Task AddAsync(Owner owner);
        Task<List<Owner>> GetAllAsync();


    }
}
