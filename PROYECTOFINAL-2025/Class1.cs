using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Linq;
using NAudio.Wave;

namespace PROYECTOFINAL_2025
{
    public class Sensor
    {
        public int Id { get; set; }
        public string Tipo { get; set; }
        public bool Estado { get; set; } = true;
        public double Valor { get; set; } = 0;
        public bool AlarmaActivada { get; set; } = false;
        public string Unidad { get; set; } = "";

        public Sensor(int id, string tipo, string unidad = "")
        {
            Id = id;
            Tipo = tipo;
            Unidad = unidad;
        }
    }

    public class PanelCentral
    {
        private List<Sensor> sensores;
        private bool energiaPrincipal = true;
        private bool energiaRespaldo = true;
        private bool alarmaSistema = false;
        private List<string> luces = new List<string> { "Luz_1", "Luz_2", "Luz_3", "Luz_4" };
        private List<string> eventosLog = new List<string>();
        private IWavePlayer reproductor;
        private WaveStream audioFileReader;
        private Random random = new Random();

        public PanelCentral()
        {
            sensores = new List<Sensor>();
            reproductor = new WaveOutEvent();

            // SENSORES MEJORADOS CON UNIDADES
            AgregarSensor(1, "TEMPERATURA", "°C");
            AgregarSensor(2, "HUMO", "%");
            AgregarSensor(3, "MANUAL", "");
            AgregarSensor(4, "ENERGIA", "V");
        }

        public void AgregarSensor(int id, string tipo, string unidad) =>
            sensores.Add(new Sensor(id, tipo, unidad));

        public void ActualizarSensor(int id, double valor)
        {
            for (int i = 0; i < sensores.Count; i++)
            {
                if (sensores[i].Id == id)
                {
                    sensores[i].Valor = valor;
                    eventosLog.Add($"[{DateTime.Now:HH:mm:ss}] Sensor {id} ({sensores[i].Tipo}) actualizado a {valor}{sensores[i].Unidad}");
                    break;
                }
            }
        }

        public bool VerificarAlarma()
        {
            bool hayAlarma = false;
            for (int i = 0; i < sensores.Count; i++)
            {
                var s = sensores[i];
                if (!s.Estado) continue;

                // DETECTAR FALLO DE SENSORES
                if (s.Valor < 0 || double.IsNaN(s.Valor) || s.Valor == -999)
                {
                    s.AlarmaActivada = true;
                    hayAlarma = true;
                    eventosLog.Add($"[{DateTime.Now:HH:mm:ss}] 🚨 FALLO CRÍTICO: Sensor {s.Id} ({s.Tipo})");
                    continue;
                }

                if (s.Tipo == "TEMPERATURA" && s.Valor > 60)
                {
                    s.AlarmaActivada = true;
                    hayAlarma = true;
                    eventosLog.Add($"[{DateTime.Now:HH:mm:ss}] 🔥 ALERTA: TEMPERATURA {s.Id} → {s.Valor}{s.Unidad}");
                }
                else if (s.Tipo == "HUMO" && s.Valor > 40)
                {
                    s.AlarmaActivada = true;
                    hayAlarma = true;
                    eventosLog.Add($"[{DateTime.Now:HH:mm:ss}] 💨 ALERTA: HUMO {s.Id} → {s.Valor}{s.Unidad}");
                }
                else if (s.Tipo == "MANUAL" && s.Valor == 1)
                {
                    s.AlarmaActivada = true;
                    hayAlarma = true;
                    eventosLog.Add($"[{DateTime.Now:HH:mm:ss}] 🚨 ESTACIÓN MANUAL {s.Id} ACTIVADA");
                }
                else if (s.Tipo == "ENERGIA" && s.Valor == 0)
                {
                    s.AlarmaActivada = true;
                    hayAlarma = true;
                    eventosLog.Add($"[{DateTime.Now:HH:mm:ss}] ⚡ FALLO ELÉCTRICO: Sensor {s.Id}");
                }
            }
            return hayAlarma;
        }

        // 🔊 NUEVA FUNCIÓN PARA ALARMA DE FALLO ELÉCTRICO
        public void ReproducirAlarmaFalloElectrico()
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("\n⚡ REPRODUCIENDO ALARMA DE FALLO ELÉCTRICO...");
                Console.ResetColor();

                string ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "alarma_fallo.wav");
                if (!File.Exists(ruta))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Archivo 'alarma_fallo.wav' no encontrado - usando alarma.wav");
                    ReproducirAlarma(); // Fallback
                    return;
                }

                reproductor.Stop();
                audioFileReader?.Dispose();
                audioFileReader = new MediaFoundationReader(ruta);
                reproductor.Init(audioFileReader);
                reproductor.Play();
                eventosLog.Add($"[{DateTime.Now:HH:mm:ss}] ⚡ Alarma FALLOELECTRICO.wav activada");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reproduciendo FALLOELECTRICO.wav: {ex.Message}");
                ReproducirAlarma(); // Fallback
            }
        }

        public void ReproducirAlarma()
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n🔊 REPRODUCIENDO ALARMA DE INCENDIO...");
                Console.ResetColor();

                string ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "alarma.wav");
                if (!File.Exists(ruta))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Archivo 'alarma.wav' no encontrado");
                    Console.ResetColor();
                    return;
                }

                reproductor.Stop();
                audioFileReader?.Dispose();
                audioFileReader = new MediaFoundationReader(ruta);
                reproductor.Init(audioFileReader);
                reproductor.Play();
                eventosLog.Add($"[{DateTime.Now:HH:mm:ss}] 🔥 Alarma de incendio activada");
            }
            catch { }
        }

        public void DetenerAlarmaAudio()
        {
            reproductor?.Stop();
            audioFileReader?.Dispose();
            audioFileReader = null;
            eventosLog.Add($"[{DateTime.Now:HH:mm:ss}] 🔇 Alarma sonora detenida");
        }

        public void ActivarAlarma()
        {
            if (!VerificarEnergia()) return;
            alarmaSistema = true;
            eventosLog.Add($"[{DateTime.Now:HH:mm:ss}] 🚨 *** ALARMA GENERAL ACTIVADA ***");

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n╔════════════════════════════════════╗");
            Console.WriteLine("  ║    🚨 ALARMA CONTRA INCENDIOS 🚨  ║");
            Console.WriteLine("  ╚════════════════════════════════════╝\n");
            Console.ResetColor();

            ActivarLuces();
            ReproducirAlarma();
            GenerarReporte();
        }

        // 🔥 NUEVA ALARMA ESPECÍFICA PARA FALLO ELÉCTRICO
        public void ActivarAlarmaFalloElectrico()
        {
            if (!VerificarEnergia()) return;
            alarmaSistema = true;
            eventosLog.Add($"[{DateTime.Now:HH:mm:ss}] ⚡ *** ALARMA FALLO ELÉCTRICO ACTIVADA ***");

            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("\n╔════════════════════════════════════╗");
            Console.WriteLine("  ║    ⚡ ALARMA FALLO ELÉCTRICO ⚡   ║");
            Console.WriteLine("  ╚════════════════════════════════════╝\n");
            Console.ResetColor();

            ActivarLuces();
            ReproducirAlarmaFalloElectrico(); // 🔊 NUEVA ALARMA
            GenerarReporteFalloElectrico();
        }

        public void DesactivarAlarma()
        {
            DetenerAlarmaAudio();
            alarmaSistema = false;
            DesactivarLuces();

            for (int i = 0; i < sensores.Count; i++)
            {
                sensores[i].AlarmaActivada = false;
                if (sensores[i].Tipo != "MANUAL")
                    sensores[i].Valor = random.NextDouble() * 25 + 20; // Valores normales aleatorios
            }

            energiaPrincipal = true;
            eventosLog.Add($"[{DateTime.Now:HH:mm:ss}] ✅ Alarma desactivada - Sistema restaurado");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n✅ Alarma desactivada correctamente");
            Console.WriteLine("🔌 Energía principal restaurada");
            Console.WriteLine("💡 Luces estroboscópicas apagadas");
            Console.ResetColor();
        }

        public bool VerificarEnergia() => energiaPrincipal || energiaRespaldo;

        public void ActivarLuces()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("💡 LUCES ESTROBOSCOPICAS ACTIVADAS:");
            for (int i = 0; i < luces.Count; i++)
            {
                Console.WriteLine($"   {luces[i]} - 🔆 PARPADEO RÁPIDO");
                Thread.Sleep(150);
            }
            Console.ResetColor();
        }

        public void DesactivarLuces()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("💡 Luces estroboscópicas DESACTIVADAS");
            Console.ResetColor();
        }

        public void GenerarReporte()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\n📊 REPORTE DE ALARMA:");
            Console.WriteLine("═══════════════════════════════════════");
            for (int i = 0; i < sensores.Count; i++)
            {
                if (sensores[i].AlarmaActivada)
                {
                    Console.WriteLine($"   🚨 [{sensores[i].Id}] {sensores[i].Tipo}: {sensores[i].Valor}{sensores[i].Unidad}");
                }
            }
            Console.ResetColor();
        }

        // 📋 NUEVO REPORTE ESPECÍFICO PARA FALLO ELÉCTRICO
        public void GenerarReporteFalloElectrico()
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("\n⚡ REPORTE DE FALLO ELÉCTRICO:");
            Console.WriteLine("═══════════════════════════════════════");
            Console.WriteLine($"   🔌 Energía Principal: {(energiaPrincipal ? "ACTIVA" : "❌ FALLO")}");
            Console.WriteLine($"   🔋 Energía Respaldo: {(energiaRespaldo ? "ACTIVA ✅" : "❌ FALLO")}");
            Console.WriteLine($"   ⏱️  Autonomía: {(energiaRespaldo ? "8 horas" : "SIN RESPALDO")}");
            Console.ResetColor();
        }

        // 🔥 SIMULACIÓN DE FALLO DE ENERGÍA ÉPICA (COMPLETA)
        public void SimularFalloEnergia()
        {
            eventosLog.Add($"[{DateTime.Now:HH:mm:ss}] ⚡ INICIANDO SIMULACIÓN FALLO ELÉCTRICO");

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("⚡⚡⚡ SIMULACIÓN DE FALLO ELÉCTRICO ⚡⚡⚡");
            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.ResetColor();

            // FASE 1: ALERTA DE INESTABILIDAD
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n🚨 ALERTA: INESTABILIDAD ELÉCTRICA DETECTADA");
            Console.WriteLine("   🔌 Voltaje: 220V → 185V → 210V → 165V → 120V");
            eventosLog.Add($"[{DateTime.Now:HH:mm:ss}] 🚨 Inestabilidad eléctrica detectada");
            Thread.Sleep(2000);

            // FASE 2: FALLO TOTAL CON EFECTO VISUAL
            for (int i = 0; i < 4; i++)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("⚡🔥 FALLO ELÉCTRICO TOTAL 🔥⚡");
                Console.WriteLine("   💥 CORTOCIRCUITO DETECTADO");
                Console.WriteLine("   🔌 ENERGÍA PRINCIPAL DESCONECTADA");

                if (i % 2 == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("\n                    ");
                    Console.WriteLine("       ⚡ chispaS ⚡       ");
                    Console.WriteLine("                    \n");
                }
                Console.ResetColor();
                Thread.Sleep(600);
            }

            // FASE 3: ACTIVACIÓN RESPALDO
            energiaPrincipal = false;
            energiaRespaldo = true;
            eventosLog.Add($"[{DateTime.Now:HH:mm:ss}] 🔋 ENERGÍA DE RESPALDO ACTIVADA");

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("✅ SISTEMA DE RESPALDO ACTIVADO");
            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.WriteLine("🔋 Baterías de emergencia: 100% carga");
            Console.WriteLine("⏱️  Autonomía estimada: 8 horas 30 minutos");
            Console.WriteLine("💡 Luces de emergencia: ACTIVAS");
            Console.WriteLine("🔊 Alarma sonora: MANTENIDA");
            Console.WriteLine("📡 Comunicación: FUNCIONAL");
            Console.ResetColor();

            // FASE 4: SIMULAR CONSUMO DE BATERÍA
            double bateria = 100;
            for (int minuto = 1; minuto <= 4; minuto++)
            {
                bateria -= random.Next(6, 10); // Consumo aleatorio 6-10%
                Console.WriteLine($"\n⏳ Minuto {minuto} | 🔋 Batería: {bateria:F1}%");
                eventosLog.Add($"[{DateTime.Now:HH:mm:ss}] 🔋 Batería respaldo: {bateria:F1}%");
                Thread.Sleep(1200);
            }

            // FASE 5: RECUPERACIÓN CON Picos
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n⚡ DETECTANDO RECUPERACIÓN DE ENERGÍA");
            Console.WriteLine("   🔌 Iniciando reconexión...");
            Thread.Sleep(2000);

            for (int i = 0; i < 3; i++)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"   ⚡ PICO DE VOLTAJE: {250 + random.Next(0, 50)}V (SOBRETENSIÓN!)");
                Thread.Sleep(600);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"   ✅ Estabilizando: 220V");
                Thread.Sleep(800);
            }

            // FASE 6: RECUPERACIÓN EXITOSA
            energiaPrincipal = true;
            eventosLog.Add($"[{DateTime.Now:HH:mm:ss}] ✅ ENERGÍA PRINCIPAL RESTAURADA");

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("✅ RECUPERACIÓN EXITOSA");
            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.WriteLine("🔌 Energía principal: RESTAURADA ✓");
            Console.WriteLine("🔋 Sistema respaldo: DESACTIVADO");
            Console.WriteLine("⚡ Voltaje estable: 220V");
            Console.WriteLine("🛡️ Autodiagnóstico: COMPLETADO");
            Console.WriteLine("📊 Sistema 100% operativo");

            // ACTIVAR ALARMA DE FALLO ELÉCTRICO
            ActualizarSensor(4, 0); // Sensor energía a 0
            if (VerificarAlarma())
            {
                ActivarAlarmaFalloElectrico(); // 🔊 NUEVA ALARMA
            }

            eventosLog.Add($"[{DateTime.Now:HH:mm:ss}] ✅ Simulación de fallo eléctrico completada");
            Console.WriteLine("\n🎉 SIMULACIÓN COMPLETADA CON ÉXITO");
            Console.ResetColor();
            Thread.Sleep(4000);
        }

        public void MonitorearSistema()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("🔍 MONITOREO EN TIEMPO REAL");
            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.ResetColor();

            int ciclos = 0;
            while (true)
            {
                ciclos++;
                Console.Clear();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"📊 CICLO DE MONITOREO #{ciclos} - {DateTime.Now:HH:mm:ss}");
                Console.WriteLine("═══════════════════════════════════════════════════════");

                // VARIACIÓN NATURAL ALEATORIA
                if (sensores[0].Valor < 50)
                    sensores[0].Valor = 20 + random.NextDouble() * 30;
                if (sensores[1].Valor < 30)
                    sensores[1].Valor = random.NextDouble() * 20;

                for (int i = 0; i < sensores.Count; i++)
                {
                    var s = sensores[i];
                    string estado = s.AlarmaActivada ? "🚨 ALARMA" : "✅ NORMAL";
                    ConsoleColor color = s.AlarmaActivada ? ConsoleColor.Red :
                                       (s.Estado ? ConsoleColor.Green : ConsoleColor.Gray);

                    Console.ForegroundColor = color;
                    Console.WriteLine($" [{s.Id}] {s.Tipo.PadRight(12)} | {s.Valor:F1}{s.Unidad} | {estado}");
                    Console.ResetColor();
                }

                Console.WriteLine("\n🔌 ENERGÍA:");
                Console.ForegroundColor = energiaPrincipal ? ConsoleColor.Green : ConsoleColor.Red;
                Console.WriteLine($"   Principal: {(energiaPrincipal ? "ACTIVA ✅" : "❌ FALLO")}");
                Console.ResetColor();

                Console.ForegroundColor = energiaRespaldo ? ConsoleColor.Green : ConsoleColor.Red;
                Console.WriteLine($"   Respaldo: {(energiaRespaldo ? "ACTIVA ✅" : "❌ FALLO")}");
                Console.ResetColor();

                Console.ForegroundColor = alarmaSistema ? ConsoleColor.Red : ConsoleColor.Green;
                Console.WriteLine($"🚨 Alarma General: {(alarmaSistema ? "ACTIVA 🚨" : "INACTIVA ✅")}");
                Console.ResetColor();

                if (VerificarAlarma() && !alarmaSistema)
                {
                    ActivarAlarma();
                }

                Thread.Sleep(3000);
            }
        }

        public void SimularIncendio()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n🔥 SIMULANDO INCENDIO...");
            Console.ResetColor();
            Thread.Sleep(500);

            // VALORES ALEATORIOS REALISTAS
            double tempAleatoria = 70 + random.NextDouble() * 40;  // 70-110°C
            double humoAleatorio = 50 + random.NextDouble() * 50;  // 50-100%

            ActualizarSensor(1, tempAleatoria);
            Thread.Sleep(300);
            ActualizarSensor(2, humoAleatorio);

            if (VerificarAlarma()) ActivarAlarma();
        }

        public void SimularEstacionManual()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n🚨 SIMULANDO ESTACIÓN MANUAL...");
            Console.ResetColor();
            ActualizarSensor(3, 1);
            if (VerificarAlarma()) ActivarAlarma();
        }

        public void MostrarEventos()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("📋 REGISTRO DE EVENTOS:");
            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.ResetColor();

            if (eventosLog.Count == 0)
            {
                Console.WriteLine("No hay eventos registrados");
            }
            else
            {
                for (int i = 0; i < eventosLog.Count; i++)
                {
                    string evento = eventosLog[i];
                    ConsoleColor color = evento.Contains("ALERTA") || evento.Contains("ALARMA") ||
                                       evento.Contains("FALLO") ? ConsoleColor.Red :
                                       evento.Contains("✅") ? ConsoleColor.Green : ConsoleColor.White;

                    Console.ForegroundColor = color;
                    Console.WriteLine($"[{i + 1:000}] {evento}");
                    Console.ResetColor();
                }
            }
            Console.WriteLine($"\n📊 Total de eventos: {eventosLog.Count}");
        }

        public void LimpiarEventos()
        {
            eventosLog.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("🧹 Registro de eventos LIMPIADO");
            Console.ResetColor();
        }

        // MÉTODO BURBUJA – PARA EL PROFE
        public void OrdenarSensoresBurbuja()
        {
            int n = sensores.Count;
            int comparaciones = 0;

            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n - i - 1; j++)
                {
                    comparaciones++;
                    if (sensores[j].Id > sensores[j + 1].Id)
                    {
                        var temp = sensores[j];
                        sensores[j] = sensores[j + 1];
                        sensores[j + 1] = temp;
                    }
                }
            }

            eventosLog.Add($"[{DateTime.Now:HH:mm:ss}] 🔄 Sensores ordenados BURBUJA ({comparaciones} comparaciones)");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n🎯 ¡SENSORES ORDENADOS CON MÉTODO BURBUJA!");
            Console.WriteLine($"📊 Comparaciones realizadas: {comparaciones}");
            Console.ResetColor();

            // MOSTRAR ORDENADOS
            Console.WriteLine("\n📋 Sensores ordenados por ID:");
            for (int i = 0; i < sensores.Count; i++)
            {
                Console.WriteLine($"   [{sensores[i].Id}] {sensores[i].Tipo} - {sensores[i].Valor}{sensores[i].Unidad}");
            }
        }
    }
}