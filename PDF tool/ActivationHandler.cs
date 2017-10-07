using System;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PDF_tool.Properties;
using PDF_tool.Security;

namespace PDF_tool {
    public class ActivationHandler {
        /// <summary>
        /// Activate the software with the server asynchronously
        /// </summary>
        /// <param name="productKey">The product key to use for activation.</param>
        /// <returns>Returns the activation response sent by the server.</returns>
        public async Task<ActivationResponse> ActivateAsync(string productKey) {
            var hwid = FingerPrint.Value();

            var server = new ActivationServerConnection();
            var request = $@"{{""id"":""activation"",""productKey"":""{productKey}"",""hwid"":""{hwid}""";
            var response =
                JObject.Parse(
                    Encoding.ASCII.GetString(await server.SendRequestAsync(Encoding.ASCII.GetBytes(request))));
            var activationResponse = (ActivationResponse) response["activationResponse"].ToObject<int>();

            if (activationResponse == ActivationResponse.Succesful) {
                Settings.Default.ProductKey = productKey;
                Settings.Default.SignedKey = response["signedKey"].ToString();
            }

            return activationResponse;
        }

        /// <summary>
        /// Validate if the product has been activated.
        /// </summary>
        /// <returns>Returns true if the product has been activated activated.</returns>
        public bool Validate() {
            var unsignedKey = Encoding.ASCII.GetBytes(Settings.Default.ProductKey + " " + FingerPrint.Value());
            var signedKey = Convert.FromBase64String(Settings.Default.SignedKey);
            return GetCsp()
                .VerifyHash(SHA512.Create().ComputeHash(unsignedKey),
                    CryptoConfig.MapNameToOID("SHA512"), signedKey);
        }

        /// <summary>
        /// Get the crypto service provider of the product activation public certificate.
        /// </summary>
        /// <returns>the crypto service providor of the product activation public certificate.</returns>
        private RSACryptoServiceProvider GetCsp() {
            return (RSACryptoServiceProvider) new X509Certificate2(Resources.PDF_Tool_Activation_Client, string.Empty)
                .PublicKey.Key;
        }
    }

    /// <summary>
    /// Possible activation responses from the activation server.
    /// </summary>
    public enum ActivationResponse {
        /// <summary>
        /// The product was activated successfully by the server.
        /// </summary>
        Succesful,

        /// <summary>
        /// The product could not be activated by the server, because the product key is invalid.
        /// </summary>
        InvalidProductKey,

        /// <summary>
        /// The product could not be activated by the server, because the product has been activated with another device.
        /// </summary>
        ProductKeyTaken
    }

    #region https://www.codeproject.com/Articles/28678/Generating-Unique-Key-Finger-Print-for-a-Computer

    namespace Security {
        /// <summary>
        /// Generates a 16 byte Unique Identification code of a computer
        /// Example: 4876-8DB5-EE85-69D3-FE52-8CF7-395D-2EA9
        /// </summary>
        public class FingerPrint {
            private static string fingerPrint = string.Empty;

            public static string Value() {
                if (string.IsNullOrEmpty(fingerPrint)) {
                    fingerPrint = GetHash("CPU >> " + cpuId() + "\nBIOS >> " +
                                          biosId() + "\nBASE >> " + baseId()
                                          + "\nDISK >> " + diskId() + "\nVIDEO >> " +
                                          videoId() + "\nMAC >> " + macId()
                    );
                }
                return fingerPrint;
            }

            private static string GetHash(string s) {
                MD5 sec = new MD5CryptoServiceProvider();
                ASCIIEncoding enc = new ASCIIEncoding();
                byte[] bt = enc.GetBytes(s);
                return GetHexString(sec.ComputeHash(bt));
            }

            private static string GetHexString(byte[] bt) {
                string s = string.Empty;
                for (int i = 0; i < bt.Length; i++) {
                    byte b = bt[i];
                    int n, n1, n2;
                    n = (int) b;
                    n1 = n & 15;
                    n2 = (n >> 4) & 15;
                    if (n2 > 9)
                        s += ((char) (n2 - 10 + (int) 'A')).ToString();
                    else
                        s += n2.ToString();
                    if (n1 > 9)
                        s += ((char) (n1 - 10 + (int) 'A')).ToString();
                    else
                        s += n1.ToString();
                    if ((i + 1) != bt.Length && (i + 1) % 2 == 0) s += "-";
                }
                return s;
            }

            #region Original Device ID Getting Code

            //Return a hardware identifier
            private static string identifier
                (string wmiClass, string wmiProperty, string wmiMustBeTrue) {
                string result = "";
                System.Management.ManagementClass mc =
                    new System.Management.ManagementClass(wmiClass);
                System.Management.ManagementObjectCollection moc = mc.GetInstances();
                foreach (System.Management.ManagementObject mo in moc) {
                    if (mo[wmiMustBeTrue].ToString() == "True") {
                        //Only get the first one
                        if (result == "") {
                            try {
                                result = mo[wmiProperty].ToString();
                                break;
                            }
                            catch {
                            }
                        }
                    }
                }
                return result;
            }

            //Return a hardware identifier
            private static string identifier(string wmiClass, string wmiProperty) {
                string result = "";
                System.Management.ManagementClass mc =
                    new System.Management.ManagementClass(wmiClass);
                System.Management.ManagementObjectCollection moc = mc.GetInstances();
                foreach (System.Management.ManagementObject mo in moc) {
                    //Only get the first one
                    if (result == "") {
                        try {
                            result = mo[wmiProperty].ToString();
                            break;
                        }
                        catch {
                        }
                    }
                }
                return result;
            }

            private static string cpuId() {
                //Uses first CPU identifier available in order of preference
                //Don't get all identifiers, as it is very time consuming
                string retVal = identifier("Win32_Processor", "UniqueId");
                if (retVal == "") //If no UniqueID, use ProcessorID
                {
                    retVal = identifier("Win32_Processor", "ProcessorId");
                    if (retVal == "") //If no ProcessorId, use Name
                    {
                        retVal = identifier("Win32_Processor", "Name");
                        if (retVal == "") //If no Name, use Manufacturer
                        {
                            retVal = identifier("Win32_Processor", "Manufacturer");
                        }
                        //Add clock speed for extra security
                        retVal += identifier("Win32_Processor", "MaxClockSpeed");
                    }
                }
                return retVal;
            }

            //BIOS Identifier
            private static string biosId() {
                return identifier("Win32_BIOS", "Manufacturer")
                       + identifier("Win32_BIOS", "SMBIOSBIOSVersion")
                       + identifier("Win32_BIOS", "IdentificationCode")
                       + identifier("Win32_BIOS", "SerialNumber")
                       + identifier("Win32_BIOS", "ReleaseDate")
                       + identifier("Win32_BIOS", "Version");
            }

            //Main physical hard drive ID
            private static string diskId() {
                return identifier("Win32_DiskDrive", "Model")
                       + identifier("Win32_DiskDrive", "Manufacturer")
                       + identifier("Win32_DiskDrive", "Signature")
                       + identifier("Win32_DiskDrive", "TotalHeads");
            }

            //Motherboard ID
            private static string baseId() {
                return identifier("Win32_BaseBoard", "Model")
                       + identifier("Win32_BaseBoard", "Manufacturer")
                       + identifier("Win32_BaseBoard", "Name")
                       + identifier("Win32_BaseBoard", "SerialNumber");
            }

            //Primary video controller ID
            private static string videoId() {
                return identifier("Win32_VideoController", "DriverVersion")
                       + identifier("Win32_VideoController", "Name");
            }

            //First enabled network card ID
            private static string macId() {
                return identifier("Win32_NetworkAdapterConfiguration",
                    "MACAddress", "IPEnabled");
            }

            #endregion
        }
    }

    #endregion
}