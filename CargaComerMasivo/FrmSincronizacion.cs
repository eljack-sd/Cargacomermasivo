using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CargaComerMasivo
{
    /// <summary>
    /// Formulario de sincronización manual de catálogo.
    /// Flujo: Extrae CONTPAQi → POST API → Crea faltantes en CONTPAQi.
    /// El botón es manual — NO se ejecuta automáticamente.
    /// </summary>
    internal class FrmSincronizacion : Form
    {
        // ── Controles ─────────────────────────────────────────────────────────
        private Label       lblNumProd;
        private Label       lblNumCli;
        private Label       lblNumErr;
        private Label       lblStatus;
        private RichTextBox rtbLog;
        private ProgressBar pbProgreso;
        private Button      btnSincronizar;
        private Button      btnDiagnosticar;
        private Button      btnCerrar;

        private bool _corriendo = false;

        // ═════════════════════════════════════════════════════════════════════
        public FrmSincronizacion()
        {
            ConstruirUI();
            UITheme.AplicarIconoForm(this);
        }

        // ═════════════════════════════════════════════════════════════════════
        // UI
        // ═════════════════════════════════════════════════════════════════════
        private void ConstruirUI()
        {
            Text            = "Sincronización de Catálogo";
            Size            = new Size(740, 580);
            MinimumSize     = new Size(740, 580);
            MaximumSize     = new Size(740, 580);
            StartPosition   = FormStartPosition.CenterParent;
            BackColor       = UITheme.BgForm;
            ForeColor       = UITheme.TextMain;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox     = false;
            Font            = UITheme.FntDefault;

            // ── HEADER ────────────────────────────────────────────────────────
            var pnlHeader = new Panel { Dock = DockStyle.Top, Height = 72, BackColor = UITheme.BgHeader };
            pnlHeader.Paint += (s, e) =>
            {
                using (var pen = new Pen(UITheme.BorderHi, 1))
                    e.Graphics.DrawLine(pen, 0, pnlHeader.Height - 1, pnlHeader.Width, pnlHeader.Height - 1);
            };
            pnlHeader.Controls.Add(new Label
            {
                Text = "Sincronización de Catálogo", ForeColor = UITheme.TextMain,
                Font = UITheme.FntTitle, AutoSize = true, Location = new Point(20, 13)
            });
            pnlHeader.Controls.Add(new Label
            {
                Text      = "Extrae catálogo de CONTPAQi  ·  Envía a API  ·  Crea los faltantes",
                ForeColor = UITheme.TextMuted, Font = new Font("Segoe UI", 8f),
                AutoSize  = true, Location = new Point(22, 48)
            });

            // ── STAT CARDS ────────────────────────────────────────────────────
            int cW = 214, cH = 74, cY = 86;
            Panel pProd, pCli, pErr;
            lblNumProd = MkCard(20,               cY, cW, cH, "PRODUCTOS CREADOS", UITheme.Accent,  out pProd);
            lblNumCli  = MkCard(20 + cW + 14,     cY, cW, cH, "CLIENTES CREADOS",  UITheme.Success, out pCli);
            lblNumErr  = MkCard(20 + (cW + 14)*2, cY, cW, cH, "ERRORES",           UITheme.Danger,  out pErr);

            // ── STATUS ────────────────────────────────────────────────────────
            lblStatus = new Label
            {
                Text      = "Presiona  Sincronizar  para iniciar.",
                ForeColor = UITheme.TextDim, Font = UITheme.FntSm,
                AutoSize  = true, Location = new Point(20, cY + cH + 12)
            };

            // ── LOG ───────────────────────────────────────────────────────────
            rtbLog = new RichTextBox
            {
                Location    = new Point(20, cY + cH + 34),
                Size        = new Size(700, 290),
                BackColor   = UITheme.BgCard,
                ForeColor   = UITheme.TextDim,
                Font        = new Font("Courier New", 8.5f),
                ReadOnly    = true,
                BorderStyle = BorderStyle.None,
                ScrollBars  = RichTextBoxScrollBars.Vertical,
                WordWrap    = false
            };

            // ── PROGRESS BAR ──────────────────────────────────────────────────
            pbProgreso = new ProgressBar
            {
                Location              = new Point(20, cY + cH + 332),
                Size                  = new Size(700, 3),
                Style                 = ProgressBarStyle.Marquee,
                MarqueeAnimationSpeed = 20,
                Visible               = false
            };

            // ── BOTONES ───────────────────────────────────────────────────────
            btnSincronizar = new Button
            {
                Text = "▶  Sincronizar Ahora",
                Size = new Size(180, 38), Location = new Point(20, cY + cH + 343)
            };
            UITheme.StyleBtn(btnSincronizar, UITheme.Accent, large: true);
            btnSincronizar.Click += BtnSincronizar_Click;

            btnDiagnosticar = new Button
            {
                Text = "🔍  Diagnosticar 400",
                Size = new Size(160, 38), Location = new Point(210, cY + cH + 343)
            };
            UITheme.StyleBtn(btnDiagnosticar, UITheme.Warning, large: true);
            btnDiagnosticar.Click += BtnDiagnosticar_Click;

            btnCerrar = new Button
            {
                Text = "Cerrar",
                Size = new Size(90, 38), Location = new Point(630, cY + cH + 343)
            };
            UITheme.StyleBtn(btnCerrar, UITheme.Neutral, large: true);
            btnCerrar.Click += (s, e) => { if (!_corriendo) Close(); };

            // ── FOOTER ────────────────────────────────────────────────────────
            var pnlFooter = new Panel { Dock = DockStyle.Bottom, Height = 28, BackColor = UITheme.BgHeader };
            pnlFooter.Paint += (s, e) =>
            {
                using (var pen = new Pen(UITheme.Border, 1))
                    e.Graphics.DrawLine(pen, 0, 0, pnlFooter.Width, 0);
            };
            pnlFooter.Controls.Add(new Label
            {
                Text      = "Los elementos faltantes se crean directamente en CONTPAQi  ·  Proceso manual",
                ForeColor = UITheme.TextMuted, Font = UITheme.FntSection,
                AutoSize  = true, Location = new Point(14, 8)
            });

            // ── ENSAMBLAR ─────────────────────────────────────────────────────
            Controls.AddRange(new Control[]
            {
                pnlHeader, pProd, pCli, pErr,
                lblStatus, rtbLog, pbProgreso,
                btnSincronizar, btnDiagnosticar, btnCerrar, pnlFooter
            });
        }

        // ─── Stat card helper (local — evita dependencia de FrmControl) ───────
        private Label MkCard(int x, int y, int w, int h, string titulo, Color color, out Panel panel)
        {
            var p = new Panel { Location = new Point(x, y), Size = new Size(w, h), BackColor = UITheme.BgCardAlt };
            p.Paint += (s, e) =>
            {
                using (var pen = new Pen(UITheme.Border, 1))
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    e.Graphics.DrawRoundedRectangle(pen, new Rectangle(0, 0, p.Width - 1, p.Height - 1), 4);
                }
            };
            var lNum = new Label
            {
                Text = "—", ForeColor = UITheme.TextMuted, Font = UITheme.FntHuge,
                AutoSize = false, Size = new Size(w, 46), Location = new Point(0, 4),
                TextAlign = ContentAlignment.MiddleCenter
            };
            var lTit = new Label
            {
                Text = titulo, ForeColor = UITheme.TextMuted, Font = UITheme.FntSection,
                AutoSize = false, Size = new Size(w, 18), Location = new Point(0, h - 20),
                TextAlign = ContentAlignment.MiddleCenter
            };
            p.Controls.AddRange(new Control[] { lNum, lTit });
            panel = p;
            return lNum;
        }

        // ═════════════════════════════════════════════════════════════════════
        // SINCRONIZAR
        // ═════════════════════════════════════════════════════════════════════
        private async void BtnSincronizar_Click(object sender, EventArgs e)
        {
            _corriendo             = true;
            btnSincronizar.Enabled = false;
            btnCerrar.Enabled      = false;
            btnDiagnosticar.Enabled = false;
            pbProgreso.Visible     = true;
            rtbLog.Clear();
            ResetCards();

            int prodCreados = 0, cliCreados = 0, errores = 0;

            try
            {
                // ── PASO 1: Extraer catálogo del SDK (UI thread obligatorio) ──
                SetStatus("Paso 1 / 3 — Extrayendo catálogo de CONTPAQi...");
                List<ProductoItem> productos;
                List<ClienteItem>  clientes;
                try
                {
                    productos = SincronizadorCatalogo.ExtraerProductos();
                    clientes  = SincronizadorCatalogo.ExtraerClientes();
                    Log($"[{Ts()}] Catálogo extraído: {productos.Count} productos,  {clientes.Count} clientes.");
                }
                catch (Exception ex)
                {
                    Log($"[{Ts()}] ERROR extrayendo catálogo: {ex.Message}", UITheme.Danger);
                    return;
                }

                // ── PASO 2: POST a la API (Task.Run — red, no SDK) ────────────
                SetStatus("Paso 2 / 3 — Enviando catálogo a API...");
                SyncResponse respuesta;
                try
                {
                    respuesta = await Task.Run(() =>
                        SincronizadorCatalogo.EnviarCatalogo(productos, clientes,
                            msg => BeginInvoke((Action)(() => Log(msg)))));

                    int nP = respuesta?.ProductosFaltantes?.Count ?? 0;
                    int nC = respuesta?.ClientesFaltantes?.Count  ?? 0;
                    Log($"[{Ts()}] API responde: {nP} productos faltantes,  {nC} clientes faltantes.");
                    Log("");
                }
                catch (Exception ex)
                {
                    Log($"[{Ts()}] ERROR llamando API: {ex.Message}", UITheme.Danger);
                    return;
                }

                // ── PASO 3: Crear faltantes en CONTPAQi (UI thread) ───────────
                SetStatus("Paso 3 / 3 — Creando elementos faltantes en CONTPAQi...");

                // Productos
                if (respuesta?.ProductosFaltantes != null && respuesta.ProductosFaltantes.Count > 0)
                {
                    Log($"[{Ts()}] ── Productos ({respuesta.ProductosFaltantes.Count}) ──────────────────",
                        UITheme.AccentDim);
                    foreach (var prod in respuesta.ProductosFaltantes)
                    {
                        if (SincronizadorCatalogo.CrearProducto(prod, out string err))
                        {
                            prodCreados++;
                            Log($"[{Ts()}]  + {prod.Codigo,-14} {prod.Nombre}", UITheme.Accent);
                        }
                        else
                        {
                            errores++;
                            Log($"[{Ts()}]  ✗ {prod.Codigo,-14} {err}", UITheme.Danger);
                        }
                        // Refrescar UI cada 5 items
                        if ((prodCreados + errores) % 5 == 0)
                        {
                            lblNumProd.Text      = prodCreados.ToString();
                            lblNumProd.ForeColor = UITheme.Accent;
                            lblNumErr.Text       = errores.ToString();
                            lblNumErr.ForeColor  = errores > 0 ? UITheme.Danger : UITheme.TextMuted;
                            Application.DoEvents();
                        }
                    }
                }

                // Clientes
                if (respuesta?.ClientesFaltantes != null && respuesta.ClientesFaltantes.Count > 0)
                {
                    Log("");
                    Log($"[{Ts()}] ── Clientes ({respuesta.ClientesFaltantes.Count}) ──────────────────",
                        UITheme.AccentDim);
                    foreach (var cli in respuesta.ClientesFaltantes)
                    {
                        if (SincronizadorCatalogo.CrearCliente(cli, out string err))
                        {
                            cliCreados++;
                            Log($"[{Ts()}]  + {cli.Codigo,-14} {cli.RazonSocial}", UITheme.Success);
                        }
                        else
                        {
                            errores++;
                            Log($"[{Ts()}]  ✗ {cli.Codigo,-14} {err}", UITheme.Danger);
                        }
                        if ((cliCreados + errores) % 5 == 0)
                        {
                            lblNumCli.Text      = cliCreados.ToString();
                            lblNumCli.ForeColor = UITheme.Success;
                            lblNumErr.Text      = errores.ToString();
                            lblNumErr.ForeColor = errores > 0 ? UITheme.Danger : UITheme.TextMuted;
                            Application.DoEvents();
                        }
                    }
                }
            }
            finally
            {
                // Stat cards finales
                lblNumProd.Text      = prodCreados.ToString();
                lblNumProd.ForeColor = prodCreados > 0 ? UITheme.Accent  : UITheme.TextMuted;
                lblNumCli.Text       = cliCreados.ToString();
                lblNumCli.ForeColor  = cliCreados  > 0 ? UITheme.Success : UITheme.TextMuted;
                lblNumErr.Text       = errores.ToString();
                lblNumErr.ForeColor  = errores      > 0 ? UITheme.Danger  : UITheme.TextMuted;

                Log("");
                Log($"[{Ts()}] ═══ Completado: {prodCreados} productos, {cliCreados} clientes creados, {errores} errores. ═══",
                    errores > 0 ? UITheme.Warning : UITheme.Accent);

                SetStatus($"Completado — {prodCreados} productos y {cliCreados} clientes creados,  {errores} errores.");
                pbProgreso.Visible     = false;
                btnSincronizar.Enabled  = true;
                btnDiagnosticar.Enabled = true;
                btnCerrar.Enabled       = true;
                _corriendo              = false;
            }
        }

        // ═════════════════════════════════════════════════════════════════════
        // DIAGNOSTICAR (búsqueda binaria para encontrar el registro con 400)
        // ═════════════════════════════════════════════════════════════════════
        private async void BtnDiagnosticar_Click(object sender, EventArgs e)
        {
            _corriendo              = true;
            btnSincronizar.Enabled  = false;
            btnDiagnosticar.Enabled = false;
            btnCerrar.Enabled       = false;
            pbProgreso.Visible      = true;
            rtbLog.Clear();
            ResetCards();

            SetStatus("Diagnóstico — extrayendo catálogo...");
            List<ProductoItem> productos;
            List<ClienteItem>  clientes;
            try
            {
                productos = SincronizadorCatalogo.ExtraerProductos();
                clientes  = SincronizadorCatalogo.ExtraerClientes();
                Log($"[{Ts()}] Catálogo: {productos.Count} productos, {clientes.Count} clientes.");
            }
            catch (Exception ex)
            {
                Log($"[{Ts()}] ERROR extrayendo catálogo: {ex.Message}", UITheme.Danger);
                pbProgreso.Visible      = false;
                btnSincronizar.Enabled  = true;
                btnDiagnosticar.Enabled = true;
                btnCerrar.Enabled       = true;
                _corriendo              = false;
                return;
            }

            SetStatus("Diagnóstico — búsqueda binaria en curso (puede tomar varios minutos)...");
            Log($"[{Ts()}] Iniciando búsqueda binaria...", UITheme.Warning);
            Log($"[{Ts()}] Cada petición tarda ~2-5s — con {productos.Count} productos y {clientes.Count} clientes");
            Log($"[{Ts()}] espera aprox. {(int)((Math.Log(productos.Count, 2) + Math.Log(clientes.Count, 2)) * 4)} segundos.");
            Log("");

            string resultado;
            try
            {
                resultado = await Task.Run(() =>
                    SincronizadorCatalogo.DiagnosticarBinarySearch(productos, clientes,
                        msg => BeginInvoke((Action)(() => Log(msg, UITheme.TextDim)))));
            }
            catch (Exception ex)
            {
                Log($"[{Ts()}] ERROR durante diagnóstico: {ex.Message}", UITheme.Danger);
                resultado = null;
            }

            Log("");
            if (resultado != null)
            {
                Log($"[{Ts()}] ══════════════════════════════════════════", UITheme.Warning);
                Log($"[{Ts()}] RESULTADO:", UITheme.Warning);
                foreach (var linea in resultado.Split('\n'))
                    Log($"  {linea}", UITheme.Danger);
                Log($"[{Ts()}] ══════════════════════════════════════════", UITheme.Warning);
                SetStatus("Diagnóstico completado — registro problemático encontrado.");
            }
            else
            {
                Log($"[{Ts()}] No se encontró registro problemático.", UITheme.Accent);
                SetStatus("Diagnóstico completado — sin registro problemático identificado.");
            }

            pbProgreso.Visible      = false;
            btnSincronizar.Enabled  = true;
            btnDiagnosticar.Enabled = true;
            btnCerrar.Enabled       = true;
            _corriendo              = false;
        }

        // ── Helpers ───────────────────────────────────────────────────────────
        private void SetStatus(string msg) { lblStatus.Text = msg; Application.DoEvents(); }

        private void ResetCards()
        {
            lblNumProd.Text = "—"; lblNumProd.ForeColor = UITheme.TextMuted;
            lblNumCli.Text  = "—"; lblNumCli.ForeColor  = UITheme.TextMuted;
            lblNumErr.Text  = "—"; lblNumErr.ForeColor  = UITheme.TextMuted;
        }

        private void Log(string msg, Color? color = null)
        {
            rtbLog.SuspendLayout();
            int sel = rtbLog.TextLength;
            rtbLog.AppendText(msg + "\n");
            rtbLog.Select(sel, msg.Length);
            rtbLog.SelectionColor = color ?? UITheme.TextDim;
            rtbLog.SelectionLength = 0;
            rtbLog.ScrollToCaret();
            rtbLog.ResumeLayout();
        }

        private string Ts() => DateTime.Now.ToString("HH:mm:ss");
    }
}
