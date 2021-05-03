using ControlDrones.Comm;
using ControlDrones.Dron;
using ControlDrones.Logger;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ControlDrones
{
    public partial class PanelPrincipal : System.Web.UI.Page
    {

        private static CocheRC cocheRC;
        private static String logsConsola;

        //Variables JS
        public static String NumeroBotonInicio { get; set; }
        public static String NumeroBotonStop { get; set; }
        public static String NumeroBotonAnularDireccion { get; set; }
        public static String NumeroBotonAnularPotencia { get; set; }

        public static String NumeroAxesDireccion { get; set; }
        public static String ValorAxes1GirarDerecha { get; set; }
        public static String ValorAxes1GirarIzquierda { get; set; }

        public static String NumeroAxesPotencia { get; set; }
        public static String ValorAxes1AumentarPotencia { get; set; }
        public static String ValorAxes1DisminuirPotencia { get; set; }

        public static String NumeroAxesDireccion2 { get; set; }
        public static String ValorAxes2GirarDerecha { get; set; }
        public static String ValorAxes2GirarIzquierda { get; set; }

        public static String NumeroAxesPotencia2 { get; set; }
        public static String ValorAxes2AumentarPotencia { get; set; }
        public static String ValorAxes2DisminuirPotencia { get; set; }

        public static String VolverANeutraDireccion { get; set; }
        public static String VolverANeutraPotencia { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //Inicializar el coche RC
                cocheRC = new CocheRC(new ArduinoComm());

                //Definir estado inicial de los logs
                logsConsola = "";

                //Leer la configuración del gamepad
                NumeroBotonInicio = ConfigurationManager.AppSettings["NUMERO_BOTON_INICIO"];
                NumeroBotonStop = ConfigurationManager.AppSettings["NUMERO_BOTON_STOP"];
                NumeroBotonAnularDireccion = ConfigurationManager.AppSettings["NUMERO_BOTON_ANULAR_DIRECCION"];
                NumeroBotonAnularPotencia = ConfigurationManager.AppSettings["NUMERO_BOTON_ANULAR_POTENCIA"];

                NumeroAxesDireccion = ConfigurationManager.AppSettings["NUMERO_AXES_DIRECCION"];
                ValorAxes1GirarDerecha = ConfigurationManager.AppSettings["VALOR_AXES1_GIRAR_DERECHA"];
                ValorAxes1GirarIzquierda = ConfigurationManager.AppSettings["VALOR_AXES1_GIRAR_IZQUIERDA"];

                NumeroAxesPotencia = ConfigurationManager.AppSettings["NUMERO_AXES_POTENCIA"];
                ValorAxes1AumentarPotencia = ConfigurationManager.AppSettings["VALOR_AXES1_AUMENTAR_POTENCIA"];
                ValorAxes1DisminuirPotencia = ConfigurationManager.AppSettings["VALOR_AXES1_DISMINUIR_POTENCIA"];

                NumeroAxesDireccion2 = ConfigurationManager.AppSettings["NUMERO_AXES_DIRECCION_2"];
                ValorAxes2GirarDerecha = ConfigurationManager.AppSettings["VALOR_AXES2_GIRAR_DERECHA"];
                ValorAxes2GirarIzquierda = ConfigurationManager.AppSettings["VALOR_AXES2_GIRAR_IZQUIERDA"];

                NumeroAxesPotencia2 = ConfigurationManager.AppSettings["NUMERO_AXES_POTENCIA_2"];
                ValorAxes2AumentarPotencia = ConfigurationManager.AppSettings["VALOR_AXES2_AUMENTAR_POTENCIA"];
                ValorAxes2DisminuirPotencia = ConfigurationManager.AppSettings["VALOR_AXES2_DISMINUIR_POTENCIA"];

                VolverANeutraDireccion = ConfigurationManager.AppSettings["VOLVER_A_NEUTRA_DIRECCION"];
                VolverANeutraPotencia = ConfigurationManager.AppSettings["VOLVER_A_NEUTRA_POTENCIA"];
            }
        }


        //Encender y apagar
        [System.Web.Services.WebMethod]
        public static bool Iniciar()
        {
            bool hayConexion = false;
            var timer = Stopwatch.StartNew();

            try
            {
                AnadirLogConsola("Iniciando y calibrando...");
                hayConexion = cocheRC.Iniciar();

                if (hayConexion)
                {
                    AnadirLogConsola("Coche RC preparado!");
                }
                else
                {
                    AnadirLogConsola("No existe conexión con Arduino");
                }
            }
            catch (Exception e)
            {
                Log.WriteError("Iniciar: " + e.Message);
                AnadirLogConsola("Se ha producido un error iniciando el panel de control. Revise las conexiones.");
            }

            timer.Stop();
            Log.WriteInfo("Fin de iniciar: " + timer.ElapsedMilliseconds);
            return hayConexion;
        }

        [System.Web.Services.WebMethod]
        public static void Apagar()
        {
            var timer = Stopwatch.StartNew();

            try
            {
                AnadirLogConsola("Apagando...");
                cocheRC.Apagar();
                AnadirLogConsola("Apagado");
            }
            catch (Exception e)
            {
                Log.WriteError("Apagar: " + e.Message);
                AnadirLogConsola("Se ha producido un error apagando el panel control. Revise las conexiones.");
            }

            timer.Stop();
            Log.WriteInfo("Fin de apagar: " + timer.ElapsedMilliseconds);
        }



        //Lecturas
        [System.Web.Services.WebMethod]
        public static String LeerPotencia()
        {
            return cocheRC.Potencia.ToString();
        }

        [System.Web.Services.WebMethod]
        public static String LeerDireccion()
        {
            return cocheRC.Direccion.ToString();
        }

        [System.Web.Services.WebMethod]
        public static LecturasSensores LeerVelocidad()
        {
            LecturasSensores lecturas = null;
            
            try
            {
                lecturas = cocheRC.LeerVelocidad();
            }
            catch (Exception e)
            {
                Log.WriteError("Leer velocidad: " + e.Message);
                AnadirLogConsola("Se ha producido un error de conexión leyendo la velocidad,");
            }

            return lecturas;
        }

        [System.Web.Services.WebMethod]
        public static LecturasSensores LeerOrientacion()
        {
            LecturasSensores lecturas = new LecturasSensores();

            try
            {
                lecturas = cocheRC.LeerOrientacion();
            }
            catch (Exception e)
            {
                Log.WriteError("Leer orientación: " + e.Message);
                AnadirLogConsola("Se ha producido un error de conexión leyendo la orientación.");
            }

            return lecturas;
        }

        [System.Web.Services.WebMethod]
        public static LecturasSensores LeerBarometrico()
        {
            LecturasSensores lecturaSensores = new LecturasSensores();

            try
            {
                lecturaSensores = cocheRC.LeerBarometrico();
            }
            catch (Exception e)
            {
                Log.WriteError("Leer barométrico: " + e.Message);
                AnadirLogConsola("Se ha producido un error de conexión leyendo la información barométrica.");
            }

            return lecturaSensores;
        }

        [System.Web.Services.WebMethod]
        public static LecturasSensores Sincronizar()
        {
            var timer = Stopwatch.StartNew();
            LecturasSensores lecturaSensores = new LecturasSensores();

            try
            {
                lecturaSensores = cocheRC.Sincronizar();
            }
            catch (Exception e)
            {
                Log.WriteError("Sincronizar: " + e.Message);
                AnadirLogConsola("Se ha producido un error de conexión de sicronización con la placa Arduino.");
            }

            timer.Stop();
            Log.WriteInfo("Fin de sincronizar: " + timer.ElapsedMilliseconds);
            return lecturaSensores;
        }



        //Actualizar dirección
        [System.Web.Services.WebMethod]
        public static int GirarDerecha()
        {
            return cocheRC.GirarDerecha();
        }

        [System.Web.Services.WebMethod]
        public static int GirarDerechaFuerte()
        {
            return cocheRC.GirarDerechaFuerte();
        }

        [System.Web.Services.WebMethod]
        public static int GirarIzquierda()
        {
            return cocheRC.GirarIzquierda();
        }

        [System.Web.Services.WebMethod]
        public static int GirarIzquierdaFuerte()
        {
            return cocheRC.GirarIzquierdaFuerte();
        }

        [System.Web.Services.WebMethod]
        public static int AnularDireccion()
        {
            return cocheRC.AnularDireccion();
        }



        //Actualizar Potencia
        [System.Web.Services.WebMethod]
        public static int DisminuirPotencia()
        {
            return cocheRC.DisminuirPotencia();
        }

        [System.Web.Services.WebMethod]
        public static int DisminuirPotenciaFuerte()
        {
            return cocheRC.DisminuirPotenciaFuerte();
        }

        [System.Web.Services.WebMethod]
        public static int AumentarPotencia()
        {
            return cocheRC.AumentarPotencia();
        }

        [System.Web.Services.WebMethod]
        public static int AumentarPotenciaFuerte()
        {
            return cocheRC.AumentarPotenciaFuerte();
        }

        [System.Web.Services.WebMethod]
        public static int AnularPotencia()
        {
            return cocheRC.AnularPotencia();
        }




        //Logs
        public void ActualizarConsola(object sender, EventArgs e)
        {
            this.lbLog.Text = logsConsola;
            
            String script = @"var objDiv = document.getElementById('paLog');
                                objDiv.scrollTop = objDiv.scrollHeight;";

            ScriptManager.RegisterStartupScript(this, GetType(), ClientID, script, true);
        }

        private static void AnadirLogConsola(String log)
        {
            logsConsola = logsConsola + "[" + DateTime.Now.ToLongTimeString() + "] " + log + "<br>";
        }
    }
}