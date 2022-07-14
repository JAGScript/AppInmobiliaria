using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace AppInmobiliaria
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapaPropiedad : ContentPage
    {
        public MapaPropiedad()
        {
            InitializeComponent();

            Position position = new Position(36.9628066, -122.0194722);
            MapSpan mapSpan = new MapSpan(position, 0.01, 0.01);
            Map map = new Map(mapSpan);
        }
    }
}