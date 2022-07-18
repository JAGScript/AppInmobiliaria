using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppInmobiliaria
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DetallePropiedad : ContentPage
    {
        private readonly HttpClient client = new HttpClient();

        private const string UrlPropiedad = "propiedad.php";
        private ObservableCollection<AppInmobiliaria.Models.Propiedad> _propiedad;

        private const string UrlTipoPropiedad = "tipopropiedad.php";
        private ObservableCollection<AppInmobiliaria.Models.TipoPropiedad> _tipoPropiedad;

        private const string UrlBarrio = "barrio.php";
        private ObservableCollection<AppInmobiliaria.Models.Barrio> _barrio;

        private const string UrlParroquia = "parroquia.php";
        private ObservableCollection<AppInmobiliaria.Models.Parroquia> _parroquia;

        private const string UrlCanton = "canton.php";
        private ObservableCollection<AppInmobiliaria.Models.Canton> _canton;

        private const string UrlProvincia = "provincia.php";
        private ObservableCollection<AppInmobiliaria.Models.Provincia> _provincia;

        private const string UrlCaracteristica = "caracteristica.php";
        private ObservableCollection<AppInmobiliaria.Models.Caracteristica> _caracteristica;

        private const string UrlUsuario = "usuarios.php";
        private ObservableCollection<AppInmobiliaria.Models.Usuario> _asesor;

        Models.Usuario userLogueado = new Models.Usuario();
        string idPropiedad = "0";
        Models.Propiedad propiedad = new Models.Propiedad();
        string ubicacionPropiedad = "";
        private string Path = "";
        private string PathImagenes = "";

        public DetallePropiedad(Models.Usuario usuario, string propiedad, string url, string urlImagenes)
        {
            InitializeComponent();
            userLogueado = usuario;
            lblNomUser.Text = userLogueado.us_nombre;
            idPropiedad = propiedad;
            Path = url;
            PathImagenes = urlImagenes;
            CargarDetalle();
        }

        public async void CargarDetalle()
        {
            string propiedadParam = "?IdPropiedad=" + idPropiedad;
            var content = await client.GetStringAsync(Path + UrlPropiedad + propiedadParam);
            List<AppInmobiliaria.Models.Propiedad> propiedades = JsonConvert.DeserializeObject<List<AppInmobiliaria.Models.Propiedad>>(content);
            _propiedad = new ObservableCollection<AppInmobiliaria.Models.Propiedad>(propiedades);

            if(_propiedad.Count == 1)
            {
                propiedad = _propiedad.FirstOrDefault();
                lblImgPrincipal.Source = PathImagenes + propiedad.pr_foto_principal;
                lblPrecio.Text = propiedad.pr_precio.ToString("C");

                string tipoPropiedadParam = "?IdTipo=" + propiedad.pr_tipo_id.ToString();
                var rspTipoPropiedad = await client.GetStringAsync(Path + UrlTipoPropiedad + tipoPropiedadParam);
                List<AppInmobiliaria.Models.TipoPropiedad> tiposPropiedad = JsonConvert.DeserializeObject<List<AppInmobiliaria.Models.TipoPropiedad>>(rspTipoPropiedad);
                _tipoPropiedad = new ObservableCollection<AppInmobiliaria.Models.TipoPropiedad>(tiposPropiedad);

                Models.TipoPropiedad tipoPropiedad = new Models.TipoPropiedad();
                Models.Barrio barrio = new Models.Barrio();
                Models.Parroquia parroquia = new Models.Parroquia();
                Models.Canton canton = new Models.Canton();
                Models.Provincia provincia = new Models.Provincia();
                Models.Caracteristica caracteristica = new Models.Caracteristica();

                if (_tipoPropiedad.Count == 1)
                {
                    tipoPropiedad = _tipoPropiedad.FirstOrDefault();
                    lblTipo.Text = tipoPropiedad.tp_nombre;
                }
                else
                    lblTipo.Text = "No definido";

                string nomProvincia = "";
                string nomCanton = "";
                string nomParroquia = "";
                string nomBarrio = "";

                string barrioParam = "?IdBarrio=" + propiedad.pr_barrio_id.ToString();
                var rspBarrio = await client.GetStringAsync(Path + UrlBarrio + barrioParam);
                List<AppInmobiliaria.Models.Barrio> barrios = JsonConvert.DeserializeObject<List<AppInmobiliaria.Models.Barrio>>(rspBarrio);
                _barrio = new ObservableCollection<AppInmobiliaria.Models.Barrio>(barrios);

                if (_barrio.Count == 1)
                {
                    barrio = _barrio.FirstOrDefault();

                    string parroquiaParam = "?IdParroquia=" + barrio.ba_parroquia_id.ToString();
                    var rspParroquia = await client.GetStringAsync(Path + UrlParroquia + parroquiaParam);
                    List<AppInmobiliaria.Models.Parroquia> parroquias = JsonConvert.DeserializeObject<List<AppInmobiliaria.Models.Parroquia>>(rspParroquia);
                    _parroquia = new ObservableCollection<AppInmobiliaria.Models.Parroquia>(parroquias);

                    nomBarrio = barrio.ba_nombre;

                    if (_parroquia.Count == 1)
                    {
                        parroquia = _parroquia.FirstOrDefault();

                        string cantonParam = "?IdCanton=" + parroquia.pq_canton.ToString();
                        var rspCanton = await client.GetStringAsync(Path + UrlCanton + cantonParam);
                        List<AppInmobiliaria.Models.Canton> cantones = JsonConvert.DeserializeObject<List<AppInmobiliaria.Models.Canton>>(rspCanton);
                        _canton = new ObservableCollection<AppInmobiliaria.Models.Canton>(cantones);

                        nomParroquia = parroquia.pq_nombre;

                        if (_canton.Count == 1)
                        {
                            canton = _canton.FirstOrDefault();

                            string provinciaParam = "?IdProvincia=" + canton.ca_provincia_id.ToString();
                            var rspProvincia = await client.GetStringAsync(Path + UrlProvincia + provinciaParam);
                            List<AppInmobiliaria.Models.Provincia> provincias = JsonConvert.DeserializeObject<List<AppInmobiliaria.Models.Provincia>>(rspProvincia);
                            _provincia = new ObservableCollection<AppInmobiliaria.Models.Provincia>(provincias);

                            nomCanton = canton.ca_nombre;

                            if (_provincia.Count == 1)
                            {
                                provincia = _provincia.FirstOrDefault();

                                nomProvincia = provincia.pv_nombre;
                            }
                        }
                    }

                    ubicacionPropiedad = nomProvincia + " / " + nomCanton + "/" + nomParroquia + "/" + nomBarrio;
                }
                else
                    ubicacionPropiedad = "No definido";

                lblUbicacion.Text = ubicacionPropiedad;

                string carParam = "?IdCaracteristica=" + propiedad.pr_caracteristica_id.ToString();
                var rspCaracteristica = await client.GetStringAsync(Path + UrlCaracteristica + carParam);
                List<AppInmobiliaria.Models.Caracteristica> caracteristicas = JsonConvert.DeserializeObject<List<AppInmobiliaria.Models.Caracteristica>>(rspCaracteristica);
                _caracteristica = new ObservableCollection<AppInmobiliaria.Models.Caracteristica>(caracteristicas);

                if (_caracteristica.Count == 1)
                {
                    caracteristica = _caracteristica.FirstOrDefault();

                    lblMetros.Text = Math.Round(caracteristica.ca_metros, 2).ToString();
                    lblPlantas.Text = caracteristica.ca_plantas.ToString();
                    lblBanios.Text = caracteristica.ca_banios.ToString();
                    lblHabitaciones.Text = caracteristica.ca_habitaciones.ToString();
                    lblParqueaderos.Text = caracteristica.ca_parqueaderos.ToString();
                    lblServicios.Text = caracteristica.ca_servicios.ToString();
                    lblOtros.Text = caracteristica.ca_otros.ToString();
                }
                else 
                {
                    lblMetros.Text = "0.00";
                    lblPlantas.Text = "0";
                    lblBanios.Text = "0";
                    lblHabitaciones.Text = "0";
                    lblParqueaderos.Text = "0";
                    lblServicios.Text = "No definido";
                    lblOtros.Text = "No definido";
                }
            }
            else
            {
                await DisplayAlert("Alerta", "Error al consultar propiedad", "Ok");
                await Navigation.PushAsync(new Propiedad(userLogueado, Path, PathImagenes));
            }
        }

        private async void btnMapa_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MapaPropiedad());
        }

        private async void btnAlbum_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new FotosPropiedad(userLogueado, idPropiedad, Path, PathImagenes));
        }

        private async void btnVolver_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Propiedad(userLogueado, Path, PathImagenes));
        }

        private async void btnContactar_Clicked(object sender, EventArgs e)
        {
            string usuarioParam = "?IdUsuario=" + propiedad.pr_usuario_id;
            var content = await client.GetStringAsync(Path + UrlUsuario + usuarioParam);
            List<AppInmobiliaria.Models.Usuario> asesores = JsonConvert.DeserializeObject<List<AppInmobiliaria.Models.Usuario>>(content);
            _asesor = new ObservableCollection<AppInmobiliaria.Models.Usuario>(asesores);

            if (_asesor.Count > 0)
            {
                var tipoPropiedad = lblTipo.Text;
                var precio = lblPrecio.Text;
                var asesor = _asesor.FirstOrDefault();

                try
                {
                    MailMessage mail = new MailMessage();
                    SmtpClient SmtpServer = new SmtpClient("smtp.office365.com");

                    mail.From = new MailAddress("rbinmobiliaria_pruebas@hotmail.com");
                    mail.To.Add(asesor.us_correo);
                    mail.Subject = "Código de verificación";
                    mail.Body = "Estimado, " + asesor.us_nombre + " \n\n" +
                        "Le informamos que el cliente: " + userLogueado.us_nombre + " se encuentra interesado por la propiedad:  \n\n" +
                        "Tipo: " + tipoPropiedad + " \n\n" +
                        "Precio: " + precio + " \n\n" +
                        "Ubicación: " + ubicacionPropiedad + " \n\n" +
                        "Datos del Cliente: \n\n" +
                        "Nombre: " + userLogueado.us_nombre + " \n\n" +
                        "Correo: " + userLogueado.us_correo + " \n\n" +
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
            else
            {

            }

            await DisplayAlert("Alerta", "Se ha enviado un mensaje al asesor, pronto se podrá en contacto con usted.", "Ok");
        }

        private async void btnCerrarSesion_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Login());
        }
    }
}