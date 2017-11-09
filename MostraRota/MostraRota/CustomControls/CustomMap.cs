using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace MostraRota.CustomControls
{
    public class CustomMap : Map
    {
        public static readonly BindableProperty RouteCoordinatesProperty =
                BindableProperty.Create(nameof(RouteCoordinates),   // nome da BindableProperty
                                        typeof(List<MyPosition>),   // tipo da propriedade
                                        typeof(CustomMap),          // tipo do objeto declarante
                                        new List<MyPosition>(),     // valor default
                                        BindingMode.TwoWay);        // BindingMode para usar em SetBinding(), se nenhum BindingMode for dado.
                                                                    // Parâmetro opcional. Valor default é BindingMode.OneWay

        public List<MyPosition> RouteCoordinates
        {
            get { return (List<MyPosition>)GetValue(RouteCoordinatesProperty); }
            set { SetValue(RouteCoordinatesProperty, value); }
        }
    }

    public class MyPosition
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime Horario { get; set; } 

        public MyPosition(double lat, double lng, DateTime dthr)
        {
            Latitude = lat;
            Longitude = lng;
            Horario = dthr;
        }
    }
}
