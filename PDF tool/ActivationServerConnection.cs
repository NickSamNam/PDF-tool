using System;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace PDF_tool {
    public class ActivationServerConnection {
        private const string Host = "server.aftersoft.nl";
        private const int Port = 420;
        private readonly TcpClient _client;
        private readonly SslStream _stream;

        /// <summary>
        /// Establish a connection with the authentication server.
        /// </summary>
        /// <exception cref="AuthenticationException"></exception>
        public ActivationServerConnection() {
            try {
                _client = new TcpClient(Host, Port);
                _stream = new SslStream(_client.GetStream(), false, ValidateServerCertificate, null);
                _stream.AuthenticateAsClient("PdfToolTls", null, SslProtocols.Tls12, false);
            }
            catch (AuthenticationException) {
                Close();
                throw;
            }
        }

        /// <summary>
        /// Send a request to the activation server and receive data.
        /// </summary>
        /// <param name="request">The request to send.</param>
        /// <returns>Returns the response data.</returns>
        public async Task<byte[]> SendRequestAsync(byte[] request) {
            var prefix = BitConverter.GetBytes(request.Length);
            var buffer = new byte[prefix.Length + request.Length];
            Buffer.BlockCopy(prefix, 0, buffer, 0, prefix.Length);
            Buffer.BlockCopy(request, 0, buffer, prefix.Length, request.Length);
            await _stream.WriteAsync(buffer, 0, buffer.Length);

            var sizeBuffer = new byte[4];
            await _stream.ReadAsync(sizeBuffer, 0, sizeBuffer.Length);
            var messageLength = BitConverter.ToInt32(sizeBuffer, 0);
            var messageBuffer = new byte[messageLength];
            var read = 0;
            do {
                read += await _stream.ReadAsync(messageBuffer, 0, messageBuffer.Length);
            } while (read < messageLength);
            return messageBuffer;
        }

        /// <summary>
        /// Closes the network stream and tcp client.
        /// </summary>
        public void Close() {
            _stream.Close();
            _client.Close();
        }

        private bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain,
            SslPolicyErrors sslpolicyerrors) => sslpolicyerrors == SslPolicyErrors.None;
    }
}