using CapaModelo_Seguridad.Contratos;
using CapaModelo_Seguridad.Entidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;

namespace CapaModelo_Seguridad.Repositorios
{
    public class ClsRepositorioBitacora : ClsSentencias, IRepositorioBitacora
    {
        private string _SelectAll;
        private string _Insert;

        public ClsRepositorioBitacora()
        {
            _SelectAll = "SELECT b.idBitacora, b.idUsuario, u.nombreUsuario, b.accionBitacora, b.tablaBitacora, b.idRegistroBitacora, b.detallesBitacora, b.ipBitacora, b.fechaHoraBitacora " +
                         "FROM tblBitacora b " +
                         "LEFT JOIN tblUsuario u ON b.idUsuario = u.idUsuario " +
                         "ORDER BY b.idBitacora DESC";
            _Insert = "INSERT INTO tblBitacora (idUsuario, accionBitacora, tablaBitacora, idRegistroBitacora, detallesBitacora, ipBitacora, fechaHoraBitacora) VALUES (?, ?, ?, ?, ?, ?, ?)";
        }

        public int SeguridadMetAgregar(ClsBitacora Entidad)
        {
            var Parametros = new List<OdbcParameter>();
            Parametros.Add(new OdbcParameter("p_idUsuario", Entidad.IdUsuario.HasValue ? (object)Entidad.IdUsuario.Value : DBNull.Value));
            Parametros.Add(new OdbcParameter("p_accionBitacora", Entidad.AccionBitacora));
            Parametros.Add(new OdbcParameter("p_tablaBitacora", Entidad.TablaBitacora));
            Parametros.Add(new OdbcParameter("p_idRegistroBitacora", Entidad.IdRegistroBitacora));
            Parametros.Add(new OdbcParameter("p_detallesBitacora", Entidad.DetallesBitacora));
            Parametros.Add(new OdbcParameter("p_ipBitacora", Entidad.IpBitacora));
            Parametros.Add(new OdbcParameter("p_fechaHoraBitacora", Entidad.FechaHoraBitacora.ToString("yyyy-MM-dd HH:mm:ss")));

            return SeguridadMetEjecucionNonQuery(_Insert, Parametros, CommandType.Text);
        }

        // Inicio cambio - Gabriel André Guillén Pocón - 0901-23-1998
        // SOBRECARGA TRANSACCIONAL (solicitada por el componente Navegador).
        // ¿Qué es? El mismo SeguridadMetAgregar de arriba, pero inserta la bitácora usando la
        // conexión y la transacción recibidas (mediante la sobrecarga de ClsSentencias).
        // ¿Para qué? Para que el INSERT de la bitácora viaje en la MISMA transacción que la
        // operación (insertar/modificar/eliminar) del Navegador: si algo falla, un solo
        // Rollback deshace las dos cosas y nunca queda una sin la otra.
        // La bitácora sigue registrándose SIEMPRE por Seguridad. El método original queda intacto.
        public int SeguridadMetAgregar(ClsBitacora Entidad, OdbcConnection Conexion, OdbcTransaction Transaccion)
        {
            var Parametros = new List<OdbcParameter>();
            Parametros.Add(new OdbcParameter("p_idUsuario", Entidad.IdUsuario.HasValue ? (object)Entidad.IdUsuario.Value : DBNull.Value));
            Parametros.Add(new OdbcParameter("p_accionBitacora", Entidad.AccionBitacora));
            Parametros.Add(new OdbcParameter("p_tablaBitacora", Entidad.TablaBitacora));
            Parametros.Add(new OdbcParameter("p_idRegistroBitacora", Entidad.IdRegistroBitacora));
            Parametros.Add(new OdbcParameter("p_detallesBitacora", Entidad.DetallesBitacora));
            Parametros.Add(new OdbcParameter("p_ipBitacora", Entidad.IpBitacora));
            Parametros.Add(new OdbcParameter("p_fechaHoraBitacora", Entidad.FechaHoraBitacora.ToString("yyyy-MM-dd HH:mm:ss")));

            return SeguridadMetEjecucionNonQuery(_Insert, Parametros, CommandType.Text, Conexion, Transaccion);
        }
        // Fin cambio - Gabriel André Guillén Pocón - 0901-23-1998

        public int SeguridadMetEditar(ClsBitacora Entidad)
        {
            throw new NotImplementedException("No se permite editar registros de la bitácora.");
        }

        public int SeguridadMetRemover(ClsBitacora Entidad)
        {
            throw new NotImplementedException("No se permite eliminar registros de la bitácora.");
        }

        public IEnumerable<ClsBitacora> SeguridadMetObtenerTodos()
        {
            var ListaBitacora = new List<ClsBitacora>();
            var TablaDatos = SeguridadMetEjecucionConsulta(_SelectAll, CommandType.Text);
            foreach (DataRow Fila in TablaDatos.Rows)
            {
                var Bitacora = new ClsBitacora();
                Bitacora.IdBitacora = Convert.ToInt32(Fila[0]);
                Bitacora.IdUsuario = Fila[1] == DBNull.Value ? (int?)null : Convert.ToInt32(Fila[1]);
                Bitacora.NombreUsuario = Fila[2] == DBNull.Value ? (Bitacora.IdUsuario.HasValue ? "Usuario " + Bitacora.IdUsuario : "Sistema") : Fila[2].ToString();
                Bitacora.AccionBitacora = Fila[3].ToString();
                Bitacora.TablaBitacora = Fila[4].ToString();
                Bitacora.IdRegistroBitacora = Convert.ToInt32(Fila[5]);
                Bitacora.DetallesBitacora = Fila[6].ToString();
                Bitacora.IpBitacora = Fila[7].ToString();
                Bitacora.FechaHoraBitacora = Convert.ToDateTime(Fila[8]);
                ListaBitacora.Add(Bitacora);
            }
            return ListaBitacora;
        }
    }
}
