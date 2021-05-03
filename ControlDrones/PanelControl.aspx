<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PanelControl.aspx.cs" Inherits="ControlDrones.PanelPrincipal" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Control y monitorización de drones terrestres</title>
    <link href="Content/css/control_drones.css" rel="stylesheet" />
    <script src="Scripts/jquery-2.1.4.min.js"></script>
    <script src="Scripts/steelseries-min.js"></script>
    <script src="Scripts/tween.js"></script>
    <script src="Scripts/controldrones.js"></script>
    <script>
        var numeroBotonInicio = <%=NumeroBotonInicio %>;
        var numeroBotonStop = <%=NumeroBotonStop %>;
        var numeroBotonAnularDireccion = <%=NumeroBotonAnularDireccion %>;
        var numeroBotonAnularPotencia = <%=NumeroBotonAnularPotencia %>;
        
        var numeroAxesDireccion = <%=NumeroAxesDireccion %>;
        var valorAxes1GirarDerecha = <%=ValorAxes1GirarDerecha %>;
        var valorAxes1GirarIzquierda = <%=ValorAxes1GirarIzquierda %>;

        var numeroAxesPotencia = <%=NumeroAxesPotencia %>;
        var valorAxes1AumentarPotencia = <%=ValorAxes1AumentarPotencia %>;
        var valorAxes1DisminuirPotencia = <%=ValorAxes1DisminuirPotencia %>;

        var numeroAxesDireccion2 = <%=NumeroAxesDireccion2 %>;
        var valorAxes2GirarDerecha = <%=ValorAxes2GirarDerecha %>;
        var valorAxes2GirarIzquierda = <%=ValorAxes2GirarIzquierda %>;

        var numeroAxesPotencia2 = <%=NumeroAxesPotencia2 %>;
        var valorAxes2AumentarPotencia = <%=ValorAxes2AumentarPotencia %>;
        var valorAxes2DisminuirPotencia = <%=ValorAxes2DisminuirPotencia %>;

        var volverANeutraDireccion = <%=VolverANeutraDireccion %>;
        var volverANeutraPotencia = <%=VolverANeutraPotencia %>;
        
        
    </script>
</head>
<body onload="init()">
    <form id="form1" runat="server">

        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>

        <asp:Panel ID="paContenido" runat="server">

            <asp:Panel ID="paMandoControl" runat="server">

                <asp:Panel ID="paLecturas" runat="server">
                    <asp:Label ID="lbMiniTitulo" runat="server" Text="Panel de control"></asp:Label>
                </asp:Panel>

                <asp:Panel ID="paSensores" runat="server">
                    <canvas id="canvasLedPad" width="50" height="50"></canvas>
                    <canvas id="canvasAltitud"></canvas>
                    <canvas id="canvasPresion"></canvas>
                    <canvas id="canvasTemperatura"></canvas>
                    <canvas id="canvasLedOn" width="50" height="50"></canvas>
                </asp:Panel>

                <asp:Panel ID="paControles" runat="server">

                    <asp:Panel ID="paBrujula" runat="server" CssClass="panelControl">
                        <canvas id="canvasBrujula" width="200" height="200"></canvas>
                    </asp:Panel>

                    <asp:Panel ID="paVelocidad" runat="server" CssClass="panelControl">
                        <canvas id="canvasVelocidad" width="200" height="200"></canvas>
                    </asp:Panel>

                    <asp:Panel ID="paDireccion" runat="server" CssClass="panelControl">
                        <canvas id="canvasDireccion" width="200" height="200">No canvas in your browser</canvas>
                        <asp:Panel ID="paBotoneraDireccion" runat="server" CssClass="panelBotonera">
                            <asp:ImageButton ID="btIzquierdaFuerte" runat="server" OnClientClick="return false" ImageUrl="~/Content/images/flechaRojaIzda.png" CssClass="btFlecha"/>
                            <asp:ImageButton ID="btIzquierda" runat="server" OnClientClick="return false" ImageUrl="~/Content/images/flechaAzulIzda.png" CssClass="btFlecha"/>
                            <asp:ImageButton ID="btAnularDireccion" runat="server" OnClientClick="return false" ImageUrl="~/Content/images/stopIcono.png" CssClass="btFlecha"/>
                            <asp:ImageButton ID="btDerecha" runat="server" OnClientClick="return false" ImageUrl="~/Content/images/flechaAzulDcha.png" CssClass="btFlecha"/>
                            <asp:ImageButton ID="btDerechaFuerte" runat="server" OnClientClick="return false" ImageUrl="~/Content/images/flechaRojaDcha.png" CssClass="btFlecha"/>
                        </asp:Panel>
                    </asp:Panel>

                    <asp:Panel ID="paPotencia" runat="server" CssClass="panelControl">
	                    <canvas id="canvasPotencia" width="200" height="200"></canvas>
                        <asp:Panel ID="paBotoneraPotencia" runat="server" CssClass="panelBotonera">
                            <asp:ImageButton ID="btAcelerarFuerte" runat="server" OnClientClick="return false" ImageUrl="~/Content/images/flechaRojaArriba.png" CssClass="btFlecha"/>
                            <asp:ImageButton ID="btAcelerar" runat="server" OnClientClick="return false" ImageUrl="~/Content/images/flechaAzulArriba.png" CssClass="btFlecha"/>
                            <asp:ImageButton ID="btAnularPotencia" runat="server" OnClientClick="return false" ImageUrl="~/Content/images/stopIcono.png" CssClass="btFlecha"/>
                            <asp:ImageButton ID="btFrenar" runat="server" OnClientClick="return false" ImageUrl="~/Content/images/flechaAzulAbajo.png" CssClass="btFlecha"/>
                            <asp:ImageButton ID="btFrenarFuerte" runat="server" OnClientClick="return false" ImageUrl="~/Content/images/flechaRojaAbajo.png" CssClass="btFlecha"/>
                        </asp:Panel>
                    </asp:Panel>

                </asp:Panel>

                <asp:Panel ID="paInicio" runat="server">
                    <asp:ImageButton ID="btGamePad" runat="server" OnClientClick="return false" ImageUrl="~/Content/images/gamepadIcono.png" />
                    <asp:Label ID="lbGamePad" runat="server" Text=""></asp:Label>
                    <asp:Button ID="btIniciar" runat="server" Text="INICIAR" onClientClick="return false"/>
                    <asp:Button ID="btApagar" runat="server" Text="APAGAR" onClientClick="return false"/>
                </asp:Panel>

            </asp:Panel>

            <asp:UpdatePanel ID="paLog" runat="server">
                <ContentTemplate>
                    <asp:Timer runat="server" id="timerLog" Interval="1000" OnTick="ActualizarConsola"></asp:Timer>
                    <asp:Label ID="lbLog" runat="server"></asp:Label>            
                </ContentTemplate>
            </asp:UpdatePanel>

        </asp:Panel>
    </form>
</body>
</html>
