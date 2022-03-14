using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class LikesController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly ILikesRepository _likesRepository;
        public LikesController(
            IUserRepository userRepository, 
            ILikesRepository likesRepository)
        {
            _likesRepository = likesRepository;
            _userRepository = userRepository;
        }

        [HttpPost("{username}")]//POST api/likes/{username}
        public async Task<ActionResult> AddLike(string username) {
            var sourceUserId = User.GetUserId();
            var likedUser = await _userRepository.GetUserByUserNameAsync(username);
            // this was the problem, I need to get the sourceUser from  _likesRepository
            // and then it's collection of LikedUsers are guaranteed to be at least empty and not null
            // this is because we use Include to query for the user in 'GetUserWithLikes' method.
            var sourceUser = await _likesRepository.GetUserWithLikes(sourceUserId);

            if(likedUser == null) return NotFound();

            if(sourceUser.UserName == username) return BadRequest("you can't like yourself");

            var userLike = await _likesRepository.GetUserLike(sourceUserId, likedUser.Id);
            if(userLike != null) return BadRequest("you already like this user");


            userLike = new UserLike {
                SourceUserId = sourceUserId,
                LikedUserId = likedUser.Id
            };

            sourceUser.LikedUsers.Add(userLike);

            if(await _userRepository.SaveAllAsync()) return Ok();
            return BadRequest("Failed to like user");
        }

        
        public async Task<ActionResult<IEnumerable<LikeDto>>> GetUserLikes(string predicate) {
            var users = await _likesRepository.GetUserLikes(predicate, User.GetUserId());
            return Ok(users);
        }
    }
}