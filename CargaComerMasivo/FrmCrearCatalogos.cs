using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace CargaComerMasivo
{
    /// <summary>
    /// Muestra los clientes y productos que faltan en CONTPAQi,
    /// pide confirmación y los crea vía SDK. El log se guarda en un
    /// archivo .txt que se abre automáticamente en el Bloc de notas.
    /// </summary>
    internal class FrmCrearCatalogos : Form
    {
        // ── DTOs ──────────────────────────────────────────────────────────────
        internal class DatoCliente
        {
            public string Rfc    { get; set; }
            public string Nombre { get; set; }
        }

        internal class DatoProducto
        {
            public string Clave       { get; set; }
            public string Descripcion { get; set; }
            public string Unidad      { get; set; }
        }

        // ── Controles ─────────────────────────────────────────────────────────
        private DataGridView dgvClientes;
        private DataGridView dgvProductos;
        private Button       btnCrear;
        private Button       btnCerrar;
        private Label        lblEstado;

        private readonly List<DatoCliente>  _clientes;
        private readonly List<DatoProducto> _productos;
        private bool          _creacionRealizada = false;
        private StringBuilder _logSb             = new StringBuilder();

        // ─────────────────────────────────────────────────────────────────────
        public FrmCrearCatalogos(List<DatoCliente> clientes, List<DatoProducto> productos)
        {
            _clientes  = clientes  ?? new List<DatoCliente>();
            _productos = productos ?? new List<DatoProducto>();
            ConstruirUI();
            CargarGrids();
        }

        // ═════════════════════════════════════════════════════════════════════
        // UI
        // ═════════════════════════════════════════════════════════════════════
        private void ConstruirUI()
        {
            var clrDark   = Color.FromArgb(26, 31, 46);
            var clrBorder = Color.FromArgb(220, 225, 235);
            var clrHdr    = Color.FromArgb(248, 249, 252);
            var clrMuted  = Color.FromArgb(100, 110, 130);
            var clrGreen  = Color.FromArgb(22, 163, 74);
            var clrInput  = Color.FromArgb(245, 247, 250);
            var fnt8b     = new Font("Segoe UI", 8f, FontStyle.Bold);
            var fnt9      = new Font("Segoe UI", 9f);
            var fnt9b     = new Font("Segoe UI", 9f, FontStyle.Bold);
            var fnt10b    = new Font("Segoe UI", 10f, FontStyle.Bold);

            Text          = "Registros faltantes — Crear en CONTPAQi";
            ClientSize    = new Size(860, 560);
            MinimumSize   = new Size(700, 480);
            StartPosition = FormStartPosition.CenterParent;
            BackColor     = Color.FromArgb(238, 241, 247);
            Font          = fnt9;

            // ── Panel botones (Bottom) ────────────────────────────────────────
            var pnlBtns = new Panel
            {
                Dock      = DockStyle.Bottom,
                Height    = 58,
                BackColor = Color.FromArgb(242, 244, 248)
            };

            // Etiqueta de estado (dentro del panel de botones, a la izquierda)
            lblEstado = new Label
            {
                Text      = "",
                Font      = fnt9,
                ForeColor = clrMuted,
                AutoSize  = false,
                Size      = new Size(400, 38),
                Location  = new Point(12, 10),
                TextAlign = ContentAlignment.MiddleLeft
            };

            btnCrear = new Button
            {
                Text      = "✔  Crear todo",
                Font      = fnt10b,
                Size      = new Size(164, 38),
                FlatStyle = FlatStyle.Flat,
                BackColor = clrGreen,
                ForeColor = Color.White,
                Cursor    = Cursors.Hand,
                Anchor    = AnchorStyles.Top | AnchorStyles.Right,
                Location  = new Point(580, 10)
            };
            btnCrear.FlatAppearance.BorderColor = clrGreen;
            btnCrear.Click += BtnCrear_Click;

            btnCerrar = new Button
            {
                Text      = "Cancelar",
                Font      = fnt9,
                Size      = new Size(100, 38),
                FlatStyle = FlatStyle.Flat,
                BackColor = clrInput,
                ForeColor = clrDark,
                Cursor    = Cursors.Hand,
                Anchor    = AnchorStyles.Top | AnchorStyles.Right,
                Location  = new Point(748, 10)
            };
            btnCerrar.FlatAppearance.BorderColor = clrBorder;
            btnCerrar.Click += (s, e) =>
            {
                DialogResult = _creacionRealizada ? DialogResult.OK : DialogResult.Cancel;
                Close();
            };

            pnlBtns.Controls.AddRange(new Control[] { lblEstado, btnCrear, btnCerrar });
            pnlBtns.Resize += (s, e) =>
            {
                btnCerrar.Left = pnlBtns.Width - btnCerrar.Width - 12;
                btnCrear.Left  = btnCerrar.Left - btnCrear.Width - 8;
            };
            Controls.Add(pnlBtns);

            // ── Panel principal: info + grids ─────────────────────────────────
            //   info(46) + cli(200) + prod(200) + márgenes = ~498px mínimo
            var pnlTop = new Panel
            {
                Dock      = DockStyle.Fill,
                BackColor = Color.Transparent
            };

            // Info
            var pnlInfo = new Panel
            {
                Location    = new Point(12, 12),
                Size        = new Size(836, 46),
                BackColor   = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Anchor      = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            pnlInfo.Controls.Add(new Label
            {
                Text      = $"Se van a crear  {_clientes.Count} cliente(s)  y  {_productos.Count} producto(s)  nuevos en CONTPAQi.",
                Font      = fnt9b,
                ForeColor = clrDark,
                AutoSize  = false,
                Size      = new Size(pnlInfo.Width - 20, 46),
                Location  = new Point(10, 0),
                TextAlign = ContentAlignment.MiddleLeft
            });
            pnlTop.Controls.Add(pnlInfo);

            // Grid clientes
            var pnlCli = CrearPanelGrid(fnt8b, fnt9, clrHdr, clrMuted, clrDark, clrBorder,
                $"CLIENTES A CREAR  ({_clientes.Count})", 70, 200);
            dgvClientes = (DataGridView)pnlCli.Tag;
            dgvClientes.Columns.Add(ColFixed("ColRfc",   "RFC",    150));
            dgvClientes.Columns.Add(ColFill("ColNombre", "Nombre"));
            pnlTop.Controls.Add(pnlCli);

            // Grid productos
            var pnlProd = CrearPanelGrid(fnt8b, fnt9, clrHdr, clrMuted, clrDark, clrBorder,
                $"PRODUCTOS A CREAR  ({_productos.Count})", 282, 200);
            dgvProductos = (DataGridView)pnlProd.Tag;
            dgvProductos.Columns.Add(ColFixed("ColClave",  "Clave SAT",   130));
            dgvProductos.Columns.Add(ColFill("ColDesc",    "Descripción"));
            dgvProductos.Columns.Add(ColFixed("ColUnidad", "Unidad",       80));
            pnlTop.Controls.Add(pnlProd);

            pnlTop.Resize += (s, e) =>
            {
                int pw = pnlTop.Width;
                pnlInfo.Width = pw - 24;
                pnlCli.Width  = pw - 24;
                pnlProd.Width = pw - 24;
            };
            Controls.Add(pnlTop);
        }

        // ─────────────────────────────────────────────────────────────────────
        private Panel CrearPanelGrid(Font fnt8b, Font fnt9, Color clrHdr, Color clrMuted,
            Color clrDark, Color clrBorder, string titulo, int top, int height)
        {
            var panel = new Panel
            {
                Location    = new Point(12, top),
                Size        = new Size(836, height),
                BackColor   = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Anchor      = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            var hdr = new Panel { Dock = DockStyle.Top, Height = 26, BackColor = clrHdr };
            hdr.Controls.Add(new Label
            {
                Text      = titulo,
                Font      = fnt8b, ForeColor = clrMuted,
                AutoSize  = false, Size = new Size(600, 26), Location = new Point(8, 0),
                TextAlign = ContentAlignment.MiddleLeft
            });
            var dgv = new DataGridView
            {
                ReadOnly              = true,
                AllowUserToAddRows    = false,
                AllowUserToResizeRows = false,
                AutoSizeColumnsMode   = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor       = Color.White,
                RowHeadersVisible     = false,
                SelectionMode         = DataGridViewSelectionMode.FullRowSelect,
                BorderStyle           = BorderStyle.None,
                GridColor             = clrBorder,
                Font                  = fnt9,
                Dock                  = DockStyle.Fill
            };
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = clrDark;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(148, 163, 184);
            dgv.ColumnHeadersDefaultCellStyle.Font      = fnt8b;
            dgv.ColumnHeadersHeight                     = 26;
            dgv.ColumnHeadersHeightSizeMode             = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            panel.Controls.AddRange(new Control[] { hdr, dgv });
            panel.Tag = dgv;
            return panel;
        }

        private static DataGridViewTextBoxColumn ColFixed(string name, string header, int width)
            => new DataGridViewTextBoxColumn
            {
                Name = name, HeaderText = header, Width = width,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            };

        private static DataGridViewTextBoxColumn ColFill(string name, string header)
            => new DataGridViewTextBoxColumn
            {
                Name = name, HeaderText = header,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            };

        private void CargarGrids()
        {
            foreach (var c in _clientes)
                dgvClientes.Rows.Add(c.Rfc, c.Nombre);
            foreach (var p in _productos)
                dgvProductos.Rows.Add(p.Clave, p.Descripcion, p.Unidad);
        }

        // ═════════════════════════════════════════════════════════════════════
        // CREAR
        // ═════════════════════════════════════════════════════════════════════
        private void BtnCrear_Click(object sender, EventArgs e)
        {
            btnCrear.Enabled  = false;
            btnCerrar.Enabled = false;
            lblEstado.Text    = "Procesando...";
            _logSb.Clear();
            int okTotal  = 0;
            int errTotal = 0;

            Log($"=== Inicio de creación: {DateTime.Now:yyyy-MM-dd HH:mm:ss} ===");
            Log($"BD empresa: {(string.IsNullOrEmpty(Program.ConnStrEmpresa) ? "(sin conexión)" : "conectada")}");
            Log("");

            // ── Clientes ──────────────────────────────────────────────────────
            if (_clientes.Count > 0)
            {
                Log("─── CLIENTES ────────────────────────────────────────────────────");
                foreach (var c in _clientes)
                {
                    Log($"  Procesando cliente: RFC={c.Rfc}  Nombre={c.Nombre}");
                    try
                    {
                        int res = SdkComercial.fInsertaCteProv();
                        Log($"    fInsertaCteProv() → {res}  {(res == 0 ? "OK" : SdkComercial.DescribirError(res))}");

                        if (res == 0)
                        {
                            int r1 = SdkComercial.fSetDatoCteProv("CCODIGOCLIENTE",      Trunc(c.Rfc,    30));
                            int r2 = SdkComercial.fSetDatoCteProv("CRAZONSOCIAL",        Trunc(c.Nombre, 60));
                            int r3 = SdkComercial.fSetDatoCteProv("CRFC",                Trunc(c.Rfc,    20));
                            int r4 = SdkComercial.fSetDatoCteProv("CTIPOCLIENTE",        "0");
                            int r5 = SdkComercial.fSetDatoCteProv("CESTATUS",            "1");   // 1 = Activo
                            int r6 = SdkComercial.fSetDatoCteProv("CLISTAPRECIOCLIENTE", "1");
                            Log($"    fSetDatoCteProv CCODIGOCLIENTE      → {r1}");
                            Log($"    fSetDatoCteProv CRAZONSOCIAL        → {r2}  (valor: {Trunc(c.Nombre, 60)})");
                            Log($"    fSetDatoCteProv CRFC                → {r3}");
                            Log($"    fSetDatoCteProv CTIPOCLIENTE        → {r4}");
                            Log($"    fSetDatoCteProv CESTATUS            → {r5}");
                            Log($"    fSetDatoCteProv CLISTAPRECIOCLIENTE → {r6}");

                            int resG = SdkComercial.fGuardaCteProv();
                            Log($"    fGuardaCteProv() → {resG}  {(resG == 0 ? "OK" : SdkComercial.DescribirError(resG))}");

                            if (resG == 0)
                            {
                                okTotal++;
                                Log($"    [OK] Cliente creado: {c.Rfc}");
                            }
                            else
                            {
                                errTotal++;
                                SdkComercial.fCancelarModificacionCteProv();
                                Log($"    [ERROR] No se pudo guardar cliente {c.Rfc}: código {resG}");
                            }
                        }
                        else
                        {
                            errTotal++;
                            Log($"    [ERROR] fInsertaCteProv devolvió {res} para {c.Rfc}");
                        }
                    }
                    catch (Exception ex)
                    {
                        errTotal++;
                        Log($"    [EXCEPCION] {c.Rfc}: {ex.GetType().Name}: {ex.Message}");
                        Log($"    {ex.StackTrace}");
                    }
                    Log("");
                }
            }

            // ── Productos ─────────────────────────────────────────────────────
            if (_productos.Count > 0)
            {
                Log("─── PRODUCTOS ───────────────────────────────────────────────────");
                foreach (var p in _productos)
                {
                    Log($"  Procesando producto: Clave={p.Clave}  Desc={p.Descripcion}");
                    try
                    {
                        // fBuscaProducto: 0 = encontrado y posicionado, distinto de 0 = no existe
                        int busca = SdkComercial.fBuscaProducto(p.Clave.Trim());
                        Log($"    fBuscaProducto({p.Clave}) → {busca}  {(busca == 0 ? "YA EXISTE" : "no existe - creando nuevo")}");

                        if (busca == 0)
                        {
                            // Existe (posiblemente inactivo) → editar el producto posicionado
                            // fEditaProducto(0) = editar el producto actualmente posicionado
                            int resEdit = SdkComercial.fEditaProducto(0);
                            Log($"    fEditaProducto(0=posicionado) → {resEdit}  {(resEdit == 0 ? "OK" : SdkComercial.DescribirError(resEdit))}");
                            if (resEdit == 0)
                            {
                                int rSt = SdkComercial.fSetDatoProducto("CSTATUSPRODUCTO", "1");
                                Log($"    fSetDatoProducto CSTATUSPRODUCTO → {rSt}  {(rSt == 0 ? "OK" : SdkComercial.DescribirError(rSt))}");
                                int resG = SdkComercial.fGuardaProducto();
                                Log($"    fGuardaProducto() → {resG}  {(resG == 0 ? "OK" : SdkComercial.DescribirError(resG))}");
                                if (resG == 0) { okTotal++; Log($"    [OK] Producto activado: {p.Clave}"); }
                                else { errTotal++; SdkComercial.fCancelarModificacionProducto(); Log($"    [ERROR] No se pudo activar {p.Clave}: {resG}"); }
                            }
                            else { errTotal++; Log($"    [ERROR] No se pudo editar producto posicionado"); }
                        }
                        else
                        {
                            // No existe → crear nuevo activo
                            int res = SdkComercial.fInsertaProducto();
                            Log($"    fInsertaProducto() → {res}  {(res == 0 ? "OK" : SdkComercial.DescribirError(res))}");

                            if (res == 0)
                            {
                                int r1 = SdkComercial.fSetDatoProducto("CCODIGOPRODUCTO",  Trunc(p.Clave,       30));
                                int r2 = SdkComercial.fSetDatoProducto("CNOMBREPRODUCTO",  Trunc(p.Descripcion, 60));
                                int r3 = SdkComercial.fSetDatoProducto("CCLAVESAT",        Trunc(p.Clave,       30));
                                int r4 = SdkComercial.fSetDatoProducto("CPRECIO1",         "0");
                                int r5 = SdkComercial.fSetDatoProducto("CCOSTOESTANDAR",   "0");
                                int r6 = SdkComercial.fSetDatoProducto("CSTATUSPRODUCTO",  "1");
                                Log($"    fSetDatoProducto CCODIGOPRODUCTO  → {r1}");
                                Log($"    fSetDatoProducto CNOMBREPRODUCTO  → {r2}");
                                Log($"    fSetDatoProducto CCLAVESAT        → {r3}");
                                Log($"    fSetDatoProducto CPRECIO1         → {r4}");
                                Log($"    fSetDatoProducto CCOSTOESTANDAR   → {r5}");
                                Log($"    fSetDatoProducto CSTATUSPRODUCTO  → {r6}  {(r6 == 0 ? "OK" : SdkComercial.DescribirError(r6))}");

                                int resG = SdkComercial.fGuardaProducto();
                                Log($"    fGuardaProducto() → {resG}  {(resG == 0 ? "OK" : SdkComercial.DescribirError(resG))}");
                                if (resG == 0) { okTotal++; Log($"    [OK] Producto creado activo: {p.Clave}"); }
                                else { errTotal++; SdkComercial.fCancelarModificacionProducto(); Log($"    [ERROR] No se pudo guardar {p.Clave}: {resG}"); }
                            }
                            else
                            {
                                errTotal++;
                                Log($"    [ERROR] fInsertaProducto devolvió {res} para {p.Clave}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        errTotal++;
                        Log($"    [EXCEPCION] {p.Clave}: {ex.GetType().Name}: {ex.Message}");
                        Log($"    {ex.StackTrace}");
                    }
                    Log("");
                }
            }

            Log("─────────────────────────────────────────────────────────────────");
            Log($"RESULTADO FINAL:  {okTotal} creados correctamente  /  {errTotal} con error");
            Log($"=== Fin: {DateTime.Now:yyyy-MM-dd HH:mm:ss} ===");

            // ── Guardar log en archivo y abrir Notepad ────────────────────────
            string rutaLog = AbrirLogEnNotepad();

            _creacionRealizada = true;
            btnCerrar.Text    = "Cerrar  ✔";
            btnCerrar.Enabled = true;
            lblEstado.Text    = errTotal == 0 ? $"✔ {okTotal} creado(s)" : $"⚠ {okTotal} ok / {errTotal} error(es)";
            lblEstado.ForeColor = errTotal == 0 ? Color.FromArgb(22, 163, 74) : Color.FromArgb(180, 80, 0);

            // Resumen en MessageBox
            string rutaInfo = string.IsNullOrEmpty(rutaLog) ? "" : $"\n\nLog guardado en:\n{rutaLog}";
            string resumen = errTotal == 0
                ? $"✔ Se crearon {okTotal} registro(s) correctamente en CONTPAQi.{rutaInfo}\n\nCierra esta ventana para continuar."
                : $"⚠ Resultado:\n  ✔ {okTotal} creados correctamente\n  ✘ {errTotal} con error\n\nSe abrió el log en el Bloc de notas con los detalles.{rutaInfo}\n\nCierra esta ventana para continuar.";

            MessageBox.Show(resumen, "Resultado de creación",
                MessageBoxButtons.OK,
                errTotal == 0 ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
        }

        // ─────────────────────────────────────────────────────────────────────
        /// <summary>
        /// Escribe el log acumulado a un archivo .txt en la carpeta temporal
        /// y lo abre en el Bloc de notas. Devuelve la ruta del archivo.
        /// </summary>
        private string AbrirLogEnNotepad()
        {
            try
            {
                string nombre  = $"CargaXML_Creacion_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                string carpeta = Path.Combine(Path.GetTempPath(), "CargaComerMasivo");
                Directory.CreateDirectory(carpeta);
                string ruta = Path.Combine(carpeta, nombre);
                File.WriteAllText(ruta, _logSb.ToString(), Encoding.UTF8);
                Process.Start("notepad.exe", ruta);
                return ruta;
            }
            catch
            {
                // Si no se puede abrir Notepad al menos no rompemos el flujo
                try
                {
                    string ruta = Path.Combine(Path.GetTempPath(), $"CargaXML_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
                    File.WriteAllText(ruta, _logSb.ToString(), Encoding.UTF8);
                    Process.Start("notepad.exe", ruta);
                    return ruta;
                }
                catch { }
                return "";
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        /// <summary>Agrega una línea al buffer de log interno.</summary>
        private void Log(string msg)
        {
            _logSb.AppendLine(msg);
        }

private static string Trunc(string s, int max)
            => string.IsNullOrEmpty(s) ? "" : s.Length > max ? s.Substring(0, max) : s;
    }
}
