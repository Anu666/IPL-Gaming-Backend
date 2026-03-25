using IPL.Gaming.Common.Models.CosmosDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPL.Gaming.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllUsers();
    }
}
