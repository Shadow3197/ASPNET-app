using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface IBlockRepository
    {
        void UnblockUser(BlockUser blockUser);
        Task<BlockUser> GetBlockedUser(int sourceUserId, int blockUserId);
        Task<BlockDto> GetBlockedUsers(BlockParams blockParams);
        Task<AppUser> GetUserWithBlock(int userId);
    }
}