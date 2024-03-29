﻿using System;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Activation_server {
    public static class Activator {
        /// <summary>
        /// Generate random key.
        /// </summary>
        /// <returns>Returns the generated key.</returns>
        public static string GenerateKey() {
            const string valid = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            var sb = new StringBuilder();
            using (var rng = new RNGCryptoServiceProvider()) {
                var uintBuffer = new byte[sizeof(uint)];
                var length = 25;
                while (length-- > 0) {
                    rng.GetBytes(uintBuffer);
                    var num = BitConverter.ToUInt32(uintBuffer, 0);
                    sb.Append(valid[(int) (num % (uint) valid.Length)]);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Generate a signed activation key.
        /// </summary>
        /// <param name="productKey">The client's product key.</param>
        /// <param name="hwid">The client's hardware id.</param>
        /// <returns>Returns the signed activation key.</returns>
        public static string GenerateSignedKey(string productKey, string hwid) {
            var unsignedKey = productKey + " " + hwid;
            var signedKey = GetCsp().SignData(Encoding.ASCII.GetBytes(unsignedKey), new SHA1CryptoServiceProvider());
            return Convert.ToBase64String(signedKey);
        }

        /// <summary>
        /// Get the crypto service provider of the product activation private certificate.
        /// </summary>
        /// <returns>the crypto service providor of the product activation private certificate.</returns>
        private static RSACryptoServiceProvider GetCsp() {
            var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);
            var cert = store.Certificates.Find(X509FindType.FindByThumbprint,
                "DC39F68D203AD6FEB0410FA1C24C1984F87EB259", true)[0];
            return (RSACryptoServiceProvider) cert.PrivateKey;
        }
    }
}