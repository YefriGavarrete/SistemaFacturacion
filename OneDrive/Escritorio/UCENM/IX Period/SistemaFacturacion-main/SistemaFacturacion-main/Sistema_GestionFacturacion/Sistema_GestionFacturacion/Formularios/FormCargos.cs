using Sistema_GestionFacturacion.Clases;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sistema_GestionFacturacion.Formularios
{
    public partial class FormCargos : Form
    {

        AlertasDelSistema Alertas = new AlertasDelSistema();
        ConsultasSQL consulta = new ConsultasSQL();
        Conexion conexion = new Conexion();
        public FormCargos()
        {
            InitializeComponent();
            MostrarRegistros("Activo");
            rbDatosActivos.Checked = true;
        }

        void LimpiarCampos()
        {
            txtIdCargos.Clear();
            txtCargos.Clear();
        }

        void HabilitarNuevosRegistros(bool valor)
        {
            btnGuardarRegistro.Enabled = valor;
            btnCancelarRegistro.Enabled = valor;
            txtCargos.Enabled = valor;
            btnNuevoRegistro.Enabled = !valor;
        }

        void GuardarCargo()
        {
            string Cargo = txtCargos.Text.Trim();

            if (string.IsNullOrEmpty(Cargo))
            {
                Alertas.Advertencia("Por favor, complete el campo de Cargo antes de guardar.");
                return;
            }

            string estado = "Activo";
            string columnas = "Cargo, Estado";
            string valores = $"'{Cargo}', '{(estado)}'";
            if (consulta.Guardar("Cargos", columnas, valores))
            {
                Alertas.Realizado($"El Cargo {Cargo} se registro con éxito");
                MostrarRegistros("Activo");
                rbDatosActivos.Checked = true;
                HabilitarNuevosRegistros(false);
                LimpiarCampos();
            }
            else
            {
                Alertas.Advertencia("No se pudo guardar el Cargo. Intente nuevamente.");
            }
        }
        private void MostrarRegistros(string estado)
        {
            try
            {
                string columnas = "IdCargo, Cargo, Estado";
                string condicion = $"Estado = '{estado}'";
                DataTable dt = consulta.Buscar("Cargos", columnas, condicion);

                if (dt != null)
                {
                    foreach (DataColumn col in dt.Columns)
                    {
                        Debug.WriteLine($"DT Column: Name = '{col.ColumnName}' Type ={col.DataType}");
                    }
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


        void EnviarDatosParaEditar(DataGridViewCellEventArgs e)
        {
            try
            {
                lblOperacion.Text = "Editando";
                lblOperacion.Visible = true;

                if (e.RowIndex >= 0)
                {
                    DataGridViewRow fila = dgvDatos.Rows[e.RowIndex];
                    txtIdCargos.Text = fila.Cells["IdCargo"].Value.ToString();
                    txtCargos.Text = fila.Cells["Cargo"].Value.ToString();
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


        void ActualizarCargo()
        {

            string msg = "¿Desea actualizar este Cargo?";
            if (Alertas.Confirmacion(msg))
            {
                try
                {
                    int idCargos = int.Parse(txtIdCargos.Text);
                    string Cargos = txtCargos.Text.Trim();
                    string estado = lblEstado.Text.Trim();

                    string actualizar = $" Cargo = '{Cargos}', " +
                              $" Estado = '{estado}'";


                    string condicion = $"IdCargo = '{idCargos}'";

                    if (consulta.update("dbo.Cargos", actualizar, condicion) > 0)
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
                        Alertas.Advertencia("Error al actualizar el Cargo");
                    }

                }
                catch (Exception ex)
                {
                    Alertas.Advertencia($"Error al actualizar, Pruebe otra manera:{ex.Message}");

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
                GuardarCargo();
            }
            else if (lblOperacion.Text == "Editando")
            {
                ActualizarCargo();
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

        private void btnDesactivarRegistro_Click(object sender, EventArgs e)
        {
            ActivarDesactivarRegistro("Activo", "Inactivo");
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

        private void btnTestConexion_Click(object sender, EventArgs e)
        {
            conexion.validarConexion();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void rbDatosInactivos_CheckedChanged(object sender, EventArgs e)
        {
            if (rbDatosInactivos.Checked)
            {
                MostrarRegistros("Inactivo");
            }
        }

        private void rbDatosActivos_CheckedChanged(object sender, EventArgs e)
        {
            if (rbDatosActivos.Checked)
            {
                MostrarRegistros("Activo");
            }
        }

        private void txtFiltrar_TextChanged(object sender, EventArgs e)
        {
            string texto = txtFiltrar.Text;
            DataTable dt = dgvDatos.DataSource as DataTable;
            if (dt != null)
            {
                dt.DefaultView.RowFilter = $"Convert(Cargo, 'System.String') LIKE '%{texto}%'";
            }
            colorColumnaEstado();

        }

        private void dgvDatos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            EnviarDatosParaEditar(e);
        }
    }
}
