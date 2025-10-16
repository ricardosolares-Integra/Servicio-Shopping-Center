using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCenter.WebServices.Base.Utilidades
{

    public class Seguridad
    {
        string msg = string.Empty;

        //ASEGURARSE QUE SEAN DE 16 BYTES
        private const string key = "1&7EGR4SA9GU@T.-";
        private const string IV = "V3cT%RI4te8r@Sa9";

        /// <summary>
        /// Encripta una cadena con el método Rijndael
        /// </summary>
        /// <param name="Cadena">Cadena encriptada</param>
        /// <returns></returns>
        public string Encripta(string Cadena)
        {

            byte[] inputBytes = Encoding.ASCII.GetBytes(Cadena);
            byte[] keyBytes = Encoding.ASCII.GetBytes(key);
            byte[] ivBytes = Encoding.ASCII.GetBytes(IV);
            byte[] encripted;

            try
            {
                RijndaelManaged cripto = new RijndaelManaged();
                using (MemoryStream ms = new MemoryStream(inputBytes.Length))
                {
                    using (CryptoStream objCryptoStream = new CryptoStream(ms, cripto.CreateEncryptor(keyBytes, ivBytes), CryptoStreamMode.Write))
                    {
                        objCryptoStream.Write(inputBytes, 0, inputBytes.Length);
                        objCryptoStream.FlushFinalBlock();
                        objCryptoStream.Close();
                    }
                    encripted = ms.ToArray();
                }

            }
            catch
            {
                msg = "Encriptar - Error al intentar encriptar una cadena.";
                encripted = null;
            }

            return Convert.ToBase64String(encripted);
        }

        /// <summary>
        /// Desencripta una cadena encriptaca con el método Rijndael
        /// </summary>
        /// <param name="Cadena">Cadena desencriptada</param>
        /// <returns></returns>
        public string Desencripta(string Cadena)
        {
            byte[] inputBytes = Convert.FromBase64String(Cadena);
            byte[] keyBytes = Encoding.ASCII.GetBytes(key);
            byte[] ivBytes = Encoding.ASCII.GetBytes(IV);
            byte[] resultBytes = new byte[inputBytes.Length];
            string textoLimpio = String.Empty;

            try
            {
                RijndaelManaged cripto = new RijndaelManaged();
                using (MemoryStream ms = new MemoryStream(inputBytes))
                {
                    using (CryptoStream objCryptoStream = new CryptoStream(ms, cripto.CreateDecryptor(keyBytes, ivBytes), CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(objCryptoStream, true))
                        {
                            textoLimpio = sr.ReadToEnd();
                        }
                    }
                }
            }
            catch
            {
                msg = "Desencriptar - Error al intentar desencriptar una cadena.";

            }

            return textoLimpio;
        }
    }
}
