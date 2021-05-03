using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ControlDrones.Logger
{
    class EspecificacionLog
    {
        public String NombreNamespace { get; set; }
        public String NombreClase { get; set; }
        public String NombreMetodo { get; set; }

        public EspecificacionLog()
        {
        }

        public EspecificacionLog(String nombreClase, String nombreMetodo)
        {
            try
            {
                if (nombreClase == null || nombreClase == "" || nombreMetodo == null || nombreMetodo == "")
                {
                    StackFrame frame = new StackFrame(3);
                    var method = frame.GetMethod();
                    this.NombreNamespace = method.DeclaringType.Namespace;
                    this.NombreClase = method.DeclaringType.FullName;
                    this.NombreMetodo = method.Name;
                }
                else
                {
                    this.NombreNamespace = nombreClase.Split('.')[0];
                    this.NombreClase = nombreClase;
                    this.NombreMetodo = nombreMetodo;
                }
            }
            catch (Exception exp)
            {
                this.NombreNamespace = "Indeterminated";
                this.NombreClase = nombreClase;
                this.NombreMetodo = nombreMetodo;
            }
        }
    }
}
