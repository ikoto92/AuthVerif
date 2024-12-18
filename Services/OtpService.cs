using System.Security.Cryptography;
using System.Text;

namespace AuthVerif.Services
{
    public interface IOtpService
    {
        string GenerateOTP();
    }

    public class OtpService : IOtpService
    {
        public string GenerateOTP()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] randomNumber = new byte[6];
                rng.GetBytes(randomNumber);

               
                StringBuilder otp = new StringBuilder();
                foreach (byte b in randomNumber)
                {
                    otp.Append(b % 10); 
                }

                return otp.ToString();
            }
        }
    }
}
