using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ControlDrones.Dron
{
    public interface IDronComm
    {
        //Sincronizar
        LecturasSensores Sincronizar(int direccion, int potencia);

        //Aplicar dirección y potencia
        String DefinirCanalDireccion();
        String DefinirCanalMotor();
        String AplicarDireccion(int direccion);
        String AplicarPotencia(int potencia);

        //Lectura de sensores
        LecturasSensores LeerSensorVelocidad();
        LecturasSensores LeerSensorBrujula();
        LecturasSensores LeerSensorBarometrico();

        //Test conexión
        Boolean HayConexion();
    }
}