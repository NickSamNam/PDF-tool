namespace Activation_server {
    public static class DatabaseHandler {
        /// <summary>
        /// Generates a unique product key and adds it to the database.
        /// </summary>
        /// <returns>Returns the generated product key.</returns>
        public static string AddProductKey() {
//            TODO replace dummy code
            return "";
        }

        /// <summary>
        /// Attempts to register the hardware id with the product key.
        /// </summary>
        /// <param name="productKey">The software's product key.</param>
        /// <param name="hwid">The device's hardware id.</param>
        /// <returns>Returns the response of the activation.</returns>
        public static ActivationResponse ActivateProduct(string productKey, string hwid) {
//            TODO replace dummy code
            return ActivationResponse.Succesful;
        }

        /// <summary>
        /// Attempts to deregister any hardware id with the product key.
        /// </summary>
        /// <param name="productKey">The software's product key.</param>
        /// <returns>Returns the response of the deactivation.</returns>
        public static ActivationResponse DeactivateProduct(string productKey) {
//            TODO replace dummy code
            return ActivationResponse.Succesful;
        }
    }
}