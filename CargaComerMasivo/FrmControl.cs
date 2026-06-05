using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OfficeOpenXml;

namespace CargaComerMasivo
{
    public class FrmControl : Form
    {
        // ── Controles UI ──────────────────────────────────────────────────────
        private Panel    pnlHeader;
        private Panel    pnlEstado;
        private Panel    pnlBotones;
        private Panel    pnlFooter;

        private Label    lblTitulo;
        private Label    lblBrand;

        private Label    lblEstadoHora;
        private Label    lblEstadoProximo;
        private Label    lblEstadoLog;
        private Button   btnAbrirLog;
        private ProgressBar pbProgreso;

        private Label    lblNumSubidos;
        private Label    lblNumSkip;
        private Label    lblNumErrores;

        private Label    lblDotEmpresa;
        private Label    lblEmpresaNombre;
        private Label    lblProceso;

        private Button   btnIniciar;
        private Button   btnCancelarDocs;
        private Button   btnSalir;

        // ── Estado interno ────────────────────────────────────────────────────
        private System.Windows.Forms.Timer _timerCiclo;
        private System.Windows.Forms.Timer _timerCountdown;
        private System.Windows.Forms.Timer _timerPulso;
        private bool     _procesando  = false;
        private bool     _iniciado    = false;
        private bool     _primerCiclo = true;
        private bool     _pulsoBrillo = false;
        private DateTime _proximoCiclo = DateTime.MinValue;

        // ── API ───────────────────────────────────────────────────────────────
        private const string API_KEY      = "uiZ0KTBmaFHArbxF0WMIfCOY9Yc88m30czyfBCccIQQ9BNTKob8o4fU3LKe26bXOrmtD1G7MGnHXbTPltCUA26YNAW809cuBWiK6BL5h3CjSco2upvLRQBpKoofkzvKh";
        private const string API_SPAN_URL = "https://cafeelgrande.hades.vraia.com/ceg/orders/from/{0}/span/{1}/excel";

        // ── Log ───────────────────────────────────────────────────────────────
        private static readonly string CarpetaLog = System.IO.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "CargaComerMasivo", "Logs");

        // ═════════════════════════════════════════════════════════════════════
        // CONSTRUCTOR
        // ═════════════════════════════════════════════════════════════════════
        public FrmControl()
        {
            ConstruirUI();
            UITheme.AplicarIconoForm(this);
            try { RegistroProcesados.InicializarBD(); }
            catch (Exception exBD) { EscribirLog($"[ERROR] InicializarBD: {exBD.Message}"); }
        }

        // ═════════════════════════════════════════════════════════════════════
        // CONSTRUIR UI
        // ═════════════════════════════════════════════════════════════════════
        private void ConstruirUI()
        {
            this.Text            = "Comercial Masivo — Connector WinetPC";
            this.Size            = new Size(660, 470);
            this.MinimumSize     = new Size(660, 470);
            this.MaximumSize     = new Size(660, 470);
            this.StartPosition   = FormStartPosition.CenterScreen;
            this.BackColor       = UITheme.BgForm;
            this.ForeColor       = UITheme.TextMain;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox     = false;
            this.Font            = UITheme.FntDefault;

            // ── HEADER ────────────────────────────────────────────────────────
            pnlHeader = new Panel { Dock = DockStyle.Top, Height = 86, BackColor = UITheme.BgHeader };
            pnlHeader.Paint += (s, e) =>
            {
                using (var pen = new Pen(UITheme.BorderHi, 1))
                    e.Graphics.DrawLine(pen, 0, pnlHeader.Height - 1, pnlHeader.Width, pnlHeader.Height - 1);
            };

            var picLogo = new PictureBox
            {
                Size     = new Size(54, 54),
                Location = new Point(20, 16),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = UITheme.BgHeader
            };
            try { picLogo.Image = Properties.Resources.icono; } catch { }

            lblTitulo = new Label
            {
                Text      = "Comercial Masivo",
                ForeColor = UITheme.TextMain,
                Font      = UITheme.FntTitle,
                AutoSize  = true,
                Location  = new Point(88, 14)
            };

            lblBrand = new Label
            {
                Text      = "CONNECTOR WINETPC  ·  CONTPAQi Comercial Premium",
                ForeColor = UITheme.TextMuted,
                Font      = new Font("Segoe UI", 7.5f, FontStyle.Regular),
                AutoSize  = true,
                Location  = new Point(90, 50)
            };

            pnlHeader.Controls.AddRange(new Control[] { picLogo, lblTitulo, lblBrand });

            // ── ESTADO ────────────────────────────────────────────────────────
            pnlEstado = new Panel
            {
                BackColor = UITheme.BgCard,
                Location  = new Point(20, 100),
                Size      = new Size(620, 255),
            };
            pnlEstado.Paint += PintarBorde;

            var lblSecEstado = new Label
            {
                Text      = "ESTADO DEL PROCESO",
                ForeColor = UITheme.TextMuted,
                Font      = UITheme.FntSection,
                AutoSize  = true,
                Location  = new Point(18, 12)
            };

            lblEstadoHora = new Label
            {
                Text      = "En espera — presiona  ▶ Iniciar Proceso",
                ForeColor = UITheme.TextDim,
                Font      = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                AutoSize  = true,
                Location  = new Point(18, 30)
            };

            int cardW = 184, cardH = 76, cardY = 58, gap = 14;
            int[] xs = { 18, 18 + cardW + gap, 18 + (cardW + gap) * 2 };

            lblNumSubidos = CrearStatCard(pnlEstado, xs[0], cardY, cardW, cardH, "SUBIDOS",  UITheme.Accent,  out var _pSubidos);
            lblNumSkip    = CrearStatCard(pnlEstado, xs[1], cardY, cardW, cardH, "OMITIDOS", UITheme.TextDim, out var _pSkip);
            lblNumErrores = CrearStatCard(pnlEstado, xs[2], cardY, cardW, cardH, "ERRORES",  UITheme.Danger,  out var _pErr);

            lblEstadoProximo = new Label
            {
                Text      = "Próximo ciclo: —",
                ForeColor = UITheme.TextMuted,
                Font      = UITheme.FntSm,
                AutoSize  = true,
                Location  = new Point(18, cardY + cardH + 12)
            };

            lblEstadoLog = new Label
            {
                Text      = "Log: (no iniciado)",
                ForeColor = UITheme.TextMuted,
                Font      = new Font("Segoe UI", 7.5f),
                AutoSize  = false,
                Size      = new Size(460, 16),
                Location  = new Point(18, cardY + cardH + 34)
            };

            btnAbrirLog = new Button
            {
                Text     = "Ver log",
                Location = new Point(526, cardY + cardH + 26),
                Size     = new Size(76, 24),
                Font     = new Font("Segoe UI", 7.5f, FontStyle.Bold)
            };
            UITheme.StyleBtn(btnAbrirLog, UITheme.Neutral);
            btnAbrirLog.Click += (s, e) =>
            {
                string ruta = RutaLogHoy();
                if (File.Exists(ruta))
                    System.Diagnostics.Process.Start("notepad.exe", ruta);
                else
                    MessageBox.Show("Aún no hay log para hoy.", "Log",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            pbProgreso = new ProgressBar
            {
                Location = new Point(18, cardY + cardH + 58),
                Size     = new Size(584, 3),
                Style    = ProgressBarStyle.Marquee,
                MarqueeAnimationSpeed = 25,
                Visible  = false
            };

            pnlEstado.Controls.AddRange(new Control[]
            {
                lblSecEstado, lblEstadoHora,
                _pSubidos, _pSkip, _pErr,
                lblEstadoProximo, lblEstadoLog, btnAbrirLog, pbProgreso
            });

            // ── FOOTER ────────────────────────────────────────────────────────
            pnlFooter = new Panel { Dock = DockStyle.Bottom, Height = 30, BackColor = UITheme.BgHeader };
            pnlFooter.Paint += (s, e) =>
            {
                using (var pen = new Pen(UITheme.Border, 1))
                    e.Graphics.DrawLine(pen, 0, 0, pnlFooter.Width, 0);
            };

            lblDotEmpresa = new Label { Size = new Size(8, 8), Location = new Point(16, 11) };
            lblDotEmpresa.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                Color c = _iniciado
                    ? (_pulsoBrillo ? Color.FromArgb(200, 200, 200) : Color.FromArgb(100, 100, 100))
                    : Color.FromArgb(80, 80, 80);
                using (var br = new SolidBrush(c))
                    e.Graphics.FillEllipse(br, 0, 0, 8, 8);
            };

            lblEmpresaNombre = new Label
            {
                Text      = "Empresa abierta",
                ForeColor = UITheme.TextMuted,
                Font      = UITheme.FntSm,
                AutoSize  = true,
                Location  = new Point(30, 7)
            };

            lblProceso = new Label
            {
                Text      = "Proceso detenido",
                ForeColor = UITheme.TextMuted,
                Font      = UITheme.FntSm,
                AutoSize  = true,
                Location  = new Point(460, 7)
            };

            pnlFooter.Controls.AddRange(new Control[] { lblDotEmpresa, lblEmpresaNombre, lblProceso });

            // ── BOTONES ───────────────────────────────────────────────────────
            pnlBotones = new Panel
            {
                BackColor = UITheme.BgForm,
                Location  = new Point(20, 370),
                Size      = new Size(620, 50)
            };

            btnIniciar = new Button
            {
                Text     = "▶   Iniciar Proceso",
                Size     = new Size(196, 42),
                Location = new Point(0, 4)
            };
            UITheme.StyleBtn(btnIniciar, UITheme.Accent, large: true);
            btnIniciar.Click += BtnIniciar_Click;

            btnCancelarDocs = new Button
            {
                Text     = "Cancelar Documentos",
                Size     = new Size(170, 42),
                Location = new Point(204, 4)
            };
            UITheme.StyleBtn(btnCancelarDocs, UITheme.Neutral, large: true);
            btnCancelarDocs.Click += (s, e) => new FrmCancelaDoctos().ShowDialog();

            btnSalir = new Button
            {
                Text     = "Salir",
                Size     = new Size(130, 42),
                Location = new Point(382, 4)
            };
            UITheme.StyleBtn(btnSalir, UITheme.Neutral, large: true);
            btnSalir.Click += BtnSalir_Click;

            pnlBotones.Controls.AddRange(new Control[] { btnIniciar, btnCancelarDocs, btnSalir });

            // ── ENSAMBLAR ─────────────────────────────────────────────────────
            this.Controls.AddRange(new Control[] { pnlHeader, pnlEstado, pnlBotones, pnlFooter });
            this.FormClosed += FrmControl_FormClosed;
        }

        private Label CrearStatCard(Panel padre, int x, int y, int w, int h,
                                    string titulo, Color colorNum, out Panel panel)
        {
            var p = new Panel { Location = new Point(x, y), Size = new Size(w, h), BackColor = UITheme.BgCardAlt };
            p.Paint += (s, e) =>
            {
                using (var pen = new Pen(UITheme.Border, 1))
                {
                    var r = new Rectangle(0, 0, p.Width - 1, p.Height - 1);
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    e.Graphics.DrawRoundedRectangle(pen, r, 4);
                }
            };

            var lblNum = new Label
            {
                Text      = "0",
                ForeColor = colorNum,
                Font      = UITheme.FntHuge,
                AutoSize  = false,
                Size      = new Size(w, 48),
                Location  = new Point(0, 6),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var lblNom = new Label
            {
                Text      = titulo,
                ForeColor = UITheme.TextMuted,
                Font      = UITheme.FntSection,
                AutoSize  = false,
                Size      = new Size(w, 18),
                Location  = new Point(0, h - 20),
                TextAlign = ContentAlignment.MiddleCenter
            };

            p.Controls.Add(lblNum);
            p.Controls.Add(lblNom);
            panel = p;
            return lblNum;
        }

        private void PintarBorde(object sender, PaintEventArgs e)
        {
            var p = (Panel)sender;
            using (var pen = new Pen(UITheme.Border, 1))
            {
                var rect = new Rectangle(0, 0, p.Width - 1, p.Height - 1);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.DrawRoundedRectangle(pen, rect, 6);
            }
        }

        private string BuscarPrimerCodigoAlmacen()
        {
            try
            {
                using (var conn = new SqlConnection(Program.ConnStrEmpresa))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(
                        "SELECT TOP 1 CCODIGOALMACEN FROM admAlmacenes " +
                        "WHERE LTRIM(RTRIM(CCODIGOALMACEN)) = '1'", conn))
                    {
                        var result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                            return result.ToString().Trim();
                    }
                    using (var cmd = new SqlCommand(
                        "SELECT TOP 1 CCODIGOALMACEN FROM admAlmacenes " +
                        "WHERE CIDALMACEN > 0 ORDER BY CIDALMACEN", conn))
                    {
                        var result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                            return result.ToString().Trim();
                    }
                }
            }
            catch { }
            return "1";
        }

        private string BuscarCodConceptoPorNombre(string nombreConcepto)
        {
            try
            {
                using (var conn = new SqlConnection(Program.ConnStrEmpresa))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(
                        "SELECT TOP 1 CCODIGOCONCEPTO FROM admConceptos " +
                        "WHERE UPPER(LTRIM(RTRIM(CNOMBRECONCEPTO))) = UPPER(LTRIM(RTRIM(@nom)))", conn))
                    {
                        cmd.Parameters.AddWithValue("@nom", nombreConcepto);
                        var result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                            return result.ToString().Trim();
                    }
                }
            }
            catch { }
            return "";
        }

        // ═════════════════════════════════════════════════════════════════════
        // BOTÓN INICIAR / DETENER
        // ═════════════════════════════════════════════════════════════════════
        private void BtnIniciar_Click(object sender, EventArgs e)
        {
            if (_iniciado)
            {
                _timerCiclo?.Stop();
                _timerCountdown?.Stop();
                _timerPulso?.Stop();
                _iniciado = false;

                UITheme.StyleBtn(btnIniciar, UITheme.Accent, large: true);
                btnIniciar.Text      = "▶   Iniciar Proceso";
                lblProceso.Text      = "Proceso detenido";
                lblProceso.ForeColor = UITheme.TextMuted;
                lblEstadoHora.Text   = "Proceso detenido manualmente.";
                lblEstadoHora.ForeColor = UITheme.TextMuted;
                lblEstadoProximo.Text   = "Próximo ciclo: —";
                pbProgreso.Visible      = false;
                lblDotEmpresa.Invalidate();
                EscribirLog($"[{DateTime.Now:HH:mm:ss}] Proceso detenido manualmente.");
                return;
            }

            _iniciado    = true;
            _primerCiclo = true;

            UITheme.StyleBtn(btnIniciar, UITheme.Danger, large: true);
            btnIniciar.Text         = "⏹   Detener Proceso";
            lblProceso.Text         = "Proceso activo";
            lblProceso.ForeColor    = UITheme.Accent;
            lblEstadoHora.Text      = "Iniciando primer ciclo...";
            lblEstadoHora.ForeColor = UITheme.TextDim;

            EscribirLog($"[{DateTime.Now:HH:mm:ss}] ════ Proceso iniciado ════");

            _timerCiclo          = new System.Windows.Forms.Timer();
            _timerCiclo.Interval = 3000;
            _timerCiclo.Tick    += TimerCicloTick;
            _timerCiclo.Start();

            _timerPulso       = new System.Windows.Forms.Timer { Interval = 700 };
            _timerPulso.Tick += (s, ev) => { _pulsoBrillo = !_pulsoBrillo; lblDotEmpresa.Invalidate(); };
            _timerPulso.Start();
        }

        // ═════════════════════════════════════════════════════════════════════
        // TIMER CICLO — cada 30 minutos
        // ═════════════════════════════════════════════════════════════════════
        private async void TimerCicloTick(object sender, EventArgs e)
        {
            _timerCiclo.Interval = 5 * 1000; // 5 segundos
            if (_procesando) return;
            await EjecutarCicloAsync();
        }

        // ═════════════════════════════════════════════════════════════════════
        // CICLO ASÍNCRONO
        // ═════════════════════════════════════════════════════════════════════
        private async Task EjecutarCicloAsync()
        {
            _procesando             = true;
            pbProgreso.Visible      = true;
            lblEstadoHora.ForeColor = UITheme.TextDim;

            int nuevos = 0, skip = 0, errores = 0;

            try
            {
                // ── 1. SINCRONIZAR CATÁLOGO (productos + clientes) ────────────
                // Primero SQL + HTTP en background, luego creación en UI thread (SDK)
                // Solo se loguea si hay algo que crear o si ocurre un error.
                lblEstadoHora.Text = "Sincronizando catálogo...";
                try
                {
                    SyncResponse syncResp = null;
                    await Task.Run(() =>
                    {
                        var productos = SincronizadorCatalogo.ExtraerProductos();
                        var clientes  = SincronizadorCatalogo.ExtraerClientes();
                        syncResp      = SincronizadorCatalogo.EnviarCatalogo(productos, clientes);
                    });

                    // Crear productos faltantes en UI thread (requiere SDK)
                    if (syncResp?.ProductosFaltantes != null)
                        foreach (var p in syncResp.ProductosFaltantes)
                        {
                            bool ok = SincronizadorCatalogo.CrearProducto(p, out string err);
                            EscribirLog(ok
                                ? $"[{DateTime.Now:HH:mm:ss}] PROD+ {p.Codigo}"
                                : $"[{DateTime.Now:HH:mm:ss}] PROD! {p.Codigo}: {err}");
                        }

                    // Crear clientes faltantes en UI thread (requiere SDK)
                    if (syncResp?.ClientesFaltantes != null)
                        foreach (var c in syncResp.ClientesFaltantes)
                        {
                            bool ok = SincronizadorCatalogo.CrearCliente(c, out string err);
                            EscribirLog(ok
                                ? $"[{DateTime.Now:HH:mm:ss}] CLI+  {c.Codigo}"
                                : $"[{DateTime.Now:HH:mm:ss}] CLI!  {c.Codigo}: {err}");
                        }
                    // No se loguea "catálogo sincronizado" en cada ciclo vacío
                }
                catch (Exception exSync)
                {
                    // Fallo en sync no detiene la carga de documentos
                    EscribirLog($"[{DateTime.Now:HH:mm:ss}] AVISO sync catálogo: {exSync.Message}");
                }

                // NOTA: fCierraEmpresa + fAbreEmpresa intencionalmente NO se llaman aquí.
                // El SDK se inicializa una sola vez en FrmConexion y debe mantenerse abierto
                // durante toda la sesión. Reiniciarlo mid-ciclo corrompe el estado interno
                // de conceptos con CTIPOFOLIO especial y provoca error 130101 en fAltaDocumento.

                // ── 2. DESCARGAR Y PROCESAR DOCUMENTOS ───────────────────────
                lblEstadoHora.Text = "Descargando pedidos...";

                string rutaArchivo, url;
                long   unix;

                if (_primerCiclo)
                {
                    // 4 semanas atrás + 4 semanas adelante = 56 días total
                    // Cubre pedidos con fecha futura que ya existen en la API
                    unix         = DateTimeOffset.UtcNow.AddDays(-28).ToUnixTimeSeconds();
                    long span56d = 56 * 24 * 60 * 60;
                    url          = string.Format(API_SPAN_URL, unix, span56d);
                    rutaArchivo  = RutaDescargaLocal(DateTime.UtcNow.Date.ToString("yyyy-MM-dd") + "_full");
                    EscribirLog($"[{DateTime.Now:HH:mm:ss}] Primer ciclo — 4 semanas atrás y 4 semanas adelante.");
                    await Task.Run(() => DescargarArchivo(url, rutaArchivo));
                    _primerCiclo = false;
                }
                else
                {
                    // Ciclo regular — desde 60 s atrás con span de 28 días hacia adelante
                    // Así captura pedidos nuevos (últimos 60 s) aunque su fecha sea futura
                    unix        = DateTimeOffset.UtcNow.AddSeconds(-60).ToUnixTimeSeconds();
                    long span28d = 28 * 24 * 60 * 60;
                    url         = string.Format(API_SPAN_URL, unix, span28d);
                    rutaArchivo = RutaDescargaLocal("span_tmp");
                    await Task.Run(() => DescargarArchivo(url, rutaArchivo));
                }

                lblEstadoHora.Text = "Procesando documentos...";

                DataTable dt = await Task.Run(() => LeerExcel(rutaArchivo));

                // Borrar el archivo temporal inmediatamente después de leerlo
                try { if (File.Exists(rutaArchivo)) File.Delete(rutaArchivo); } catch { }

                if (dt.Rows.Count > 0)
                    ProcesarBloques(dt, out nuevos, out skip, out errores);
                // Sin pedidos: no se loguea nada para mantener el log limpio
            }
            catch (WebException wex)
            {
                if (wex.Response is HttpWebResponse resp && (int)resp.StatusCode == 404)
                { /* sin pedidos en este rango — silencioso para no saturar el log */ }
                else
                {
                    string cod = wex.Response is HttpWebResponse r2 ? $" HTTP {(int)r2.StatusCode}" : "";
                    errores++;
                    EscribirLog($"[{DateTime.Now:HH:mm:ss}] ERROR descarga{cod}: {wex.Message}");
                }
            }
            catch (Exception ex)
            {
                errores++;
                EscribirLog($"[{DateTime.Now:HH:mm:ss}] ERROR ciclo: {ex.Message}");
            }
            finally
            {
                pbProgreso.Visible = false;
                ActualizarResumen(nuevos, skip, errores);
                IniciarCountdown();
                _procesando = false;
            }
        }

        // ═════════════════════════════════════════════════════════════════════
        // COUNTDOWN PRÓXIMO CICLO
        // ═════════════════════════════════════════════════════════════════════
        private void IniciarCountdown()
        {
            _proximoCiclo = DateTime.Now.AddSeconds(5);
            _timerCountdown?.Stop();
            _timerCountdown       = new System.Windows.Forms.Timer { Interval = 1000 };
            _timerCountdown.Tick += (s, e) =>
            {
                if (!_iniciado) { _timerCountdown.Stop(); return; }
                TimeSpan rest = _proximoCiclo - DateTime.Now;
                lblEstadoProximo.Text = rest.TotalSeconds <= 0
                    ? "Próximo ciclo: ejecutando..."
                    : $"Próximo ciclo en:  {(int)rest.TotalSeconds}s";
            };
            _timerCountdown.Start();
        }

        // ═════════════════════════════════════════════════════════════════════
        // PROCESAR BLOQUES
        // ═════════════════════════════════════════════════════════════════════
        private void ProcesarBloques(DataTable dt, out int nuevos, out int skip, out int errores)
        {
            nuevos  = 0;
            skip    = 0;
            errores = 0;

            string codAlmacen = BuscarPrimerCodigoAlmacen();
            string fechaApi   = DateTime.Today.ToString("yyyy-MM-dd");
            string fechaSDK   = DateTime.Today.ToString("MM/dd/yyyy");

            int i = 0;
            while (i < dt.Rows.Count)
            {
                string codCte = dt.Rows[i][0].ToString().Trim();
                if (string.IsNullOrEmpty(codCte)) { i++; continue; }

                int desde = i, hasta = i;
                while (hasta + 1 < dt.Rows.Count &&
                       !string.IsNullOrEmpty(dt.Rows[hasta + 1][0].ToString().Trim()))
                    hasta++;

                string idOrden = dt.Columns.Count > 8
                    ? dt.Rows[desde][8].ToString().Trim() : "";

                try
                {
                    if (!string.IsNullOrEmpty(idOrden) && RegistroProcesados.YaFueProcesado(idOrden))
                    {
                        skip++;
                        EscribirLog($"[{DateTime.Now:HH:mm:ss}] SKIP  — ID {idOrden} | {codCte}");
                        i = hasta + 1;
                        continue;
                    }
                }
                catch (Exception exCheck) { EscribirLog($"[ERROR] YaFueProcesado: {exCheck.Message}"); }

                if (BuscarIdClienteSQL(codCte) <= 0)
                {
                    errores++;
                    EscribirLog($"[{DateTime.Now:HH:mm:ss}] ERROR — Cliente '{codCte}' no encontrado");
                    i = hasta + 1;
                    continue;
                }

                try
                {
                    int idDoc = CrearDocumento(dt, desde, hasta, codCte, codAlmacen, fechaSDK);
                    if (idDoc > 0)
                    {
                        nuevos++;
                        EscribirLog($"[{DateTime.Now:HH:mm:ss}] OK    — ID {idOrden} | {codCte} | Doc #{idDoc}");
                        try
                        {
                            if (!string.IsNullOrEmpty(idOrden))
                                RegistroProcesados.MarcarProcesado(idOrden, fechaApi, codCte, idDoc);
                        }
                        catch (Exception exReg) { EscribirLog($"[ERROR] MarcarProcesado: {exReg.Message}"); }
                    }
                    else
                    {
                        errores++;
                        EscribirLog($"[{DateTime.Now:HH:mm:ss}] ERROR — ID {idOrden} | {codCte} | SDK={idDoc}");
                    }
                }
                catch (Exception ex)
                {
                    try { SdkComercial.fCancelarModificacionDocumento(); } catch { }
                    errores++;
                    EscribirLog($"[{DateTime.Now:HH:mm:ss}] ERROR — ID {idOrden} | {codCte} | {ex.Message}");
                }

                i = hasta + 1;
            }

            try { RegistroProcesados.LimpiarAntiguos(); } catch { }
        }

        // ═════════════════════════════════════════════════════════════════════
        // CREAR DOCUMENTO EN SDK
        // ═════════════════════════════════════════════════════════════════════
        private int CrearDocumento(DataTable dt, int desde, int hasta,
                                   string codCte, string codAlmacen, string fecha)
        {
            string serie  = dt.Columns.Count > 6 ? dt.Rows[desde][6].ToString().Trim() : "";
            string obsDoc = dt.Columns.Count > 4 ? dt.Rows[desde][4].ToString().Trim() : "";

            string nombreConcepto = string.IsNullOrEmpty(serie)
                ? "Pedido"
                : "Pedido " + serie.ToUpper();
            string codConcepto = BuscarCodConceptoPorNombre(nombreConcepto);
            if (string.IsNullOrEmpty(codConcepto))
                throw new Exception($"Concepto '{nombreConcepto}' no encontrado en admConceptos.");

            var doc = new tDocumento
            {
                aFolio         = 0,
                aNumMoneda     = 1,
                aTipoCambio    = 1.0,
                aImporte       = 0,
                aDescuentoDoc1 = 0,
                aDescuentoDoc2 = 0,
                aSistemaOrigen = 205, // CSISTORIG del concepto en admConceptos
                aCodConcepto   = codConcepto,
                aSerie         = "", // Debe ser vacío: el SDK usa CSERIEPOROMISION del concepto. Pasar la serie causa 130101.
                aFecha         = fecha,
                aCodigoCteProv = codCte,
                aCodigoAgente  = "",
                aReferencia    = "",
                aAfecta        = 1,
                aGasto1        = 0,
                aGasto2        = 0,
                aGasto3        = 0
            };

            // aFolio=0: el SDK asigna el folio automáticamente según la configuración del concepto.
            // Este comportamiento funcionaba antes de agregar el sync y debe mantenerse igual.
            EscribirLog($"[{DateTime.Now:HH:mm:ss}] fAltaDoc → concepto='{codConcepto}' serie='{serie}' cte='{codCte}'");

            int idDocumento = 0;
            int resAlta = SdkComercial.fAltaDocumento(ref idDocumento, ref doc);
            if (resAlta != 0)
                throw new Exception($"fAltaDocumento [{resAlta}]: " + SdkComercial.DescribirError(resAlta));

            if (!string.IsNullOrEmpty(obsDoc))
                SdkComercial.fSetDatoDocumento("COBSERVACIONES", obsDoc);

            int consecutivo = 1;
            for (int i = desde; i <= hasta; i++)
            {
                var row    = dt.Rows[i];
                string cod = dt.Columns.Count > 1 ? row[1].ToString().Trim() : "";
                if (string.IsNullOrEmpty(cod)) continue;

                double.TryParse(
                    dt.Columns.Count > 2 ? row[2].ToString().Replace(",", ".") : "1",
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out double unidades);
                if (unidades <= 0) unidades = 1;

                string rawPrecio = dt.Columns.Count > 5 ? row[5].ToString().Trim() : "0";
                if (!double.TryParse(rawPrecio.Replace(",", "."),
                        System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out double precio))
                    precio = 0;

                string obsMov = dt.Columns.Count > 3 ? row[3].ToString().Trim() : "";

                var mov = new tMovimiento
                {
                    aConsecutivo      = consecutivo++,
                    aUnidades         = unidades,
                    aPrecio           = precio,
                    aCosto            = precio,
                    aCodProdSer       = cod,
                    aCodAlmacen       = codAlmacen,
                    aReferencia       = "",
                    aCodClasificacion = ""
                };

                int idMov  = 0;
                int resMov = SdkComercial.fAltaMovimiento(idDocumento, ref idMov, ref mov);

                // 135617 = producto inactivo → activarlo y reintentar automáticamente
                if (resMov == 135617 || resMov == -135617)
                {
                    EscribirLog($"[{DateTime.Now:HH:mm:ss}] Producto '{cod}' inactivo → activando...");
                    if (ActivarProducto(cod))
                    {
                        idMov  = 0;
                        resMov = SdkComercial.fAltaMovimiento(idDocumento, ref idMov, ref mov);
                        if (resMov == 0)
                            EscribirLog($"[{DateTime.Now:HH:mm:ss}] Producto '{cod}' activado y movimiento OK.");
                    }
                }

                if (resMov != 0)
                {
                    try { SdkComercial.fCancelarModificacionDocumento(); } catch { }
                    throw new Exception($"fAltaMovimiento '{cod}': {SdkComercial.DescribirError(resMov)}");
                }

                if (!string.IsNullOrEmpty(obsMov))
                    SdkComercial.fSetDatoMovimiento("COBSERVAMOV", obsMov);

                int resGM = SdkComercial.fGuardaMovimiento();
                if (resGM < 0)
                {
                    try { SdkComercial.fCancelarModificacionDocumento(); } catch { }
                    throw new Exception($"fGuardaMovimiento '{cod}': {SdkComercial.DescribirError(resGM)}");
                }
            }

            int resFinal = SdkComercial.fGuardaDocumento();
            if (resFinal < 0)
            {
                try { SdkComercial.fCancelarModificacionDocumento(); } catch { }
                throw new Exception("fGuardaDocumento: " + SdkComercial.DescribirError(resFinal));
            }

            if (idDocumento <= 0)
            {
                string sId = SdkComercial.LeerCampoDocumento("CIDDOCUMENTO");
                int.TryParse(sId, out idDocumento);
                if (idDocumento <= 0) idDocumento = 1;
            }

            return idDocumento;
        }

        // ═════════════════════════════════════════════════════════════════════
        // ACTUALIZAR RESUMEN
        // ═════════════════════════════════════════════════════════════════════
        private void ActualizarResumen(int nuevos, int skip, int errores)
        {
            lblNumSubidos.Text      = nuevos.ToString();
            lblNumSubidos.ForeColor = nuevos > 0 ? UITheme.Accent : UITheme.TextMuted;

            lblNumSkip.Text      = skip.ToString();
            lblNumSkip.ForeColor = UITheme.TextDim;

            lblNumErrores.Text      = errores.ToString();
            lblNumErrores.ForeColor = errores > 0 ? UITheme.Danger : UITheme.TextMuted;

            lblEstadoHora.Text      = $"Último ciclo:  {DateTime.Now:HH:mm:ss}";
            lblEstadoHora.ForeColor = UITheme.TextDim;

            lblEstadoLog.Text = "Log: " + RutaLogHoy();

            // Solo loguear resumen si hubo actividad real
            if (nuevos > 0 || errores > 0)
                EscribirLog($"[{DateTime.Now:HH:mm:ss}] ── Resumen: {nuevos} subidos, {skip} omitidos, {errores} errores.");
        }

        // ═════════════════════════════════════════════════════════════════════
        // HELPERS
        // ═════════════════════════════════════════════════════════════════════
        /// <summary>
        /// Activa un producto inactivo en CONTPAQi vía SDK.
        /// Se llama automáticamente cuando fAltaMovimiento devuelve error 135617.
        /// Retorna true si el producto quedó activo (ya lo estaba o se activó exitosamente).
        /// </summary>
        private bool ActivarProducto(string codProducto)
        {
            try
            {
                // 1. Obtener el ID interno del producto y verificar su estado
                int idProd = 0;
                using (var conn = new SqlConnection(Program.ConnStrEmpresa))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(
                        "SELECT CIDPRODUCTO, ISNULL(CSTATUSPRODUCTO,0) FROM admProductos " +
                        "WHERE CCODIGOPRODUCTO=@cod", conn))
                    {
                        cmd.Parameters.AddWithValue("@cod", codProducto);
                        using (var r = cmd.ExecuteReader())
                        {
                            if (!r.Read()) return false;
                            idProd = Convert.ToInt32(r[0]);
                            if (Convert.ToInt32(r[1]) == 1) return true; // 1=activo en admProductos, ya no necesita activación
                        }
                    }
                }

                if (idProd <= 0) return false;

                // 2. Editar y activar el producto vía SDK (requiere UI thread)
                int resEdita = SdkComercial.fEditaProducto(idProd);
                if (resEdita != 0) return false;

                SdkComercial.fSetDatoProducto("CSTATUSPRODUCTO", "1"); // 1=activo en CONTPAQi Comercial

                int resGuarda = SdkComercial.fGuardaProducto();
                if (resGuarda != 0)
                {
                    try { SdkComercial.fCancelarModificacionProducto(); } catch { }
                    return false;
                }
                return true;
            }
            catch { return false; }
        }

        private int BuscarIdClienteSQL(string cod)
        {
            if (string.IsNullOrEmpty(Program.ConnStrEmpresa)) return 0;
            try
            {
                using (var conn = new SqlConnection(Program.ConnStrEmpresa))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(
                        "SELECT CIDCLIENTEPROVEEDOR FROM admClientes WHERE CCODIGOCLIENTE = @cod", conn))
                    {
                        cmd.Parameters.AddWithValue("@cod", cod);
                        object val = cmd.ExecuteScalar();
                        return val == null || val == DBNull.Value ? 0 : Convert.ToInt32(val);
                    }
                }
            }
            catch { return 0; }
        }


        private string RutaDescargaLocal(string sufijo)
        {
            string carpeta = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            if (!Directory.Exists(carpeta)) Directory.CreateDirectory(carpeta);
            return Path.Combine(carpeta, $"PedidosCEG_{sufijo}.xlsx");
        }

        private void DescargarArchivo(string url, string rutaLocal)
        {
            using (var wc = new WebClient())
            {
                wc.Headers.Add("X-CEG-API-Key", API_KEY);
                wc.DownloadFile(url, rutaLocal);
            }
        }

        private DataTable LeerExcel(string ruta)
        {
            var dt = new DataTable();
            if (!File.Exists(ruta)) return dt;
            using (var pkg = new ExcelPackage(new FileInfo(ruta)))
            {
                var ws = pkg.Workbook.Worksheets[1];
                if (ws.Dimension == null) return dt;
                int cols = ws.Dimension.Columns;
                int rows = ws.Dimension.Rows;

                for (int c = 1; c <= cols; c++)
                    dt.Columns.Add(ws.Cells[1, c].Text ?? ("Col" + c));

                int ultima = 1;
                for (int r = 2; r <= rows; r++)
                    for (int c = 1; c <= cols; c++)
                        if (!string.IsNullOrEmpty(ws.Cells[r, c].Text)) { ultima = r; break; }

                for (int r = 2; r <= ultima; r++)
                {
                    var row = dt.NewRow();
                    for (int c = 1; c <= cols; c++) row[c - 1] = ws.Cells[r, c].Text ?? "";
                    dt.Rows.Add(row);
                }
            }
            return dt;
        }

        private string RutaLogHoy()
        {
            if (!Directory.Exists(CarpetaLog)) Directory.CreateDirectory(CarpetaLog);
            return Path.Combine(CarpetaLog, $"Log_{DateTime.Today:yyyy-MM-dd}.txt");
        }

        private void EscribirLog(string mensaje)
        {
            try { File.AppendAllText(RutaLogHoy(), mensaje + Environment.NewLine, Encoding.UTF8); }
            catch { }
        }

        private void BtnSalir_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("¿Deseas cerrar la empresa y salir?", "Confirmar",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                this.Close();
        }

        private void FrmControl_FormClosed(object sender, FormClosedEventArgs e)
        {
            _timerCiclo?.Stop();
            _timerCountdown?.Stop();
            _timerPulso?.Stop();
            SdkComercial.fCierraEmpresa();
            Application.Exit();
        }

        private class ItemCombo
        {
            public int    Id   { get; }
            public string Code { get; }
            private string Nombre { get; }
            public ItemCombo(int id, string nombre, string code = "")
            { Id = id; Code = code ?? ""; Nombre = nombre; }
            public override string ToString() => Nombre;
        }
    }

    internal static class GraphicsExtensions
    {
        public static void DrawRoundedRectangle(this Graphics g, Pen pen, Rectangle rect, int radius)
        {
            int d = radius * 2;
            using (var path = new System.Drawing.Drawing2D.GraphicsPath())
            {
                path.AddArc(rect.X,         rect.Y,          d, d, 180, 90);
                path.AddArc(rect.Right - d, rect.Y,          d, d, 270, 90);
                path.AddArc(rect.Right - d, rect.Bottom - d, d, d,   0, 90);
                path.AddArc(rect.X,         rect.Bottom - d, d, d,  90, 90);
                path.CloseFigure();
                g.DrawPath(pen, path);
            }
        }
    }
}
