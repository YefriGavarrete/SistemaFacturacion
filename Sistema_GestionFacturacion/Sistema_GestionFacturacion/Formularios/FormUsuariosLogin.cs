using Sistema_GestionFacturacion.Clases;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sistema_GestionFacturacion.Formularios
{
    public partial class FormUsuariosLogin : Form
    {
        ConsultasSQL consulta = new ConsultasSQL();
        AlertasDelSistema Alertas = new AlertasDelSistema();
        Conexion conexion = new Conexion();

        // PBKDF2 parametros
        const int PBKDF2_ITERATIONS = 10000;
        const int PBKDF2_HASH_BYTES = 32; // 256-bit
        const int SALT_BYTES = 16;

        public FormUsuariosLogin()
        {
            InitializeComponent();
            CargarRoles();
        }
        void CargarRoles()
        {
            try
            {
                DataTable dtRoles = consulta.Buscar("Roles", "IdRol,Rol", "Estado = 'Activo'");
                if (dtRoles != null && dtRoles.Rows.Count > 0)
                {
                    cmbRol.DataSource = dtRoles;
                    cmbRol.DisplayMember = "Rol";
                    cmbRol.ValueMember = "IdRol";
                    cmbRol.Enabled = false;
                    cmbRol.SelectedIndex = -1;
                }
                else
                {
                    cmbRol.DataSource = null;
                    cmbRol.Items.Clear();
                }
            }
            catch (Exception ex)
            {
                Alertas.Advertencia($"Error al mostrar Roles: {ex.Message}");
            }

        }

        void LimpiarCampos()
        {
            txtNombre.Clear();
            txtApellido.Clear();
            txtUsuario.Clear();
            txtClave.Clear();
            cmbRol.SelectedIndex = -1;
            try { txtIdUsuario.Text = string.Empty; } catch { }
        }

        void HabilitarNuevosRegistros(bool valor)
        {
            btnGuardarRegistro.Enabled = valor;
            btnCancelarRegistro.Enabled = valor;
            txtNombre.Enabled = valor;
            txtApellido.Enabled = valor;
            txtUsuario.Enabled = valor;
            txtClave.Enabled = valor;
            cmbRol.Enabled = valor;
            btnNuevoRegistro.Enabled = !valor;
        }


        // Genera salt 
        static byte[] GenerarSalt(int size = SALT_BYTES)
        {
            var salt = new byte[size];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }
        // Calcula el hash PBKDF2
        static byte[] ComputeHash(string password, byte[] salt, int iterations = PBKDF2_ITERATIONS, int outputBytes = PBKDF2_HASH_BYTES)
        {
            using (var pbk = new Rfc2898DeriveBytes(password, salt, iterations))
            {
                return pbk.GetBytes(outputBytes);
            }
        }

        void GuardarUsuario()
        {
            string nombre = txtNombre.Text.Trim();
            string apellido = txtApellido.Text.Trim();
            string usuario = txtUsuario.Text.Trim();
            string clave = txtClave.Text;
            object selectedRol = cmbRol.SelectedValue;

            if (string.IsNullOrEmpty(nombre))
            {
                Alertas.Advertencia("Por favor, complete el campo Nombre antes de guardar.");
                return;
            }
            if (string.IsNullOrEmpty(usuario))
            {
                Alertas.Advertencia("Por favor, complete el campo Usuario antes de guardar.");
                return;
            }

            if (selectedRol == null || !int.TryParse(selectedRol.ToString(), out int idRol))
            {
                Alertas.Advertencia("Por favor, seleccione un Rol válido.");
                return;
            }

            if (string.IsNullOrEmpty(clave))
            {
                Alertas.Advertencia("Por favor, ingrese una contraseña.");
                return;
            }

            string estado = "Activo";

            try
            {
                byte[] saltBytes = GenerarSalt();
                byte[] hashBytes = ComputeHash(clave, saltBytes, PBKDF2_ITERATIONS, PBKDF2_HASH_BYTES);
                // Construi un comando parametrizado que envíe byte[] a columnas VARBINARY
                string sql = @"INSERT INTO Usuarios (Nombre, Apellido, Usuario, Clave, Sal, Iteraciones, IdRol, Estado)
                               VALUES (@Nombre, @Apellido, @Usuario, @Clave, @Sal, @Iteraciones, @IdRol, @Estado);";
                using (var cmd = new SqlCommand(sql)) //ocupe el SqlCommand para comandos parametrizados y evitar inyecciones incorrectas en SQL
                {
                    cmd.Parameters.Add("@Nombre", SqlDbType.NVarChar, 100).Value = nombre;
                    cmd.Parameters.Add("@Apellido", SqlDbType.NVarChar, 100).Value = apellido;
                    cmd.Parameters.Add("@Usuario", SqlDbType.NVarChar, 50).Value = usuario;
                    cmd.Parameters.Add("@Clave", SqlDbType.VarBinary, PBKDF2_HASH_BYTES).Value = hashBytes;
                    cmd.Parameters.Add("@Sal", SqlDbType.VarBinary, SALT_BYTES).Value = saltBytes;
                    cmd.Parameters.Add("@Iteraciones", SqlDbType.Int).Value = PBKDF2_ITERATIONS;
                    cmd.Parameters.Add("@IdRol", SqlDbType.Int).Value = idRol;
                    cmd.Parameters.Add("@Estado", SqlDbType.VarChar, 15).Value = estado;

                    int filas = consulta.EjecutarComando(cmd);
                    if (filas > 0)
                    {
                        Alertas.Realizado($"El Usuario {nombre} se registró con éxito");
                        MostrarRegistros("Activo");
                        rbDatosActivos.Checked = true;
                        HabilitarNuevosRegistros(false);
                        LimpiarCampos();
                    }
                    else
                    {
                        Alertas.Advertencia("No se pudo guardar el Usuario. Intente nuevamente.");
                    }
                }
            }
            catch (Exception ex)
            {
                Alertas.Advertencia($"Error al guardar usuario: {ex.Message}");
            }
        }

        void ActualizarUsuario()
        {
            string msg = "¿Desea actualizar este Usuario?";

            if (!Alertas.Confirmacion(msg)) return;

            try
            {
                int idUsuario = -1;
                int.TryParse(txtIdUsuario.Text.Trim(), out idUsuario);
                string nombre = txtNombre.Text.Trim();
                string apellido = txtApellido.Text.Trim();
                string usuario = txtUsuario.Text.Trim();
                string clave = txtClave.Text;
                object selectRol = cmbRol.SelectedValue;
                string estado = lblEstado.Text.Trim();

                if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(usuario))
                {
                    Alertas.Advertencia("Nombre y Usuario son obligatorios.");
                    return;
                }

                if (selectRol == null || !int.TryParse(selectRol.ToString(), out int idRol))
                {
                    Alertas.Advertencia("Seleccione un rol válido.");
                    return;
                }
                if (!string.IsNullOrEmpty(clave))
                {
                    byte[] saltBytes = GenerarSalt();
                    byte[] hashBytes = ComputeHash(clave, saltBytes, PBKDF2_ITERATIONS, PBKDF2_HASH_BYTES);

                    string sql = @"UPDATE Usuarios
                                   SET Nombre = @Nombre,
                                       Apellido = @Apellido,
                                       Usuario = @Usuario,
                                       Clave = @Clave,
                                       Sal = @Sal,
                                       Iteraciones = @Iteraciones,
                                       IdRol = @IdRol,
                                       Estado = @Estado
                                   WHERE IdUsuario = @IdUsuario;";

                    using (var cmd = new SqlCommand(sql))
                    {
                        cmd.Parameters.Add("@Nombre", SqlDbType.NVarChar, 100).Value = nombre;
                        cmd.Parameters.Add("@Apellido", SqlDbType.NVarChar, 100).Value = apellido;
                        cmd.Parameters.Add("@Usuario", SqlDbType.NVarChar, 50).Value = usuario;
                        cmd.Parameters.Add("@Clave", SqlDbType.VarBinary, PBKDF2_HASH_BYTES).Value = hashBytes;
                        cmd.Parameters.Add("@Sal", SqlDbType.VarBinary, SALT_BYTES).Value = saltBytes;
                        cmd.Parameters.Add("@Iteraciones", SqlDbType.Int).Value = PBKDF2_ITERATIONS;
                        cmd.Parameters.Add("@IdRol", SqlDbType.Int).Value = idRol;
                        cmd.Parameters.Add("@Estado", SqlDbType.VarChar, 15).Value = estado;
                        cmd.Parameters.Add("@IdUsuario", SqlDbType.Int).Value = idUsuario;

                        int filas = consulta.EjecutarComando(cmd);
                        if (filas > 0)
                        {
                            Alertas.Realizado("Los datos se actualizaron con exito");
                            MostrarRegistros("Activo");
                            HabilitarNuevosRegistros(false);
                            LimpiarCampos();
                            btnDesactivarRegistro.Enabled = false;
                            btnReactivarRegistro.Enabled = false;
                            rbDatosActivos.Checked = true;
                            lblEstado.Visible = false;
                        }
                        else
                        {
                            Alertas.Advertencia("Error al actualizar el usuario");
                        }
                    }
                }
                else
                {
                    string sql = @"UPDATE Usuarios
                                   SET Nombre = @Nombre,
                                       Apellido = @Apellido,
                                       Usuario = @Usuario,
                                       IdRol = @IdRol,
                                       Estado = @Estado
                                   WHERE IdUsuario = @IdUsuario;";
                    using (var cmd = new SqlCommand(sql))
                    {
                        cmd.Parameters.Add("@Nombre", SqlDbType.NVarChar, 100).Value = nombre;
                        cmd.Parameters.Add("@Apellido", SqlDbType.NVarChar, 100).Value = apellido;
                        cmd.Parameters.Add("@Usuario", SqlDbType.NVarChar, 50).Value = usuario;
                        cmd.Parameters.Add("@IdRol", SqlDbType.Int).Value = idRol;
                        cmd.Parameters.Add("@Estado", SqlDbType.VarChar, 15).Value = estado;
                        cmd.Parameters.Add("@IdUsuario", SqlDbType.Int).Value = idUsuario;

                        int filas = consulta.EjecutarComando(cmd);
                        if (filas > 0)
                        {
                            Alertas.Realizado("Los datos se actualizaron con exito");
                            MostrarRegistros("Activo");
                            HabilitarNuevosRegistros(false);
                            LimpiarCampos();
                            btnDesactivarRegistro.Enabled = false;
                            btnReactivarRegistro.Enabled = false;
                            rbDatosActivos.Checked = true;
                        }
                        else
                        {
                            Alertas.Advertencia("Error al actualizar el usuario");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Alertas.Advertencia($"Error al actualizar el usuario: {ex.Message}");
            }
        }
        
        void MostrarRegistros(string estado)
        {
            try
            
                { 
                string columnas = "IdUsuario, Nombre, Apellido, Usuario, IdRol, Estado";
                string condicion = $"Estado = '{estado}'";
                DataTable dt = consulta.Buscar("Usuarios", columnas, condicion);
                if (dt != null)
                {
                    if (dt.Columns.Contains("Clave")) dt.Columns.Remove("Clave");
                    if (dt.Columns.Contains("Sal")) dt.Columns.Remove("Sal");
                    if (dt.Columns.Contains("Iteraciones")) dt.Columns.Remove("Iteraciones");
                }
                dgvDatos.DataSource = dt;
                dgvDatos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgvDatos.Refresh();

                colorColumnaEstado();
            }
            catch (Exception ex)
            {
                Alertas.Advertencia($"Error al mostrar registros: {ex.Message}");
            }
        }
        void ActivarDesactivarRegistros(string estadoActual, string nuevoEstado)
        {
            string estado = lblEstado.Text;

            if (estado == estadoActual)
            {
                lblEstado.Text = nuevoEstado;
            }
            else if (nuevoEstado == lblEstado.Text)
            {
                Alertas.Advertencia("Usted ya " + nuevoEstado + " este usuario. Guarde los cambios");
            }
        }

     

        void colorColumnaEstado()
        {
            foreach (DataGridViewRow fila in dgvDatos.Rows)
            {
                if (fila.Cells["Estado"].Value != null)
                {
                    string valor = fila.Cells["Estado"].Value.ToString();

                    if (valor == "Activo")
                    {
                        fila.Cells["Estado"].Style.BackColor = Color.LightGreen;
                        fila.Cells["Estado"].Style.ForeColor = Color.Black;
                    }
                    else if (valor == "Inactivo")
                    {
                        fila.Cells["Estado"].Style.BackColor = Color.LightCoral;
                        fila.Cells["Estado"].Style.ForeColor = Color.White;
                    }
                }
            }
        }

        void EnviarDatosParaEditar(DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    DataGridViewRow fila = dgvDatos.Rows[e.RowIndex];
                    try { txtIdUsuario.Text = fila.Cells["IdUsuario"].Value.ToString(); } catch { txtIdUsuario.Text = string.Empty; }
                    txtNombre.Text = fila.Cells["Nombre"].Value.ToString();
                    txtApellido.Text = fila.Cells["Apellido"].Value.ToString();
                    txtUsuario.Text = fila.Cells["Usuario"].Value.ToString();
                    // No rellenar el campo de contraseña con el hash - dejar vacío para forzar cambio si se desea
                    txtClave.Text = string.Empty;
                    try { cmbRol.SelectedValue = fila.Cells["IdRol"].Value; } catch { }
                    lblEstado.Text = fila.Cells["Estado"].Value.ToString();
                    lblOperacion.Text = "Editando...";
                    HabilitarNuevosRegistros(true);
                    if (lblEstado.Text == "Activo")
                    {
                        lblEstado.Visible = true;
                        btnDesactivarRegistro.Enabled = true;
                        btnReactivarRegistro.Enabled = false;
                        HabilitarNuevosRegistros(true);
                    }
                    else if (lblEstado.Text == "Inactivo")
                    {
                        lblEstado.Visible = true;
                        btnDesactivarRegistro.Enabled = false;
                        btnReactivarRegistro.Enabled = true;
                        HabilitarNuevosRegistros(true);
                    }

                    lblOperacion.Visible = true;
                }
            }
            catch (Exception ex)
            {
                Alertas.Advertencia($"Error al cargar los datos: {ex.Message}");
                lblOperacion.Visible = false;
            }
        }

        void TestConexion()
        {
            conexion.validarConexion();
        }

        private void btnNuevoRegistro_Click(object sender, EventArgs e)
        {
            lblOperacion.Text = "Registrando...";
            HabilitarNuevosRegistros(true);
        }

        private void btnGuardarRegistro_Click(object sender, EventArgs e)
        {
            if (lblOperacion.Text == "Registrando...")
            {
                GuardarUsuario();
            }
            else if (lblOperacion.Text == "Editando...")
            {
                ActualizarUsuario();
            }
        }

        private void btnDesactivarRegistro_Click(object sender, EventArgs e)
        {
            ActivarDesactivarRegistros("Activo", "Inactivo");
        }

        private void rbDatosActivos_CheckedChanged(object sender, EventArgs e)
        {
            if (rbDatosActivos.Checked)
            {
                MostrarRegistros("Activo");
            }
        }

        private void FormUsuariosLogin_Load(object sender, EventArgs e)
        {
            MostrarRegistros("Activo");
        }

        private void btnCancelarRegistro_Click(object sender, EventArgs e)
        {
            HabilitarNuevosRegistros(false);
            btnDesactivarRegistro.Enabled = false;
            btnReactivarRegistro.Enabled = false;
            lblEstado.Visible = false;
            LimpiarCampos();
        }

        private void btnActualizarDGV_Click(object sender, EventArgs e)
        {
            MostrarRegistros("Activo");
            Alertas.Confirmacion("Datos Actualizados con Exito");
            rbDatosActivos.Checked = true;
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtFiltrar_TextChanged(object sender, EventArgs e)
        {
            string text = txtFiltrar.Text;

            DataTable dt = dgvDatos.DataSource as DataTable;
            if (dt != null)
            {
                dt.DefaultView.RowFilter =
                    $"Convert(Nombre, 'System.String') LIKE '%{text}%' OR " +
                    $"Apellido LIKE '%{text}%' OR " +
                    $"Usuario LIKE '%{text}%'";
            }
            colorColumnaEstado();
        }

        private void btnReactivarRegistro_Click(object sender, EventArgs e)
        {
            ActivarDesactivarRegistros("Inactivo", "Activo");
        }

        private void dgvDatos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            EnviarDatosParaEditar(e);
        }

        private void btnTestConexion_Click(object sender, EventArgs e)
        {
            TestConexion();
        }

        private void rbDatosInactivos_CheckedChanged(object sender, EventArgs e)
        {
            if (rbDatosInactivos.Checked)
            {
                MostrarRegistros("Inactivo");
            }
        }
    }
}
