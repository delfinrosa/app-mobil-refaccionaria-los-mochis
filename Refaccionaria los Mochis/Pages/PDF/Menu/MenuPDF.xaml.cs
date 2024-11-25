using CapaEntidad;
using Refaccionaria_los_Mochis.Pages.Reportes;
using Refaccionaria_los_Mochis.Pages.Registrar;
using Refaccionaria_los_Mochis.Pages.Mantenimiento;
using Refaccionaria_los_Mochis.Models;

namespace Refaccionaria_los_Mochis.Pages.PDF.Menu;
public partial class MenuPDF : ContentPage
{

    public List<MenuCLS> lista { get; set; }
    public MenuCLS oMenuCLS { get; set; }

    public MenuPDF()
    {
        InitializeComponent();


        lista = new List<MenuCLS>();

        lista.Add(new MenuCLS { idmenu = 1, nombreopcion = "Leer QR", nombreicono = Icono.IconoQR });
        lista.Add(new MenuCLS { idmenu = 2, nombreopcion = "Reportes", nombreicono = Icono.IconoTabla });
        //lista.Add(new MenuCLS { idmenu = 3, nombreopcion = "Leer Code 93", nombreicono = Icono.IconoHome });
        //lista.Add(new MenuCLS { idmenu = 4, nombreopcion = "Leer Code 128", nombreicono = Icono.IconoHome });
        //lista.Add(new MenuCLS { idmenu = 5, nombreopcion = "Productos", nombreicono = Icono.IconoHome });
        //lista.Add(new MenuCLS { idmenu = 6, nombreopcion = "Leer Productos", nombreicono = Icono.IconoHome });
        lista.Add(new MenuCLS { idmenu = 7, nombreopcion = "Insertar Productos", nombreicono = Icono.IconoPlusCuadrado });
        lista.Add(new MenuCLS { idmenu = 8, nombreopcion = "Ecribir Codigo de Barras", nombreicono = Icono.IconoPlusNote });
        lista.Add(new MenuCLS { idmenu = 11, nombreopcion = "Convertidor", nombreicono = Icono.IconoCalculadora });
        lista.Add(new MenuCLS { idmenu = 12, nombreopcion = "Wagner", nombreicono = Icono.IconoTabla });
        lista.Add(new MenuCLS { idmenu = 13, nombreopcion = "Reportes Mejorado", nombreicono = Icono.IconoTabla });
        lista.Add(new MenuCLS { idmenu = 14, nombreopcion = "Reportes Ultimos 10", nombreicono = Icono.Icono10Atras });
        lista.Add(new MenuCLS { idmenu = 15, nombreopcion = "Registrar Marca", nombreicono = Icono.IconoPlusCuadrado });
        lista.Add(new MenuCLS { idmenu = 16, nombreopcion = "Mantenimiento", nombreicono = Icono.IconoComputer });
        lista.Add(new MenuCLS { idmenu = 17, nombreopcion = "Reporte Rack", nombreicono = Icono.IconoQR });
        lista.Add(new MenuCLS { idmenu = 18, nombreopcion = "PDF Compras", nombreicono = Icono.IconoClipTexto });
        lista.Add(new MenuCLS { idmenu = 19, nombreopcion = "Fecha Mantenimiento", nombreicono = Icono.IconoCalendarioEvento });

        lista.Add(new MenuCLS { idmenu = 20, nombreopcion = "Codigo Barras", nombreicono = Icono.IconoTabla });

        lista.Add(new MenuCLS { idmenu = 21, nombreopcion = "Compras", nombreicono = Icono.IconoTabla });


        lista.Add(new MenuCLS { idmenu = 9, nombreopcion = "Login", nombreicono = Icono.IconoUser });
        lista.Add(new MenuCLS { idmenu = 10, nombreopcion = "Salir", nombreicono = Icono.IconoBackDoor });




        BindingContext = this;


    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        string userId = await SecureStorage.GetAsync("UserId");

        if (!string.IsNullOrEmpty(userId))
        {
            int idUsuario = int.Parse(userId);
            if (idUsuario <= 0)
            {
                App.Current.MainPage = new login();
            }
        }
        else
        {
            App.Current.MainPage = new login();

        }
        string nombre = await SecureStorage.GetAsync("UserNombre");
        NombreCuentas.Text = "Cuenta: " + nombre;
    }

    private void lstMenu_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        if (oMenuCLS.idmenu == 1)
        {
            App.Current.MainPage = new ScanQR();

        }
        else if (oMenuCLS.idmenu == 2)
        {
            App.Current.MainPage = new Code39();
        }
        else if (oMenuCLS.idmenu == 3)
        {
            App.Current.MainPage = new Code93();

        }
        else if (oMenuCLS.idmenu == 4)
        {
            App.Current.MainPage = new Code128();

        }
        else if (oMenuCLS.idmenu == 5)
        {
            App.Current.MainPage = new ProductosPage();

        }
        else if (oMenuCLS.idmenu == 6)
        {
            App.Current.MainPage = new LeerProducto();

        }
        else if (oMenuCLS.idmenu == 7)
        {
            App.Current.MainPage = new LeerProductoInsert();

        }
        else if (oMenuCLS.idmenu == 8)
        {
            App.Current.MainPage = new EntryChange();

        }
        else if (oMenuCLS.idmenu == 9)
        {
            App.Current.MainPage = new login();

        }
        else if (oMenuCLS.idmenu == 10)
        {

            SecureStorage.SetAsync("UserId", "0");

            App.Current.MainPage = new login();
        }
        else if (oMenuCLS.idmenu == 11)
        {
            App.Current.MainPage = new converidor();

        }
        else if (oMenuCLS.idmenu == 12)
        {
            App.Current.MainPage = new ReporteWagner();

        }
        else if (oMenuCLS.idmenu == 13)
        {
            App.Current.MainPage = new ReporteNoParteBoton();

        }
        else if (oMenuCLS.idmenu == 14)
        {
            App.Current.MainPage = new ReporteUltimosRegistros();

        }
        else if (oMenuCLS.idmenu == 15)
        {
            App.Current.MainPage = new RegistrarMarca();

        }
        else if (oMenuCLS.idmenu == 16)
        {
            App.Current.MainPage = new LectorCodigoManteniemiento();

        }
        else if (oMenuCLS.idmenu == 17)
        {
            App.Current.MainPage = new LectorCodigoRack();

        }
        else if (oMenuCLS.idmenu == 18)
        {
            App.Current.MainPage = new ReporteCompraPDF();

        }
        else if (oMenuCLS.idmenu == 19)
        {
            App.Current.MainPage = new DiasFaltantes();

        }
        else if (oMenuCLS.idmenu == 20)
        {
            App.Current.MainPage = new VerificarCodigoBarras();

        }
        else if (oMenuCLS.idmenu == 21)
        {
            App.Current.MainPage = new PrincipalMenuPDF();

        }
    }
}