using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Activation_server {
    public static class Activator {
        /// <summary>
        /// Generate a signed activation key.
        /// </summary>
        /// <param name="productKey">The client's product key.</param>
        /// <param name="hwid">The client's hardware id.</param>
        /// <returns>Returns the signed activation key.</returns>
        public static string GenerateSignedKey(string productKey, string hwid) {
            var unsignedKey = productKey + " " + hwid;
            var signedKey = GetCsp().SignData(Encoding.ASCII.GetBytes(unsignedKey), HashAlgorithmName.SHA512);
            return Convert.ToBase64String(signedKey);
        }

        /// <summary>
        /// Get the crypto service provider of the product activation private certificate.
        /// </summary>
        /// <returns>the crypto service providor of the product activation private certificate.</returns>
        private static RSACryptoServiceProvider GetCsp() {
            var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);
            var cert = store.Certificates.Find(X509FindType.FindByThumbprint, "‎‎DC39F68D203AD6FEB0410FA1C24C1984F87EB259", true)[0];
            return (RSACryptoServiceProvider) cert.PrivateKey;
        }
    }
}