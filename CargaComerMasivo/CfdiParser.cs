using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;

namespace CargaComerMasivo
{
    public static class CfdiParser
    {
        private const string NS_CFDI = "http://www.sat.gob.mx/cfd/4";
        private const string NS_TFD  = "http://www.sat.gob.mx/TimbreFiscalDigital";

        public static List<FacturaXml> ParseCarpeta(string carpeta)
        {
            var result = new List<FacturaXml>();
            if (!Directory.Exists(carpeta)) return result;

            foreach (string archivo in Directory.GetFiles(carpeta, "*.xml", SearchOption.TopDirectoryOnly))
            {
                try
                {
                    result.Add(ParseXml(archivo));
                }
                catch (Exception ex)
                {
                    result.Add(new FacturaXml
                    {
                        RutaArchivo   = archivo,
                        NombreArchivo = Path.GetFileName(archivo),
                        Estado        = "Error al leer: " + ex.Message
                    });
                }
            }
            return result;
        }

        public static FacturaXml ParseXml(string rutaArchivo)
        {
            var f = new FacturaXml
            {
                RutaArchivo   = rutaArchivo,
                NombreArchivo = Path.GetFileName(rutaArchivo)
            };

            var doc = new XmlDocument();
            doc.Load(rutaArchivo);

            var ns = new XmlNamespaceManager(doc.NameTable);
            ns.AddNamespace("cfdi", NS_CFDI);
            ns.AddNamespace("tfd",  NS_TFD);

            // El nodo raíz puede buscarse como DocumentElement o con XPath
            XmlNode comp = doc.DocumentElement;
            if (comp == null || !comp.LocalName.Equals("Comprobante", StringComparison.OrdinalIgnoreCase))
                comp = doc.SelectSingleNode("//cfdi:Comprobante", ns);
            if (comp == null)
                throw new Exception("No se encontró cfdi:Comprobante.");

            f.Version           = Attr(comp, "Version");
            f.FechaStr          = Attr(comp, "Fecha");
            f.Folio             = Attr(comp, "Folio");
            f.FormaPago         = Attr(comp, "FormaPago");
            f.MetodoPago        = Attr(comp, "MetodoPago");
            f.Moneda            = Attr(comp, "Moneda");
            f.LugarExpedicion   = Attr(comp, "LugarExpedicion");
            f.TipoDeComprobante = Attr(comp, "TipoDeComprobante");

            DateTime.TryParse(f.FechaStr, out DateTime fecha);
            f.Fecha = fecha;

            f.SubTotal = ParseDec(Attr(comp, "SubTotal"));
            f.Total    = ParseDec(Attr(comp, "Total"));

            // Emisor
            var emisor = comp.SelectSingleNode("cfdi:Emisor", ns)
                      ?? comp.SelectSingleNode("*[local-name()='Emisor']");
            if (emisor != null)
            {
                f.EmisorRfc           = Attr(emisor, "Rfc");
                f.EmisorNombre        = Attr(emisor, "Nombre");
                f.EmisorRegimenFiscal = Attr(emisor, "RegimenFiscal");
            }

            // Receptor
            var receptor = comp.SelectSingleNode("cfdi:Receptor", ns)
                        ?? comp.SelectSingleNode("*[local-name()='Receptor']");
            if (receptor != null)
            {
                f.ReceptorRfc             = Attr(receptor, "Rfc");
                f.ReceptorNombre          = Attr(receptor, "Nombre");
                f.ReceptorDomicilioFiscal = Attr(receptor, "DomicilioFiscalReceptor");
                f.ReceptorRegimenFiscal   = Attr(receptor, "RegimenFiscalReceptor");
                f.ReceptorUsoCFDI         = Attr(receptor, "UsoCFDI");
            }

            // Impuestos totales
            var impuestos = comp.SelectSingleNode("cfdi:Impuestos", ns)
                         ?? comp.SelectSingleNode("*[local-name()='Impuestos']");
            if (impuestos != null)
                f.TotalImpuestos = ParseDec(Attr(impuestos, "TotalImpuestosTrasladados"));

            // Conceptos
            var conceptos = comp.SelectNodes("cfdi:Conceptos/cfdi:Concepto", ns);
            if (conceptos == null || conceptos.Count == 0)
                conceptos = comp.SelectNodes("*[local-name()='Conceptos']/*[local-name()='Concepto']");

            if (conceptos != null)
            {
                foreach (XmlNode c in conceptos)
                {
                    var con = new CfdiConcepto
                    {
                        Cantidad         = ParseDec(Attr(c, "Cantidad")),
                        ClaveProdServ    = Attr(c, "ClaveProdServ"),
                        ClaveUnidad      = Attr(c, "ClaveUnidad"),
                        Descripcion      = Attr(c, "Descripcion"),
                        NoIdentificacion = Attr(c, "NoIdentificacion"),
                        ValorUnitario    = ParseDec(Attr(c, "ValorUnitario")),
                        Importe          = ParseDec(Attr(c, "Importe")),
                        ObjetoImp        = Attr(c, "ObjetoImp"),
                        Unidad           = Attr(c, "Unidad")
                    };

                    // Primer traslado del concepto
                    var traslado = c.SelectSingleNode("cfdi:Impuestos/cfdi:Traslados/cfdi:Traslado", ns)
                                ?? c.SelectSingleNode("*[local-name()='Impuestos']/*[local-name()='Traslados']/*[local-name()='Traslado']");
                    if (traslado != null)
                    {
                        con.TasaIVA        = ParseDec(Attr(traslado, "TasaOCuota"));
                        con.ImporteImpuesto = ParseDec(Attr(traslado, "Importe"));
                    }

                    f.Conceptos.Add(con);
                }
            }

            // TimbreFiscalDigital
            var timbre = comp.SelectSingleNode("cfdi:Complemento/tfd:TimbreFiscalDigital", ns)
                      ?? comp.SelectSingleNode("*[local-name()='Complemento']/*[local-name()='TimbreFiscalDigital']");
            if (timbre != null)
            {
                f.UUID             = Attr(timbre, "UUID");
                f.FechaTimbradoStr = Attr(timbre, "FechaTimbrado");
                f.NoCertificadoSAT = Attr(timbre, "NoCertificadoSAT");
                f.RfcProvCertif    = Attr(timbre, "RfcProvCertif");
            }

            // Si no hay UUID, usar nombre del archivo como identificador temporal
            if (string.IsNullOrEmpty(f.UUID))
                f.UUID = Path.GetFileNameWithoutExtension(rutaArchivo);

            return f;
        }

        private static string Attr(XmlNode node, string name)
            => node?.Attributes?[name]?.Value?.Trim() ?? "";

        private static decimal ParseDec(string val)
        {
            decimal.TryParse(val, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result);
            return result;
        }
    }
}
