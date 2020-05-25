using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace Photobox.Helpers {
    /// <summary>
    /// Hash and verify password
    /// </summary>
    public sealed class PasswordHelper {
        public byte Version => 1;
        public int Pbkdf2IterCount { get; } = 50000;
        public int Pbkdf2SubkeyLength { get; } = 256 / 8; // 256 bits
        public int SaltSize { get; } = 128 / 8; // 128 bits
        public HashAlgorithmName HashAlgorithmName { get; } = HashAlgorithmName.SHA256;

        /// <summary>
        /// Hash a password string
        /// </summary>
        /// <param name="password"></param>
        /// <returns>Hashed Base64String</returns>
        public string HashPassword (string password) {
            if (String.IsNullOrEmpty (password) || String.IsNullOrWhiteSpace (password))
                throw new ArgumentNullException (nameof (password));

            if (password.Contains (" "))
                throw new ArgumentException ("Spaces not allowed.");

            byte[] salt;
            byte[] bytes;
            using (var rfc2898DeriveBytes = new Rfc2898DeriveBytes (password, SaltSize, Pbkdf2IterCount, HashAlgorithmName)) {
                salt = rfc2898DeriveBytes.Salt;
                bytes = rfc2898DeriveBytes.GetBytes (Pbkdf2SubkeyLength);
            }

            var inArray = new byte[1 + SaltSize + Pbkdf2SubkeyLength];
            inArray[0] = Version;
            Buffer.BlockCopy (salt, 0, inArray, 1, SaltSize);
            Buffer.BlockCopy (bytes, 0, inArray, 1 + SaltSize, Pbkdf2SubkeyLength);

            return Convert.ToBase64String (inArray);
        }

        /// <summary>
        /// Verify a hashed password with a non-hashed one
        /// </summary>
        /// <param name="hashedPassword"></param>
        /// <param name="password"></param>
        /// <returns>PasswordVerificationResult.Success or Failure</returns>
        public PasswordVerificationResult VerifyHashedPassword (string hashedPassword, string password) {
            if (password == null)
                throw new ArgumentNullException (nameof (password));

            if (hashedPassword == null)
                return PasswordVerificationResult.Failed;

            byte[] numArray = Convert.FromBase64String (hashedPassword);
            if (numArray.Length < 1)
                return PasswordVerificationResult.Failed;

            byte version = numArray[0];
            if (version > Version)
                return PasswordVerificationResult.Failed;

            byte[] salt = new byte[SaltSize];
            Buffer.BlockCopy (numArray, 1, salt, 0, SaltSize);
            byte[] a = new byte[Pbkdf2SubkeyLength];
            Buffer.BlockCopy (numArray, 1 + SaltSize, a, 0, Pbkdf2SubkeyLength);
            byte[] bytes;
            using (var rfc2898DeriveBytes = new Rfc2898DeriveBytes (password, salt, Pbkdf2IterCount, HashAlgorithmName)) {
                bytes = rfc2898DeriveBytes.GetBytes (Pbkdf2SubkeyLength);
            }

            if (FixedTimeEquals (a, bytes))
                return PasswordVerificationResult.Success;

            return PasswordVerificationResult.Failed;
        }

        [MethodImpl (MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static bool FixedTimeEquals (byte[] left, byte[] right) {
            if (left.Length != right.Length) {
                return false;
            }

            int length = left.Length;
            int accum = 0;

            for (int i = 0; i < length; i++) {
                accum |= left[i] - right[i];
            }

            return accum == 0;
        }
    }

    /// <summary>
    /// Enum to determine if verification was a success or a failure
    /// </summary>
    public enum PasswordVerificationResult {
        Failed,
        Success,
    }
}