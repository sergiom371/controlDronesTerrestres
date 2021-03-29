using ControlDrones.Dron;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace ControlDrones.Comm
{
    public class LecturasArduino : LecturasSensores
    {
        public LecturasArduino(String lecturas)
        {
            String[] valores = lecturas.Split('|');
            this.Velocidad = Math.Abs(Math.Round(double.Parse(valores[0], new CultureInfo("en")), 2));
            this.Orientacion = Math.Round(double.Parse(valores[1], new CultureInfo("en")), 2);
            this.Temperatura = Math.Round(double.Parse(valores[2], new CultureInfo("en")), 2);
            this.Presion = Math.Round(double.Parse(valores[3], new CultureInfo("en")), 0);
            this.Altitud = Math.Round(double.Parse(valores[6], new CultureInfo("en")), 2);
        }
    }
}