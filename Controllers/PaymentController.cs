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


namespace WalletQ.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly Repository<Payment> _paymentRepository;
        private readonly IUserRepository _userRepository;

        public PaymentController(Repository<Payment> paymentRepository,IUserRepository userReposotary)
        {
            this._paymentRepository = paymentRepository;
            this._userRepository = userReposotary;
        }


        [HttpPost("Create")]
        public async Task<IActionResult> Create(uint amount,int validationTimeByMinutes)
        {
            if (validationTimeByMinutes < 1)
                return BadRequest("Please enter valid time");

            if (amount < 1)
                return BadRequest("the amount should be more than 0");

            Payment payment = new Payment
            {
                creator = await _userRepository.getUser(Guid.Parse(User.Claims.Where(a => a.Type == ClaimTypes.NameIdentifier).FirstOrDefault().Value)),
                amount = amount,
                validationTime = validationTimeByMinutes
            };

            _paymentRepository.Add(payment);
            if (!await _paymentRepository.Save())
                return BadRequest("an error occured!");

            return Created($"api/payment/get/{payment.Id.ToString()}", payment);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> get(Guid paymentId)
        {
            var payment = await _paymentRepository.Get(paymentId);
            if (payment is null)
                return BadRequest("Payment was not found");

            return Ok(payment);
        }
    }
}
