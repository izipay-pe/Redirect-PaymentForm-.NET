using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Redirect_PaymentForm_.NET.Services;
using Redirect_PaymentForm_.NET.Models;

namespace Redirect_PaymentForm_.NET.Controllers
{
    public class McwController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly McwPayment _mcwPayment;

        public McwController(IConfiguration configuration)
        {
            _configuration = configuration;
            _mcwPayment = new McwPayment(configuration);
        }

        // @@ Manejo de solicitudes GET para la ruta raíz @@
        [HttpGet]
        public IActionResult Index()
        {
            string orderId = "Order-" + DateTime.Now.ToString("yyyyMMddHHmmss");
            ViewBag.OrderId = orderId;
            return View();
        }

        // @@ Manejo de solicitudes POST para checkout @@
        [HttpPost("/checkout")]
        public IActionResult Checkout([FromForm] PaymentRequest paymentRequest)
        {
            try
            {
                // Calcular el Signature y los valores dinámicos para el formulario
                var formParams = _mcwPayment.GenerateFormData(paymentRequest);
                // Renderiza el template
                return View("Checkout", formParams);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // @@ Manejo de solicitudes POST para result @@
        [HttpPost("/result")]
        public IActionResult Result([FromForm] IFormCollection form)
        {
            if (!form.Any())
                throw new Exception("No post data received!");

            // Válida que la respuesta sea íntegra comparando el signature recibido con el generado
            if (!_mcwPayment.CheckSignature(form))
                throw new Exception("Invalid signature");

            var formData = new PaymentResponse
            {
                // Convierte los datos del formulario en un diccionario de clave-valor
                Parameters = form.ToDictionary(k => k.Key, v => v.Value.ToString()),
                // Convierte los datos a un JSON con indentación legible
                PrettyJson = JsonSerializer.Serialize(
                    form.ToDictionary(k => k.Key, v => v.Value.ToString()),
                    new JsonSerializerOptions { WriteIndented = true }
                )
            };
            // Renderiza el template
            return View("Result", formData);
        }

        // @@ Manejo de solicitudes POST para la ipn @@
        [HttpPost("/ipn")]
        public IActionResult Ipn([FromForm] IFormCollection form)
        {
            if (!form.Any())
                throw new Exception("no post data received!");

            // Válida que la respuesta sea íntegra comparando el signature recibido con el generado
            if (!_mcwPayment.CheckSignature(form))
                throw new Exception("Invalid signature");

            // Almacena algunos datos de la respuesta IPN en variables
            var orderStatus = form["vads_trans_status"].ToString();
            var orderId = form["vads_order_id"].ToString();
            var uuid = form["vads_trans_uuid"].ToString();

            // Retorna en la respuesta el Order Status
            return Ok($"OK! OrderStatus is {orderStatus}");
        }
    }
}