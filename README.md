<p align="center">
  <img src="https://github.com/izipay-pe/Imagenes/blob/main/logos_izipay/logo-izipay-banner-1140x100.png?raw=true" alt="Formulario" width=100%/>
</p>

# Redirect-PaymentForm-.NET

## Índice

➡️ [1. Introducción](https://github.com/izipay-pe/Redirect-PaymentForm-.NET/tree/main?tab=readme-ov-file#%EF%B8%8F-1-introducci%C3%B3n)  
🔑 [2. Requisitos previos](https://github.com/izipay-pe/Redirect-PaymentForm-.NET/tree/main?tab=readme-ov-file#-2-requisitos-previos)  
🚀 [3. Ejecutar ejemplo](https://github.com/izipay-pe/Redirect-PaymentForm-.NET/tree/main?tab=readme-ov-file#-3-ejecutar-ejemplo)  
🔗 [4. Pasos de integración](https://github.com/izipay-pe/Redirect-PaymentForm-.NET/tree/main?tab=readme-ov-file#4-pasos-de-integraci%C3%B3n)  
💻 [4.1. Desplegar pasarela](https://github.com/izipay-pe/Redirect-PaymentForm-.NET/tree/main?tab=readme-ov-file#41-desplegar-pasarela)  
💳 [4.2. Analizar resultado de pago](https://github.com/izipay-pe/Redirect-PaymentForm-.NET/tree/main?tab=readme-ov-file#42-analizar-resultado-del-pago)  
📡 [4.3. Pase a producción](https://github.com/izipay-pe/Redirect-PaymentForm-.NET/tree/main?tab=readme-ov-file#43pase-a-producci%C3%B3n)  
🎨 [5. Personalización](https://github.com/izipay-pe/Redirect-PaymentForm-.NET/tree/main?tab=readme-ov-file#-5-personalizaci%C3%B3n)  
📚 [6. Consideraciones](https://github.com/izipay-pe/Redirect-PaymentForm-.NET/tree/main?tab=readme-ov-file#-6-consideraciones)

## ➡️ 1. Introducción

En este manual podrás encontrar una guía paso a paso para configurar un proyecto de **[Springboot]** con la pasarela de pagos de IZIPAY. Te proporcionaremos instrucciones detalladas y credenciales de prueba para la instalación y configuración del proyecto, permitiéndote trabajar y experimentar de manera segura en tu propio entorno local.
Este manual está diseñado para ayudarte a comprender el flujo de la integración de la pasarela para ayudarte a aprovechar al máximo tu proyecto y facilitar tu experiencia de desarrollo.


<p align="center">
  <img src="https://github.com/izipay-pe/Imagenes/blob/main/formulario_redireccion/Imagen-Formulario-Redireccion.png?raw=true" alt="Formulario" width="750"/>
</p>

## 🔑 2. Requisitos Previos

- Comprender el flujo de comunicación de la pasarela. [Información Aquí](https://secure.micuentaweb.pe/doc/es-PE/rest/V4.0/javascript/guide/start.html)
- Extraer credenciales del Back Office Vendedor. [Guía Aquí](https://github.com/izipay-pe/obtener-credenciales-de-conexion)
- Para este proyecto utilizamos .NET 8.0.
- Visual Studio

> [!NOTE]
> Tener en cuenta que, para que el desarrollo de tu proyecto, eres libre de emplear tus herramientas preferidas.

## 🚀 3. Ejecutar ejemplo

### Instalar Visual Studio

Visual Studio IDE compatible con .NET.

1. Dirigirse a la página web de [Microsoft](https://visualstudio.microsoft.com/es/).
2. Descargarlo e instalarlo.

### Clonar el proyecto
```sh
git clone https://github.com/izipay-pe/Embedded-PaymentForm-.NET
``` 

### Datos de conexión 

Reemplace **[CHANGE_ME]** con sus credenciales de `API REST` extraídas desde el Back Office Vendedor, revisar [Requisitos previos](https://github.com/izipay-pe/Embedded-PaymentForm-.NET/tree/main?tab=readme-ov-file#-2-requisitos-previos).

- Editar el archivo `appsettings.json` en la ruta raíz:
```json
"ApiCredentials": {
        "USERNAME": "CHANGE_ME_USER_ID",
        "KEY": "CHANGE_ME_KEY"
    }
```

### Ejecutar proyecto

1. Una vez dentro del código ejecutamos el proyecto con el comando F5 y se abrirá en tu navegador predeterminado accediendo a la siguiente ruta:

```sh
https://localhost:7079/
``` 

## 🔗4. Pasos de integración

<p align="center">
  <img src="https://i.postimg.cc/pT6SRjxZ/3-pasos.png" alt="Formulario" />
</p>

## 💻4.1. Desplegar pasarela
### Autentificación
Extraer las claves de `identificador de tienda` y `clave de test o producción` del Backoffice Vendedor y agregarlo en los parámetros `vads_site_id` y en el método `CalculateSignature()`. Este último permite calcular la firma transmitida de los datos de pago. Podrás encontrarlo en el archivo `Controllers/McwPayment.cs`.
```c#
public Dictionary<string, string> GenerateFormData(PaymentRequest parameters)
        {
            // Obteniendo claves API
            var username = _configuration["ApiCredentials:USERNAME"];

            // Definir los parámetros vads_ y sus valores
            var formParams = new Dictionary<string, string>
            {
                ["vads_site_id"] = username,
                ...
                ...
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
            ...
            ...
            return Convert.ToBase64String(hash);
        }
```

ℹ️ Para más información: [Autentificación](https://secure.micuentaweb.pe/doc/es-PE/form-payment/quick-start-guide/identificarse-durante-los-intercambios.html)
### Visualizar formulario
Para desplegar la pasarela, crea un formulario **HTML** de tipo **POST** con el valor del **ACTION** con la url de servidor de la pasarela de pago y agregale los parámetros de pago como etiquetas `<input type="hidden" name="..." value="@Model[...]" />`. Como se muestra el ejemplo en la ruta del archivo `Views/Mcw/Checkout.cshtml` 

```html
   <form class="from-checkout" action="https://secure.micuentaweb.pe/vads-payment/" method="post">
        <!-- Inputs generados dinámicamente -->
        <input type="hidden" name="vads_action_mode" value="@Model["vads_action_mode"]" />
        ...
        ...
        <input type="hidden" name="signature" value="@Model["signature"]" />

        <button class="btn btn-checkout" type="submit" name="pagar">Pagar</button>
   </form>
```
ℹ️ Para más información: [Formulario de pago en POST](https://secure.micuentaweb.pe/doc/es-PE/form-payment/quick-start-guide/enviar-un-formulario-de-pago-en-post.html)

## 💳4.2. Analizar resultado del pago

### Validación de firma
Se configura el método `CalculateSignature()` que generará la firma de los datos de la respuesta de pago y el método `CheckSignature()` que se encargara de validar la firma. Podrás encontrarlo en el archivo `Controllers/McwPayment.cs`.

```c#
        public string CalculateSignature(Dictionary<string, string> parameters)
        {
            ...
            ...
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
```

Se valida que la firma recibida es correcta en el archivo `Controllers/McwController.cs`.

```c#
         // @@ Manejo de solicitudes POST para result @@
        [HttpPost("/result")]
        public IActionResult Result([FromForm] IFormCollection form)
        {
            ...
            ...
            // Válida que la respuesta sea íntegra comparando el signature recibido con el generado
            if (!_mcwPayment.CheckSignature(form))
                throw new Exception("Invalid signature");
            ...
            ...
            // Renderiza el template
            return View("Result", formData);
        }
```
En caso que la validación sea exitosa, se renderiza el template con los valores. Como se muestra en el archivo `Mcw/Result.cshtml`.

```html
<p><strong>Estado:</strong> <span>@Model.Parameters["vads_trans_status"]</span></p>
<p><strong>Monto:</strong> <span>@currency</span><span>@amount</span></p>
<p><strong>Order-id:</strong> <span>@Model.Parameters["vads_order_id"]</span></p>
```
ℹ️ Para más información: [Analizar resultado del pago](https://secure.micuentaweb.pe/doc/es-PE/form-payment/quick-start-guide/recuperar-los-datos-devueltos-en-la-respuesta.html)

### IPN
La IPN es una notificación de servidor a servidor (servidor de Izipay hacia el servidor del comercio) que facilita información en tiempo real y de manera automática cuando se produce un evento, por ejemplo, al registrar una transacción.

Se realiza la verificación de la firma y se retorna la respuesta del estado del pago. Podrás encontrarlo en el archivo `Controllers/McwController.cs`.

```c#
        // @@ Manejo de solicitudes POST para la ipn @@
        [HttpPost("/ipn")]
        public IActionResult Ipn([FromForm] IFormCollection form)
        {
            ...
            ...
            // Válida que la respuesta sea íntegra comparando el signature recibido con el generado
            if (!_mcwPayment.CheckSignature(form))
                throw new Exception("Invalid signature");

            // Almacena algunos datos de la respuesta IPN en variables
            var orderStatus = form["vads_trans_status"].ToString();

            // Retorna en la respuesta el Order Status
            return Ok($"OK! OrderStatus is {orderStatus}");
        }
```

La IPN debe ir configurada en el Backoffice Vendedor, en `Configuración -> Reglas de notificación -> URL de notificación al final del pago`

<p align="center">
  <img src="https://github.com/izipay-pe/Imagenes/blob/main/formulario_redireccion/Url-Notificacion-Redireccion.png?raw=true" alt="Url de notificacion en redireccion" width="650" />
</p>

ℹ️ Para más información: [Analizar IPN](https://secure.micuentaweb.pe/doc/es-PE/form-payment/quick-start-guide/implementar-la-ipn.html)

## 5. Transacción de prueba

Antes de poner en marcha su pasarela de pago en un entorno de producción, es esencial realizar pruebas para garantizar su correcto funcionamiento. 

Puede intentar realizar una transacción utilizando una tarjeta de prueba (en la parte inferior del formulario).

<p align="center">
  <img src="https://github.com/izipay-pe/Imagenes/blob/main/formulario_redireccion/Imagen-Formulario-Redireccion-testcard.png?raw=true" alt="Tarjetas de prueba" width="450"/>
</p>

- También puede encontrar tarjetas de prueba en el siguiente enlace. [Tarjetas de prueba](https://secure.micuentaweb.pe/doc/es-PE/rest/V4.0/api/kb/test_cards.html)

## 📡4.3.Pase a producción

Reemplace **[CHANGE_ME]** con sus credenciales de PRODUCCIÓN extraídas desde el Back Office Vendedor, revisar [Requisitos Previos](https://github.com/izipay-pe/Redirect-PaymentForm-.NET/tree/main?tab=readme-ov-file#-2-requisitos-previos).

- Editar el archivo `appsettings.json` en la ruta raíz:
```json
"ApiCredentials": {
        "USERNAME": "CHANGE_ME_USER_ID",
        "KEY": "CHANGE_ME_KEY"
    }
```

## 🎨 5. Personalización

Si deseas aplicar cambios específicos en la apariencia de la página de pago, puedes lograrlo mediante las opciones de personalización en el Backoffice. En este enlace [Personalización - Página de pago](https://youtu.be/hy877zTjpS0?si=TgSeoqw7qiaQDV25) podrá encontrar un video para guiarlo en la personalización.

<p align="center">
  <img src="https://github.com/izipay-pe/Imagenes/blob/main/formulario_redireccion/Personalizacion-formulario-redireccion.png?raw=true" alt="Personalizacion de formulario en redireccion"  width="750" />
</p>

## 📚 6. Consideraciones

Para obtener más información, echa un vistazo a:

- [Formulario incrustado: prueba rápida](https://secure.micuentaweb.pe/doc/es-PE/rest/V4.0/javascript/quick_start_js.html)
- [Primeros pasos: pago simple](https://secure.micuentaweb.pe/doc/es-PE/rest/V4.0/javascript/guide/start.html)
- [Servicios web - referencia de la API REST](https://secure.micuentaweb.pe/doc/es-PE/rest/V4.0/api/reference.html)
