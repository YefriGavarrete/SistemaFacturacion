using Sistema_GestionFacturacion.Clases;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace Sistema_GestionFacturacion.Formularios
{
    public partial class FormEmpleado : Form
    {
        ConsultasSQL consulta = new ConsultasSQL();
        AlertasDelSistema Alertas = new AlertasDelSistema();
        Conexion conexion = new Conexion();
        public FormEmpleado()
        {
            InitializeComponent();
            CargarCargos();
        }
        void CargarCargos()
        {
            try
            {
                DataTable dtRoles = consulta.Buscar("Cargos", "IdCargo, Cargo", "Estado = 'Activo'");
                if (dtRoles != null && dtRoles.Rows.Count > 0)
                {
                    cmbCargo.Enabled = false;
                    cmbCargo.DataSource = dtRoles;
                    cmbCargo.DisplayMember = "Cargo";
                    cmbCargo.ValueMember = "IdCargo";
                    cmbCargo.SelectedIndex = -1;
                }
                else
                {
                    cmbCargo.DataSource = null;
                    cmbCargo.Items.Clear();
                }
            }
            catch (Exception ex)
            {
                Alertas.Advertencia($"Error al mostrar los Cargos: {ex.Message}");
            }

        }
        void LimpiarCampos()
        {
            txtIdEmpleado.Clear();
            txtNombre.Clear();
            txtApellido.Clear();
            txtDNI.Clear();
            cmbCargo.SelectedIndex = -1;
            
        }
        void HabilitarNuevosRegistros(bool valor)
        {
            btnGuardarRegistro.Enabled = valor;
            btnCancelarRegistro.Enabled = valor;
            txtNombre.Enabled = valor;
            txtApellido.Enabled = valor;
            txtDNI.Enabled = valor;
            cmbCargo.Enabled = valor;
            btnNuevoRegistro.Enabled = !valor;
        }
        void GuardarEmpleado()
        {
            string nombreEmpleado= txtNombre.Text.Trim();
            string apellidoEmpleado = txtApellido.Text.Trim();
            string DNI = txtDNI.Text;
            object selectedCargo = cmbCargo.SelectedValue;

            if (string.IsNullOrEmpty(nombreEmpleado)
                || string.IsNullOrEmpty(apellidoEmpleado)
                || string.IsNullOrEmpty(DNI))

            {
                Alertas.Advertencia("Por favor, complete todos los campos antes de registrar.");
                return;
            }
            if (selectedCargo== null || !int.TryParse(selectedCargo.ToString(), out int cargo))
            {
                Alertas.Advertencia("Por favor, seleccione un Cargo válido.");
                return;
            }
            string estado = "Activo";
            string columnas = "NombreEmpleado, ApellidoEmpleado, IdCargo, DNI, Estado";
            string valores = $"'{nombreEmpleado}', '{apellidoEmpleado}', '{cargo}',  '{DNI}', '{(estado)}'";
            if (consulta.Guardar("Empleados", columnas, valores))
            {
                Alertas.Realizado($"El Empleado {nombreEmpleado} se registro con éxito");
                MostrarRegistros("Activo");
                rbDatosActivos.Checked = true;
                HabilitarNuevosRegistros(false);
                LimpiarCampos();
            }
            else
            {
                Alertas.Advertencia("No se pudo guardar el Empleado. Intente nuevamente.");
            }
        }
        private void MostrarRegistros(string estado)
        {
            try
            {
                string columnas = "IdEmpleado, NombreEmpleado, ApellidoEmpleado, IdCargo, DNI, Estado";
                string condicion = $"Estado = '{estado}'";
                DataTable dt = consulta.Buscar("Empleados", columnas, condicion);
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
        void EnviarDatosParaEditar(DataGridViewCellEventArgs e)
        {
            try
            {
                lblOperacion.Text = "Editando";
                lblOperacion.Visible = true;

                if (e.RowIndex >= 0)
                {
                    DataGridViewRow fila = dgvDatos.Rows[e.RowIndex];
                    txtIdEmpleado.Text = fila.Cells["IdEmpleado"].Value.ToString();
                    txtNombre.Text = fila.Cells["NombreEmpleado"].Value.ToString();
                    txtApellido.Text = fila.Cells["ApellidoEmpleado"].Value.ToString();
                    try { cmbCargo.SelectedValue = fila.Cells["IdCargo"].Value; } catch { cmbCargo.SelectedIndex = -1; }
                    txtDNI.Text = fila.Cells["DNI"].Value.ToString();
                    lblEstado.Text = fila.Cells["Estado"].Value.ToString();
                }

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

                lblOperacion.Visible = false;
            }
            catch (Exception ex)
            {
                Alertas.Advertencia($"Error al cargar datos: {ex.Message}");
                lblOperacion.Visible = false;
            }
        }

        void ActualizarEmpleado()
        {

            string msg = "¿Desea actualizar este Empleado?";
            if (Alertas.Confirmacion(msg))
            {
                try
                {

                    int IdEmpleado = int.Parse(txtIdEmpleado.Text);
                    
                    string nombreEmpleado = txtNombre.Text.Trim();
                    string apellidoEmpleado = txtApellido.Text.Trim();
                    string DNI = txtDNI.Text.Trim();
                    object selectedCargo = cmbCargo.SelectedValue;

                    if (selectedCargo == null || !int.TryParse(selectedCargo.ToString(), out int idcargo))
                    {
                        Alertas.Advertencia("Seleccione un cargo");
                        return;

                    }
                    string estado = lblEstado.Text.Trim();

                    string actualizar = $" NombreEmpleado = '{nombreEmpleado}', " +
                        $" ApellidoEmpleado = '{apellidoEmpleado}', " +
                        $" IdCargo = '{idcargo}', " +
                        $" DNI = '{DNI}', " +
                        $" Estado = '{estado}'";

                    string condicion = $"IdEmpleado='{IdEmpleado}'";
                     
                    if (consulta.update("Empleados", actualizar, condicion) > 0)
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
                        Alertas.Advertencia("Error al actualizar el Empleado, Intente de nuevo");
                    }

                }
                catch (Exception ex)
                {
                    Alertas.Advertencia($"Error al actualizar, Pruebe de otra manera :{ex.Message}");

                }
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


        void ActivarDesactivarRegistro(string estadoActual, string nuevoEstado)
        {
            string estado = lblEstado.Text;

            if (estado == estadoActual)
            {
                lblEstado.Text = nuevoEstado;
            }
            else if (nuevoEstado == estado)
            {
                Alertas.Advertencia("Usted ya " + nuevoEstado + " este registro. GUARDE los cambios..!!");
            }
        }

 
        private void btnNuevoRegistro_Click(object sender, EventArgs e)
        {
            lblOperacion.Text = "Registrando";
            HabilitarNuevosRegistros(true);
        }

        private void btnGuardarRegistro_Click(object sender, EventArgs e)
        {
            if (lblOperacion.Text == "Registrando")
            {
                GuardarEmpleado();
            }
            else if (lblOperacion.Text == "Editando")
            {
                ActualizarEmpleado();

            }

        }

        private void btnCancelarRegistro_Click(object sender, EventArgs e)
        {
            HabilitarNuevosRegistros(false);
            btnDesactivarRegistro.Enabled = false;
            btnReactivarRegistro.Enabled = false;
            lblEstado.Visible = false;
            LimpiarCampos();

        }

        private void btnTestConexion_Click(object sender, EventArgs e)
        {
            conexion.validarConexion();
        }

        private void btnActualizarDGV_Click(object sender, EventArgs e)
        {
            MostrarRegistros("Activo");
            Alertas.Confirmacion("Datos Actualizados con Exito");
            rbDatosActivos.Checked = true;
        }

        private void txtFiltrar_TextChanged(object sender, EventArgs e)
        {
            string texto = txtFiltrar.Text.Trim();

            DataTable dt = dgvDatos.DataSource as DataTable;
            if (dt != null)
            {
                dt.DefaultView.RowFilter =
                    $"Convert(NombreEmpleado, 'System.String') LIKE '%{texto}%' OR " +
                    $"ApellidoEmpleado LIKE '%{texto}%' OR " +
                    $"DNI LIKE '%{texto}%'";
            }
            colorColumnaEstado();
        }

        private void rbDatosActivos_CheckedChanged(object sender, EventArgs e)
        {
            if (rbDatosActivos.Checked)
            {
                MostrarRegistros("Activo");
            }

        }

        private void FormEmpleado_Load(object sender, EventArgs e)
        {
            MostrarRegistros("Activo");

        }

        private void rbDatosInactivos_CheckedChanged_1(object sender, EventArgs e)
        {
            if (rbDatosInactivos.Checked)
            {
                MostrarRegistros("Inactivo");
            }

        }
        private void btnDesactivarRegistro_Click_1(object sender, EventArgs e)
        {
            ActivarDesactivarRegistro("Activo", "Inactivo");
        }

        private void btnReactivarRegistro_Click_1(object sender, EventArgs e)
        {
            ActivarDesactivarRegistro("Inactivo", "Activo");

        }

        private void dgvDatos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            EnviarDatosParaEditar(e);
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
