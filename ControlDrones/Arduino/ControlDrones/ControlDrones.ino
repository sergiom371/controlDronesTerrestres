#include <Bridge.h>
#include <BridgeServer.h>
#include <BridgeClient.h>
#include <Servo.h>
#include <Wire.h>
#include <ADXL345.h>
#include <Adafruit_Sensor.h>
#include <Adafruit_HMC5883_U.h>
#include <Adafruit_BMP085.h>

// Brigde
BridgeServer server;
Servo servoDireccion;
Servo servoMotor;

//Sensores
Adafruit_HMC5883_Unified mag = Adafruit_HMC5883_Unified(12345);
Adafruit_BMP085 bmp;
ADXL345 acc;

//Variables internas
int INTERVAL_TIME = 100;
double aceleracionActual;
double nivelCero;
double aceleracionPrevia;
double velocidadPrevia;
double aceleracionesNulas;

struct LecturaSensores
 {  
  double velocidad; 
  float orientacion; 
  float temperatura;
  float presion;    
  float altitud;
  float presionMar;
  float altitudMar;
 };




void setup() 
{
  Serial.begin(9600);
  Serial.println("Iniciando...");

  //Iniciar bridge
  Bridge.begin();
  server.listenOnLocalhost();
  server.begin();
  Serial.println("Bridge Ok...");

  //Iniciar acelerómetro
  acc.begin();
  nivelCero = calibrar();
  aceleracionPrevia = 0;
  velocidadPrevia = 0;
  aceleracionesNulas = 0;
  Serial.println("Acelerómetro Ok...");

  //Iniciar brújula
  if(!mag.begin())
  {
    Serial.println("No se detecta la brújula!");
    while(1);
  }
  Serial.println("Brújula Ok...");

  //Iniciar temperatura
  if (!bmp.begin()) 
  {
    Serial.println("No se detecta el termómetro!");
    while(1);
  }
  Serial.println("Termómetro Ok...");

  //Finalizando la iniciación
  digitalWrite(13, HIGH);
  Serial.println("PREPARADO!!");
}

double calibrar() 
{
  double sumatorio, Xg, Yg, Zg;
  sumatorio = 0;
  for (int n=0; n<1000; n++)
  {
    acc.read(&Xg, &Yg, &Zg);
    sumatorio = sumatorio + Zg;
  }

  return sumatorio / 1000;
}







void loop() 
{
  //Procesar las solicitudes del servidor REST
  BridgeClient client = server.accept();
  if (client) {
    process(client);
    client.stop();
  }

  //Actualizar la velocidad
  calcularVelocidad();

  //Esperar 100 ms
  delay(100); 
}


void calcularVelocidad()
{
  double aceleracionActual, Xg, Yg, aceleracionLeidaZg;
  
  //Leer aceleración
  acc.read(&Xg, &Yg, &aceleracionLeidaZg);

  //Obtener aceleración calibrada
  double aceleracionCalibrada = aceleracionLeidaZg - nivelCero;

  //Obtener aceleración real actual
  if (abs(aceleracionCalibrada) > 0.15)
  {
      aceleracionActual = aceleracionCalibrada * 9.8;
      aceleracionesNulas = 0;
  }
  else
  {
      aceleracionActual = 0;
      aceleracionesNulas = aceleracionesNulas + 1;
  }

  //Calcular velocidad
  //xVelocity = prev_xVelocity +
  //    (prev_xAcceleration +
  //    ((xAcceleration - prev_xAcceleration) / 2.0)) * UPDATE_TIME;
  double velocidad = 0;
  if (aceleracionesNulas > 10)
  {
    velocidad = 0;
  }
  else
  {
    velocidad = velocidadPrevia + (aceleracionPrevia + ((aceleracionActual - aceleracionPrevia) / 2)) * INTERVAL_TIME/1000 * 3.6;
  }
  
  //Actualizar valores previos
  aceleracionPrevia = aceleracionActual;
  velocidadPrevia = velocidad;

  //Imprimir feedback
  /*
  Serial.print("Xg: ");
  Serial.print(Xg);
  Serial.print(", Yg: ");
  Serial.print(Yg);
  Serial.print(", Zg: ");
  Serial.print(aceleracionLeidaZg);
  Serial.print(", aceleración previa: ");
  Serial.print(aceleracionPrevia);
  Serial.print(", transformado: ");
  Serial.print(aceleracionCalibrada);
  Serial.print(", aceleracion actual: ");
  Serial.print(aceleracionActual);
  Serial.print(", velocidad: ");
  Serial.print(velocidadPrevia);
  Serial.print(" (nivel cero: ");
  Serial.print(nivelCero);
  Serial.println(")");*/
}



void process(BridgeClient client) 
{
  //Leer el comando
  String command = client.readStringUntil('/');

  if (command == "estado"){
    sincronizarCommand(client);
  }
  else if (command == "velocimetro"){
    accCommand(client);
  }
  else if (command == "brujula"){
    compassCommand(client);
  }
  else if (command == "barometrico"){
    bmpCommand(client);
  }
  else if (command == "servo") {
    servoCommand(client);
  }
  else if (command == "servoMotor") {
    servoMotorCommand(client);
  }
  else if (command == "pin") {
    modeCommand(client);
  }
  else if (command == "conexion"){
    testCommand(client);
  }
  else {
    Serial.println("Operación desconocida: " + command);
    client.print(F("Operación desconocida: "));
    client.print(command);
  }
}



void servoCommand(BridgeClient client) 
{
  int value, angle;

  if (client.read() == '/') {
    
    //Procesar la petición
    value = client.parseInt();
    angle = map(value, 0, 100, 0, 180);
    servoDireccion.write(angle);

    //Enviar feedback
    Serial.print("SET SERVO: Ángulo ");
    Serial.print(angle);
    Serial.print(" con potencia ");
    Serial.println(value);

    client.print("SET SERVO: Ángulo ");
    client.print(angle);
    client.print(" con potencia ");
    client.println(value);
  } 
}

void servoMotorCommand(BridgeClient client) 
{
  int value, angle;

  if (client.read() == '/') {
    
    //Procesar la petición
    value = client.parseInt();
    angle = map(value, 0, 100, 180, 90);
    servoMotor.write(angle);

    //Enviar feedback
    Serial.print("SET SERVO MOTOR: Ángulo ");
    Serial.print(angle);
    Serial.print(" con potencia ");
    Serial.println(value);

    client.print("SET SERVO MOTOR: Ángulo ");
    client.print(angle);
    client.print(" con potencia ");
    client.println(value);
  } 
}


void testCommand(BridgeClient client) 
{
  //Enviar feedback
  Serial.println("TEST: OK");
  client.print("OK");
}


void modeCommand(BridgeClient client) 
{
  int pin;

  //Leer pin
  pin = client.parseInt();

  //Si el siguiente carácter no es '/', se trata de una url mal construída
  if (client.read() != '/') {
    client.print(F("error"));
    return;
  }

  String mode = client.readStringUntil('\r');

  if (mode == "servo") 
  {
    //Procesar la petición
    servoDireccion.attach(pin);
    
    //Enviar feedback
    Serial.print(F("Pin D"));
    Serial.print(pin);
    Serial.println(F(" configured as SERVO!"));
    
    client.print(F("Pin D"));
    client.print(pin);
    client.print(F(" configured as SERVO!"));
  } 

  else if (mode == "servoMotor") 
  {
    //Procesar la petición
    servoMotor.attach(pin);
    
    //Enviar feedback
    Serial.print(F("Pin D"));
    Serial.print(pin);
    Serial.println(F(" configured as SERVO MOTOR!"));
    
    client.print(F("Pin D"));
    client.print(pin);
    client.print(F(" configured as SERVO MOTOR!"));
  } 

  else
  {
    //Enviar feedback
    Serial.print(F("error: invalid mode "));
    Serial.println(mode);
    
    client.print(F("error: invalid mode "));
    client.print(mode);
  }
}


void sincronizarCommand(BridgeClient client)
{
  int valueDireccion, valueMotor, angle;

  //Definir el servo de dirección
  valueDireccion = client.parseInt();
  angle = map(valueDireccion, 0, 100, 180, 0);
  servoDireccion.write(angle);

  //Definir el servo motor
  if (client.read() == '/') 
  {
    valueMotor = client.parseInt();
    angle = map(valueMotor, 0, 100, 99, 120);
    servoMotor.write(angle);
  }

  //Realizar lecturas
  LecturaSensores lecturas;
  lecturas = calcularLecturasTermometro();
  lecturas.orientacion = leerBrujula();
  lecturas.velocidad = velocidadPrevia;

  //Enviar feedback
  Serial.print("SET SERVO: Angulo ");
  Serial.print(angle);
  Serial.print(" con potencia ");
  Serial.println(valueDireccion);

  //Enviar feedback
  Serial.print("SET SERVO MOTOR: Angulo ");
  Serial.print(angle);
  Serial.print(" con potencia ");
  Serial.println(valueMotor);
  /*
  Serial.print("Temperatura = ");
  Serial.print(lecturas.temperatura);
  Serial.print(" *C");
    
  Serial.print("; Presión = ");
  Serial.print(lecturas.presion);
  Serial.print(" Pa");

  Serial.print("; Altitud = ");
  Serial.print(lecturas.altitud);
  Serial.print(" meters");

  Serial.print("; Presión en el nivel de mar (calculado) = ");
  Serial.print(lecturas.presionMar);
  Serial.print(" Pa");

  Serial.print("; Altutud real = ");
  Serial.print(lecturas.altitudMar);
  Serial.println(" meters");*/

  client.print(lecturas.velocidad);
  client.print(F("|"));
  client.print(lecturas.orientacion);
  client.print(F("|"));
  client.print(lecturas.temperatura);
  client.print(F("|"));
  client.print(lecturas.presion);
  client.print(F("|"));
  client.print(lecturas.altitud);
  client.print(F("|"));
  client.print(lecturas.presionMar);
  client.print(F("|"));
  client.print(lecturas.altitudMar);
}

void accCommand(BridgeClient client) 
{
  //Enviar feedback
  Serial.print("GET VELOCIDAD: ");
  Serial.println(velocidadPrevia);

  client.print(velocidadPrevia);
  client.print(F("|"));
  client.print(0);
  client.print(F("|"));
  client.print(0);
  client.print(F("|"));
  client.print(0);
  client.print(F("|"));
  client.print(0);
  client.print(F("|"));
  client.print(0);
  client.print(F("|"));
  client.print(0);
}

void compassCommand(BridgeClient client)
{
  float headingDegrees = leerBrujula();

  //Enviar feedback
  Serial.print("ORIENTACIÓN: "); 
  Serial.println(headingDegrees);

  client.print(0);
  client.print(F("|"));
  client.print(headingDegrees);
  client.print(F("|"));
  client.print(0);
  client.print(F("|"));
  client.print(0);
  client.print(F("|"));
  client.print(0);
  client.print(F("|"));
  client.print(0);
  client.print(F("|"));
  client.print(0);
}

float leerBrujula()
{
  //Leer el sensor
  sensors_event_t event; 
  mag.getEvent(&event);
 
  //Enviar feedback
  Serial.print("X: "); 
  Serial.print(event.magnetic.x); 
  Serial.print(" Y: "); 
  Serial.print(event.magnetic.y); 
  Serial.print(" Z: "); 
  Serial.print(event.magnetic.z); 
  Serial.println(" uT");

  float heading = atan2(-event.magnetic.y, event.magnetic.z);
  if(heading < 0)
    heading += 2*PI;
  if(heading > 2*PI)
    heading -= 2*PI;
   
  //Convertir de radianes a grados
  return heading * 180/M_PI; 
}



void bmpCommand(BridgeClient client)
{
  LecturaSensores lecturas = calcularLecturasTermometro();

  //Enviar feedback
  Serial.print("Temperatura = ");
  Serial.print(lecturas.temperatura);
  Serial.print(" *C");
    
  Serial.print("; Presión = ");
  Serial.print(lecturas.presion);
  Serial.print(" Pa");

  Serial.print("; Altitud = ");
  Serial.print(lecturas.altitud);
  Serial.print(" meters");

  Serial.print("; Presión en el nivel de mar (calculado) = ");
  Serial.print(lecturas.presionMar);
  Serial.print(" Pa");

  Serial.print("; Altutud real = ");
  Serial.print(lecturas.altitudMar);
  Serial.println(" meters");

  client.print(0);
  client.print(F("|"));
  client.print(0);
  client.print(F("|"));
  client.print(lecturas.temperatura);
  client.print(F("|"));
  client.print(lecturas.presion);
  client.print(F("|"));
  client.print(lecturas.altitud);
  client.print(F("|"));
  client.print(lecturas.presionMar);
  client.print(F("|"));
  client.print(lecturas.altitudMar);
}

LecturaSensores calcularLecturasTermometro()
{
  LecturaSensores lecturas;
   
  //Procesar la petición
  lecturas.temperatura = bmp.readTemperature();
  lecturas.presion = bmp.readPressure();
  lecturas.altitud = bmp.readAltitude();
  lecturas.presionMar = bmp.readSealevelPressure();
  lecturas.altitudMar = bmp.readAltitude(101500);

  return lecturas;
}


