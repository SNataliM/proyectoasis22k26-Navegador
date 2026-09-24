using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;

namespace CapaModelo_Seguridad
{
    public abstract class ClsSentencias : ClsConexion
    {
        private DataTable _TablaDatos;
        public int SeguridadMetEjecucionNonQuery(string ComandoTexto, List<OdbcParameter> Parametros, CommandType ComandoTipo)
        {
            using (var ConexionActiva = SeguridadMetObtenerConexion())
            {
                ConexionActiva.Open();
                using (var Comando = new OdbcCommand())
                {
                    Comando.Connection = ConexionActiva;
                    Comando.CommandText = ComandoTexto;
                    Comando.CommandType = ComandoTipo;
                    Comando.Parameters.AddRange(Parametros.ToArray());
                    return Comando.ExecuteNonQuery();
                }
            }
        }
        // Inicio cambio - Gabriel André Guillén Pocón - 0901-23-1998
        // SOBRECARGA TRANSACCIONAL (solicitada por el componente Navegador).
        // ¿Qué es? El mismo método de arriba (mismo nombre), pero con dos parámetros más:
        // una conexión y una transacción que ya vienen abiertas desde afuera.
        // ¿Para qué? El método original abre y cierra su PROPIA conexión en cada llamada, y en
        // ODBC una transacción pertenece a una sola conexión. Así, si el Navegador guarda un
        // registro y la bitácora por separado, no hay forma de deshacer ambos con un Rollback.
        // Esta sobrecarga ejecuta el comando sobre la conexión/transacción recibidas.
        // Importante: NO abre, NO cierra y NO confirma nada; eso lo decide quien la llama
        // (Commit / Rollback / cierre de la conexión). El método original queda intacto.
        public int SeguridadMetEjecucionNonQuery(string ComandoTexto, List<OdbcParameter> Parametros, CommandType ComandoTipo, OdbcConnection Conexion, OdbcTransaction Transaccion)
        {
            using (var Comando = new OdbcCommand())
            {
                Comando.Connection = Conexion;
                Comando.Transaction = Transaccion;
                Comando.CommandText = ComandoTexto;
                Comando.CommandType = ComandoTipo;
                Comando.Parameters.AddRange(Parametros.ToArray());
                return Comando.ExecuteNonQuery();
            }
        }
        // Fin cambio - Gabriel André Guillén Pocón - 0901-23-1998

        public DataTable SeguridadMetEjecucionConsulta(string ComandoTexto, CommandType ComandoTipo)
        {
            _TablaDatos = new DataTable();
            using (var ConexionActiva = SeguridadMetObtenerConexion())
            {
                ConexionActiva.Open();
                using (var Comando = new OdbcCommand())
                {
                    Comando.Connection = ConexionActiva;
                    Comando.CommandText = ComandoTexto;
                    Comando.CommandType = ComandoTipo;
                    using (var LectorDatos = Comando.ExecuteReader())
                        _TablaDatos.Load(LectorDatos);
                }
                return _TablaDatos; 
            }
        }

        public DataTable SeguridadMetEjecucionConsulta(string ComandoTexto, CommandType ComandoTipo, List<OdbcParameter> Parametros)
        {
            _TablaDatos = new DataTable();
            using (var ConexionActiva = SeguridadMetObtenerConexion())
            {
                ConexionActiva.Open();
                using (var Comando = new OdbcCommand())
                {
                    Comando.Connection = ConexionActiva;
                    Comando.CommandText = ComandoTexto;
                    Comando.CommandType = ComandoTipo;
                    Comando.Parameters.AddRange(Parametros.ToArray());
                    using (var LectorDatos = Comando.ExecuteReader())
                        _TablaDatos.Load(LectorDatos);
                }
                return _TablaDatos;
            }
        }
    }
}