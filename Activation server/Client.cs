using System.Net.Security;
using System.Net.Sockets;

namespace Activation_server {
    public class Client {
        private readonly TcpClient _client;
        private readonly SslStream _stream;

        /// <summary>
        /// Create a new activation client.
        /// </summary>
        /// <param name="client">The <see cref="TcpClient"/> via which the client is connected.</param>
        public Client(TcpClient client) {
            _client = client;
            _stream = new SslStream(_client.GetStream());
        }

        /// <summary>
        /// Send the activation response to the client.
        /// </summary>
        /// <param name="response">Response of the activation.</param>
        private void SendActivationResponse(ActivationResponse response) {
//            TODO fill body
        }

        /// <summary>
        /// Send the signed key to the client.
        /// </summary>
        /// <param name="signedKey">The signed activation key.</param>
        private void SendSignedKey(string signedKey) {
//            TODO fill body
        }
    }
}