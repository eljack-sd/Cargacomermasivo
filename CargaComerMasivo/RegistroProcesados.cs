using System;
using System.Data;
using System.Data.SqlClient;

namespace CargaComerMasivo
{
    internal static class RegistroProcesados
    {
        private const string NombreDB = "CargaMasiva_Control";

        private const string CrearTabla = @"
            IF NOT EXISTS (
                SELECT 1 FROM sys.objects
                WHERE object_id = OBJECT_ID(N'pedidos_procesados') AND type = 'U'
            )
            CREATE TABLE pedidos_procesados (
                id_pedido     NVARCHAR(100) NOT NULL,
                fecha_api     NVARCHAR(10)  NOT NULL,
                cliente       NVARCHAR(50)  NULL,
                id_doc_contpaq INT          NULL,
                fecha_subida  DATETIME      NOT NULL DEFAULT GETDATE(),
                CONSTRAINT PK_pedidos_procesados PRIMARY KEY (id_pedido)
            );";

        private static string BuildConnStr()
        {
            if (string.IsNullOrEmpty(Program.ConnStrEmpresa))
                throw new InvalidOperationException("No hay conexion activa a CONTPAQi.");
            var builder = new SqlConnectionStringBuilder(Program.ConnStrEmpresa);
            builder.InitialCatalog = NombreDB;
            builder.ConnectTimeout = 10;
            return builder.ConnectionString;
        }

        private static string BuildMasterConnStr()
        {
            var builder = new SqlConnectionStringBuilder(Program.ConnStrEmpresa);
            builder.InitialCatalog = "master";
            builder.ConnectTimeout = 10;
            return builder.ConnectionString;
        }

        public static void InicializarBD()
        {
            using (var conn = new SqlConnection(BuildMasterConnStr()))
            {
                conn.Open();
                using (var cmd = new SqlCommand(
                    "IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = '" + NombreDB + "') " +
                    "CREATE DATABASE [" + NombreDB + "]", conn))
                    cmd.ExecuteNonQuery();
            }
            using (var conn = new SqlConnection(BuildConnStr()))
            {
                conn.Open();
                using (var cmd = new SqlCommand(CrearTabla, conn))
                    cmd.ExecuteNonQuery();
            }
        }

        public static bool YaFueProcesado(string idPedido)
        {
            if (string.IsNullOrEmpty(idPedido)) return false;
            using (var conn = new SqlConnection(BuildConnStr()))
            {
                conn.Open();
                using (var cmd = new SqlCommand(
                    "SELECT 1 FROM pedidos_procesados WHERE id_pedido = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", idPedido);
                    return cmd.ExecuteScalar() != null;
                }
            }
        }

        public static void MarcarProcesado(string idPedido, string fechaApi, string cliente, int idDocContpaq)
        {
            if (string.IsNullOrEmpty(idPedido)) return;
            using (var conn = new SqlConnection(BuildConnStr()))
            {
                conn.Open();
                using (var cmd = new SqlCommand(@"
                    IF NOT EXISTS (SELECT 1 FROM pedidos_procesados WHERE id_pedido = @id)
                    INSERT INTO pedidos_procesados (id_pedido, fecha_api, cliente, id_doc_contpaq)
                    VALUES (@id, @fecha, @cliente, @idDoc)", conn))
                {
                    cmd.Parameters.AddWithValue("@id",      idPedido);
                    cmd.Parameters.AddWithValue("@fecha",   fechaApi);
                    cmd.Parameters.AddWithValue("@cliente", (object)cliente ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@idDoc",   idDocContpaq);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void LimpiarAntiguos()
        {
            try
            {
                using (var conn = new SqlConnection(BuildConnStr()))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(
                        "DELETE FROM pedidos_procesados WHERE fecha_subida < DATEADD(DAY, -60, GETDATE())", conn))
                        cmd.ExecuteNonQuery();
                }
            }
            catch { }
        }

        /// <summary>
        /// Elimina el registro de pedidos_procesados asociado a un documento de CONTPAQi.
        /// Se llama al borrar físicamente un documento desde FrmCancelaDoctos.
        /// </summary>
        public static void EliminarPorIdDoc(int idDocContpaq)
        {
            if (idDocContpaq <= 0) return;
            try
            {
                using (var conn = new SqlConnection(BuildConnStr()))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(
                        "DELETE FROM pedidos_procesados WHERE id_doc_contpaq = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", idDocContpaq);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch { }
        }
    }
}
