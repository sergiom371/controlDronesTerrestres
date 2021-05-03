using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;

namespace ControlDrones.Dron
{
    public class CocheRC
    {
        private const int POTENCIA_MAXIMA = 100;
        private const int POTENCIA_MINIMA = 0;
        private const int POTENCIA_INICIAL = 0;
        private const int DIRECCION_MAXIMA = 100;
        private const int DIRECCION_MINIMA = 0;
        private const int DIRECCION_INICIAL = 50;

        private int GIRO;
        private int GIRO_FUERTE;
        private int INCREMENTO_POTENCIA;
        private int INCREMENTO_POTENCIA_FUERTE;

        private IDronComm comm;
        private bool encendido;

        private int potencia;
        public int Potencia {
            get
            {
                return potencia;
            }
        }

        private int direccion;
        public int Direccion {
            get
            {
                return direccion;
            }
        }


        public CocheRC(IDronComm comm)
        {
            //Valores del fichero de configuración
            this.INCREMENTO_POTENCIA = int.Parse(ConfigurationManager.AppSettings["INCREMENTO_POTENCIA"]);
            this.INCREMENTO_POTENCIA_FUERTE = int.Parse(ConfigurationManager.AppSettings["INCREMENTO_POTENCIA_FUERTE"]);
            this.GIRO = int.Parse(ConfigurationManager.AppSettings["GIRO"]);
            this.GIRO_FUERTE = int.Parse(ConfigurationManager.AppSettings["GIRO_FUERTE"]);

            //Módulo de comunicaciones
            this.comm = comm;

            //Valores por defecto de potencia y dirección
            this.AnularPotencia();
            this.AnularDireccion();
        }


        //Encender y apagar
        public bool Iniciar()
        {
            if (!this.encendido && comm.HayConexion())
            {
                //Set encendido
                this.encendido = true;

                //Definir los canales en Arduino
                comm.DefinirCanalDireccion();
                comm.DefinirCanalMotor();

                //Definir valores iniciales
                this.AnularDireccion();
                this.AnularPotencia();

                //Sincronizar
                this.comm.Sincronizar(this.direccion, this.potencia);
            }

            return this.encendido;
        }

        public void Apagar()
        {
            if (this.encendido)
            {
                //Definir valores iniciales
                this.AnularDireccion();
                this.AnularPotencia();

                //Sincronizar
                this.comm.Sincronizar(this.direccion, this.potencia);

                //Set encendido
                this.encendido = false;
            }
        }




        //Actualizar dirección
        public int GirarDerecha()
        {
            if (this.encendido)
            {
                this.direccion = this.direccion + this.GIRO;
                if (this.direccion > DIRECCION_MAXIMA) this.direccion = DIRECCION_MAXIMA;
            }

            return this.direccion;
        }

        public int GirarDerechaFuerte()
        {
            if (this.encendido)
            {
                this.direccion = Direccion + GIRO_FUERTE;
                if (this.direccion > DIRECCION_MAXIMA) this.direccion = DIRECCION_MAXIMA;
            }

            return Direccion;
        }

        public int GirarIzquierda()
        {
            if (this.encendido)
            {
                this.direccion = this.direccion - GIRO;
                if (this.direccion < DIRECCION_MINIMA) this.direccion = DIRECCION_MINIMA;
            }

            return this.direccion;
        }

        public int GirarIzquierdaFuerte()
        {
            if (this.encendido)
            {
                this.direccion = this.direccion - GIRO_FUERTE;
                if (this.direccion < DIRECCION_MINIMA) this.direccion = DIRECCION_MINIMA;
            }

            return this.direccion;
        }

        public int AnularDireccion()
        {
            if (this.encendido)
            {
                this.direccion = DIRECCION_INICIAL;
            }

            return Direccion;
        }



        //Actualizar Potencia
        public int DisminuirPotencia()
        {
            if (this.encendido)
            {
                this.potencia = this.potencia - INCREMENTO_POTENCIA;
                if (this.potencia < POTENCIA_MINIMA) this.potencia = POTENCIA_MINIMA;
            }

            return this.potencia;
        }

        public int DisminuirPotenciaFuerte()
        {
            if (this.encendido)
            {
                this.potencia = this.potencia - INCREMENTO_POTENCIA_FUERTE;
                if (this.potencia < POTENCIA_MINIMA) this.potencia = POTENCIA_MINIMA;
            }

            return this.potencia;
        }

        public int AumentarPotencia()
        {
            if (this.encendido)
            {
                this.potencia = this.potencia + INCREMENTO_POTENCIA;
                if (this.potencia > POTENCIA_MAXIMA) this.potencia = POTENCIA_MAXIMA;
            }

            return this.potencia;
        }

        public int AumentarPotenciaFuerte()
        {
            if (this.encendido)
            {
                this.potencia = this.potencia + INCREMENTO_POTENCIA_FUERTE;
                if (this.potencia > POTENCIA_MAXIMA) this.potencia = POTENCIA_MAXIMA;
            }

            return this.potencia;
        }

        public int AnularPotencia()
        {
            if (this.encendido)
            {
                this.potencia = POTENCIA_INICIAL;
            }

            return this.potencia;
        }



        //Lecturas
        public LecturasSensores LeerVelocidad()
        {
            LecturasSensores lecturas = null;

            if (this.encendido)
            {
                lecturas = comm.LeerSensorVelocidad();
            }

            return lecturas;
        }


        public LecturasSensores LeerOrientacion()
        {
            LecturasSensores lecturas = null;

            if (this.encendido)
            {
                lecturas = comm.LeerSensorBrujula();
            }

            return lecturas;
        }

        public LecturasSensores LeerBarometrico()
        {
            LecturasSensores lecturas = null;

            if (this.encendido)
            {
                lecturas = comm.LeerSensorBarometrico();
            }

            return lecturas;
        }



        //Sincronizar
        public LecturasSensores Sincronizar()
        {
            LecturasSensores lecturas = null;

            if (this.encendido)
            {
                lecturas = comm.Sincronizar(this.direccion, this.potencia);
            }

            return lecturas;
        }
    }
}