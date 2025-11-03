using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_GestionFacturacion.Clases
{
    internal class ConsultasSQL: Conexion
    {
        AlertasDelSistema Alertas = new AlertasDelSistema();

        string consulta;
        int filasAfectadas = 0;


        private static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["ConexionDB"]?.ConnectionString ?? string.Empty;
        }

        public bool Guardar(string tabla, string columnas, string valores)
        {
            consulta = $"INSERT INTO {tabla}({columnas}) VALUES ({valores})";

            int filasAfectadas = EjecutarConsulta(consulta);

            return filasAfectadas > 0;
        }


        public int update(string tablename, string data, string condicion = "")
        {
            if (condicion == "")
            {
                consulta = $"UPDATE {tablename} SET {data}";
            }
            else
            {
                consulta = $"UPDATE {tablename} SET {data} WHERE {condicion}";
            }

            filasAfectadas = EjecutarConsulta(consulta);

            return filasAfectadas;
        }

        public int Eliminar(string tablename, string condicion = "")
        {
            if (condicion == "")
            {
                consulta = $"DELETE FROM {tablename}";
            }
            else
            {
                consulta = $"DELETE FROM {tablename} WHERE {condicion}";
            }
            filasAfectadas = EjecutarConsulta(consulta);
            return filasAfectadas;
        }
        public DataTable Buscar(string tabla, string columnas, string condicion = "")
        {
            DataTable resultados = new DataTable();
            string consulta = string.IsNullOrEmpty(condicion) ?
                $"SELECT {columnas} FROM {tabla}" :
                $"SELECT {columnas} FROM {tabla} WHERE {condicion}";
            string connString = GetConnectionString();
            try
            {
                using (var conn = new SqlConnection(connString))
                using (var comando = new SqlCommand(consulta, conn))
                {
                    conn.Open();
                    using (var lector = comando.ExecuteReader())
                    {
                        resultados.Load(lector);
                    }
                }
            }
            catch (SqlException error)
            {
                Alertas.Realizado($"Error al recuperar datos:\n{error.Message}");
            }


            return resultados;
        }
        public int EjecutarConsulta(string consulta)
        {
            int filasAfectadas = 0;
            string connString = GetConnectionString();
            try
            {
                using (var conn = new SqlConnection(connString))
                using (var comando = new SqlCommand(consulta, conn))
                {
                    conn.Open();
                    filasAfectadas = comando.ExecuteNonQuery();
                }
            }
            catch (SqlException error)
            {
                Alertas.Realizado($"Error al ejecutar consulta:\n{error.Message}");
            }

            return filasAfectadas;
        }
        public int EjecutarComando(SqlCommand comando)
        {
            int filasAfectadas = 0;
            string connString = GetConnectionString();
            try
            {
                using (var conn = new SqlConnection(connString))
                {
                    comando.Connection = conn;
                    conn.Open();
                    filasAfectadas = comando.ExecuteNonQuery();
                }
            }
            catch (SqlException error)
            {
                Alertas.Realizado($"Error al ejecutar comando parametrizado:\n{error.Message}");
            }

            return filasAfectadas;
        }
    }
}
