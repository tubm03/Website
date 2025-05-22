using System.Text;

namespace PetStoreProject.Helpers
{
    public static class GeneratePassword
    {
        public static string GenerateAutoPassword(int passwordLength)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            StringBuilder generatePassword = new StringBuilder(passwordLength);

            Random random = new Random();

            for (int i = 0; i < passwordLength; i++)
            {
                generatePassword.Append(chars[random.Next(chars.Length)]);
            }
            return generatePassword.ToString();
        }
    }
}
