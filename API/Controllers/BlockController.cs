using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
  [Authorize]
    public class BlockController : BaseApiController
  {
    private readonly IUnitOfWork _unitOfWork;
    public BlockController(IUnitOfWork unitOfWork)
    {
      _unitOfWork = unitOfWork;
    }

    [HttpPost("{username}")]
    public async Task<ActionResult> BlockUser(string username)
    {
      var sourceUserId = User.GetUserId();
      var blockedUser = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);
      var sourceUser = await _unitOfWork.blockRepository.GetUserWithBlock(sourceUserId);

      if (blockedUser == null) return NotFound();

      if (sourceUser.UserName == username) return BadRequest("You cannot block yourself");

      var userBlock = await _unitOfWork.blockRepository.GetBlockedUser(sourceUserId, blockedUser.Id);

      if (userBlock != null) {
        return BadRequest("You already blocked this user ");
      } else {
          userBlock = new BlockUser
        {
          SourceUserId = sourceUserId,
          BlockedUserId = blockedUser.Id
        };

        sourceUser.BlockedUsers.Add(userBlock);

        if (await _unitOfWork.Complete()) return Ok();

        return BadRequest("Failed to block user");
      }      
    }

    [HttpDelete("{username}")]
    public async Task<ActionResult> UnblockUser(string username)
    {
      var sourceUserId = User.GetUserId();
      var blockedUser = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);
      var sourceUser = await _unitOfWork.blockRepository.GetUserWithBlock(sourceUserId);

      if (blockedUser == null) return NotFound();

      if (sourceUser.UserName == username) return BadRequest("You cannot block yourself");

      var userBlock = await _unitOfWork.blockRepository.GetBlockedUser(sourceUserId, blockedUser.Id);

      if(userBlock != null) _unitOfWork.blockRepository.UnblockUser(userBlock);

      if (await _unitOfWork.Complete()) return Ok();

      return BadRequest("Failed to unblock user");
    }

    [HttpGet]
    public async Task<ActionResult<BlockDto>> GetBlockedUsers([FromQuery] BlockParams blockParams)
    {
      blockParams.UserId = User.GetUserId();
      
      var user = await _unitOfWork.blockRepository.GetBlockedUsers(blockParams);

      return Ok(user);
    }

    [HttpGet("{username}")]
    public async Task<ActionResult<BlockUser>> GetBlockedUser(string username)
    {

      var sourceUserId = User.GetUserId();
      var blockedUser = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);

      var userBlock = await _unitOfWork.blockRepository.GetBlockedUser(sourceUserId, blockedUser.Id);
      if(userBlock != null)
      {return Ok(userBlock.BlockedUserId);}
      else {return Ok(userBlock);}
      
    }
  }
}