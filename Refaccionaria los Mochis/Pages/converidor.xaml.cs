namespace Refaccionaria_los_Mochis.Pages;


public partial class converidor : ContentPage
{
    public converidor()
    {
        InitializeComponent();
    }
    private void OnTextChanged(object sender, TextChangedEventArgs e)
    {
        ActualizarResultado();
    }

    private void BtnConvertir_Clicked(object sender, EventArgs e)
    {
        ActualizarResultado();
    }

    private void ActualizarResultado()
    {
        if (!string.IsNullOrEmpty(textBox1.Text) && !string.IsNullOrEmpty(textBox2.Text))
        {
            double precio = Convert.ToDouble(textBox1.Text);
            double porcentajeGanancia = Convert.ToDouble(textBox2.Text);

            double precioConIVA = precio * 1.16;
            double precioConIVAYGanancia = precioConIVA + (precioConIVA * (porcentajeGanancia / 100));

            lblResultado.Text = $"\nPrecio con IVA (16%) es de: {precioConIVA:C}\n\nEl Precio con IVA (16%) y la Ganancia es de: {precioConIVAYGanancia:C}";
        }
        else
        {
            lblResultado.Text = string.Empty;
        }
    }

    private void textBox2_TextChanged(object sender, TextChangedEventArgs e)
    {
        ActualizarResultado();

    }

    private void textBox1_TextChanged(object sender, TextChangedEventArgs e)
    {
        ActualizarResultado();

    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        App.Current.MainPage = new PrincipalPage();

    }
}