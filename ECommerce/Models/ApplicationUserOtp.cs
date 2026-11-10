using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Models
{
    public class ApplicationUserOtp
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        [ForeignKey(nameof(ApplicationUserId))]
        public ApplicationUser ApplicationUser { get; set; }
        public string OTP { get; set; }
        public bool IsValid { get; set; }
        public DateTime ValidTo { get; set; }
        public DateTime CreatedAt { get; set; }

        public ApplicationUserOtp(string otp , string userId)
        {
            ApplicationUserId = userId;
            OTP = otp;
            IsValid = true;
            CreatedAt = DateTime.UtcNow;
            ValidTo = DateTime.UtcNow.AddMinutes(10);
        }

        public ApplicationUserOtp()
        {

        }
    }
}
