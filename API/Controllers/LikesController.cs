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
  public class LikesController : BaseApiController
  {
    private readonly IUnitOfWork _unitOfWork;
    public LikesController(IUnitOfWork unitOfWork)
    {
      _unitOfWork = unitOfWork;
    }

    [HttpPost("{username}")]
    public async Task<ActionResult> AddLike(string username)
    {
      var sourceUserId = User.GetUserId();
      var likedUser = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);
      var sourceUser = await _unitOfWork.likesRepository.GetUserWithLikes(sourceUserId);

      if (likedUser == null) return NotFound();

      if (sourceUser.UserName == username) return BadRequest("You cannot like yourself");

      var userLike = await _unitOfWork.likesRepository.GetUserLike(sourceUserId, likedUser.Id);

      if (userLike != null) {
        return BadRequest("You already liked this user ");
      } else {
          userLike = new UserLike
        {
          SourceUserId = sourceUserId,
          LikeUserId = likedUser.Id
        };

        sourceUser.LikedUsers.Add(userLike);

        if (await _unitOfWork.Complete()) return Ok();

        return BadRequest("Failed to unlike user");
      }      
    }

    [HttpDelete("{username}")]
    public async Task<ActionResult> RemoveLike(string username)
    {
      var sourceUserId = User.GetUserId();
      var likedUser = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);
      var sourceUser = await _unitOfWork.likesRepository.GetUserWithLikes(sourceUserId);

      if (likedUser == null) return NotFound();

      if (sourceUser.UserName == username) return BadRequest("You cannot unlike yourself");

      var userLike = await _unitOfWork.likesRepository.GetUserLike(sourceUserId, likedUser.Id);

      if(userLike != null) _unitOfWork.likesRepository.RemoveLike(userLike);

      if (await _unitOfWork.Complete()) return Ok();

      return BadRequest("Failed to unlike user");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LikeDto>>> GetUserLikes([FromQuery] LikesParams likesParams)
    {
      likesParams.UserId = User.GetUserId();
      var users = await _unitOfWork.likesRepository.GetUserLikes(likesParams);

      Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

      return Ok(users);
    }

    [HttpGet("{username}")]
    public async Task<ActionResult<UserLike>> GetUserLike(string username)
    {

      var sourceUserId = User.GetUserId();
      var likedUser = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);

      var userLike = await _unitOfWork.likesRepository.GetUserLike(sourceUserId, likedUser.Id);
      if(userLike != null)
      {return Ok(userLike.LikeUserId);}
      else {return Ok(userLike);}
      
  }
}
}