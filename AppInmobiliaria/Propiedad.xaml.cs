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
    public partial class Propiedad : ContentPage
    {
        private const string UrlPropiedad = "propiedad.php";
        private readonly HttpClient client = new HttpClient();
        private ObservableCollection<AppInmobiliaria.Models.Propiedad> _listaPropiedades;
        Models.Usuario userLogueado = new Models.Usuario();
        private string Path = "";
        private string PathImagenes = "";

        public Propiedad(Models.Usuario usuario, string url, string urlImagenes)
        {
            InitializeComponent();
            userLogueado = usuario;
            lblNomUser.Text = userLogueado.us_nombre;
            Path = url;
            PathImagenes = urlImagenes;
            CargarPropiedades();
        }

        private async void CargarPropiedades()
        {
            var content = await client.GetStringAsync(Path + UrlPropiedad);
            List<AppInmobiliaria.Models.Propiedad> propiedades = JsonConvert.DeserializeObject<List<AppInmobiliaria.Models.Propiedad>>(content);
            _listaPropiedades = new ObservableCollection<AppInmobiliaria.Models.Propiedad>(propiedades);

            if (_listaPropiedades.Count > 0)
            {
                foreach (var item in _listaPropiedades)
                {
                    item.pr_foto_principal = PathImagenes + item.pr_foto_principal;
                }

                listaPropiedades.ItemsSource = _listaPropiedades;
            }
        }

        private async void btnDetalle_Clicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var code = button.CommandParameter;

            await Navigation.PushAsync(new DetallePropiedad(userLogueado, code.ToString(), Path, PathImagenes));
        }
    }
}