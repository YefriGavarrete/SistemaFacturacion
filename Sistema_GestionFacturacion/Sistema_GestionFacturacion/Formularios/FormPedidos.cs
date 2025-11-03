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
    public partial class FormPedidos : Form
    {
       
        string EmpleadoNombre;
        string EmpleadoApellido;
        public FormPedidos(string nombre, string apellido)
        {
            EmpleadoNombre = nombre ?? string.Empty;
            EmpleadoApellido = apellido ?? string.Empty;
            InitializeComponent();
        }


        public void SetEmpleado(string nombre, string apellido)
        {
            EmpleadoNombre = nombre ?? string.Empty;
            EmpleadoApellido = apellido ?? string.Empty;
            MostrarEmpleado();
        }

        private void MostrarEmpleado()
        {
            lblEmpleado.Text = $"Empleado: {EmpleadoNombre} {EmpleadoApellido}";
        }
    }
}
