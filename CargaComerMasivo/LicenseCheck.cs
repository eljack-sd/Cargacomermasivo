using System;
using System.IO;
using System.Net;
using System.Text;
using Microsoft.Win32;

namespace CargaComerMasivo
{
    // ─────────────────────────────────────────────────────────────────────────
    // LicenseCheck — verifica que la máquina esté en el Gist de licencias.
    //
    // ID de máquina = MachineName + "-" + últimos 4 chars del MachineGuid
    //   Ejemplo:  SERVER-A3F8
    //
    // Formato del Gist (licencias.json):
    //   {
    //     "SERVER-A3F8":  true,          ← licencia permanente
    //     "LAPTOP-CC12":  "2026-12-31",  ← expira el 31-dic-2026
    //     "PC-MOROSA":    false          ← bloqueado
    //   }
    // ─────────────────────────────────────────────────────────────────────────
    internal static class LicenseCheck
    {
        private const string GistBase =
            "https://gist.githubusercontent.com/eljack-sd/05a54a0cf890fe65ebc1a0ec5f9546cb/raw/licencias.json";

        private static string UrlFresca() =>
            GistBase + "?_=" + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        // ── ID único: MachineName + sufijo del MachineGuid ────────────────────
        internal static string MachineId()
        {
            string name   = Environment.MachineName.Trim().ToUpperInvariant();
            string suffix = "";
            try
            {
                using (var base64 = RegistryKey.OpenBaseKey(
                    RegistryHive.LocalMachine, RegistryView.Registry64))
                using (var key = base64.OpenSubKey(
                    @"SOFTWARE\Microsoft\Cryptography", false))
                {
                    if (key != null)
                    {
                        string guid  = key.GetValue("MachineGuid") as string ?? "";
                        string clean = guid.Replace("-", "").ToUpperInvariant();
                        if (clean.Length >= 4)
                            suffix = "-" + clean.Substring(clean.Length - 4);
                    }
                }
            }
            catch { }
            return name + suffix;
        }

        // ── Guarda el ID en Temp para diagnóstico (llamado desde Program.cs) ──
        internal static void GuardarIdParaDiagnostico()
        {
            try
            {
                string ruta = Path.Combine(Path.GetTempPath(), "ccm_mid.dat");
                File.WriteAllText(ruta, MachineId(), Encoding.UTF8);
                File.SetAttributes(ruta, FileAttributes.Hidden);
            }
            catch { }
        }

        // ── Punto de entrada — llamado desde Program.cs ───────────────────────
        public static bool EsValida()
        {
            string machineId = MachineId();

            // Descargar JSON con cache-busting
            string json;
            try
            {
                var req         = (HttpWebRequest)WebRequest.Create(UrlFresca());
                req.Timeout     = 7000;
                req.UserAgent   = "CCM/1.0";
                req.CachePolicy = new System.Net.Cache.RequestCachePolicy(
                                      System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
                using (var resp = (HttpWebResponse)req.GetResponse())
                using (var sr   = new StreamReader(resp.GetResponseStream(), Encoding.UTF8))
                    json = sr.ReadToEnd();
            }
            catch { return false; }

            return EvaluarLicencia(json, machineId);
        }

        // ── Evalúa el valor: true / false / "AAAA-MM-DD" ─────────────────────
        private static bool EvaluarLicencia(string json, string machineId)
        {
            if (string.IsNullOrEmpty(json)) return false;

            string clave = "\"" + machineId + "\"";
            int idx = json.IndexOf(clave, StringComparison.OrdinalIgnoreCase);
            if (idx < 0) return false;

            int pos = idx + clave.Length;
            while (pos < json.Length &&
                   (json[pos] == ' ' || json[pos] == ':' ||
                    json[pos] == '\t' || json[pos] == '\r' || json[pos] == '\n'))
                pos++;

            if (pos >= json.Length) return false;

            // Valor entre comillas → fecha "AAAA-MM-DD"
            if (json[pos] == '"')
            {
                pos++;
                var sb = new StringBuilder();
                while (pos < json.Length && json[pos] != '"')
                    sb.Append(json[pos++]);

                if (DateTime.TryParseExact(sb.ToString().Trim(), "yyyy-MM-dd",
                        System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.None, out DateTime expira))
                    return DateTime.UtcNow.Date <= expira.Date;

                return false;
            }

            // Valor sin comillas → true / false
            var token = new StringBuilder();
            while (pos < json.Length &&
                   json[pos] != ',' && json[pos] != '}' &&
                   json[pos] != ' '  && json[pos] != '\r' &&
                   json[pos] != '\n' && json[pos] != '\t')
                token.Append(json[pos++]);

            return token.ToString().Trim().Equals("true", StringComparison.OrdinalIgnoreCase);
        }
    }
}
