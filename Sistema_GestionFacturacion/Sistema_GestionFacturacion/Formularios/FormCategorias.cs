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
    public partial class FormCategorias : Form
    {

        ConsultasSQL consulta = new ConsultasSQL();
        Conexion conexion = new Conexion();
        AlertasDelSistema Alertas = new AlertasDelSistema();
        public FormCategorias()
        {
            InitializeComponent();
            MostrarRegistros("Activo");
            rbDatosActivos.Checked = true;
        }

        void LimpiarCampos()
        {
            txtIdCategorias.Clear();
            txtCategorias.Clear();
        }

        void HabilitarNuevosRegistros(bool valor)
        {
            btnGuardarRegistro.Enabled = valor;
            btnCancelarRegistro.Enabled = valor;
            txtCategorias.Enabled = valor;
            btnNuevoRegistro.Enabled = !valor;
        }

        void GuardarCategoria()
        {
            string categoria = txtCategorias.Text.Trim();

            if (string.IsNullOrEmpty(categoria))
            {
                Alertas.Advertencia("Por favor, complete todos los campos antes de registrar.");
                return;
            }
            string estado = "Activo";
            string columnas = "Categoria, Estado";
            string valores = $"'{categoria}', '{(estado)}'";
            if (consulta.Guardar("Categorias", columnas, valores))
            {
                Alertas.Realizado($"La categoria {categoria} se registro con éxito");
                MostrarRegistros("Activo");
                rbDatosActivos.Checked = true;
                HabilitarNuevosRegistros(false);
                LimpiarCampos();
            }
            else
            {
                Alertas.Advertencia("No se pudo guardar la Categoria. Intente nuevamente.");
            }
        }
        private void MostrarRegistros(string estado)
        {
            try
            {
                string columnas = "IdCategoria,Categoria, Estado";
                string condicion = $"Estado = '{estado}'";
                DataTable dt = consulta.Buscar("Categorias", columnas, condicion);
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
                    txtIdCategorias.Text = fila.Cells["IdCategoria"].Value.ToString();
                    txtCategorias.Text = fila.Cells["Categoria"].Value.ToString();
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

        void ActualizarCategoria()
        {

            string msg = "¿Desea actualizar esta Categoria?";
            if (Alertas.Confirmacion(msg))
            {
                try
                {

                    int idCategorias = int.Parse(txtIdCategorias.Text);
                    string categorias = txtCategorias.Text.Trim();
                    string estado = lblEstado.Text.Trim();

                    string actualizar = $" Categoria = '{categorias}', " +
                        $" Estado = '{estado}'";

                    string condicion = $"IdCategoria= '{idCategorias}'";

                    if (consulta.update("Categorias", actualizar, condicion) > 0)
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
                        Alertas.Advertencia("Error al actualizar la Categoria, Intente de nuevo");
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
                GuardarCategoria();
            }
            else if (lblOperacion.Text == "Editando")
            {
                ActualizarCategoria();
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

        private void btnReactivarRegistro_Click_1(object sender, EventArgs e)
        {
            ActivarDesactivarRegistro("Inactivo", "Activo");
        }

        private void btnDesactivarRegistro_Click_1(object sender, EventArgs e)
        {
            ActivarDesactivarRegistro("Activo", "Inactivo");
        }

        private void txtFiltrar_TextChanged(object sender, EventArgs e)
        {
            string texto = txtFiltrar.Text.Trim();

            DataTable dt = dgvDatos.DataSource as DataTable;
            if (dt != null)
            {
                dt.DefaultView.RowFilter =
                    $"Convert(Categoria, 'System.String') LIKE '%{texto}%'";
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

        private void FormCategorias_Load(object sender, EventArgs e)
        {
    
        }
    }
}
