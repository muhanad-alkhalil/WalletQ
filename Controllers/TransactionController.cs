using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WalletQ.DataAccess.Repositories;
using WalletQ.DTOs.TransactionDTO;
using WalletQ.Models;

namespace WalletQ.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IUserRepository _userRepository;

        public TransactionController(ITransactionRepository transactionRepository, IUserRepository userReposotary)
        {
            this._transactionRepository = transactionRepository;
            this._userRepository = userReposotary;
        }
        [HttpGet("get/{id}")]
        public async Task<IActionResult> get(Guid id)
        {
            var Transactions = await _transactionRepository.GetTransaction(id);
            return Ok(Transactions);
        }

        [HttpGet("getLastTrasnaction")]
        public async Task<IActionResult> GetLastTransactions()
        {
            Guid UserId = Guid.Parse(User.Claims.Where(a => a.Type == ClaimTypes.NameIdentifier)
                                                .FirstOrDefault().Value);
            var Transactions = await _transactionRepository.GetLastTransactions(UserId);

            var TransactionsDTO = new List<TransactionPlainDTO>();
            foreach (var item in Transactions)
            {
                TransactionPlainDTO plainDTO;
                if (item.Sender.Id == UserId)
                {
                    plainDTO = new TransactionPlainDTO
                    {
                        Type = "Send",
                        With = item.Reciver.Name,
                        Amount = item.Amount
                    };
                }
                else
                {
                    plainDTO = new TransactionPlainDTO
                    {
                        Type = "Recive",
                        With = item.Sender.Name,
                        Amount = item.Amount
                    };
                }

                TransactionsDTO.Add(plainDTO);
            }

            return Ok(TransactionsDTO);
        }

        [HttpGet("getAll/{page}")]
        public async Task<IActionResult> getAll(int page)
        {
            var userId = Guid.Parse(User.Claims.Where(a => a.Type == ClaimTypes.NameIdentifier).FirstOrDefault().Value);
            var Transactions = await _transactionRepository.GetAllTransactions(page, userId);
            var total = await _transactionRepository.TransactionsCount(userId);

            Guid UserId = Guid.Parse(User.Claims.Where(a => a.Type == ClaimTypes.NameIdentifier)
                                               .FirstOrDefault().Value);

            var TransactionsDTO = new List<TransactionDetailsDTO>();
            foreach (var item in Transactions)
            {
                TransactionDetailsDTO plainDTO;
                if (item.Sender.Id == UserId)
                {
                    plainDTO = new TransactionDetailsDTO
                    {
                        Id = item.Id,
                        CreatedAt = item.CreatedAt,
                        Type = "Send",
                        With = item.Reciver.Name,
                        Amount = item.Amount
                    };
                }
                else
                {
                    plainDTO = new TransactionDetailsDTO
                    {
                        Id = item.Id,
                        CreatedAt = item.CreatedAt,
                        Type = "Recive",
                        With = item.Sender.Name,
                        Amount = item.Amount
                    };
                }
                TransactionsDTO.Add(plainDTO);
            }

            return Ok(new { transactions = TransactionsDTO, total = total });
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(CreateTransactionDTO createTransactionDTO)
        {
            var sender = await _userRepository.getUser(Guid.Parse(User.Claims
                                                                .Where(a => a.Type == ClaimTypes.NameIdentifier)
                                                                .FirstOrDefault().Value));

            if (string.IsNullOrEmpty(createTransactionDTO.password) || !VerfyPasswordHash(createTransactionDTO.password, sender.PasswordHash, sender.PasswordSalt))
                return BadRequest("Authentication failed!");

            if (createTransactionDTO.amount < 1)
                return BadRequest("the amount should be more than 0");

            Guid ReciverId;

            if (!Guid.TryParse(createTransactionDTO.id, out ReciverId))
            {
                return BadRequest("Please Enter valid user id");
            }

            var ReciverUser = await _userRepository.getUserByWalletId(ReciverId);
            if(ReciverUser is null)
            {
                return BadRequest("The user has not been found");
            }

            var SenderId = Guid.Parse(
                User.Claims
                .Where(User => User.Type == ClaimTypes.NameIdentifier)
                .FirstOrDefault().Value
                );

            var SenderUser = await _userRepository.getUser(SenderId);

            if (SenderUser.Id == ReciverUser.Id)
                return BadRequest("you canot send transaction to your self!");

            if (SenderUser.wallet.Balance < createTransactionDTO.amount)
                return BadRequest("There is not enough balance");

            var transaction = new Transaction
            {
                Sender = SenderUser,
                Reciver = ReciverUser,
                Amount = createTransactionDTO.amount
            };

            _transactionRepository.Add(transaction);

            SenderUser.wallet.Balance -= createTransactionDTO.amount;
            ReciverUser.wallet.Balance += createTransactionDTO.amount;
            _userRepository.Update(SenderUser);
            _userRepository.Update(ReciverUser);

            await _transactionRepository.Save();
            return Ok(new { message = "done" });

        }
        private bool VerfyPasswordHash(string Password, byte[] PasswordHash, byte[] PassWordSalt)
        {
            using (var Hmac = new System.Security.Cryptography.HMACSHA512(PassWordSalt))
            {
                var computedHash = Hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(Password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != PasswordHash[i])
                        return false;
                }
                return true;
            }
        }

    }
}
