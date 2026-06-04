using System;
using System.Collections.Generic;

namespace CargaComerMasivo
{
    public class FacturaXml
    {
        // ── Archivo ───────────────────────────────────────────────────────────
        public string RutaArchivo    { get; set; }
        public string NombreArchivo  { get; set; }

        // ── Comprobante ───────────────────────────────────────────────────────
        public string   Version             { get; set; }
        public string   FechaStr            { get; set; }
        public DateTime Fecha               { get; set; }
        public string   Folio               { get; set; }
        public string   FormaPago           { get; set; }
        public string   MetodoPago          { get; set; }
        public string   Moneda              { get; set; }
        public string   LugarExpedicion     { get; set; }
        public string   TipoDeComprobante   { get; set; }
        public decimal  SubTotal            { get; set; }
        public decimal  Total               { get; set; }
        public decimal  TotalImpuestos      { get; set; }

        // ── Emisor ────────────────────────────────────────────────────────────
        public string EmisorRfc            { get; set; }
        public string EmisorNombre         { get; set; }
        public string EmisorRegimenFiscal  { get; set; }

        // ── Receptor ──────────────────────────────────────────────────────────
        public string ReceptorRfc             { get; set; }
        public string ReceptorNombre          { get; set; }
        public string ReceptorDomicilioFiscal { get; set; }
        public string ReceptorRegimenFiscal   { get; set; }
        public string ReceptorUsoCFDI         { get; set; }

        // ── TimbreFiscalDigital ───────────────────────────────────────────────
        public string UUID             { get; set; }
        public string FechaTimbradoStr { get; set; }
        public string NoCertificadoSAT { get; set; }
        public string RfcProvCertif    { get; set; }

        // ── Conceptos ─────────────────────────────────────────────────────────
        public List<CfdiConcepto> Conceptos { get; set; } = new List<CfdiConcepto>();

        // ── Validación CONTPAQi ───────────────────────────────────────────────
        public string Estado               { get; set; } = "Pendiente";
        public string CodigoClienteContpaq { get; set; }

        public bool EsValida => Estado == "OK";
    }

    public class CfdiConcepto
    {
        public decimal Cantidad          { get; set; }
        public string  ClaveProdServ     { get; set; }
        public string  ClaveUnidad       { get; set; }
        public string  Descripcion       { get; set; }
        public string  NoIdentificacion  { get; set; }
        public decimal ValorUnitario     { get; set; }
        public decimal Importe           { get; set; }
        public string  ObjetoImp         { get; set; }
        public string  Unidad            { get; set; }

        // Impuesto
        public decimal TasaIVA         { get; set; }
        public decimal ImporteImpuesto  { get; set; }

        // Validación
        public string CodigoProductoContpaq { get; set; }
        public string ErrorProducto         { get; set; }
        public bool   ProductoEncontrado    => !string.IsNullOrEmpty(CodigoProductoContpaq);
    }
}
