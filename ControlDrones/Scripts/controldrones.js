//radiales
var radialVelocidad;
var radialDireccion;
var radialPotencia;
var radialBrujula;
var singleTemperatura;
var singlePresion;
var singleAltitud;
var ledOn;
var ledPad;

//funciones periódicas
var intervalSicronizacion;
var intervalCheckPad;
var intervalPad;

//flags
var encendido = false;
var hasGP = false;
var accionServo = false;
var accionMotor = false;
var iniciando = false;
var apagando = false;



//Funciones del game pad
function canGame() {
    return "getGamepads" in navigator;
}

function reportOnGamepad() {
    var gp = navigator.getGamepads()[0];

    if (gp != null) {
        //Iniciar
        if (gp.buttons[numeroBotonInicio].pressed) {
            Iniciar();
        }

        //Apagar
        if (gp.buttons[numeroBotonStop].pressed) {
            Apagar();
        }

        //Anular dirección
        if (gp.buttons[numeroBotonAnularDireccion].pressed) {
            AnularDireccion();
        }

        //Anulas potencia
        if (gp.buttons[numeroBotonAnularPotencia].pressed) {
            AnularPotencia();
        }
        
        //Dirección
        if (gp.axes[numeroAxesDireccion] == valorAxes1GirarDerecha || gp.axes[numeroAxesDireccion2] == valorAxes2GirarDerecha) {
            accionServo = true;
            GirarDerecha();
        }
        else if (gp.axes[numeroAxesDireccion] == valorAxes1GirarIzquierda || gp.axes[numeroAxesDireccion2] == valorAxes2GirarIzquierda) {
            accionServo = true;
            GirarIzquierda();
        }
        else if (volverANeutraDireccion && accionServo == true) {
                console.log('anula direccion');
                accionServo = false;
                AnularDireccion();
        }

        //Potencia
        if (gp.axes[numeroAxesPotencia] == valorAxes1AumentarPotencia || gp.axes[numeroAxesPotencia2] == valorAxes2AumentarPotencia) {
            accionMotor = true;
            AumentarPotencia();
        }
        else if (gp.axes[numeroAxesPotencia] == valorAxes1DisminuirPotencia || gp.axes[numeroAxesPotencia2] == valorAxes2DisminuirPotencia) {
            accionMotor = true;
            DisminuirPotencia();
        }
        else if (volverANeutraPotencia && accionMotor == true) {
                console.log('anula potencia');
                accionMotor = false;
                AnularPotencia();
        }
    }
    else {
        ledPad.toggleLed();
        hasGP = false;
        window.clearInterval(intervalPad);
        $("#btGamePad").hide();
        $("#lbGamePad").text('');

        //Definir un intervalo para Chrome
        intervalCheckPad = window.setInterval(CheckGamePad, 1000);
    }
}

function CheckGamePad() {
    if (navigator.getGamepads()[0]) {
        if (!hasGP) $(window).trigger("gamepadconnected");
        window.clearInterval(intervalCheckPad);
    }
}

function MostrarConfiguracionGamepad() {
    window.open('/Content/TestGamepad.htm', '_blank');
}



//Funciones de iniciar/apagar
function Iniciar() {
    if (!iniciando && !encendido) {
        iniciando = true;
        PageMethods.Iniciar(IniciarCallback);
    }
}

function IniciarCallback(hayConexion) {

    if (hayConexion) {

        //Flag
        encendido = true;

        //Funciones periódicas
        intervalSicronizacion = setInterval(function () { Sincronizar(); }, 1000)

        //Interfaz gráfica
        LeerPotencia();
        LeerDireccion();

        //Led
        ledOn.toggleLed();
    }

    iniciando = false;
}

function Apagar() {
    if (!apagando && encendido) {
        apagando = true;
        PageMethods.Apagar(ApagarCallback);
    }
}

function ApagarCallback() {

    //Funciones periódicas
    clearInterval(intervalSicronizacion);

    //Interfaz gráfica
    radialPotencia.setValueAnimated(0);
    radialDireccion.setValueAnimated(0);
    radialVelocidad.setValueAnimated(0);
    radialBrujula.setValueAnimated(0);
    singleTemperatura.setValue(0);
    singlePresion.setValue(0);
    singleAltitud.setValue(0);

    //Flag
    encendido = false;

    //Led
    ledOn.toggleLed();

    apagando = false;
}


//Funciones de lectura
//Potencia
function LeerPotencia() {
    PageMethods.LeerPotencia(LeerPotenciaCallback);
}

function LeerPotenciaCallback(potencia) {
    radialPotencia.setValueAnimated(potencia);
}


//Direción
function LeerDireccion() {
    PageMethods.LeerDireccion(LeerDireccionCallback);
}

function LeerDireccionCallback(direccion) {
    radialDireccion.setValueAnimated(direccion);
}



//Sincronizar
function Sincronizar() {
    PageMethods.Sincronizar(SincronizarCallback);
}

function SincronizarCallback(lecturaSensores) {
    if (encendido) {
        radialVelocidad.setValueAnimated(lecturaSensores.Velocidad);
        radialBrujula.setValueAnimated(lecturaSensores.Orientacion);
        singleTemperatura.setValue(lecturaSensores.Temperatura);
        singlePresion.setValue(lecturaSensores.Presion);
        singleAltitud.setValue(lecturaSensores.Altitud);
    }
}

//Velocidad
function LeerVelocidad() {
    PageMethods.LeerVelocidad(LeerVelocidadCallback);
}

function LeerVelocidadCallback(lecturaSensores) {
    if (encendido) {
        radialVelocidad.setValueAnimated(lecturaSensores.Velocidad);
    }
}

//Brújula
function LeerBrujula() {
    PageMethods.LeerOrientacion(LeerBrujulaCallback);
}

function LeerBrujulaCallback(lecturaSensores) {
    if (encendido) {
        radialBrujula.setValueAnimated(lecturaSensores.Orientacion);
    }
}

//Barométrico
function LeerBarometrico() {
    PageMethods.LeerBarometrico(LeerBarometricoCallback);
}

function LeerBarometricoCallback(lecturaSensores) {
    if (encendido) {
        singleTemperatura.setValue(lecturaSensores.Temperatura);
        singlePresion.setValue(lecturaSensores.Presion);
        singleAltitud.setValue(lecturaSensores.Altitud);
    }
}





//Funciones de Potencia
function AumentarPotencia() {
    PageMethods.AumentarPotencia(LeerPotenciaCallback);
}

function AumentarPotenciaFuerte() {
    PageMethods.AumentarPotenciaFuerte(LeerPotenciaCallback);
}

function DisminuirPotencia() {
    PageMethods.DisminuirPotencia(LeerPotenciaCallback);
}

function DisminuirPotenciaFuerte() {
    PageMethods.DisminuirPotenciaFuerte(LeerPotenciaCallback);
}

function AnularPotencia() {
    PageMethods.AnularPotencia(LeerPotenciaCallback);
}


//Funciones de Dirección
function GirarIzquierda() {
    PageMethods.GirarIzquierda(LeerDireccionCallback);
}

function GirarIzquierdaFuerte() {
    PageMethods.GirarIzquierdaFuerte(LeerDireccionCallback);
}

function GirarDerecha() {
    PageMethods.GirarDerecha(LeerDireccionCallback);
}

function GirarDerechaFuerte() {
    PageMethods.GirarDerechaFuerte(LeerDireccionCallback);
}

function AnularDireccion() {
    PageMethods.AnularDireccion(LeerDireccionCallback);
}




function init() {
    var sections = Array(steelseries.Section(0, 15, 'rgba(0, 0, 220, 0.3)'),
                         steelseries.Section(15, 30, 'rgba(0, 220, 0, 0.3)'),
                         steelseries.Section(30, 40, 'rgba(220, 220, 0, 0.3)'));

    var areas = Array(steelseries.Section(40, 50, 'rgba(220, 0, 0, 0.3)'));

    // Crear radial de brújula
    radialBrujula = new steelseries.Compass('canvasBrujula', {
                            size: 201,
                            frameDesign: steelseries.FrameDesign.BLACK_METAL,
                            backgroundColor: steelseries.BackgroundColor.PUNCHED_SHEET,
                            rotateFace: true
    });

    // Crear radial de velocidad
    radialVelocidad = new steelseries.Radial('canvasVelocidad', {
                            section: sections,
                            area: areas,
                            titleString: 'Velocidad',
                            unitString: 'Km/h',
                            minValue: 0,
                            maxValue: 50,
                            pointerType: steelseries.PointerType.TYPE8,
                            frameDesign: steelseries.FrameDesign.BLACK_METAL,
                            //backgroundColor: steelseries.BackgroundColor.CARBON
                            backgroundColor: steelseries.BackgroundColor.BRUSHED_STAINLESS
    });

    // Crear radial de dirección
    radialDireccion = new steelseries.Radial('canvasDireccion', {
                            gaugeType: steelseries.GaugeType.TYPE2,
                            minValue: 0,
                            maxValue: 100,
                            threshold: 0,
                            titleString: 'Dirección',
                            unitString: '%',
                            frameDesign: steelseries.FrameDesign.BLACK_METAL,
                            backgroundColor: steelseries.BackgroundColor.PUNCHED_SHEET,
                            pointerType: steelseries.PointerType.TYPE7,
                            pointerColor: steelseries.ColorDef.BLUE,
                            lcdColor: steelseries.LcdColor.BLUE2,
                            ledColor: steelseries.LedColor.BLUE_LED
    });

    // Crear radial de potencia
    radialPotencia = new steelseries.Radial('canvasPotencia', {
                            gaugeType: steelseries.GaugeType.TYPE3,
                            titleString: "Potencia",
                            unitString: "%",
                            frameDesign: steelseries.FrameDesign.BLACK_METAL,
                            backgroundColor: steelseries.BackgroundColor.PUNCHED_SHEET,
                            valueColor: steelseries.ColorDef.YELLOW,
                            lcdColor: steelseries.LcdColor.YELLOW,
                            ledColor: steelseries.LedColor.YELLOW_LED
    });

    //Cuadro de temperatura
    singleTemperatura = new steelseries.DisplaySingle('canvasTemperatura', {
                            width: 135,
                            height: 50,
                            unitString: "ºC",
                            unitStringVisible: true,
                            headerString: "Temperatura",
                            headerStringVisible: true
    });

    //Cuadro de presión
    singlePresion = new steelseries.DisplaySingle('canvasPresion', {
                            width: 180,
                            height: 50,
                            unitString: "pa",
                            unitStringVisible: true,
                            headerString: "Presión",
                            headerStringVisible: true
                        });

    //Cuadro de altitud
    singleAltitud = new steelseries.DisplaySingle('canvasAltitud', {
                            width: 135,
                            height: 50,
                            unitString: "mt",
                            unitStringVisible: true,
                            headerString: "Altitud",
                            headerStringVisible: true
                        });

    //Crear led de encendido/apagado
    ledOn = new steelseries.Led('canvasLedOn', {
                            ledColor: steelseries.LedColor.RED_LED,
                            width: 50,
                            height: 50
    });

    //Crear led de encendido/apagado
    ledPad = new steelseries.Led('canvasLedPad', {
                            ledColor: steelseries.LedColor.GREEN_LED,
                            width: 50,
                            height: 50
    });

    //Definir funciones en caso de posibilidad de manejar el gamepad
    if (canGame()) {
        $(window).on("gamepadconnected", function () {
            ledPad.toggleLed();
            hasGP = true;
            $("#btGamePad").show();
            $("#lbGamePad").text(navigator.getGamepads()[0].id);
            intervalPad = window.setInterval(reportOnGamepad, 300);
        });

        //setup an interval for Chrome
        intervalCheckPad = window.setInterval(CheckGamePad, 1000);
    }
}




$(document).ready(function () {

    $("#btIniciar").click(function () {
        Iniciar();
    });

    $("#btApagar").click(function () {
        Apagar();
    });

    $("#btAcelerar").click(function () {
        AumentarPotencia();
    });

    $("#btAcelerarFuerte").click(function () {
        AumentarPotenciaFuerte();
    });

    $("#btFrenar").click(function () {
        DisminuirPotencia();
    });

    $("#btFrenarFuerte").click(function () {
        DisminuirPotenciaFuerte();
    });

    $("#btIzquierda").click(function () {
        GirarIzquierda();
    });

    $("#btIzquierdaFuerte").click(function () {
        GirarIzquierdaFuerte();
    });

    $("#btDerecha").click(function () {
        GirarDerecha();
    });

    $("#btDerechaFuerte").click(function () {
        GirarDerechaFuerte();
    });

    $("#btAnularPotencia").click(function () {
        AnularPotencia();
    });

    $("#btAnularDireccion").click(function () {
        AnularDireccion();
    });

    $("#btGamePad").click(function () {
        MostrarConfiguracionGamepad();
    });

});
