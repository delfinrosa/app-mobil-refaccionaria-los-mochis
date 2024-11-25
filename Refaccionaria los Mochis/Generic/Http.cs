using CapaEntidad;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Refaccionaria_los_Mochis.Generic
{
    public class Http
    {
        public static async Task<(int IdGenerado, string Mensaje)> PostDataAsync(object data, string url)
        {
            try
            {
                HttpClient client = new HttpClient();

                // Serializar el objeto a JSON
                var jsonData = JsonConvert.SerializeObject(data);

                // Configurar el contenido de la solicitud como JSON
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                // Realizar la solicitud POST y obtener la respuesta
                HttpResponseMessage response = await client.PostAsync(url, content);

                // Verificar si la solicitud fue exitosa
                if (response.IsSuccessStatusCode)
                {
                    // Leer y deserializar la respuesta JSON
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<dynamic>(responseBody);

                    // Devolver el ID generado y un mensaje vacío
                    return (result.IdGenerado, result.mensaje);
                }
                else
                {
                    // Leer el mensaje de error en caso de fallo
                    string errorResponse = await response.Content.ReadAsStringAsync();

                    // Devolver un ID generado 0 y el mensaje de error
                    return (0, errorResponse);
                }
            }
            catch (Exception ex)
            {
                // Capturar y manejar excepciones
                Console.WriteLine("Excepción: " + ex.Message);

                // Devolver un ID generado 0 y el mensaje de la excepción
                return (0, ex.Message);
            }
        }

        public static async Task<(int IdGenerado, string Mensaje)> PostMarca(object data, string url)
        {
            try
            {
                HttpClient client = new HttpClient();

                // Serializar el objeto a JSON
                var jsonData = JsonConvert.SerializeObject(data);

                // Configurar el contenido de la solicitud como JSON
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                // Realizar la solicitud POST y obtener la respuesta
                HttpResponseMessage response = await client.PostAsync(url, content);

                // Verificar si la solicitud fue exitosa
                if (response.IsSuccessStatusCode)
                {
                    // Leer y deserializar la respuesta JSON
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<dynamic>(responseBody);

                    int idGenerado = result.Marca != null ? (int)result.Marca : 0;

                    string mensaje = result.Mensaje != null ? (string)result.Mensaje : string.Empty;

                    // Devolver el ID generado y el mensaje obtenido
                    return (idGenerado, mensaje);
                }
                else
                {
                    // Leer y deserializar el mensaje de error
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var errorResult = JsonConvert.DeserializeObject<dynamic>(responseBody);

                    // Manejar caso de error de solicitud
                    string errorMessage = errorResult?.Mensaje ?? "Error en la solicitud";
                    return (0, errorMessage);
                }
            }
            catch (Exception ex)
            {
                // Manejar excepciones
                return (0, ex.Message);
            }
        }



        public static async Task<int> Post<T>(string url, T obj)
        {
            try
            {
                using (var cliente = new HttpClient())
                {
                    cliente.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var json = JsonConvert.SerializeObject(obj);
                    var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                    var response = await cliente.PostAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        string cadena = await response.Content.ReadAsStringAsync();
                        var jsonResponse = JsonConvert.DeserializeObject<RespuestaApi>(cadena);
                        return jsonResponse.IdGenerado;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            catch (HttpRequestException)
            {
                return 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }


        public class RespuestaApi
        {
            public int IdGenerado { get; set; }
            public string Mensaje { get; set; }
            public bool Resultado { get; set; }
        }






        public static ImageSource Convertir(string value)
        {
            try
            {
                byte[] buffer = Convert.FromBase64String(value);
                return ImageSource.FromStream(() => new MemoryStream(buffer));
            }
            catch (Exception ex)
            {
                return ImageSource.FromFile("default_image.png"); // Puedes usar una imagen por defecto o manejar el error según tu necesidad.
            }
        }

        public static async Task<int> Delete(string url)
        {
            var cliente = new HttpClient();
            var response = await cliente.DeleteAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return int.Parse(result);
            }
            else
            {
                return 0;
            }
        }

        public static async Task<List<Producto>> GetAll(string url)
        {
            var cliente = new HttpClient();
            var response = await cliente.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var jsonObject = JObject.Parse(result);
                var productos = jsonObject["Productos"].ToObject<List<Producto>>();
                return productos;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {response.StatusCode}, Details: {errorContent}");
                return new List<Producto>();
            }
        }
        public static async Task<Producto> Get(string url)
        {
            try
            {
                var cliente = new HttpClient();
                var response = await cliente.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var jsonObject = JObject.Parse(result);
                    var producto = jsonObject["Productos"].ToObject<Producto>();

                    return producto;
                }
                else
                {
                    // Si la respuesta no es exitosa, imprimir detalles del error
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error: {response.StatusCode}, Details: {errorContent}");

                    // Devolver null en caso de error
                    return null;
                }
            }
            catch (Exception ex)
            {
                // Capturar excepciones y mostrar mensaje de error
                Console.WriteLine($"Exception: {ex.Message}");

                // Devolver null en caso de excepción
                return null;
            }
        }

        public static async Task<(List<Marca>, string mensaje)> GetMarca(string url)
        {

            var cliente = new HttpClient();
            var response = await cliente.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var jsonObject = JObject.Parse(result);

                var marca = jsonObject["Marca"] != null ? jsonObject["Marca"].ToObject<List<Marca>>() : null;

                string mensaje = jsonObject["Mensaje"]?.ToString() ?? string.Empty;

                // Devolver el ID generado y el mensaje obtenido
                return (marca, mensaje);
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {response.StatusCode}, Details: {errorContent}");
                return (null, "");
            }

        }
        public static async Task<(List<Linea>, string)> GetLinea(string url)
        {
            try
            {
                HttpClient cliente = new HttpClient();
                HttpResponseMessage response = await cliente.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    var jsonObject = JObject.Parse(result);

                    var lineas = jsonObject["Linea"]?.ToObject<List<Linea>>() ?? new List<Linea>(); // Retorna una lista vacía si 'lineas' es null
                    string mensaje = jsonObject["Mensaje"]?.ToString() ?? string.Empty;

                    return (lineas, mensaje);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error: {response.StatusCode}, Details: {errorContent}");
                    return (new List<Linea>(), $"Error: {response.StatusCode}, Details: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en la solicitud HTTP: {ex.Message}");
                return (new List<Linea>(), $"Error en la solicitud HTTP: {ex.Message}");
            }
        }

        public static async Task<List<Almacen>> GetAlmacen(string url)
        {

            var cliente = new HttpClient();
            var response = await cliente.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var jsonObject = JObject.Parse(result);
                var Almacen = jsonObject["Almacenes"].ToObject<List<Almacen>>();
                return Almacen;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {response.StatusCode}, Details: {errorContent}");
                return null;
            }
        }
        public static async Task<(List<AlmacenRack>, string)> GetAlmacenRack(string url)
        {
            var cliente = new HttpClient();
            var response = await cliente.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var jsonObject = JObject.Parse(result);

                var racks = jsonObject["Racks"] != null ? jsonObject["Racks"].ToObject<List<AlmacenRack>>() : new List<AlmacenRack>();
                string mensaje = jsonObject["Mensaje"]?.ToString() ?? "";

                return (racks, mensaje);
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {response.StatusCode}, Details: {errorContent}");
                return (null, "Error al obtener los datos del servidor.");
            }
        }

        public static async Task<List<AlmacenRackSeccion>> GetAlmacenRackSeccion(string url)
        {

            var cliente = new HttpClient();
            var response = await cliente.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var jsonObject = JObject.Parse(result);
                var AlmacenRackSeccion = jsonObject["Secciones"].ToObject<List<AlmacenRackSeccion>>();
                return AlmacenRackSeccion;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {response.StatusCode}, Details: {errorContent}");
                return null;
            }
        }
       
        public async Task<bool> ExisteProducto(string codigoBarras)
        {
            try
            {

                var url = $"http://" + IP.SERVIDOR + "192.168.10.103:5210/Productos/ExisteProducto?codigoBarras={codigoBarras}";

                var cliente = new HttpClient();
                var response = await cliente.GetAsync(url);
                // Verificar si la solicitud fue exitosa (código de estado HTTP 200)
                if (response.IsSuccessStatusCode)
                {
                    // Leer el contenido de la respuesta
                    var result = await response.Content.ReadAsStringAsync();

                    // Convertir el resultado a un booleano
                    bool existeProducto = Convert.ToBoolean(result);

                    return existeProducto;
                }
                else
                {
                    // Manejar el error de la solicitud HTTP
                    Console.WriteLine($"Error al verificar si existe el producto: {response.StatusCode}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                // Manejar el error de la solicitud HTTP
                Console.WriteLine($"Error al verificar si existe el producto: {ex.Message}");
                return false;
            }
        }


        public static async Task<string> GetString(string url)
        {
            try
            {
                var cliente = new HttpClient();
                var response = await cliente.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    return "";
                }
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static async Task<int> GetInt(string url)
        {
            try
            {
                var cliente = new HttpClient();
                var response = await cliente.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return int.Parse(result);
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception)
            {
                return 0;
            }
        }




        public static async Task<List<(string, int)>> GetAllLineasProductosESTATICO()
        {
            await Task.Delay(100); // Simula la llamada a una API
            return new List<(string, int)> { ("FRENOS", 10), ("MOTOR", 15) };
        }

        public static async Task<List<(string, int)>> GetSubOpcionesAsync(string lineaProducto)
        {
            await Task.Delay(100); // Simula la llamada a una API
            return lineaProducto switch
            {
                "FRENOS" => new List<(string, int)> { ("DISCO", 4), ("TAMBOR", 6) },
                "MOTOR" => new List<(string, int)> { ("PISTÓN", 5), ("ÁRBOL DE LEVAS", 8) },
                _ => new List<(string, int)>()
            };
        }

        public static async Task<List<(string, int)>> GetMarcasAsync(string subOpcion)
        {
            await Task.Delay(100); // Simula la llamada a una API
            return subOpcion switch
            {
                "DISCO" => new List<(string, int)> { ("EAGLE", 2), ("HOLLMAN", 2) },
                "TAMBOR" => new List<(string, int)> { ("BRAKE", 3), ("STOPPER", 3) },
                "PISTÓN" => new List<(string, int)> { ("FIRE", 2), ("TORQUE", 3) },
                "ÁRBOL DE LEVAS" => new List<(string, int)> { ("CAMPRO", 4), ("VALVEMAX", 4) },
                _ => new List<(string, int)>()
            };
        }

        public static async Task<List<string>> GetNoPartesAsync(string marca)
        {
            await Task.Delay(100); // Simula la llamada a una API
            return marca switch
            {
                "EAGLE" => new List<string> { "WDC-100", "WDC-101" },
                "HOLLMAN" => new List<string> { "WHL-200", "WHL-201" },
                "BRAKE" => new List<string> { "BRK-300", "BRK-301" },
                "STOPPER" => new List<string> { "STP-400", "STP-401" },
                "FIRE" => new List<string> { "FIR-500", "FIR-501" },
                "TORQUE" => new List<string> { "TRQ-600", "TRQ-601" },
                "CAMPRO" => new List<string> { "CMP-700", "CMP-701" },
                "VALVEMAX" => new List<string> { "VMX-800", "VMX-801" },
                _ => new List<string>()
            };
        }

        public static async Task<List<Tuple<string, int>>> GetLineasAsync(string url)
        {
            using (var cliente = new HttpClient())
            {
                var response = await cliente.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var jsonArray = JArray.Parse(result);
                    var lineas = new List<Tuple<string, int>>();
                    foreach (var item in jsonArray)
                    {
                        var descripcion = item["Item1"].ToString();
                        var cantidad = int.Parse(item["Item2"].ToString());
                        lineas.Add(new Tuple<string, int>(descripcion, cantidad));
                    }
                    return lineas;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error: {response.StatusCode}, Details: {errorContent}");
                    return null;
                }
            }
        }


        public static async Task<List<Tuple<string, int>>> GetDescripcionesAsync(string url)
        {
            using (var cliente = new HttpClient())
            {
                var response = await cliente.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var jsonArray = JArray.Parse(result);
                    var descripciones = new List<Tuple<string, int>>();
                    foreach (var item in jsonArray)
                    {
                        var descripcion = item["Item1"].ToString();
                        var cantidad = int.Parse(item["Item2"].ToString());
                        descripciones.Add(new Tuple<string, int>(descripcion, cantidad));
                    }
                    return descripciones;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error: {response.StatusCode}, Details: {errorContent}");
                    return null;
                }
            }
        }


        public static async Task<List<Tuple<string, int>>> GetMarcasAsyncReporte(string url)
        {
            using (var cliente = new HttpClient())
            {
                var response = await cliente.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var jsonArray = JArray.Parse(result);
                    var marcas = new List<Tuple<string, int>>();
                    foreach (var item in jsonArray)
                    {
                        var descripcion = item["Item1"].ToString();
                        var cantidad = int.Parse(item["Item2"].ToString());
                        marcas.Add(new Tuple<string, int>(descripcion, cantidad));
                    }
                    return marcas;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error: {response.StatusCode}, Details: {errorContent}");
                    return null;
                }
            }
        }


        public static async Task<List<string>> GetNumerosParteAsync(string url)
        {
            using (var cliente = new HttpClient())
            {
                var response = await cliente.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var jsonArray = JArray.Parse(result);
                    var numerosParte = new List<string>();
                    foreach (var item in jsonArray)
                    {
                        var numeroParte = item.ToString();
                        numerosParte.Add(numeroParte);
                    }
                    return numerosParte;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error: {response.StatusCode}, Details: {errorContent}");
                    return null;
                }
            }
        }



        public static async Task<Usuario> GetUsuario(string url)
        {
            try
            {
                using (var cliente = new HttpClient())
                {
                    var response = await cliente.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        var usuario = JsonConvert.DeserializeObject<Usuario>(result);
                        return usuario;
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Error: {response.StatusCode}, Details: {errorContent}");
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }



        public static async Task<List<string>> GetNumerosParteORDENADO(string url)
        {
            using (var cliente = new HttpClient())
            {
                try
                {
                    var response = await cliente.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        var jsonArray = JArray.Parse(result);
                        var numerosParte = new List<string>();

                        foreach (var item in jsonArray)
                        {
                            var numeroParte = item.ToString();
                            numerosParte.Add(numeroParte);
                        }

                        return numerosParte;
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Error: {response.StatusCode}, Details: {errorContent}");
                        return null; // Devuelve null o una lista vacía según tu manejo de errores
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error en la solicitud HTTP: {ex.Message}");
                    throw; // Puedes manejar el error aquí o propagarlo según tu flujo de aplicación
                }
            }
        }


        public static async Task<List<Producto>> GetProductosRecientesAsync(string url)
        {
            using (var cliente = new HttpClient())
            {
                var response = await cliente.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var jsonObject = JObject.Parse(result);
                    var productos = jsonObject["Productos"].ToObject<List<Producto>>();
                    return productos;


                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error: {response.StatusCode}, Details: {errorContent}");
                    return null;
                }
            }
        }
        public static async Task<List<Producto>> GetProductosRecientesusuario(string url)
        {
            using (var cliente = new HttpClient())
            {
                var response = await cliente.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var productos = JsonConvert.DeserializeObject<List<Producto>>(result);
                    return productos;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error: {response.StatusCode}, Details: {errorContent}");
                    return null;
                }
            }
        }


        public static async Task<(bool Exito, string Mensaje, bool Existe)> VerificarEquipoAsync(string url)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        var datos = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonResponse);

                        bool existe = datos.ContainsKey("Existe") && (bool)datos["Existe"];
                        string mensaje = datos.ContainsKey("Mensaje") ? datos["Mensaje"].ToString() : string.Empty;

                        return (true, mensaje, existe);
                    }
                    else
                    {
                        return (false, $"Error: {response.StatusCode}", false);
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message, false);
            }
        }

        public static async Task<(bool Exito, string Mensaje)> PostRegistrarMantenimiento(object obj, string url)
        {
            try
            {
                // Serializar el objeto a JSON
                var json = JsonConvert.SerializeObject(obj);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using (var httpClient = new HttpClient())
                {
                    // Enviar la solicitud POST
                    var response = await httpClient.PostAsync(url, content);

                    // Verificar si la solicitud fue exitosa
                    if (response.IsSuccessStatusCode)
                    {
                        // Leer la respuesta y deserializar el contenido JSON
                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        var datos = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonResponse);

                        // Obtener el mensaje de la respuesta
                        string mensaje = datos.ContainsKey("Mensaje") ? datos["Mensaje"].ToString() : string.Empty;

                        return (true, mensaje);
                    }
                    else
                    {
                        // Si la respuesta no fue exitosa, retornar el código de error
                        return (false, $"Error: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                // Capturar cualquier excepción y retornar el mensaje de error
                return (false, ex.Message);
            }
        }
        public static async Task<(bool exito, string mensaje)> EnviarDatosProductosSinCantidadAsync(object datos, string url)
        {
            try
            {
                // Convertir los datos a JSON
                var jsonData = JsonConvert.SerializeObject(datos);

                // Configurar el cliente HTTP
                using (var client = new HttpClient())
                {
                    var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                    // Enviar la solicitud POST
                    var response = await client.PostAsync(url, content);

                    // Verificar la respuesta
                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        dynamic jsonResponse = JsonConvert.DeserializeObject(responseContent);

                        bool exito = jsonResponse.Exito;
                        string mensaje = jsonResponse.Mensaje;

                        return (exito, mensaje);
                    }
                    else
                    {
                        return (false, "Error en la solicitud HTTP: " + response.ReasonPhrase);
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejar excepciones
                return (false, "Excepción: " + ex.Message);
            }
        }

        public static async Task<(bool exito, string mensaje)> EnviarDatosCompraDtlAsync(object datos, string url)
        {
            try
            {
                // Convertir los datos a JSON
                var jsonData = JsonConvert.SerializeObject(datos);

                // Configurar el cliente HTTP
                using (var client = new HttpClient())
                {
                    var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                    // Enviar la solicitud POST
                    var response = await client.PostAsync(url, content);

                    // Verificar la respuesta
                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        dynamic jsonResponse = JsonConvert.DeserializeObject(responseContent);

                        bool exito = jsonResponse.Exito;
                        string mensaje = jsonResponse.Mensaje;

                        return (exito, mensaje);
                    }
                    else
                    {
                        return (false, "Error en la solicitud HTTP: " + response.ReasonPhrase);
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejar excepciones
                return (false, "Excepción: " + ex.Message);
            }
        }
        public static async Task<List<CompraDtl>> GetAllCompras(string url)
        {
            var cliente = new HttpClient();
            var response = await cliente.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var compras = JsonConvert.DeserializeObject<List<CompraDtl>>(result);
                return compras;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {response.StatusCode}, Details: {errorContent}");
                return new List<CompraDtl>();
            }
        }


        //ReporteRack
        public static async Task<List<Tuple<string, int>>> GetReporteRack1Async(string url)
        {
            using (var cliente = new HttpClient())
            {
                var response = await cliente.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var jsonArray = JArray.Parse(result);
                    var reportesRack = new List<Tuple<string, int>>();

                    foreach (var item in jsonArray)
                    {

                        var descripcion = item["Item1"]?.ToString();  // Descripción
                        var cantidad = item["Item2"]?.ToObject<int>() ?? 0;  // Cantidad

                        if (!string.IsNullOrEmpty(descripcion))
                        {
                            reportesRack.Add(Tuple.Create(descripcion, cantidad));
                        }
                    }

                    return reportesRack;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error: {response.StatusCode}, Details: {errorContent}");
                    return null;
                }
            }
        }


        public static async Task<List<Tuple<string, int>>> GetReporteRackMarca(string url)
        {
            using (var cliente = new HttpClient())
            {
                var response = await cliente.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var jsonArray = JArray.Parse(result);
                    var reportesRack = new List<Tuple<string, int>>();

                    foreach (var item in jsonArray)
                    {

                        var descripcion = item["Item1"]?.ToString();  // Descripción
                        var cantidad = item["Item2"]?.ToObject<int>() ?? 0;  // Cantidad

                        if (!string.IsNullOrEmpty(descripcion))
                        {
                            reportesRack.Add(Tuple.Create(descripcion, cantidad));
                        }
                    }

                    return reportesRack;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error: {response.StatusCode}, Details: {errorContent}");
                    return null;
                }
            }
        }


        public static async Task<List<Tuple<string, int>>> GetMarcasAsyncReporteRack(string url)
        {
            using (var cliente = new HttpClient())
            {
                var response = await cliente.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var jsonArray = JArray.Parse(result);
                    var marcas = new List<Tuple<string, int>>();
                    foreach (var item in jsonArray)
                    {
                        var descripcion = item["Item1"].ToString();
                        var cantidad = int.Parse(item["Item2"].ToString());
                        marcas.Add(new Tuple<string, int>(descripcion, cantidad));
                    }
                    return marcas;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error: {response.StatusCode}, Details: {errorContent}");
                    return null;
                }
            }
        }
        public static async Task<List<Tuple<string, int>>> GeTNoParteReporteRack(string url)
        {
            using (var cliente = new HttpClient())
            {
                var response = await cliente.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var jsonArray = JArray.Parse(result);
                    var marcas = new List<Tuple<string, int>>();
                    foreach (var item in jsonArray)
                    {
                        var descripcion = item["Item1"].ToString();
                        var cantidad = int.Parse(item["Item2"].ToString());
                        marcas.Add(new Tuple<string, int>(descripcion, cantidad));
                    }
                    return marcas;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error: {response.StatusCode}, Details: {errorContent}");
                    return null;
                }
            }
        }
        public static async Task<(int TotalProductos, int CantidadBajaStock, int CantidadMedianaStock, int CantidadBienStock)> GetStockReporteResumen(string url)
        {
            using (var cliente = new HttpClient())
            {
                var response = await cliente.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var jsonObject = JObject.Parse(result);

                    int totalProductos = (int)jsonObject["TotalProductos"];
                    int cantidadBaja = (int)jsonObject["CantidadBajaStock"];
                    int cantidadMediana = (int)jsonObject["CantidadMedianaStock"];
                    int cantidadBien = (int)jsonObject["CantidadBienStock"];

                    return (totalProductos, cantidadBaja, cantidadMediana, cantidadBien);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error: {response.StatusCode}, Details: {errorContent}");
                    return (0, 0, 0, 0); // Valores predeterminados en caso de error
                }
            }
        }
        public static async Task<List<Tuple<string, string, string, string>>> GetReporteComprasAsync(string url)
        {
            using (var cliente = new HttpClient())
            {
                var response = await cliente.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var jsonArray = JArray.Parse(result);
                    var reporteCompras = new List<Tuple<string, string, string, string>>();

                    foreach (var item in jsonArray)
                    {
                        var idCompra = item["IdCompra"].ToString();
                        var cantidadFaltante = item["CantidadFaltante"].ToString();
                        var fechaEstimadaEntrega = item["FechaEstimadaEntrega"].ToString();
                        var estatusCompra = item["EstatusCompra"].ToString();

                        reporteCompras.Add(new Tuple<string, string, string, string>(
                            idCompra,
                            cantidadFaltante,
                            fechaEstimadaEntrega,
                            estatusCompra
                        ));
                    }

                    return reporteCompras;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error: {response.StatusCode}, Details: {errorContent}");
                    return null;
                }
            }
        }
        public static async Task<(int totalRegistros, List<Tuple<string, string, string, string, string>> datos)> GetStockReportePaginadoAsync(string url)
        {
            using (var cliente = new HttpClient())
            {
                try
                {

                    // Hacer la solicitud GET
                    var response = await cliente.GetAsync(url);

                    // Verificar si la respuesta es exitosa
                    if (response.IsSuccessStatusCode)
                    {
                        // Leer el contenido como JSON
                        var result = await response.Content.ReadAsStringAsync();

                        // Parsear el JSON
                        var jsonObject = JObject.Parse(result);

                        // Extraer el total de registros
                        int totalRegistros = jsonObject["TotalRegistros"].Value<int>();

                        // Extraer los datos
                        var jsonArray = jsonObject["Datos"] as JArray;

                        // Convertir los datos en una lista de tuplas
                        var datos = jsonArray.Select(item => Tuple.Create(
                            item["Item1"].ToString(),
                            item["Item2"].ToString(),
                            item["Item3"].ToString(),
                            item["Item4"].ToString(),
                            item["Item5"].ToString()
                        )).ToList();

                        return (totalRegistros, datos);
                    }
                    else
                    {
                        // Manejar el error si la solicitud no es exitosa
                        var errorContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Error: {response.StatusCode}, Details: {errorContent}");
                        return (0, new List<Tuple<string, string, string, string, string>>());
                    }
                }
                catch (Exception ex)
                {
                    // Manejar excepciones
                    Console.WriteLine($"Excepción: {ex.Message}");
                    return (0, new List<Tuple<string, string, string, string, string>>());
                }
            }
        }

        public static async Task<List<Tuple<string, int, int>>> GetNumerosParteRack(string url)
        {
            using (var cliente = new HttpClient())
            {
                var response = await cliente.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var jsonArray = JArray.Parse(result);
                    var numerosParte = new List<Tuple<string, int, int>>();

                    foreach (var item in jsonArray)
                    {
                        // Supone que item["Item1"] es el número de parte y item["Item2"] es el stock
                        var numeroParte = item["Item1"].ToString();
                        var stock = (int)item["Item2"];
                        var resultado = (int)item["Item3"];
                        numerosParte.Add(new Tuple<string, int,int>(numeroParte, stock, resultado));
                    }
                    return numerosParte;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error: {response.StatusCode}, Details: {errorContent}");
                    return null;
                }
            }
        }
        public static async Task<List<Tuple<string, string, int>>> GetDiasFaltantes(string url)
        {
            using (var cliente = new HttpClient())
            {
                var response = await cliente.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var jsonArray = JArray.Parse(result);
                    var numerosParte = new List<Tuple<string, string, int>>();

                    foreach (var item in jsonArray)
                    {
                        var IDEquipo = item["Item1"].ToString();
                        var FechaUltimoMantenimiento = item["Item2"].ToString();
                        var DiasFaltantes = (int)item["Item3"];
                        numerosParte.Add(new Tuple<string, string,int>(IDEquipo, FechaUltimoMantenimiento, DiasFaltantes));
                    }
                    return numerosParte;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error: {response.StatusCode}, Details: {errorContent}");
                    return null;
                }
            }
        }



        public static async Task<List<Tuple<bool,bool,string, string>>> GetDetalleEquipo(string url)
        {
            using (var cliente = new HttpClient())
            {
                var response = await cliente.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var jsonArray = JArray.Parse(result);
                    var Detalles = new List<Tuple<bool, bool,string, string>>();

                    foreach (var item in jsonArray)
                    {
                        var bool1 = (bool)item["Item1"];
                        var bool2 = (bool)item["Item2"];
                        var string1= item["Item3"].ToString();
                        var string2= item["Item4"].ToString();
                        Detalles.Add(new Tuple<bool, bool ,string, string>(bool1, bool2,string1, string2));
                    }
                    return Detalles;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error: {response.StatusCode}, Details: {errorContent}");
                    return null;
                }
            }
        }


        public static async Task<List<Producto>> VerificarCodigoBarras(string url)
        {
            var cliente = new HttpClient();
            var response = await cliente.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var jsonObject = JsonConvert.DeserializeObject<JObject>(result);
                var productoList = jsonObject["lista"].ToObject<List<Producto>>(); 
                return productoList;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {response.StatusCode}, Details: {errorContent}");
                return new List<Producto>();
            }
        }


        public static async Task<bool> RegistrarCodigoBarrasAsync(string url, int idProducto, bool crearCodigoBarras)
        {
            var cliente = new HttpClient();
            var parametros = new Dictionary<string, string>
    {
        { "idProducto", idProducto.ToString() },
        { "crearCodigoBarras", crearCodigoBarras.ToString() }
    };

            var content = new FormUrlEncodedContent(parametros);
            var response = await cliente.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var jsonObject = JsonConvert.DeserializeObject<JObject>(result);
                bool success = jsonObject["Success"].Value<bool>();

                return success; // Retorna true si el registro fue exitoso
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {response.StatusCode}, Details: {errorContent}");
                return false; // Retorna false si hubo un error
            }
        }

        public static async Task<List<Tuple<string, string, string, string, string>>> GetCompras(string url)
        {
            using (var cliente = new HttpClient())
            {
                var response = await cliente.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var jsonArray = JArray.Parse(result);
                    var compras = new List<Tuple<string, string, string, string, string>>();

                    foreach (var item in jsonArray)
                    {
                        var idCompra = item["Item1"].ToString();
                        var fechaCompra = item["Item2"].ToString();
                        var proveedor = item["Item3"].ToString();
                        var cantidadProductos = item["Item4"].ToString();
                        var total = item["Item5"].ToString();

                        compras.Add(new Tuple<string, string, string, string, string>(
                            idCompra, fechaCompra, proveedor, cantidadProductos, total));
                    }
                    return compras;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error: {response.StatusCode}, Details: {errorContent}");
                    return null;
                }
            }
        }


    }
}