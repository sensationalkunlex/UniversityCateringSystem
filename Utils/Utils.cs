namespace UniversityCateringSystem.Utils
{
    public class Utils
    {
        public static string GenerateOtp(string Reference, int otpLen)
        {

            var random = new Random();
            return Reference+ new string(Enumerable.Repeat("0123456789", otpLen)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
