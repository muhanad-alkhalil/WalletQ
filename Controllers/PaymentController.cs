using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WalletQ.DataAccess.Repositories;
using WalletQ.Models;
using System.Web;
using WalletQ.DataAccess.Repositories.PaymentRepository;
using WalletQ.DTOs.Payment;
using AutoMapper;

namespace WalletQ.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ITransactionRepository _transactionRepository;

        public PaymentController(
            IPaymentRepository paymentRepository,
            IUserRepository userReposotary, 
            IMapper mapper,
            ITransactionRepository transactionRepository)
        {
            this._paymentRepository = paymentRepository;
            this._userRepository = userReposotary;
            this._mapper = mapper;
            this._transactionRepository = transactionRepository;
        }


        [HttpPost("Create")]
        public async Task<IActionResult> Create(CreatePaymentDTO paymentDTO)
        {


            if (paymentDTO.amount < 1)
                return BadRequest("the amount should be more than 0");

            if (paymentDTO.validationTimeByMinutes < 1)
                return BadRequest("Please enter valid time");

            Payment payment = new Payment
            {
                creator = await _userRepository.getUser(Guid.Parse(User.Claims.Where(a => a.Type == ClaimTypes.NameIdentifier).FirstOrDefault().Value)),
                amount = paymentDTO.amount,
                validationTime = paymentDTO.validationTimeByMinutes
            };

            _paymentRepository.Add(payment);
            if (!await _paymentRepository.Save())
                return BadRequest("an error occured!");

            PaymentDTO returnPayment = _mapper.Map<PaymentDTO>(payment);
            return Created($"api/payment/get/{payment.Id.ToString()}", returnPayment);
        }

        [HttpGet("getAll/{page}")]
        public async Task<IActionResult> getAll(int page)
        {
            var userId = Guid.Parse(User.Claims.Where(a => a.Type == ClaimTypes.NameIdentifier).FirstOrDefault().Value);
            var payments = await _paymentRepository.GetAllPayment(page, userId);
            var total = await _paymentRepository.PaymentsCount(userId);

            return Ok(new {payments = payments,total = total });
        }

        [HttpGet("get/{paymentId}")]
        public async Task<IActionResult> get(string paymentId)
        {
            Guid Id;

            if (!Guid.TryParse(paymentId, out Id))
            {
                return BadRequest("Please Enter valid id");
            }

            var payment = await _paymentRepository.GetPayment(Id);
            if (payment is null)
                return BadRequest("Payment was not found");

            switch (payment.paymentState)
            {
                case PaymentState.Paid:
                    return BadRequest("this payment has been paid already!");
                    break;

                case PaymentState.Expired:
                    return BadRequest("this payment is not valid any more");
                    break;

                case PaymentState.Cancelled:
                    return BadRequest("the payment has been canceled by the payment issuer");
                    break;
            }

            PaymentDTO returnPayment = _mapper.Map<PaymentDTO>(payment);

            return Ok(returnPayment);
        }

        [HttpGet("cancel/{paymentId}")]
        public async Task<IActionResult> cancel(Guid paymentId)
        {
            var payment = await _paymentRepository.Get(paymentId);
            if (payment is null)
                return BadRequest("Payment was not found");

            if (DateTime.Now > payment.CreatedAt.AddMinutes(payment.validationTime))
            {
                payment.paymentState = PaymentState.Expired;
                _paymentRepository.Update(payment);
                await _paymentRepository.Save();
                return BadRequest("The Payment is expired");
            }

            if (payment.paymentState != PaymentState.Pending)
                return BadRequest("this payment can not be cancelled");

            payment.paymentState = PaymentState.Cancelled;
            _paymentRepository.Update(payment);
            await _paymentRepository.Save();

            PaymentDTO returnPayment = _mapper.Map<PaymentDTO>(payment);

            return Ok(returnPayment);
        }

        [HttpPost("pay")]
        public async Task<IActionResult> pay(PayDTO payDTO)
        {
            Guid Id;

            if (!Guid.TryParse(payDTO.paymentId, out Id))
            {
                return BadRequest("Please Enter valid id");
            }

            var payment = await _paymentRepository.GetPayment(Id);
            if(payment is null)
                return BadRequest("Payment was not found");



            switch (payment.paymentState)
            {
                case PaymentState.Paid:
                    return BadRequest("this payment has been paid already!");
                    break;

                case PaymentState.Expired:
                    return BadRequest("this payment is not valid any more");
                    break;

                case PaymentState.Cancelled:
                    return BadRequest("the payment has been canceled by the payment issuer");
                    break;
            }

            if (DateTime.Now > payment.CreatedAt.AddMinutes(payment.validationTime))
            {
                payment.paymentState = PaymentState.Expired;
                _paymentRepository.Update(payment);
                await _paymentRepository.Save();
                return BadRequest("Payment time has been expired");
            }

            var payer = await _userRepository.getUser(Guid.Parse(User.Claims
                                                                .Where(a => a.Type == ClaimTypes.NameIdentifier)
                                                                .FirstOrDefault().Value));

            if(payer.Id == payment.creator.Id)
                return BadRequest("you canot pay to your self!");


            
            if (string.IsNullOrEmpty(payDTO.Password) || !VerfyPasswordHash(payDTO.Password, payer.PasswordHash, payer.PasswordSalt))
               return BadRequest("Authentication failed!");
            
               

            if (payer.wallet.Balance < payment.amount)
                return BadRequest("There is not enough balance");

            Transaction transaction = new Transaction
            {
                Sender = payer,
                Reciver = payment.creator,
                Amount = payment.amount
            };

            _transactionRepository.Add(transaction);

            var reciver = payment.creator;
            payer.wallet.Balance -= payment.amount;
            reciver.wallet.Balance += payment.amount;
            _userRepository.Update(payer);
            _userRepository.Update(reciver);

            payment.transaction = transaction;
            payment.paymentState = PaymentState.Paid;

            _paymentRepository.Update(payment);
            await _paymentRepository.Save();

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
