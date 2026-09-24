﻿// Inicio - Roger Yankhel de Jesús Herrera Alcántara 0901-23-2429.
// Controlador original de registros. Coordina operaciones CRUD con el modelo.
// Dylan Rene Hernandez Recinos 16/09/2026
using System;
using System.Collections.Generic;
using CapaModelo_Navegador;

namespace CapaControlador_Navegador
{
    // Controlador para las operaciones que modifican registros: Insertar, Actualizar y Eliminar.
    // La validación de datos se hace en ModeloRegistro (CapaControlador_Navegador.Validaciones).
    public class ClsCtrlRegistro
    {
        private ClsRegistros _Registros = new ClsRegistros();

        // Verifica si ya existe un registro con esa llave primaria (evita duplicados).
        public bool NavegadorFuncExisteLlavePrimaria(string NombreTabla, string[] CamposPK, string[] ValoresPK)
        {
            return _Registros.NavegadorFuncExisteLlavePrimaria(NombreTabla, CamposPK, ValoresPK);
        }

        // Verifica si un valor ya existe en un campo (para campos únicos).
        public bool NavegadorFuncExisteValorCampo(string NombreTabla, string NombreCampo, string Valor)
        {
            return _Registros.NavegadorFuncExisteValorCampo(NombreTabla, NombreCampo, Valor);
        }

        // Inserta un nuevo registro en la tabla.
        public bool NavegadorFuncInsertarRegistro(string NombreTabla, Dictionary<string, string> Datos)
        {
            return _Registros.NavegadorFuncInsertarRegistro(NombreTabla, Datos);
        }

        // Actualiza un registro existente según su llave primaria.
        public bool NavegadorFuncActualizarRegistro(string NombreTabla, Dictionary<string, string> Valores, Dictionary<string, string> ClavesPrimarias)
        {
            if (string.IsNullOrWhiteSpace(NombreTabla))
                throw new ArgumentException("El nombre de la tabla es obligatorio.");

            if (Valores == null || Valores.Count == 0)
                throw new ArgumentException("No existen datos para actualizar.");

            if (ClavesPrimarias == null || ClavesPrimarias.Count == 0)
                throw new ArgumentException("No se encontró la llave primaria del registro.");

            return _Registros.NavegadorFuncActualizarRegistro(NombreTabla, Valores, ClavesPrimarias);
        }

        // Elimina un registro según su llave primaria.
        public bool NavegadorFuncEliminarRegistro(string NombreTabla, Dictionary<string, string> ClavesPrimarias)
        {
            if (string.IsNullOrWhiteSpace(NombreTabla))
                throw new ArgumentException("El nombre de la tabla es obligatorio.");

            if (ClavesPrimarias == null || ClavesPrimarias.Count == 0)
                throw new ArgumentException("No se encontró la llave primaria del registro.");

            return _Registros.NavegadorFuncEliminarRegistro(NombreTabla, ClavesPrimarias);
        }
    }
}
// Fin - Roger Yankhel de Jesús Herrera Alcántara 0901-23-2429.
