using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthVerif.Data;
using AuthVerif.DTOs;
using AuthVerif.Models;
using AuthVerif.Services;
using System.Threading.Tasks;

namespace AuthVerif.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthUser : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;
        private readonly IOtpService _otpService;

        public AuthUser(AppDbContext context, IEmailService emailService, IOtpService otpService)
        {
            _context = context;
            _emailService = emailService;
            _otpService = otpService;
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] UserDTO.ResetPasswordRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
            {
                return NotFound("User not found");
            }

            string otp = _otpService.GenerateOTP();

            var authTokenVerif = new AuthTokenVerif
            {
                Token = otp,
                Expiration = DateTime.UtcNow.AddSeconds(20), // OTP valid for 20 seconds
                user = user
            };

            _context.AuthTokenVerifs.Add(authTokenVerif);
            await _context.SaveChangesAsync();

            await _emailService.SendEmailAsync(user.Email, otp);

            return Ok("OTP sent to your email");
        }

        [HttpPost("verify-code")]
        public async Task<IActionResult> VerifyCode([FromBody] UserDTO.VerifyCodeRequest request)
        {
            var authTokenVerif = await _context.AuthTokenVerifs
                .FirstOrDefaultAsync(a => a.Token == request.Code && a.Expiration > DateTime.UtcNow);

            if (authTokenVerif == null)
            {
                return BadRequest("Invalid or expired code");
            }

            // Code is valid, proceed to reset password
            return Ok("Code verified");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequests request)
        {
            var authTokenVerif = await _context.AuthTokenVerifs
                .FirstOrDefaultAsync(a => a.Token == request.Code && a.Expiration > DateTime.UtcNow);

            if (authTokenVerif == null)
            {
                return BadRequest("Invalid or expired code");
            }

            var user = authTokenVerif.user;
            user.Password = request.NewPassword; 

            _context.AuthTokenVerifs.Remove(authTokenVerif);
            await _context.SaveChangesAsync();

            return Ok("Password reset successfully");
        }
    }

    public class ResetPasswordRequests
    {
        public required string Code { get; set; }
        public required string NewPassword { get; set; }
    }
}
