using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WalletQ.DataAccess.Repositories;
using WalletQ.Models;
using WalletQ.DTOs.User;


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
        public async Task<IActionResult> Get(Guid id)
        {
            User user = await _repository.Get(id);
            return Ok(user);
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
