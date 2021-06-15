# Control y monitorización de drones terrestres - Página en construcción!

El objetivo del proyecto es sustituir el mecanismo de control de un dron terrestre (un coche RC), típicamente basado en un mando físico que emite las órdenes al coche por alguna tecnología de radio, por otro basado en una red de datos TCP/IP mediante el uso de una aplicación web y un mando de videojuegos. También se contemplan temas de monitorización del coche mediante la instalación en el mismo de una serie de sensores que permiten medir ciertos parámetros, como la velocidad o la temperatura ambiente. Para ello, se emplea una placa Arduino que sirve de puente entre el coche RC y la aplicación web.

Un vistazo rápido del funcionamiento y componentes del proyecto en el siguiente vídeo:

[![Alt](http://img.youtube.com/vi/hJ5HVmcTfEM/0.jpg)](http://www.youtube.com/watch?v=hJ5HVmcTfEM "Title")

# El proyecto

Algunas consideraciones importantes a tener en cuenta:

 * El objetivo del proyecto fue la experimientación y el auto aprendizaje. NO PRETENDE servir de ejemplo de código fuente limpio, patrones de diseño, etc. De hecho, el único criterio a la hora de elegir tecnología para el desarrollo (ASP.NET, .NET Framework 4.5, WebMethods, jQuery) fue únicamente que era tecnología que me resultaba familiar.
 * El proyecto se desarrolló en 2016. Seguramente existan soluciones hardware y software más efectivas que las empleadas en el proyecto en la actualidad. 
 * Es un proyecto donde los componentes electrónicos empleados tienen un gran peso, y donde algunos o muchos de los componentes hardware empleados son muy complicados obtener hoy en día al estar descatalogados, anticuados, etc. Por lo tanto, es un proyecto muy dificilmente reproducible por algún interesado hoy en dia, al menos exactamente como fue concebido.
 * Por todo ello, es importante aclarar que este proyecto está publicado por si alguien quiere curiosear por el código, comprobar cómo se comunica el arduino con el coche RC, o cosas así. El proyecto no está ahora mismo en evolución, ni esta previsto lanzar futuras versiones ampliando funcionalidades o mejorando el código.

# Componentes hardware

El coche seleccionado para el proyecto fue el modelo H.King Rattler 1/8 4WD Buggy, que se comercializa (o comercializaba) por la tienda HobbyKing. El mando de radio control se incluía en el pack de venta.

Si se levanta el chasis del coche algunos componentes del mismo que se pueden ver son:

 * Bateria. No se incluye en el pack y fue adquirida a parte. Es una 9172 Turnigy 5000mAh 2S 20C Lipo Pack.
 * Variador de velocidad. Esta pieza, comúnmente conocido como ESC o speed controller, es el componente central del coche, y se conecta simultáneamente al motor del coche, a la batería, y al receiver. Originalmente era un Waterproof Esc de 40A, y soportaba baterías lipos de 2 o 3 células. Pero en pleno desarrollo del proyecto  el variador original del sufrió una avería, y tuvo que ser sustituido por otro HobbyWing eZRun compatible con el resto de componentes del coche.
 * Motor. Es de tipo 4 pole 540 size H.King 2700 KV, y su función es proporcionar fuerza de desplazamiento.
 * Receiver. El receptor (receiver) es el puente de unión entre el mando de control y el resto de componentes del coche. Está conectado tanto al controlador de velocidad (ESC) como al servo de dirección, permitiendo de esta manera el control a distancia sobre la dirección y la potencia y sentido de desplazamiento del coche, respectivamente.
 * Servo de dirección. El servo de dirección, por último, que tiene una fuerza de 9kg. Está conectado únicamente al receptor, y su función es interpretar las órdenes que llegan del mando de control a través del receptor, y girar las ruedas delanteras del coche tantos grados como se le indique. De esta manera se controla la orientación del coche.

Para poder sustituir el mecanismo de control por radio del coche RC por otro basado en una red de datos TCP/IP, se realizaron las siguientes modificaciones sobre el diseño original.

* Placa Arduino. Para este proyeto se optó por usar una Arduino Yún, ya que esta placa permite trabajar fácilmente con señales electricas, y además este modelo en concreto aportaba una serie de capacidades añadidas (su procesador Atheros soporta una distribución Linux basada en OpenWrt llamada OpenWrt-Yun) que permitían dotar al coche de comunicación TCP/IP, tan necesaria para el proyecto. La función de la placa Arduino es la de sustitur el receptor (receiver) que, como se ha comentado, es el componente que recibe las órdenes del mando de control, y las transmite tanto al variador de velocidad (ESC) como al servo de dirección. Por tanto, con el nuevo montaje, hay que desconectar el receiver tanto del servo de dirección como del ESC, y conectar ambos componenetes a la placa Arduino a traves de cables Servo.
* GY-80.
* Circuito de energía


# Componentes software

En construcción
