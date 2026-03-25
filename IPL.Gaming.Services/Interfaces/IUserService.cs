using IPL.Gaming.Common.Models.CosmosDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPL.Gaming.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<User>> GetAllUsers();
    }
}
