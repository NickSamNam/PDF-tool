namespace Activation_server {
    /// <summary>
    /// Possible activation responses.
    /// </summary>
    public enum ActivationResponse {
        /// <summary>
        /// The product was activated successfully.
        /// </summary>
        Succesful,
        /// <summary>
        /// The product could not be activated, because the product key is invalid.
        /// </summary>
        InvalidProductKey,
        /// <summary>
        /// The product could not be activated, because the product has been activated with another device.
        /// </summary>
        ProductKeyTaken,
        /// <summary>
        /// The product could not be activated, because an error occured.
        /// </summary>
        Unsuccessful
    }
}