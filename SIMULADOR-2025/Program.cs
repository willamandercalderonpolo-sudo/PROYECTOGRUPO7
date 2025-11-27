using System;
using System.IO;
using System.Threading;
using PROYECTOFINAL_2025;

class Program
{
    static PanelCentral panel = new PanelCentral();

    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.Title = "🔥 SISTEMA CONTRA INCENDIOS v3.0 - Grupo JJM SAC 2025 🔥";

        MostrarBanner();
        AsegurarArchivosAudio();

        // Los sensores ya se crean automáticamente en el constructor del PanelCentral
        // NO necesitas agregar más aquí

        while (true)
        {
            MostrarMenu();
            string opcion = Console.ReadLine().Trim();

            switch (opcion)
            {
                case "1":
                    panel.MonitorearSistema();
                    break;

                case "2":
                    panel.SimularIncendio();
                    Pausa();
                    break;

                case "3":
                    panel.SimularEstacionManual();
                    Pausa();
                    break;

                case "4":
                    panel.SimularFalloEnergia(); // ⚡ SIMULACIÓN ÉPICA ACTUALIZADA
                    Pausa();
                    break;

                case "5":
                    ActualizarManual();
                    Pausa();
                    break;

                case "6":
                    panel.DesactivarAlarma();
                    Pausa();
                    break;

                case "7":
                    panel.MostrarEventos();
                    Pausa();
                    break;

                case "8":
                    panel.LimpiarEventos();
                    Pausa();
                    break;

                case "9":
                    panel.OrdenarSensoresBurbuja();
                    Pausa();
                    break;

                case "0":
                    Salir();
                    return;

                default:
                    ErrorOpcion();
                    break;
            }
        }
    }

    // 🔊 NUEVA FUNCIÓN: ASEGURAR AMBOS ARCHIVOS DE AUDIO
    static void AsegurarArchivosAudio()
    {
        string[] archivos = { "alarma.wav", "FALLOELECTRICO.wav" };

        foreach (string archivo in archivos)
        {
            string destino = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, archivo);
            if (File.Exists(destino)) continue;

            string origen = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", archivo);
            if (File.Exists(origen))
            {
                try
                {
                    File.Copy(origen, destino, true);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"✓ {archivo} copiado al directorio de ejecución");
                    Console.ResetColor();
                }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"⚠️  {archivo} no encontrado en carpeta origen");
                    Console.ResetColor();
                }
            }
        }
        Console.WriteLine();
    }

    static void MostrarBanner()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine("╔═════════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║  🔥SISTEMA CONTRA INCENDIOS INTELIGENTE v3.0 - GRUPO JJM SAC 2025   ║");
        Console.WriteLine("║              INGENIERÍA DE SISTEMAS COMPUTACIONALES 🔥              ║");
        Console.WriteLine("║                                                                     ║");
        Console.WriteLine("╚═════════════════════════════════════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine();
    }

    static void MostrarMenu()
    {
        Console.Clear();
        MostrarBanner();

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔═════════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                       📋 MENÚ PRINCIPAL                             ║");
        Console.WriteLine("╠═════════════════════════════════════════════════════════════════════╣");
        Console.ResetColor();

        // OPCIONES CON EMOJIS Y COLORES
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("║  1. 🔍Monitorear Sistema (Tiempo Real)                              ║");
        Console.WriteLine("║  2. 🔥Simular Incendio (con ALARMA.wav)                             ║");
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("║  3. 🚨Simular Estación Manual                                       ║");
        Console.WriteLine("║  4. ⚡Simular Fallo de Energía (FALLOELECTRICO.wav)                 ║");
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("║  5. ⚙️  Actualizar Sensor Manualmente                               ║");
        Console.WriteLine("║  6. 🛑 Desactivar Alarma                                            ║");
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("║  7. 📋 Ver Registro de Eventos                                      ║");
        Console.WriteLine("║  8. 🧹 Limpiar Eventos                                              ║");
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine("║  9. 🔄Ordenar Sensores (MÉTODO BURBUJA)                             ║");
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("║  0. 🚪 Salir del Sistema                                            ║");
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╚═════════════════════════════════════════════════════════════════════╝");
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n💡 Presione Ctrl+C en cualquier momento para forzar salida de emergencia");
        Console.Write("\n🎯 Seleccione una opción (0-9): ");
        Console.ResetColor();
    }

    static void ActualizarManual()
    {
        Console.Clear();
        MostrarBanner();

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("⚙️  ACTUALIZAR SENSOR MANUALMENTE");
        Console.WriteLine("═══════════════════════════════════════");
        Console.ResetColor();

        // MOSTRAR SENSORES DISPONIBLES
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\n📊 SENSORES DISPONIBLES:");
        Console.WriteLine("   1: TEMPERATURA (°C)");
        Console.WriteLine("   2: HUMO (%)");
        Console.WriteLine("   3: MANUAL (0=Normal, 1=Activado)");
        Console.WriteLine("   4: ENERGÍA (V)");
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("\n🔧 ID del sensor (1-4): ");
        Console.ResetColor();

        string entradaId = Console.ReadLine();
        if (int.TryParse(entradaId, out int id) && id >= 1 && id <= 4)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("📈 Nuevo valor: ");
            Console.ResetColor();

            string entradaVal = Console.ReadLine();
            if (double.TryParse(entradaVal, out double valor))
            {
                panel.ActualizarSensor(id, valor);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n✅ Sensor {id} actualizado a {valor}");
                Console.ResetColor();

                if (panel.VerificarAlarma())
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n🚨 ¡ALARMA ACTIVADA AUTOMÁTICAMENTE!");
                    Console.ResetColor();
                    panel.ActivarAlarma();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\n✅ Valor dentro de rangos normales");
                    Console.ResetColor();
                }
            }
            else
            {
                Error("❌ Valor numérico inválido");
            }
        }
        else
        {
            Error("❌ ID de sensor inválido (use 1-4)");
        }
    }

    static void ErrorOpcion()
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("\n✗ OPCIÓN NO VÁLIDA");
        Console.WriteLine("💡 Use números del 0 al 9");
        Console.ResetColor();
        Pausa();
    }

    static void Error(string mensaje)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\n{mensaje}");
        Console.ResetColor();
        Thread.Sleep(1500);
    }

    static void Pausa()
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("\n\n⌨️  Presione cualquier tecla para continuar...");
        Console.ResetColor();
        Console.ReadKey(true);
    }

    static void Salir()
    {
        Console.Clear();
        panel.DetenerAlarmaAudio();

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("╔══════════════════════════════════════════════════════╗");
        Console.WriteLine("║             🚪 CERRANDO SISTEMA                      ║");
        Console.WriteLine("║                                                      ║");
        Console.WriteLine("║ ✅ Alarma sonora detenida                            ║");
        Console.WriteLine("║ ✅ Luces estroboscópicas apagadas                    ║");
        Console.WriteLine("║ ✅ Sensores restaurados a valores normales           ║");
        Console.WriteLine("║ ✅ Sistema de respaldo desactivado                   ║");
        Console.WriteLine("║                                                      ║");
        Console.WriteLine("║ 🔥 ¡GRACIAS POR USAR SCI v3.0 - GRUPO JJM SAC! 🔥   ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════╝");
        Console.ResetColor();

        Console.WriteLine("\n📧 Contacto: grupo.jjm.sac@gmail.com");
        Console.WriteLine("🌐 Universidad Privada del Norte - 2025");
        Console.WriteLine("Calderon Polo Willan Ander");
        Console.WriteLine("Mendoza Cotrina Rodrigo");
        Console.WriteLine("Callirgos Cabanillas Zinedine Michael");
        Console.WriteLine("Caja Yopla Percy ");

        Thread.Sleep(3000);
    }
}