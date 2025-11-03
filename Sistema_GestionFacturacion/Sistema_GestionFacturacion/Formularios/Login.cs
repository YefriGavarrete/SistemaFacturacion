using Sistema_GestionFacturacion.Clases;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Windows.Forms;



namespace Sistema_GestionFacturacion.Formularios
{
    public partial class Login : Form
    {
        ConsultasSQL consulta = new ConsultasSQL();
        Conexion conexion = new Conexion();
        AlertasDelSistema Alertas = new AlertasDelSistema();

        public Login()
        {
            InitializeComponent();
            Habilitar();
        }

        void Habilitar()
        {
            txtUsuario.Enabled = true;
            txtClave.Enabled = true;
        }

        // Iniciar sesión: solo usa usuario + contraseña; abre formulario según rol.
        public void IniciarSesion()
        {
            string usuario = txtUsuario.Text.Trim();
            string clave = txtClave.Text ?? string.Empty;

            if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(clave))
            {
                Alertas.Advertencia("Ingrese usuario y contraseña.");
                return;
            }

            const string sql = @"
                SELECT TOP(1)
                    u.IdUsuario,
                    u.Nombre,
                    u.Apellido,
                    u.Clave,
                    u.Sal,
                    u.Iteraciones,
                    u.IdRol,
                    u.Estado,
                    r.Rol AS RolNombre
                FROM Usuarios u
                LEFT JOIN Roles r ON u.IdRol = r.IdRol
                WHERE u.Usuario = @Usuario;";
            try
            {
                using (var cmd = new SqlCommand(sql, Conexion.SQLConexion))
                {
                    cmd.Parameters.Add("@Usuario", SqlDbType.NVarChar, 50).Value = usuario;
                    conexion.abrirConexion();


                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            Alertas.Advertencia("Usuario no encontrado.");
                            return;
                        }
                        reader.Read();

                        string estado = reader["Estado"] != DBNull.Value ? reader["Estado"].ToString() : string.Empty;
                        if (!string.Equals(estado, "Activo", StringComparison.OrdinalIgnoreCase))
                        {
                            Alertas.Advertencia("Usuario no está activo.");
                            return;
                        }

                        int iteraciones = 10000;
                        if (reader["Iteraciones"] != DBNull.Value)
                            int.TryParse(reader["Iteraciones"].ToString(), out iteraciones);

                        byte[] storedHash = ReadDbBinary(reader, "Clave");
                        byte[] salt = ReadDbBinary(reader, "Sal");

                        if (storedHash == null || salt == null)
                        {
                            Alertas.Advertencia("Credenciales incompletas en la base de datos.");
                            return;
                        }

                        bool valido = VerificarPassword(clave, salt, storedHash, iteraciones);

                        if (!valido)
                        {
                            Alertas.Advertencia("Usuario o contraseña incorrectos.");
                            return;
                        }
                        string nombre = reader["Nombre"] != DBNull.Value ? reader["Nombre"].ToString() : string.Empty;
                        string apellido = reader["Apellido"] != DBNull.Value ? reader["Apellido"].ToString() : string.Empty;
                        string rolNombre = reader["RolNombre"] != DBNull.Value ? reader["RolNombre"].ToString() : string.Empty;

                        Alertas.Realizado("Inicio de sesión exitoso.");

                        // Open form according to role
                        if (!string.IsNullOrEmpty(rolNombre) && rolNombre.Equals("Administrador", StringComparison.OrdinalIgnoreCase))
                        {
                            try
                            {
                                var main = new FormRoles();
                                this.Hide();
                                main.Show();
                            }
                            catch (Exception ex)
                            { 
                                Alertas.Advertencia($"No se pudo abrir la aplicación: {ex.Message}");
                            }
                        }
                        else
                        {
                            try
                            {
                                // open FormularioPedidos and pass only name and surname
                                var pedidos = new FormPedidos(nombre, apellido);
                                this.Hide();
                                pedidos.Show();
                            }
                            catch (Exception ex)
                            {
                                Alertas.Advertencia($"No se pudo abrir FormularioPedidos: {ex.Message}");
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                Alertas.Advertencia($"Error de base de datos: {ex.Message}");
            }
            catch (Exception ex)
            {
                Alertas.Advertencia($"Error al iniciar sesión: {ex.Message}");
            }
            finally
            {
                conexion.cerrarConexion();
            }
        }

        // Verifica la contraseña usando PBKDF2 (Rfc2898). Comparación en tiempo constante.
        private bool VerificarPassword(string password, byte[] salt, byte[] storedHash, int iteraciones)
        {
            if (password == null || salt == null || storedHash == null) return false;

            try
            {
                using (var pbk = new Rfc2898DeriveBytes(password, salt, iteraciones))
                {
                    byte[] computed = pbk.GetBytes(storedHash.Length);
                    if (computed.Length != storedHash.Length) return false;

                    int diff = 0;
                    for (int i = 0; i < storedHash.Length; i++)
                        diff |= storedHash[i] ^ computed[i];

                    return diff == 0;
                }
            }
            catch
            {
                return false;
            }
        }

        // Lee una columna que puede contener VARBINARY (byte[]) o una cadena Base64 (NVARCHAR) y devuelve byte[]
        private byte[] ReadDbBinary(SqlDataReader reader, string columnName)
        {
            try
            {
                object val = reader[columnName];
                if (val == DBNull.Value || val == null) return null;

                if (val is byte[] bytes) return bytes;

                var s = val as string;
                if (!string.IsNullOrEmpty(s))
                {
                    try
                    {
                        return Convert.FromBase64String(s);
                    }
                    catch
                    {
                        return null;
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            IniciarSesion();
        }
    }
}