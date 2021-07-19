using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
  public class BlockRepository : IBlockRepository
  {
    private readonly DataContext _context;
    
    public BlockRepository(DataContext context)
    {
      _context = context;
    }

    public void UnblockUser(BlockUser blockUser)
    {
      _context.Blocked.Remove(blockUser);
    }

    public async Task<BlockUser> GetBlockedUser(int sourceUserId, int blockUserId)
    {
      return await _context.Blocked.FindAsync(sourceUserId, blockUserId);
    }

    public async Task<BlockDto> GetBlockedUsers(BlockParams blockParams)
    {
      var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();
      var blockedUsers = _context.Blocked.AsQueryable();

      if (blockParams.Predicate == "blockedby") 
      {
          blockedUsers = blockedUsers.Where(blocked => blocked.BlockedUserId == blockParams.UserId);
          blockedUsers = blockedUsers.Where(blocked => blocked.SourceUser.UserName == blockParams.Username);
          users = blockedUsers.Select(blocked => blocked.SourceUser);
          //users = users.AppUser.UserName == blockParams.Username;
      }

      var blockedUser = await users.Select(user => new BlockDto
      {
          Username = user.UserName,
          KnownAs = user.KnownAs,
          Age = user.DateOfBirth.CalculateAge(),
          PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url,
          City = user.City,
          Id = user.Id
      }).FirstOrDefaultAsync();

      if(blockedUser == null) {
        return null;
      }
      else {
        return blockedUser;
      }
    }

    public async Task<AppUser> GetUserWithBlock(int userId)
    {
      return await _context.Users
        .Include(x => x.BlockedUsers)
        .FirstOrDefaultAsync(x => x.Id == userId);
    }
  }
}