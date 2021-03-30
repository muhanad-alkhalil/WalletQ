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

        public PaymentController(IPaymentRepository paymentRepository,IUserRepository userReposotary, IMapper mapper)
        {
            this._paymentRepository = paymentRepository;
            this._userRepository = userReposotary;
            this._mapper = mapper;
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


        [HttpGet("get/{paymentId}")]
        public async Task<IActionResult> get(Guid paymentId)
        {
            var payment = await _paymentRepository.GetPayment(paymentId);
            if (payment is null)
                return BadRequest("Payment was not found");

            PaymentDTO returnPayment = _mapper.Map<PaymentDTO>(payment);

            return Ok(returnPayment);
        }

        [HttpGet("cancel/{paymentId}")]
        public async Task<IActionResult> cancel(Guid paymentId)
        {
            var payment = await _paymentRepository.Get(paymentId);
            if (payment is null)
                return BadRequest("Payment was not found");

            if(payment.paymentState != PaymentState.Pending)
                return BadRequest("this payment can not be cancelled");

            payment.paymentState = PaymentState.Cancelled;
            _paymentRepository.Update(payment);
            await _paymentRepository.Save();

            PaymentDTO returnPayment = _mapper.Map<PaymentDTO>(payment);

            return Ok(returnPayment);
        }
    }
}
