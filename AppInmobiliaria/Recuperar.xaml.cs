using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using System.Net.Mail;
using System.Net.Http;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using System.Net;
using static AppInmobiliaria.Login;

namespace AppInmobiliaria
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Recuperar : ContentPage
    {
        private const string UrlUsuario = "http://192.168.1.3/RBInmobiliaria/api/usuarios.php";
        private const string UrlCodigoRecuperacion = "http://192.168.1.3/RBInmobiliaria/api/tranusuarios.php";
        private readonly HttpClient client = new HttpClient();
        private ObservableCollection<AppInmobiliaria.Models.Usuario> _post;
        private ObservableCollection<AppInmobiliaria.Models.CodigoRecuperar> _listaCodigo;

        int codigoGenerado = 0;
        int usuarioEscogido = 0;

        public Recuperar()
        {
            InitializeComponent();
        }

        private async void btnRecuperar_Clicked(object sender, EventArgs e)
        {
            string identificacion = txtIdentificacion.Text;
            string correo = txtCorreo.Text;
            string username = txtLogin.Text;

            if (string.IsNullOrEmpty(identificacion) || string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(username))
            {
                await DisplayAlert("Alerta", "Existen campos vacios, llenalos para continuar", "Ok");
            }
            else
            {
                string cadenaParams = "?UserName=" + username + "&IdentificacionUsuario=" + identificacion + "&CorreoUsuario=" + correo;
                var content = await client.GetStringAsync(UrlUsuario + cadenaParams);
                List<AppInmobiliaria.Models.Usuario> posts = JsonConvert.DeserializeObject<List<AppInmobiliaria.Models.Usuario>>(content);
                _post = new ObservableCollection<AppInmobiliaria.Models.Usuario>(posts);

                if (_post.Count == 1)
                {
                    var usuario = _post.FirstOrDefault();
                    usuarioEscogido = usuario.us_id;

                    string cadenaCod = "?IdUsuario=" + usuarioEscogido;
                    var contentCod = await client.GetStringAsync(UrlCodigoRecuperacion + cadenaCod);
                    List<AppInmobiliaria.Models.CodigoRecuperar> codigos = JsonConvert.DeserializeObject<List<AppInmobiliaria.Models.CodigoRecuperar>>(contentCod);

                    if (codigos.Count > 0)
                    {
                        codigoGenerado = codigos.FirstOrDefault().cr_codigo;
                        await DisplayAlert("Alerta", "Ya existe un código generado, revisa tu correo.", "Ok");
                        seccionRecuperar.IsVisible = true;
                        btnCambiar.IsVisible = true;
                    }
                    else
                    {
                        var guid = Guid.NewGuid();
                        var soloNumeros = new String(guid.ToString().Where(Char.IsDigit).ToArray());
                        var codigo = int.Parse(soloNumeros.Substring(0, 4));

                        codigoGenerado = codigo;
                        DateTime fecha = DateTime.Now;
                        string estado = "1";

                        var parametros = new System.Collections.Specialized.NameValueCollection();

                        string strFecha = fecha.ToString("yyyy-MM-dd HH:mm:ss");

                        parametros.Add("IdSec", "");
                        parametros.Add("IdUsuario", usuario.us_id.ToString());
                        parametros.Add("Codigo", codigo.ToString());
                        parametros.Add("Fecha", strFecha);
                        parametros.Add("Estado", estado);

                        RegistrarRecuperacion(parametros);

                        EnviarCorreo(correo, usuario.us_nombre, codigo);

                        await DisplayAlert("Alerta", "Se ha enviado un correo con el código para continuar con el cambio de contraseña", "Ok");
                        seccionRecuperar.IsVisible = true;
                        btnCambiar.IsVisible = true;
                    }
                }
                else
                {
                    await DisplayAlert("Alerta", "Ya existe un usuario registrado con los datos ingresados.", "Ok");
                }
            }
        }

        private async void RegistrarRecuperacion(System.Collections.Specialized.NameValueCollection parametros)
        {
            try
            {
                WebClient cliente = new WebClient();

                cliente.UploadValues(UrlCodigoRecuperacion, "POST", parametros);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Alerta", "Mensaje de alerta " + ex.Message, "Ok");
            }
        }

        private async void EnviarCorreo(string correo, string nombre, int codigo)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.office365.com");

                mail.From = new MailAddress("rbinmobiliaria_pruebas@hotmail.com");
                mail.To.Add(correo);
                mail.Subject = "Código de verificación";
                mail.Body = "Estimado, " + nombre + " \n\n" +
                    "Para cambiar su contraseña se ha generado un código de verificación, mismo que debe ingresar en el app para continuar. \n\n" +
                    "Código: <strong>" + codigo.ToString() + " \n\n" +
                    "Saludos. \n\n" +
                    "RB Inmobiliaria \n\n" +
                    "Este correo se ha generado de forma automática, favor no responer al mismo.";

                SmtpServer.Port = 587;
                SmtpServer.Host = "smtp.office365.com";
                SmtpServer.EnableSsl = true;
                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.Credentials = new System.Net.NetworkCredential("rbinmobiliaria_pruebas@hotmail.com", "RBInmo2022");

                SmtpServer.Send(mail);

            }
            catch (Exception ex)
            {
                await DisplayAlert("Faild", ex.Message, "OK");
            }
        }

        private async void btnCambiar_Clicked(object sender, EventArgs e)
        {
            string codigo = txtCodigo.Text;
            string pass = txtNuevoPass.Text;
            string passEncriptado = Encrypt.GetMD5(pass);

            if (string.IsNullOrEmpty(codigo) || string.IsNullOrEmpty(pass))
            {
                await DisplayAlert("Alerta", "Existen campos vacios, llenalos para continuar", "Ok");
            }
            else
            {
                string cadenaParams = "?IdUsuario=" + usuarioEscogido;
                var content = await client.GetStringAsync(UrlCodigoRecuperacion + cadenaParams);
                List<AppInmobiliaria.Models.CodigoRecuperar> posts = JsonConvert.DeserializeObject<List<AppInmobiliaria.Models.CodigoRecuperar>>(content);
                _listaCodigo = new ObservableCollection<AppInmobiliaria.Models.CodigoRecuperar>(posts);

                if (_listaCodigo.Count == 1)
                {
                    var codigoUsuario = _listaCodigo.FirstOrDefault();

                    if (codigoUsuario.cr_codigo == codigoGenerado)
                    {
                        CambiarContrasena(passEncriptado);

                        EstadoCambioContrasena();

                        await DisplayAlert("Alerta", "Contraseña modificada con éxito.", "Ok");

                        await Navigation.PushAsync(new Login());
                    }
                    else
                    {
                        await DisplayAlert("Alerta", "El código ingresado es incorrecto", "Ok");
                    }
                }
                else
                {
                    await DisplayAlert("Alerta", "No existe un código registrado.", "Ok");
                }
            }
        }

        private void CambiarContrasena(string contrasena)
        {
            WebClient cliente = new WebClient();
            var usuario = _post.FirstOrDefault();

            string parametros = "";
            Models.Usuario data = new Models.Usuario();
            data = _post.FirstOrDefault();
            data.us_contrasena = contrasena;

            var content = JsonConvert.SerializeObject(data);

            parametros += "?us_id=" + usuario.us_id;

            parametros += "&us_contrasena=" + contrasena;

            var urlCompleta = new Uri(UrlUsuario + parametros);

            cliente.UploadString(urlCompleta, "PUT", content);
        }

        private void EstadoCambioContrasena()
        {
            WebClient cliente = new WebClient();

            string parametros = "";
            Models.CodigoRecuperar data = new Models.CodigoRecuperar();
            data = _listaCodigo.FirstOrDefault();
            data.cr_estado = 0;

            var content = JsonConvert.SerializeObject(data);

            parametros += "?cr_id=" + data.cr_id;

            parametros += "&cr_estado=" + data.cr_estado;

            var urlCompleta = new Uri(UrlCodigoRecuperacion + parametros);

            cliente.UploadString(urlCompleta, "PUT", content);
        }

        private async void btnVolver_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Login());
        }
    }
}