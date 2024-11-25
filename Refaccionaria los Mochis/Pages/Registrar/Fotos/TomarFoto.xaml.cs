using System.Net.Http;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Refaccionaria_los_Mochis.Generic;

namespace Refaccionaria_los_Mochis.Pages.Registrar;

public partial class TomarFoto : ContentPage
{
    private int idProducto;
    public TomarFoto(int idProducto)
    {
        InitializeComponent();
        this.idProducto = idProducto;
    }

    private async void OnTomarFotoClicked(object sender, EventArgs e)
    {
        var fotoBytes = await TomarFotoYConvertirABytesAsync();
        if (fotoBytes != null)
        {
            FotoImage.Source = ImageSource.FromStream(() => new MemoryStream(fotoBytes));

            // Convierte la imagen en Base64 y agrega a una lista
            List<string> base64Images = new List<string> { Convert.ToBase64String(fotoBytes) };

            // Envía la lista de imágenes a la Web API
            await EnviarImagenesAWepApiAsync(base64Images);
        }
    }

    private async Task<byte[]> TomarFotoYConvertirABytesAsync()
    {
        try
        {
            if (MediaPicker.Default.IsCaptureSupported)
            {
                var foto = await MediaPicker.Default.CapturePhotoAsync();
                if (foto != null)
                {
                    using var stream = await foto.OpenReadAsync();
                    using var memoryStream = new MemoryStream();
                    await stream.CopyToAsync(memoryStream);
                    return memoryStream.ToArray(); // Devuelve la imagen en formato byte[]
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al tomar la foto: {ex.Message}");
        }

        return null;
    }

    private async Task EnviarImagenesAWepApiAsync(List<string> listaImagenesBase64)
    {
        HttpClient client = new HttpClient();

        var url = $"http://" + IP.SERVIDOR + ":5210/Productos/GuardarProductoConImagenes";

        // Crear el JSON con la lista de imágenes Base64
        var json = new
        {
            listaImagenesBase64 = listaImagenesBase64,
            id= idProducto
        };

        // Serializar a JSON
        var jsonContent = JsonConvert.SerializeObject(json);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        // Enviar la solicitud POST
        HttpResponseMessage response = await client.PostAsync(url, content);
        if (response.IsSuccessStatusCode)
        {
            string responseBody = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<dynamic>(responseBody);

            await DisplayAlert("Éxito", "Imagen enviada con éxito", "OK");
        }
        else
        {
            await DisplayAlert("Error", $"Error al enviar: {response.ReasonPhrase}", "OK");
        }
    }


    private void btnRegresar_Clicked(object sender, EventArgs e)
    {
        App.Current.MainPage = new PrincipalPage();

    }
    //private async Task EnviarImagenesAWepApiAsync(byte[] imagenBytes)
    //{
    //    using var client = new HttpClient();
    //    var url = $"http://" + IP.SERVIDOR + ":5210/Productos/ProductosIA";

    //    // Crear el JSON con el arreglo de bytes de la imagen
    //    var json = new
    //    {
    //        imagenBytes = imagenBytes
    //    };

    //    // Serializar a JSON
    //    var jsonContent = JsonConvert.SerializeObject(json);
    //    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

    //    // Enviar la solicitud POST
    //    HttpResponseMessage response = await client.PostAsync(url, content);
    //    if (response.IsSuccessStatusCode)
    //    {
    //        string responseBody = await response.Content.ReadAsStringAsync();
    //        dynamic result = JsonConvert.DeserializeObject(responseBody);

    //        string mensaje = result.mensaje;
    //        await DisplayAlert("Éxito", mensaje, "OK");
    //    }
    //    else
    //    {
    //        await DisplayAlert("Error", $"Error al enviar: {response.ReasonPhrase}", "OK");
    //    }
    //}

}
