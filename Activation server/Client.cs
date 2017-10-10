using System;
using System.ComponentModel;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Activation_server {
    public class Client {
        private readonly TcpClient _client;
        private readonly SslStream _stream;
        private readonly DatabaseHandler _databaseHandler = new DatabaseHandler();

        /// <summary>
        /// Create a new activation client.
        /// </summary>
        /// <param name="client">The <see cref="TcpClient"/> via which the client is connected.</param>
        public Client(TcpClient client) {
            _client = client;
            _stream = new SslStream(_client.GetStream());
            try {
                _stream.AuthenticateAsServer(GetCertificate(), false, SslProtocols.Tls12, false);
            }
            catch (AuthenticationException) {
                Close();
            }
            catch (IOException) {
                Close();
            }
            catch (Win32Exception e) {
                if (e.NativeErrorCode == -2146893043) {
                    Console.WriteLine("Run server as admin.");
                }
                else {
                    throw;
                }
            }
        }

        public void Accept() {
            while (_client.Connected) {
                var sizeBuffer = new byte[4];
                _stream.Read(sizeBuffer, 0, sizeBuffer.Length);
                var messageLength = BitConverter.ToInt32(sizeBuffer, 0);
                var messageBuffer = new byte[messageLength];
                var messageBuilder = new StringBuilder();
                var read = 0;
                do {
                    read += _stream.Read(messageBuffer, 0, messageLength - read);
                    messageBuilder.Append(Encoding.ASCII.GetString(messageBuffer));
                } while (read < messageLength);
                try {
                    var request = JObject.Parse(messageBuilder.ToString());
                    switch (request["id"].ToObject<string>()) {
                        case "activation": {
                            var productKey = request["productKey"].ToObject<string>();
                            var hwid = request["hwid"].ToObject<string>();
                            var response = new JObject();
                            var activationResponse = _databaseHandler.ActivateProduct(productKey, hwid);
                            response.Add("activationResponse", (int) activationResponse);
                            if (activationResponse == ActivationResponse.Succesful) {
                                var signedKey = Activator.GenerateSignedKey(productKey, hwid);
                                response.Add("signedKey", signedKey);
                            }
                            Send(Encoding.ASCII.GetBytes(response.ToString()));
                        }
                            break;
                    }
                }
                catch (JsonReaderException) {
                    return;
                }
                catch (IOException) {
                    return;
                }
            }
        }

        public void Close() {
            _client.Close();
            _stream.Close();
        }

        /// <summary>
        /// Send the byte[] to the client.
        /// </summary>
        /// <param name="data">Data to be sent.</param>
        private void Send(byte[] data) {
            new Thread(() => {
                var prefix = BitConverter.GetBytes(data.Length);
                var buffer = new byte[prefix.Length + data.Length];
                Buffer.BlockCopy(prefix, 0, buffer, 0, prefix.Length);
                Buffer.BlockCopy(data, 0, buffer, prefix.Length, data.Length);
                _stream.Write(buffer, 0, buffer.Length);
            }).Start();
        }

        private X509Certificate2 GetCertificate() {
            var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);
            var cert = store.Certificates.Find(X509FindType.FindByThumbprint,
                "9D829EC37B64AE4E7D9858B66438B37711FFBCE2", true)[0];
            store.Close();
            return cert;
        }
    }
}