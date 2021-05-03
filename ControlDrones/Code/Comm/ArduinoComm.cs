using ControlDrones.Dron;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;

namespace ControlDrones.Comm
{
    public class ArduinoComm : IDronComm
    {
        private String ARDUINO_URL;
        private static int CANAL_DIRECCION;
        private static int CANAL_POTENCIA;

        private enum CommOperacion { estado, servo, servoMotor, velocimetro, brujula, barometrico, conexion, pin }
        private enum CommTipo { output, servo, servoMotor }

        public ArduinoComm()
        {
            CANAL_DIRECCION = int.Parse(ConfigurationManager.AppSettings["CANAL_DIRECCION"]);
            CANAL_POTENCIA = int.Parse(ConfigurationManager.AppSettings["CANAL_POTENCIA"]);
            this.ARDUINO_URL = ConfigurationManager.AppSettings["ARDUINO_URL"];
        }



        //Sincronizar
        public LecturasSensores Sincronizar(int direccion, int potencia)
        {
            return new LecturasArduino(
                        EnviarPeticion(
                            GenerarUrl(CommOperacion.estado, CommTipo.output, direccion.ToString(), potencia.ToString()))
                        );
        }


        //Aplicar dirección y potencia
        public String DefinirCanalDireccion()
        {
            return EnviarPeticion(
                    GenerarUrl(CommOperacion.pin, CommTipo.servo, CANAL_DIRECCION.ToString())
                    );
        }

        public String DefinirCanalMotor()
        {
            return EnviarPeticion(
                    GenerarUrl(CommOperacion.pin, CommTipo.servoMotor, CANAL_POTENCIA.ToString())
                    );
        }

        public String AplicarDireccion(int valor)
        {
            return EnviarPeticion(
                    GenerarUrl(CommOperacion.servo, CommTipo.output, CANAL_DIRECCION.ToString(), valor.ToString())
                    );
        }

        public String AplicarPotencia(int valor)
        {
            return EnviarPeticion(
                    GenerarUrl(CommOperacion.servoMotor, CommTipo.output, CANAL_POTENCIA.ToString(), valor.ToString())
                    );
        }

        //Lectura de sensores
        public LecturasSensores LeerSensorVelocidad()
        {
            return new LecturasArduino(EnviarPeticion(
                    GenerarUrl(CommOperacion.velocimetro, CommTipo.output, "0"))
                    );
        }

        public LecturasSensores LeerSensorBrujula()
        {
            return new LecturasArduino(EnviarPeticion(
                    GenerarUrl(CommOperacion.brujula, CommTipo.output, "0"))
                    );
        }

        public LecturasSensores LeerSensorBarometrico()
        {
            return new LecturasArduino(EnviarPeticion(
                    GenerarUrl(CommOperacion.barometrico, CommTipo.output, "0"))
                    );
        }


        //Test conexión
        public Boolean HayConexion()
        {
            for (int numeroIntento = 0; numeroIntento < 3; numeroIntento++)
            {
                if (EnviarPeticion(GenerarUrl(CommOperacion.conexion, CommTipo.output, "0")) == "OK")
                {
                    return true;
                }
            }

            return false;
        }




        private String GenerarUrl(CommOperacion operacion, CommTipo op, String parametro1, String parametro2 = null)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("/arduino/" + operacion.ToString() + "/" + parametro1);
            if (operacion == CommOperacion.pin)
            {
                builder.Append("/" + op);
            }
            if (parametro2 != null)
            {
                builder.Append("/" + parametro2);
            }

            return builder.ToString();
        }



        private String EnviarPeticion(String url)
        {
            String resultado = "";

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(this.ARDUINO_URL);
                    client.Timeout = new TimeSpan(0, 0, 8);
                    var result = client.PostAsync(url, null).Result;
                    resultado = result.Content.ReadAsStringAsync().Result;
                }
            }
            catch (Exception e)
            {
                resultado = e.Message;
            }

            return resultado;
        }
    }
}