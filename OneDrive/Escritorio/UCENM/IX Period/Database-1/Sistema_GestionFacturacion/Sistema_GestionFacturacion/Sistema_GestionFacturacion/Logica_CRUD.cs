using Sistema_GestionFacturacion.Clases;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sistema_GestionFacturacion
{
    public partial class Logica_CRUD : Form
    {
        ConsultasSQL consulta = new ConsultasSQL();
        Conexion conexion = new Conexion();
        AlertasDelSistema Alertas = new AlertasDelSistema();

        public Logica_CRUD()
        {
            InitializeComponent();
            MostrarRegistros("Activo");
            rbDatosActivos.Checked = true;
        }

        void HabilitarCamposNuevoRegistro(bool valor)
        {
            btnGuardarRegistro.Enabled = valor;
            btnCancelarRegistro.Enabled = valor;
            txtNumeroCuenta.Enabled = valor;
            txtNombre.Enabled = valor;
            txtApellido.Enabled = valor;
            txtTelefono.Enabled = valor;
            txtCorreo.Enabled = valor;
            dtpFechaNacimiento.Enabled = valor;
        }
        void limpiarCampos()
        {
            txtNumeroCuenta.Clear();
            txtNombre.Clear();
            txtApellido.Clear();
            txtTelefono.Clear();
            txtCorreo.Clear();
            dtpFechaNacimiento.Value = DateTime.Now;
            dtpFechaCreacionUser.Value = DateTime.Now;
            lblEdad.Text = "00";
            lblEstado.Visible = false;
            lblOperacion.Visible = false;   
            btnDesactivarRegistro.Enabled = false;
            btnReactivarRegistro.Enabled = false;
        }
        void CalcularEdad()
        {
            DateTime fechaNacimiento = dtpFechaNacimiento.Value;
            DateTime fechaActual = DateTime.Now;

            int años = fechaActual.Year - fechaNacimiento.Year;
            int meses = fechaActual.Month - fechaNacimiento.Month;

            if (fechaActual.Month < fechaNacimiento.Month)
                meses--;

            if (meses < 0)
            {
                años--;
                meses += 12;
            }

            lblEdad.Text = años.ToString();
        }

        //Logica para guardar un nuevo registro en la base de datos
        void Registrar()
        {
            string numeroCuenta = txtNumeroCuenta.Text.Trim();
            string nombre = txtNombre.Text.Trim();
            string apellido = txtApellido.Text.Trim();
            string telefono = txtTelefono.Text.Trim();
            string correo = txtCorreo.Text.Trim();

            if (string.IsNullOrEmpty(numeroCuenta) 
                || string.IsNullOrEmpty(nombre) 
                || string.IsNullOrEmpty(apellido) 
                || string.IsNullOrEmpty(telefono) 
                || string.IsNullOrEmpty(correo))
            {
                Alertas.Advertencia("Por favor, complete todos los campos antes de registrar.");
                return;
            }

            if (ExisteNumeroCuenta(numeroCuenta))
            {
                return;
            }

            string fechaSQL = "yyyy-MM-dd HH:mm:ss";

            DateTime fechaNacimiento = dtpFechaNacimiento.Value;
            DateTime fechaCreacionUsuario = DateTime.Now;
            
            string fechaNac = fechaNacimiento.ToString(fechaSQL);
            string fechaCreacion = fechaCreacionUsuario.ToString(fechaSQL);

            string edad = lblEdad.Text.Trim();
            string estado = "Activo";

            string columnas = "NumeroCuenta, Nombre, Apellido, Telefono, Correo, Edad, FechaNacimiento, FechaCreacionUsuario, Estado";
            string valores = $"'{numeroCuenta}', '{nombre}', '{apellido}', '{telefono}', '{correo}', '{edad}', '{(fechaNac)}', '{(fechaCreacion)}', '{(estado)}'";

            if (consulta.Guardar("IntegrantesGrupo5", columnas, valores))
            {
                Alertas.Realizado($"El Usuario '{nombre + apellido}' Se registro con Exito");
                MostrarRegistros("Activo");
                rbDatosActivos.Checked = true;
                HabilitarCamposNuevoRegistro(false);
                limpiarCampos();
            }
            else
            {
                Alertas.Advertencia("Error al registrar el usuario");
            }
        }
        //validar si el numero de cuenta ya existe en la base de datos
        private bool ExisteNumeroCuenta(string numeroCuenta)
        {
            try
            {
                string columnas = "NumeroCuenta";
                string condicion = $"NumeroCuenta = '{numeroCuenta}'";

                DataTable dt = consulta.Buscar("IntegrantesGrupo5", columnas, condicion);

                if (dt.Rows.Count > 0)
                {
                    Alertas.Advertencia("El numero de cuenta ya existe.");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Alertas.Advertencia($"Error al validar numero de cuenta: {ex.Message}");
                return true; 
            }
        }
        //logica para mostrar los registros en el datagridview
        private void MostrarRegistros(string estado)
        {
            try
            {
                string columnas = "NumeroCuenta, Nombre, Apellido, Telefono, Correo, Edad, FechaNacimiento, FechaCreacionUsuario, Estado";
                string condicion = $"Estado = '{estado}'";
                DataTable dt = consulta.Buscar("IntegrantesGrupo5", columnas, condicion);
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

        //logica para enviar los datos del datagridview a los textbox para editar
        void EnviarDatosParaEditar(DataGridViewCellEventArgs e)
        {
            try
            {
                lblOperacion.Text = "Editando...";
                lblOperacion.Visible = true;

                if (e.RowIndex >= 0)
                {
                    DataGridViewRow fila = dgvDatos.Rows[e.RowIndex];

                    txtNumeroCuenta.Text = fila.Cells["NumeroCuenta"].Value.ToString();
                    txtNombre.Text = fila.Cells["Nombre"].Value.ToString();
                    txtApellido.Text = fila.Cells["Apellido"].Value.ToString();
                    txtTelefono.Text = fila.Cells["Telefono"].Value.ToString();
                    txtCorreo.Text = fila.Cells["Correo"].Value.ToString();
                    lblEdad.Text = fila.Cells["Edad"].Value.ToString();

                    dtpFechaNacimiento.Value = Convert.ToDateTime(fila.Cells["FechaNacimiento"].Value);
                    dtpFechaCreacionUser.Value = Convert.ToDateTime(fila.Cells["FechaCreacionUsuario"].Value);

                    lblEstado.Text = fila.Cells["Estado"].Value.ToString();
                }

                if (lblEstado.Text == "Activo")
                {
                    lblEstado.Visible = true;
                    btnDesactivarRegistro.Enabled = true;
                    btnReactivarRegistro.Enabled = false;
                    btnNuevoRegistro.Enabled = false;

                    HabilitarCamposNuevoRegistro(true);
                }                    
                else if (lblEstado.Text == "Inactivo")
                {
                    lblEstado.Visible = true;
                    btnDesactivarRegistro.Enabled = false;
                    btnReactivarRegistro.Enabled = true;
                    btnNuevoRegistro.Enabled = false;

                    HabilitarCamposNuevoRegistro(true);
                }

                lblOperacion.Visible = false;
                txtNumeroCuenta.Enabled = false; // Evitar que se edite el numero de cuenta
            }
            catch (Exception ex)
            {
                Alertas.Advertencia($"Error al cargar datos: {ex.Message}");
                lblOperacion.Visible = false;
            }
        }

        //actualizar/editar un registro en la base de datos
        void ActualizarRegistro()
        {
            string msg = "¿Desea actualizar este usuario?";

            if (Alertas.Confirmacion(msg) == true)
            {
                try
                {
                    string numeroCuenta = txtNumeroCuenta.Text.Trim();
                    string nombre = txtNombre.Text.Trim();
                    string apellido = txtApellido.Text.Trim();
                    string telefono = txtTelefono.Text.Trim();
                    string correo = txtCorreo.Text.Trim();
                    string edad = lblEdad.Text.Trim();
                    string fechaNacimiento = dtpFechaNacimiento.Value.ToString("yyyy-MM-dd HH:mm:ss");
                    string fechaCreacion = dtpFechaCreacionUser.Value.ToString("yyyy-MM-dd HH:mm:ss");
                    string estado = lblEstado.Text.Trim(); 
                    string actualizar = $"NumeroCuenta = '{numeroCuenta}', Nombre = '{nombre}', Apellido = '{apellido}', " +
                                        $"Telefono = '{telefono}', Correo = '{correo}', Edad = '{edad}', " +
                                        $"FechaNacimiento = '{fechaNacimiento}', FechaCreacionUsuario = '{fechaCreacion}', " +
                                        $"Estado = '{estado}'";

                    string condicion = $"NumeroCuenta='{numeroCuenta}'";

                    if (consulta.update("IntegrantesGrupo5", actualizar, condicion) > 0)
                    {
                        Alertas.Realizado("Los datos se actualizaron con exito");
                        MostrarRegistros("Activo");
                        HabilitarCamposNuevoRegistro(false);
                        limpiarCampos();
                        btnNuevoRegistro.Enabled = true;
                        btnDesactivarRegistro.Enabled = false;
                        btnReactivarRegistro.Enabled = false;
                        rbDatosActivos.Checked = true;
                    }
                    else
                    {
                        Alertas.Advertencia("Error al actualizar el usuario");
                    }
                }
                catch (Exception ex)
                {
                    Alertas.Advertencia($"Error al actualizar usuario: {ex.Message}");
                }
            }
        }

        //logica para activar o desactivar un registro en la base de datos
        void ActivarDesactivarRegistro(string estadoActual, string nuevoEstado)
        {
            string estado = lblEstado.Text;

            if(estado == estadoActual)
            {
                lblEstado.Text = nuevoEstado;
            }
            else if(nuevoEstado == estado)
            {
                Alertas.Advertencia("Usted ya " + nuevoEstado + " este registro. GUARDE los cambios..!!");
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
        void TestConexion()
        {
            conexion.validarConexion();
        }

        private void btnTestConexion_Click(object sender, EventArgs e)
        {
            TestConexion();
        }

        private void dtpFechaNacimiento_ValueChanged(object sender, EventArgs e)
        {
            CalcularEdad();
        }

        private void btnNuevoRegistro_Click(object sender, EventArgs e)
        {
            lblOperacion.Text = "Registrando...";
            HabilitarCamposNuevoRegistro(true);
        }

        private void btnGuardarRegistro_Click(object sender, EventArgs e)
        {
            if (lblOperacion.Text == "Registrando...")
            {
                Registrar();
            }
            else if (lblOperacion.Text == "Editando...")
            {
                ActualizarRegistro();
            }
        }

        private void dgvDatosActivos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            EnviarDatosParaEditar(e);
        }

        private void btnDesactivarRegistro_Click(object sender, EventArgs e)
        {
            ActivarDesactivarRegistro("Activo", "Inactivo");
        }

        private void rbDatosActivos_CheckedChanged(object sender, EventArgs e)
        {
            if (rbDatosActivos.Checked)
            {
                MostrarRegistros("Activo");
            }
        }

        private void rbDatosInactivos_CheckedChanged(object sender, EventArgs e)
        {
            if (rbDatosInactivos.Checked)
            {
                MostrarRegistros("Inactivo");
            }
        }

        private void Logica_CRUD_Load(object sender, EventArgs e)
        {
            MostrarRegistros("Activo");
        }

        private void btnCancelarRegistro_Click(object sender, EventArgs e)
        {
            HabilitarCamposNuevoRegistro(false);
            limpiarCampos();
        }

        private void btnReactivarRegistro_Click(object sender, EventArgs e)
        {
            ActivarDesactivarRegistro("Inactivo", "Activo");
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
            string texto = txtFiltrar.Text.Trim();

            DataTable dt = dgvDatos.DataSource as DataTable;
            if (dt != null)
            {
                dt.DefaultView.RowFilter =
                    $"Convert(NumeroCuenta, 'System.String') LIKE '%{texto}%' OR " +
                    $"Nombre LIKE '%{texto}%' OR " +
                    $"Apellido LIKE '%{texto}%'";
            }
            colorColumnaEstado();
        }
    }
}
