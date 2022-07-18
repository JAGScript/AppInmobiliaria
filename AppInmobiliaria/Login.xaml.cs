using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Security.Cryptography;
using System.Net.Http;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace AppInmobiliaria
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Login : ContentPage
    {
        private const string Url = "http://192.168.1.3/RBInmobiliaria/api/";
        private const string UrlImagenes = "http://192.168.1.3/RBInmobiliaria/subpages/propiedad/fotos/";
        private const string UrlUsuario = Url + "usuarios.php";
        private readonly HttpClient client = new HttpClient();
        private ObservableCollection<AppInmobiliaria.Models.Usuario> _listaUsuarios;

        public Login()
        {
            InitializeComponent();
        }

        private async void btnIngresar_Clicked(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string pass = txtPass.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(pass))
            {
                await DisplayAlert("Alerta", "Existen campos vacios, llenalos para continuar", "Ok");
            }
            else
            {
                string passEncode = Encrypt.GetMD5(pass);
                string cadenaParams = "?UserName=" + username + "&Pass=" + passEncode;
                var content = await client.GetStringAsync(UrlUsuario + cadenaParams);
                List<AppInmobiliaria.Models.Usuario> posts = JsonConvert.DeserializeObject<List<AppInmobiliaria.Models.Usuario>>(content);
                _listaUsuarios = new ObservableCollection<AppInmobiliaria.Models.Usuario>(posts);

                if (_listaUsuarios.Count == 1)
                {
                    var usuario = _listaUsuarios.FirstOrDefault();

                    await DisplayAlert("Alerta", "Bienvenid@!" + usuario.us_nombre, "Ok");

                    await Navigation.PushAsync(new Propiedad(usuario, Url, UrlImagenes));
                }
                else
                {
                    await DisplayAlert("Alerta", "Los datos ingresados son incorrectos.", "Ok");
                }
            }
        }

        private async void btnRegistrarse_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Registro(Url));
        }

        public class Encrypt
        {
            public static string GetMD5(string str)
            {
                MD5 md5 = MD5CryptoServiceProvider.Create();
                ASCIIEncoding encoding = new ASCIIEncoding();
                byte[] stream = null;
                StringBuilder sb = new StringBuilder();
                stream = md5.ComputeHash(encoding.GetBytes(str));
                for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
                return sb.ToString();
            }
        }

        private async void btnRecuperar_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Recuperar(Url));
        }
    }
}