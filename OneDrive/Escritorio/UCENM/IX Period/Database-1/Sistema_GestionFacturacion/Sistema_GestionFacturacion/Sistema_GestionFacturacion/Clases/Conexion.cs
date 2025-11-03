using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sistema_GestionFacturacion.Clases
{
    internal class Conexion
    {
        AlertasDelSistema Alertas = new AlertasDelSistema();

        public static SqlConnection SQLConexion = new SqlConnection(ConfigurationManager.ConnectionStrings["ConexionDB"].ToString());

        public void abrirConexion()
        {
            try
            {
                if (SQLConexion == null)
                {
                    SQLConexion = new SqlConnection(ConfigurationManager.ConnectionStrings["ConexionDB"].ToString());
                }
                if (SQLConexion.State != ConnectionState.Open &&
                    SQLConexion.State != ConnectionState.Connecting)
                {
                    SQLConexion.Open();
                }


            }
            catch (SqlException error)
            {
                Alertas.Advertencia(error.Message);
            }
        }
        

        public void cerrarConexion()
        {
            try
            {
                if(SQLConexion != null && SQLConexion.State == System.Data.ConnectionState.Open)
                {
                    SQLConexion.Close();
                }
            }
            catch (SqlException error)
            {
                Alertas.Advertencia(error.Message);
            }
        }

        public void terminarConexion()
        {
            try
            {
                if (SQLConexion != null && SQLConexion.State == System.Data.ConnectionState.Open)
                {
                    SQLConexion.Close();
                }
            }
            catch
            {

            }
        }

        public void validarConexion()
        {
            try
            {
                abrirConexion();
                Alertas.Realizado("Conexion Exitosa a la Base de Datos");
            }
            catch (Exception ex)
            {
                Alertas.Advertencia(ex.Message);
            }
            finally
            {
                cerrarConexion();
            }
        }
    }
}
