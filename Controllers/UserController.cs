using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WalletQ.DataAccess.Repositories;
using WalletQ.Models;
using WalletQ.DTOs.User;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace WalletQ.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _repository;

        public UserController(IUserRepository Repository)
        {
            _repository = Repository;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IEnumerable<User> users = await _repository.GetAll();
            return Ok(users);
        }

        [HttpGet("{id}")]
        [Authorize]

        public async Task<IActionResult> Get(Guid id)
        {
            User user = await _repository.Get(id);
            return Ok(user);
        }

        [HttpGet("info")]
        [Authorize]
        public async Task<IActionResult> GetByToken()
        {
            var userId = Guid.Parse(User.Claims.Where(a => a.Type == ClaimTypes.NameIdentifier).FirstOrDefault().Value);
            User user = await _repository.getUser(userId);
            UserDTO userDTO = new UserDTO
            {
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                DateOfBirth = user.DateOfBirth,
                wallet =user.wallet
            };
            return Ok(userDTO);
        }


        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserRegisterDTO userData)
        {
            if (await _repository.isEmailExists(userData.Email))
                return BadRequest("The email is already used");

            byte[] PasswordHash, PasswordSalt;
            CreateUserHash(userData.Password, out PasswordHash, out PasswordSalt);

            User newUser = new User
            {
                Name = userData.Name,
                Surname = userData.Surname,
                PhoneNumber = userData.PhoneNumber,
                Email = userData.Email,
                DateOfBirth = userData.DateOfBirth,
                wallet = new Wallet(),
                PasswordHash = PasswordHash,
                PasswordSalt = PasswordSalt
            };
            _repository.Add(newUser);
            await _repository.Save();
            return StatusCode(201);
        }

        private void CreateUserHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var Hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = Hmac.Key;
                passwordHash = Hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }


    }
}
