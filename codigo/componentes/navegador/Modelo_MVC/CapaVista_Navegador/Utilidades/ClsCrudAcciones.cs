// ============================================================================
// Desarrollador: Oskar Saul Cermeño Jimenez
// Carnet:        0901-23-15379
// Fecha:         16/09/2026
// Módulo:        CapaVista_Navegador
// Descripción:   Gestor de acciones CRUD (Insertar, Modificar, Eliminar) con
//                traducción de excepciones de base de datos a mensajes legibles.
// ============================================================================

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CapaControlador_Navegador;
// Inicio cambio - Mario Alberto Taracena Pérez - 0901-23-9335
// Estos dos using son para poder usar la clase de bitácora (auditoría) del componente Seguridad.
//using CapaControlador_Seguridad;
//using CapaControlador_Seguridad.Objetos_de_valor;
// Fin cambio - Mario Alberto Taracena Pérez - 0901-23-9335
using CapaEntidades_Navegador;

namespace CapaVista_Navegador
{
    public class ClsCrudAcciones
    {
        private ClsCtrlRegistro _CtrlRegistro = new ClsCtrlRegistro();

        // Inicio cambio - Mario Alberto Taracena Pérez - 0901-23-9335
        //private ClsModeloBitacora _Bitacora = new ClsModeloBitacora();
        // Fin cambio - Mario Alberto Taracena Pérez - 0901-23-9335

        public bool NavegadorFuncConfirmarAccion(string Titulo, string Mensaje)
        {
            return MessageBox.Show(Mensaje, Titulo, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        private string NavegadorFuncResumenDatos(Dictionary<string, string> Datos)
        {
            string Resumen = "";

            foreach (KeyValuePair<string, string> Dato in Datos)
                Resumen += Dato.Key + ": " + Dato.Value + "\n";

            return Resumen;
        }

        public bool NavegadorFuncGuardar(string Tabla, List<ClsColumnaInfo> Esquema, Dictionary<string, string> DatosFormulario,
            bool ModoModificar, Dictionary<string, string> PkOriginal, out string Mensaje)
        {
            Mensaje = "";

            Dictionary<string, string> Datos = new Dictionary<string, string>();

            foreach (ClsColumnaInfo Columna in Esquema)
            {
                if (!DatosFormulario.ContainsKey(Columna.Nombre))
                    continue;

                if (!ModoModificar && Columna.EsAutoincremento)
                    continue;

                if (ModoModificar && Columna.EsPK)
                    continue;

                Datos[Columna.Nombre] = DatosFormulario[Columna.Nombre];
            }

            // Delegación completa de validación (nulos, tipos e integridad) a la Capa Controlador
            List<string> Errores = _CtrlRegistro.NavegadorFuncValidarRegistro(Datos, Tabla);

            if (Errores.Count > 0)
            {
                Mensaje = string.Join("\n", Errores);
                return false;
            }

            if (!ModoModificar)
                return NavegadorFuncInsertar(Tabla, Esquema, Datos, out Mensaje);

            return NavegadorFuncModificar(Tabla, Datos, PkOriginal, out Mensaje);
        }

        private bool NavegadorFuncInsertar(string Tabla, List<ClsColumnaInfo> Esquema, Dictionary<string, string> Datos, out string Mensaje)
        {
            Mensaje = "";

            List<string> CamposPK = new List<string>();
            List<string> ValoresPK = new List<string>();

            foreach (ClsColumnaInfo Columna in Esquema)
            {
                if (!Columna.EsPK) continue;

                string Valor;

                if (Datos.TryGetValue(Columna.Nombre, out Valor))
                {
                    CamposPK.Add(Columna.Nombre);
                    ValoresPK.Add(Valor);
                }
            }

            if (CamposPK.Count > 0)
            {
                bool Duplicada = false;

                try
                {
                    Duplicada = _CtrlRegistro.NavegadorFuncExisteLlavePrimaria(
                        Tabla, CamposPK.ToArray(), ValoresPK.ToArray());
                }
                catch (Exception Excepcion)
                {
                    Mensaje = "No se pudo verificar la llave primaria: " +
                        NavegadorFuncMensajeAmigable(Excepcion);
                    return false;
                }

                if (Duplicada)
                {
                    Mensaje = "Ya existe un registro con esta llave primaria (" +
                        string.Join(", ", CamposPK.ToArray()) + " = " +
                        string.Join(", ", ValoresPK.ToArray()) + ").";
                    return false;
                }
            }

            if (!NavegadorFuncConfirmarAccion("Confirmar ingreso",
                "¿Desea ingresar el siguiente registro en la tabla '" + Tabla + "'?\n\n" +
                NavegadorFuncResumenDatos(Datos)))
                return false;

            // Inicio cambio - Mario Alberto Taracena Pérez - 0901-23-9335
            bool Insertado = _CtrlRegistro.NavegadorFuncInsertarRegistro(Tabla, Datos);

            if (Insertado)
            {
                int IdRegistro = 0;
                if (ValoresPK.Count > 0)
                    int.TryParse(ValoresPK[0], out IdRegistro);

                //_Bitacora.SeguridadMetRegistrarBitacora(
                //    ClsSesionSeguridad.IdUsuario, "INSERT", Tabla, IdRegistro,
                //    "Se insertó un registro en " + Tabla + ": " + NavegadorFuncResumenDatos(Datos), null);
            }

            return Insertado;
            // Fin cambio - Mario Alberto Taracena Pérez - 0901-23-9335
        }

        private bool NavegadorFuncModificar(string Tabla, Dictionary<string, string> Datos,
            Dictionary<string, string> ClavesPrimarias, out string Mensaje)
        {
            Mensaje = "";

            if (ClavesPrimarias == null || ClavesPrimarias.Count == 0)
            {
                Mensaje = "No se encontró la llave primaria del registro seleccionado.";
                return false;
            }

            if (Datos.Count == 0)
            {
                Mensaje = "No hay campos disponibles para Modificar.";
                return false;
            }

            if (!NavegadorFuncConfirmarAccion("Confirmar modificación",
                "¿Desea guardar los cambios en la tabla '" + Tabla + "'?\n\n" +
                NavegadorFuncResumenDatos(Datos)))
                return false;

            // Inicio cambio - Mario Alberto Taracena Pérez - 0901-23-9335
            bool Actualizado = _CtrlRegistro.NavegadorFuncActualizarRegistro(Tabla, Datos, ClavesPrimarias);

            if (Actualizado)
            {
                int IdRegistro = 0;
                foreach (string ValorPk in ClavesPrimarias.Values) { int.TryParse(ValorPk, out IdRegistro); break; }

                //_Bitacora.SeguridadMetRegistrarBitacora(
                //    ClsSesionSeguridad.IdUsuario, "UPDATE", Tabla, IdRegistro,
                //    "Se actualizó un registro en " + Tabla + ": " + NavegadorFuncResumenDatos(Datos), null);
            }

            return Actualizado;
            // Fin cambio - Mario Alberto Taracena Pérez - 0901-23-9335
        }

        public bool NavegadorFuncEliminar(string Tabla, Dictionary<string, string> ClavesPrimarias, out string Mensaje)
        {
            Mensaje = "";

            if (ClavesPrimarias == null || ClavesPrimarias.Count == 0)
            {
                Mensaje = "La tabla no tiene una llave primaria detectable.";
                return false;
            }

            foreach (KeyValuePair<string, string> Clave in ClavesPrimarias)
            {
                if (string.IsNullOrWhiteSpace(Clave.Value))
                {
                    Mensaje = "No se pudo obtener el valor de la llave primaria del registro seleccionado.";
                    return false;
                }
            }

            if (!NavegadorFuncConfirmarAccion("Confirmar eliminación",
                "¿Desea eliminar el registro seleccionado de la tabla '" + Tabla + "'?"))
                return false;

            // Inicio cambio - Mario Alberto Taracena Pérez - 0901-23-9335
            bool Eliminado = _CtrlRegistro.NavegadorFuncEliminarRegistro(Tabla, ClavesPrimarias);

            if (Eliminado)
            {
                int IdRegistro = 0;
                foreach (string ValorPk in ClavesPrimarias.Values) { int.TryParse(ValorPk, out IdRegistro); break; }

                //_Bitacora.SeguridadMetRegistrarBitacora(
                //    ClsSesionSeguridad.IdUsuario, "DELETE", Tabla, IdRegistro,
                //    "Se eliminó un registro de " + Tabla + ".", null);
            }

            return Eliminado;
            // Fin cambio - Mario Alberto Taracena Pérez - 0901-23-9335
        }

        public string NavegadorFuncMensajeAmigable(Exception Excepcion)
        {
            string TextoMinusculas = (Excepcion.Message ?? "").ToLowerInvariant();

            if (TextoMinusculas.Contains("doesn't exist") || TextoMinusculas.Contains("does not exist") ||
                TextoMinusculas.Contains("unknown table") || TextoMinusculas.Contains("no existe") ||
                TextoMinusculas.Contains("invalid object name"))
                return "La tabla indicada no existe o el nombre está escrito incorrectamente. Verifique el nombre configurado para el CRUD.";

            if (TextoMinusculas.Contains("foreign key") || TextoMinusculas.Contains("fk_") || TextoMinusculas.Contains("reference constraint"))
                return "El registro no puede guardarse o eliminarse porque existe una relación de llave foránea.";

            if (TextoMinusculas.Contains("duplicate entry") || TextoMinusculas.Contains("duplicate key") ||
                TextoMinusculas.Contains("unique constraint") || TextoMinusculas.Contains("violation of unique") ||
                TextoMinusculas.Contains("violation of primary key"))
                return "Ya existe un registro con el mismo valor en un campo único.";

            if (TextoMinusculas.Contains("cannot be null") || TextoMinusculas.Contains("null value") ||
                TextoMinusculas.Contains("not-null constraint") || TextoMinusculas.Contains("insert the value null"))
                return "Hay un campo obligatorio que no puede quedar vacío.";

            if (TextoMinusculas.Contains("data too long") || TextoMinusculas.Contains("truncat") ||
                TextoMinusculas.Contains("string or binary data would be truncated"))
                return "Uno de los valores ingresados es demasiado largo para el campo correspondiente.";

            if ((TextoMinusculas.Contains("incorrect") && TextoMinusculas.Contains("value")) ||
                TextoMinusculas.Contains("conversion failed") || TextoMinusculas.Contains("invalid input syntax"))
                return "Uno de los valores ingresados tiene un formato incorrecto para su campo.";

            return Excepcion.Message;
        }
    }
}