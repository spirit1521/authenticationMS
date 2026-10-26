using AuthMicroservice.Application.DTO;
using AuthMicroservice.Application.Interface;
using AuthMicroservice.Infrastructure.Data;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using OtpNet;
using QRCoder;

namespace AuthMicroservice.Infrastructure.Service
{
    public class TwoFactorService : ITwoFactorService
    {
        AppDbContext db;
        IDataProtector protector;

        public TwoFactorService(AppDbContext db, IDataProtectionProvider provider)
        {
            this.db = db;
            protector = provider.CreateProtector("AuthMicroservice.TwoFactor");
        }

        public async Task<TwoFactorSetupDto?> Setup(int userId)
        {
            var user = await db.Users.FindAsync(userId);
            if (user == null || user.Status != "Active" || user.IsTwoFactorEnabled)
            {
                return null;
            }

            if (string.IsNullOrEmpty(user.TwoFactorSecretKey))
            {
                string newSecret = Base32Encoding.ToString(KeyGeneration.GenerateRandomKey(20));
                user.TwoFactorSecretKey = protector.Protect(newSecret);
                user.LastTwoFactorTimeStep = null;
                await db.SaveChangesAsync();
            }

            string secret = protector.Unprotect(user.TwoFactorSecretKey);
            string name = Uri.EscapeDataString(user.Username);

            string uri = $"otpauth://totp/InsuranceApplication:{name}" + $"?secret={secret}&issuer=InsuranceApplication";

            using var generator = new QRCodeGenerator();
            using var data = generator.CreateQrCode(uri, QRCodeGenerator.ECCLevel.Q);
            using var qr = new PngByteQRCode(data);

            return new TwoFactorSetupDto
            {
                ManualEntryKey = secret,
                QrCode = "data:image/png;base64," +  Convert.ToBase64String(qr.GetGraphic(5))
            };
        }

        public async Task<bool> Enable(int userId, string code)
        {
            return await VerifyCode(userId, code, true);
        }

        public async Task<bool> VerifyCode(int userId, string code)
        {
            return await VerifyCode(userId, code, false);
        }

        private async Task<bool> VerifyCode(int userId, string code, bool enable)
        {
            var user = await db.Users.FindAsync(userId);

            if (user == null || user.Status != "Active" || string.IsNullOrEmpty(user.TwoFactorSecretKey) ||
                user.IsTwoFactorEnabled == enable || string.IsNullOrEmpty(code) ||
                code.Length != 6 || code.Any(x => x < '0' || x > '9'))
            {
                return false;
            }

            string secret = protector.Unprotect(user.TwoFactorSecretKey);
            var totp = new Totp(Base32Encoding.ToBytes(secret));

            if (!totp.VerifyTotp(code, out long step))
            {
                return false;
            }

            int updated = await db.Users
                .Where(x => x.UserId == userId && x.Status == "Active" && x.IsTwoFactorEnabled == !enable &&
                    x.TwoFactorSecretKey == user.TwoFactorSecretKey && (x.LastTwoFactorTimeStep == null || x.LastTwoFactorTimeStep < step))
                .ExecuteUpdateAsync(x => x.SetProperty(u => u.LastTwoFactorTimeStep, (long?)step).SetProperty(u => u.IsTwoFactorEnabled, true));

            return updated == 1;
        }
    }
}