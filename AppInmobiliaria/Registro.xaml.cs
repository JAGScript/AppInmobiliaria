using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static AppInmobiliaria.Login;

namespace AppInmobiliaria
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Registro : ContentPage
    {
        private readonly HttpClient client = new HttpClient();
        private const string Url = "http://192.168.1.3/RBInmobiliaria/api/usuarios.php";
        private ObservableCollection<AppInmobiliaria.Models.Usuario> _post;

        private const string UrlPersona = "http://192.168.1.3/RBInmobiliaria/api/persona.php";

        public Registro()
        {
            InitializeComponent();
        }

        private async void btnGuardar_Clicked(object sender, EventArgs e)
        {
            string nombre = txtNombre.Text;
            string identificacion = txtIdentificacion.Text;
            string correo = txtCorreo.Text;
            string direccion = txtDireccion.Text;
            string telefono = txtTelefono.Text;
            string username = txtLogin.Text;
            string pass = txtPass.Text;

            if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(identificacion) || string.IsNullOrEmpty(correo)
                || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(pass) || string.IsNullOrEmpty(direccion) || string.IsNullOrEmpty(telefono))
            {
                await DisplayAlert("Alerta", "Existen campos vacios, llenalos para continuar", "Ok");
            }
            else
            {
                string cadenaParams = "?UserName=" + username + "&IdentificacionUsuario=" + identificacion + "&CorreoUsuario=" + correo;
                var content = await client.GetStringAsync(Url + cadenaParams);
                List<AppInmobiliaria.Models.Usuario> posts = JsonConvert.DeserializeObject<List<AppInmobiliaria.Models.Usuario>>(content);
                _post = new ObservableCollection<AppInmobiliaria.Models.Usuario>(posts);

                if (_post.Count > 0)
                {
                    await DisplayAlert("Alerta", "Ya existe un usuario registrado con los datos ingresados.", "Ok");
                }
                else
                {
                    var parametros = new System.Collections.Specialized.NameValueCollection();
                    var parametrosPersona = new System.Collections.Specialized.NameValueCollection();

                    string rol = "3";
                    string estado = "1";
                    string passEncriptado = Encrypt.GetMD5(pass);

                    parametros.Add("IdUsuario", "");
                    parametros.Add("IdRol", rol);
                    parametros.Add("NombreUsuario", nombre);
                    parametros.Add("IdentificacionUsuario", identificacion);
                    parametros.Add("CorreoUsuario", correo);
                    parametros.Add("UserName", username);
                    parametros.Add("Pass", passEncriptado);
                    parametros.Add("Estado", estado);

                    RegistrarUsuario(parametros);

                    var usuarioGuardado = await client.GetStringAsync(Url + cadenaParams);
                    List<AppInmobiliaria.Models.Usuario> nuevoUser = JsonConvert.DeserializeObject<List<AppInmobiliaria.Models.Usuario>>(usuarioGuardado);
                    var usuarioNuevo = nuevoUser.FirstOrDefault();

                    parametrosPersona.Add("IdPersona", "");
                    parametrosPersona.Add("Tipo", "2");
                    parametrosPersona.Add("Identificacion", identificacion);
                    parametrosPersona.Add("Nombre", nombre);
                    parametrosPersona.Add("Direccion", direccion);
                    parametrosPersona.Add("Telefono", telefono);
                    parametrosPersona.Add("Correo", correo);
                    parametrosPersona.Add("IdUsuario", usuarioNuevo.us_id.ToString());
                    parametrosPersona.Add("Estado", estado);

                    RegistrarPersona(parametrosPersona);

                    await DisplayAlert("Alerta", "Proceso de registro exitoso, ya puedes ingresar al APP.", "Ok");

                    await Navigation.PushAsync(new Login());
                }
            }            
        }

        private void RegistrarUsuario(System.Collections.Specialized.NameValueCollection parametros)
        {
            try
            {
                WebClient cliente = new WebClient();

                cliente.UploadValues(Url, "POST", parametros);
            }
            catch (Exception ex)
            {
                DisplayAlert("Alerta", "Mensaje de alerta " + ex.Message, "Ok");
            }
        }

        private void RegistrarPersona(System.Collections.Specialized.NameValueCollection parametrosPersona)
        {
            try
            {
                WebClient cliente = new WebClient();

                cliente.UploadValues(UrlPersona, "POST", parametrosPersona);
            }
            catch (Exception ex)
            {
                DisplayAlert("Alerta", "Mensaje de alerta " + ex.Message, "Ok");
            }
        }

        private async void btnVolver_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Login());
        }
    }
}