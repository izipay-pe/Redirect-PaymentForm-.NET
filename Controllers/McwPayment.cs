using System.Security.Cryptography;
using System.Text;
using Redirect_PaymentForm_.NET.Models;

namespace Redirect_PaymentForm_.NET.Services
{
    public class McwPayment
    {
        private readonly IConfiguration _configuration;

        public McwPayment(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Obtener datos para el formulario
        public Dictionary<string, string> GenerateFormData(PaymentRequest parameters)
        {
            // Obteniendo claves API
            var username = _configuration["ApiCredentials:USERNAME"];

            // Definir los parámetros vads_ y sus valores
            var formParams = new Dictionary<string, string>
            {
                ["vads_action_mode"] = "INTERACTIVE",
                ["vads_ctx_mode"] = "TEST",
                ["vads_page_action"] = "PAYMENT",
                ["vads_payment_config"] = "SINGLE",
                ["vads_url_success"] = "https://localhost:7079/result",
                ["vads_return_mode"] = "POST",
                ["vads_site_id"] = username,
                ["vads_cust_first_name"] = parameters.FirstName,
                ["vads_cust_last_name"] = parameters.LastName,
                ["vads_cust_email"] = parameters.Email,
                ["vads_cust_cell_phone"] = parameters.PhoneNumber,
                ["vads_cust_address"] = parameters.Address,
                ["vads_cust_country"] = parameters.Country,
                ["vads_cust_state"] = parameters.State,
                ["vads_cust_city"] = parameters.City,
                ["vads_cust_zip"] = parameters.ZipCode,
                ["vads_order_id"] = parameters.OrderId,
                ["vads_amount"] = Math.Round(parameters.Amount * 100.0, 0).ToString(),
                ["vads_currency"] = parameters.Currency,
                ["vads_cust_national_id"] = parameters.IdentityCode,
                ["vads_trans_date"] = DateTime.UtcNow.ToString("yyyyMMddHHmmss"),
                ["vads_trans_id"] = ((long)(DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond)).ToString("X").Right(6),
                ["vads_version"] = "V2",
                ["vads_redirect_success_timeout"] = "5"
            };

            // Calcula el signature con los datos del diccionario
            var signature = CalculateSignature(formParams);

            // Agrega el signature calulado al diccionario
            formParams.Add("signature", signature);

            // Retorna el diccionario
            return formParams;
        }

        // Método para calcular el signature
        public string CalculateSignature(Dictionary<string, string> parameters)
        {
            // Obtener la Key
            var key = _configuration["ApiCredentials:KEY"];

            // Ordena los parametros
            var orderedParams = parameters
                .Where(p => p.Key.StartsWith("vads_"))
                .OrderBy(p => p.Key)
                .ToList();

            // Agregar la key al final del contenido
            var contentSignature = string.Join("+", orderedParams.Select(p => p.Value)) + "+" + key;

            // Generar y retornar la firma 
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(contentSignature));
            return Convert.ToBase64String(hash);
        }

        public bool CheckSignature(IFormCollection form)
        {
            var parameters = form.ToDictionary(k => k.Key, v => v.Value.ToString());
            // Obtener el signature de la respuesta
            var receivedSignature = parameters["signature"];
            return receivedSignature == CalculateSignature(parameters);
        }
    }

    public static class StringExtensions
    {
        public static string Right(this string value, int length)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            return value.Length <= length ? value : value.Substring(value.Length - length);
        }
    }
}