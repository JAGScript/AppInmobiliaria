using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppInmobiliaria
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FotosPropiedad : ContentPage
    {
        private readonly HttpClient client = new HttpClient();

        private const string UrlFotos = "imagen.php";
        private ObservableCollection<AppInmobiliaria.Models.Imagen> _Imagenes;

        Models.Usuario userLogueado = new Models.Usuario();
        string idPropiedad = "0";
        private string Path = "";
        private string PathImagenes = "";

        public FotosPropiedad(Models.Usuario usuario, string propiedad, string url, string urlImagenes)
        {
            InitializeComponent();
            userLogueado = usuario;
            lblNomUser.Text = userLogueado.us_nombre;
            idPropiedad = propiedad;
            Path = url;
            PathImagenes = urlImagenes;
            CargarFotos();
        }

        private void btnVolver_Clicked(object sender, EventArgs e)
        {
            Volver();
        }

        private async void CargarFotos()
        {
            string fotoParam = "?IdPropiedad=" + idPropiedad;
            var content = await client.GetStringAsync(Path + UrlFotos + fotoParam);
            List<AppInmobiliaria.Models.Imagen> fotos = JsonConvert.DeserializeObject<List<AppInmobiliaria.Models.Imagen>>(content);

            if (fotos.Count > 0)
            {
                _Imagenes = new ObservableCollection<AppInmobiliaria.Models.Imagen>(fotos);
                var imagenes = fotos.FindAll(f => f.im_estado == 1);

                if (imagenes.Count > 0)
                {
                    foreach (var item in imagenes)
                    {
                        item.im_nombre = PathImagenes + item.im_nombre;
                    }

                    carouselFotos.ItemsSource = imagenes;
                }
                else
                {
                    await DisplayAlert("Alerta", "No existen fotos cargadas.", "Ok");
                    Volver();
                }
            }
            else
            {
                await DisplayAlert("Alerta", "No existen fotos cargadas.", "Ok");
                Volver();
            }
        }

        private async void Volver()
        {
            await Navigation.PushAsync(new DetallePropiedad(userLogueado, idPropiedad, Path, PathImagenes));
        }

        private async void btnCerrarSesion_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Login());
        }
    }
}