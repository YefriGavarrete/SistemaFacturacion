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
    public partial class FormRoles : Form
    {
        ConsultasSQL consulta = new ConsultasSQL();
        Conexion conexion = new Conexion();
        AlertasDelSistema Alertas = new AlertasDelSistema();
        public FormRoles()
        {
            InitializeComponent();
            MostrarRegistros("Activo");
            rbDatosActivos.Checked = true;
        }
        void LimpiarCampos()
        {
            txtIdRol.Clear();
            txtRoles.Clear();
        }

        void HabilitarNuevosRegistros(bool valor)
        {
            
            btnGuardarRegistro.Enabled = valor;
            btnCancelarRegistro.Enabled = valor;
            txtRoles.Enabled = valor;    
            btnNuevoRegistro.Enabled = !valor;
        }

        void GuardarRol()
        {
            string Rol = txtRoles.Text.Trim();

            if (string.IsNullOrEmpty(Rol))
            {
                Alertas.Advertencia("Por favor, complete el campo de Rol antes de guardar.");
                return;
            }

            string estado = "Activo";
            string columnas = "Rol, Estado";
            string valores = $"'{Rol}', '{(estado)}'";
            if (consulta.Guardar("Roles", columnas, valores))
            {
                Alertas.Realizado($"El Rol {Rol} se registro con éxito");
                MostrarRegistros("Activo");
                rbDatosActivos.Checked = true;
                HabilitarNuevosRegistros(false);
                LimpiarCampos();
            }
            else
            {
                Alertas.Advertencia("No se pudo guardar el Rol. Intente nuevamente.");
            }
        }
        private void MostrarRegistros(string estado)
        {
            try
            {
                string columnas = "IdRol, Rol, Estado";
                string condicion = $"Estado = '{estado}'";
                DataTable dt = consulta.Buscar("Roles", columnas, condicion);
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


        void ActualizarRol()
        {
            string msg = "¿Desea actualizar este Rol?";
            if (Alertas.Confirmacion(msg))
            {
                try
                {
                    string Roles = txtRoles.Text.Trim();
                    string estado = lblEstado.Text.Trim();
                    string actualizar = $"Rol = '{Roles}', " +
                         $"Estado = '{estado}'";

                    string condicion = $"Rol='{Roles}'";

                    if (consulta.update("Roles", actualizar, condicion) > 0)
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
                        Alertas.Advertencia("Error al actualizar el Rol");
                    }
                }
                catch (Exception ex)
                {
                    Alertas.Advertencia($"Error al actualizar el Rol:{ex.Message}");

                }
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
                    txtIdRol.Text = fila.Cells["IdRol"].Value.ToString();
                    txtRoles.Text = fila.Cells["Rol"].Value.ToString();
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
                GuardarRol();
            }
            else if (lblOperacion.Text == "Editando")
            {
                ActualizarRol();

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

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtFiltrar_TextChanged(object sender, EventArgs e)
        {
            string texto = txtFiltrar.Text;
            DataTable dt = dgvDatos.DataSource as DataTable;
            if (dt != null)
            {
                dt.DefaultView.RowFilter =
                    $"Convert(Rol, 'System.String') LIKE '%{texto}%'";
            }
            colorColumnaEstado();
        }

        private void btnReactivarRegistro_Click(object sender, EventArgs e)
        {
            ActivarDesactivarRegistro("Inactivo", "Activo");
           
        }

        private void dgvDatos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
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
    }
}
