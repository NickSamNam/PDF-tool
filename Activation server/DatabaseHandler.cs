using System;
using MySql.Data.MySqlClient;

namespace Activation_server {
    public class DatabaseHandler {
        private readonly MySqlConnection _sqlConn;

        public DatabaseHandler() {
            _sqlConn = new MySqlConnection(
                "Server=localhost;Database=pdftool;Uid=activation server;Pwd=228eXEx3jaNuJAxiRiKA2Az1wA8o5I;");
        }

        /// <summary>
        /// Generates a unique product key and adds it to the database.
        /// </summary>
        /// <returns>Returns the generated product key.</returns>
        public string AddProductKey() {
            var key = Activator.GenerateKey();
            using (_sqlConn) {
                _sqlConn.Open();
                var cmd = new MySqlCommand("INSERT INTO `pdftool`.`license` (`product_key`) VALUES (@Key);") {
                    Connection = _sqlConn
                };
                cmd.Parameters.AddWithValue("@Key", key);
                try {
                    cmd.ExecuteNonQuery();
                }
                catch (MySqlException e) {
                    switch (e.Number) {
                        case 1062:
                            return AddProductKey();
                        default:
                            throw;
                    }
                }
            }
            return key;
        }

        /// <summary>
        /// Attempts to register the hardware id with the product key.
        /// </summary>
        /// <param name="productKey">The software's product key.</param>
        /// <param name="hwid">The device's hardware id.</param>
        /// <returns>Returns the response of the activation.</returns>
        public ActivationResponse ActivateProduct(string productKey, string hwid) {
            using (_sqlConn) {
                _sqlConn.Open();
                var keyCheckCmd =
                    new MySqlCommand("select count(product_key) from pdftool.license where product_key=@Key") {
                        Connection = _sqlConn
                    };
                keyCheckCmd.Parameters.AddWithValue("@Key", productKey);
                using (var reader = keyCheckCmd.ExecuteReader()) {
                    reader.Read();
                    if (int.Parse(reader[0].ToString()) == 0)
                        return ActivationResponse.InvalidProductKey;
                }

                var cmd =
                    new MySqlCommand("UPDATE `pdftool`.`license` SET `hwid`=@HWID WHERE `product_key`=@Key;") {
                        Connection = _sqlConn
                    };
                cmd.Parameters.AddWithValue("@HWID", hwid);
                cmd.Parameters.AddWithValue("@Key", productKey);
                try {
                    cmd.ExecuteNonQuery();
                }
                catch (MySqlException e) {
                    switch (e.Number) {
                        case 1644:
                            return ActivationResponse.ProductKeyTaken;
                    }
                }
            }
            return ActivationResponse.Succesful;
        }

        /// <summary>
        /// Attempts to deregister any hardware id with the product key.
        /// </summary>
        /// <param name="productKey">The software's product key.</param>
        /// <returns>Returns the response of the deactivation.</returns>
        public ActivationResponse DeactivateProduct(string productKey) {
            using (_sqlConn) {
                _sqlConn.Open();
                var keyCheckCmd =
                    new MySqlCommand("select count(product_key) from pdftool.license where product_key=@Key") {
                        Connection = _sqlConn
                    };
                keyCheckCmd.Parameters.AddWithValue("@Key", productKey);
                using (var reader = keyCheckCmd.ExecuteReader()) {
                    reader.Read();
                    if (int.Parse(reader[0].ToString()) == 0)
                        return ActivationResponse.InvalidProductKey;
                }

                var cmd =
                    new MySqlCommand("UPDATE `pdftool`.`license` SET `hwid`=null WHERE `product_key`=@Key;") {
                        Connection = _sqlConn
                    };
                cmd.Parameters.AddWithValue("@Key", productKey);
                try {
                    cmd.ExecuteNonQuery();
                }
                catch (MySqlException e) {
                    switch (e.Number) {
                        case 1644:
                            return ActivationResponse.ProductKeyTaken;
                    }
                }
            }
            return ActivationResponse.Succesful;
        }
    }
}