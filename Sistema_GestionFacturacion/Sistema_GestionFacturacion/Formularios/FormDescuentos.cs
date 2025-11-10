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

namespace Sistema_GestionFacturacion.Formularios
{
    public partial class FormDescuentos : Form
    {
        ConsultasSQL consulta = new ConsultasSQL();
        AlertasDelSistema Alertas = new AlertasDelSistema();
        Conexion conexion = new Conexion();
        public FormDescuentos()
        {
            InitializeComponent();
            MostrarRegistros("Activo");
            rbDatosActivos.Checked = true;
        }
        void LimpiarCampos()
        {
            txtIdDescuento.Clear();
            txtDescuento.Clear();
            txtDescripcion.Clear();
        }

        void HabilitarNuevosRegistros(bool valor)
        {
            btnGuardarRegistro.Enabled = valor;
            btnCancelarRegistro.Enabled = valor;
            txtDescuento.Enabled = valor;
            txtDescripcion.Enabled = valor;
            btnNuevoRegistro.Enabled = !valor;
        }

        void GuardarDescuento()
        {
            string descuento = txtDescuento.Text.Trim();
            string descripcion = txtDescripcion.Text.Trim();

            if (string.IsNullOrEmpty(descuento)
                 || string.IsNullOrEmpty(descripcion))
            {
                Alertas.Advertencia("Por favor, complete todos los campos antes de registrar.");
                return;
            }
            string estado = "Activo";
            string columnas = "Descuento, Descripcion, Estado";
            string valores = $"'{descuento}', '{descripcion}', '{(estado)}'";
            if (consulta.Guardar("Descuento", columnas, valores))
            {
                Alertas.Realizado($"El Descuento {descuento} se registro con éxito");
                MostrarRegistros("Activo");
                rbDatosActivos.Checked = true;
                HabilitarNuevosRegistros(false);
                LimpiarCampos();
            }
            else
            {
                Alertas.Advertencia("No se pudo guardar el Descuento. Intente nuevamente.");
            }
            
        }
        private void MostrarRegistros(string estado)
        {
            try
            {
                string columnas = "IdDescuento, Descuento, Descripcion, Estado";
                string condicion = $"Estado = '{estado}'";
                DataTable dt = consulta.Buscar("Descuento", columnas, condicion);
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
                    txtIdDescuento.Text = fila.Cells["IdDescuento"].Value.ToString();
                    txtDescuento.Text = fila.Cells["Descuento"].Value.ToString();
                    txtDescripcion.Text = fila.Cells["Descripcion"].Value.ToString();
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

        void ActualizarDescuento()
        {

            string msg = "¿Desea actualizar este Descuento?";
            if (Alertas.Confirmacion(msg))
            {
                try
                {

                    int idDescuento = int.Parse(txtIdDescuento.Text);
                    string descuento = txtDescuento.Text.Trim();
                    string descripcion = txtDescripcion.Text.Trim();
                    string estado = lblEstado.Text.Trim();

                    string actualizar = $" Descuento = '{descuento}', " +
                        $" Descripcion = '{descripcion}', " +
                        $" Estado = '{estado}'";

                    string condicion = $"IdDescuento= '{idDescuento}'";

                    if (consulta.update("Descuento", actualizar, condicion) > 0)
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
                        Alertas.Advertencia("Error al actualizar el Descuento, Intente de nuevo");
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

        private void btnNuevoRegistro_Click_1(object sender, EventArgs e)
        {
            lblOperacion.Text = "Registrando";
            HabilitarNuevosRegistros(true);
        }

        private void btnGuardarRegistro_Click_1(object sender, EventArgs e)
        {
            if (lblOperacion.Text == "Registrando")
            {
                GuardarDescuento();
            }
            else if (lblOperacion.Text == "Editando")
            {
                ActualizarDescuento();
            }
        }

        private void btnCancelarRegistro_Click_1(object sender, EventArgs e)
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

        private void btnActualizarDGV_Click_1(object sender, EventArgs e)
        {
            MostrarRegistros("Activo");
            Alertas.Confirmacion("Datos Actualizados con Exito");
            rbDatosActivos.Checked = true;
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

        private void btnDesactivarRegistro_Click(object sender, EventArgs e)
        {
            ActivarDesactivarRegistro("Activo", "Inactivo");
        }

        private void btnReactivarRegistro_Click(object sender, EventArgs e)
        {
            ActivarDesactivarRegistro("Inactivo", "Activo");
        }

        private void FormDescuentos_Load(object sender, EventArgs e)
        {
            MostrarRegistros("Activo");
        }

        private void txtFiltrar_TextChanged_1(object sender, EventArgs e)
        {
            string texto = txtFiltrar.Text.Trim();

            DataTable dt = dgvDatos.DataSource as DataTable;
            if (dt != null)
            {
                dt.DefaultView.RowFilter =
                    $"Convert(Descuento, 'System.String') LIKE '%{texto}%' OR " +
                    $"Descripcion LIKE '%{texto}%'";
        }
            colorColumnaEstado();

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