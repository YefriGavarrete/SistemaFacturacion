using Sistema_GestionFacturacion.Clases;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sistema_GestionFacturacion.Formularios
{
    public partial class FormMenu : Form
    {

        AlertasDelSistema Alertas = new AlertasDelSistema();
        ConsultasSQL Consultas = new ConsultasSQL();
        Conexion conexion = new Conexion();
        string rol;
        string nombre;
        string apellido;

        public FormMenu()
        {
            InitializeComponent();
            InhabilitarButtons(); 
        }

        public FormMenu(string rol, string nombre, string apellido) : this()
        {
            rol = rol ?? string.Empty;
            nombre = nombre ?? string.Empty;
            apellido = apellido ?? string.Empty;
            this.IsMdiContainer = true;
            bienvenidaUsuario(nombre, apellido, rol);
            configuracionRol(rol);
        }



        
        void Regresar() {
            this.Close();
            var login = new Login();
            login.Closed += (s, args) => this.Close();
            login.Show();
        }

        void bienvenidaUsuario(string nombre, string apellido, string rol)
        {
            try
            {
                var texto = $"Bienvenido {nombre} {apellido}, {rol}".Trim();
                var lbls = this.Controls.Find("lblUsuario", true);
                if (lbls.Length == 0) lbls = this.Controls.Find("lblUsuario", true);

                if (lbls.Length > 0 && lbls[0] is Label lb)
                {
                    lb.Text = texto;
                }
            }
            catch
            {
                // se puede agregar un error en caso de falla
            }
        }
        void configuracionRol(string rol)
        {
            if (string.IsNullOrWhiteSpace(rol))
            {
                InhabilitarButtons();
                return;
            }
            var clave = rol.Trim().ToLowerInvariant();

            if (clave == "administrador" || clave == "admin")
            {
                ConfigurarButtonesAdministrador();
            }
            else
            {
                ConfigurarButtonesEmpleados();
            }
        }
        

        bool InhabilitarButtons()
        {

            //debo de agregar todos los botones para inhabilitarlos para el usuario, no  para el administrador
            btnUsuarios.Enabled = false;
            btnRoles.Enabled = false;
            btnPedidos.Enabled = false;
            btnCargos.Enabled = false;
            btnCategorias.Enabled = false;
            btnCargos.Enabled = false;
            return true;
        }

        void ConfigurarButtonesAdministrador()
        {
            var adminButtons = new[]
            {
                "btnPedidos", "btnDetallesPedidos", "btnFacturas",
                "btnCargos", "btnEmpleados", "btnProductos",
                "btnCategorias", "btnDescuentos", "btnUsuarios", "btnRoles"
            };
            establecerInhabilitarButtones(adminButtons, true);

        }

        void ConfigurarButtonesEmpleados()
        {
            var empleadosButtons = new[]
            {
                 "btnPedidos", "btnDetallesPedidos", "btnFacturas"
            };
            InhabilitarButtons();
            establecerInhabilitarButtones(empleadosButtons, true);
        }


        void establecerInhabilitarButtones(IEnumerable<string> nombres, bool habilitar)
        {
            foreach (var nombre in nombres)
            {
                try
                {
                    var button = this.Controls.Find(nombre, true);
                    foreach (var btn in button)
                    {
                        if (btn is Button b)
                        {
                            b.Enabled = habilitar;
                            b.Visible = habilitar;
                        }
                    }
                }
                catch
                {
                }
            }
        }
        public void OpenChildForm(Form childForm)
        {
            if (childForm == null) return;
            var existing = this.MdiChildren.FirstOrDefault(c => c.GetType() == childForm.GetType());
            if (existing != null)
            {
                existing.BringToFront();
                existing.WindowState = FormWindowState.Normal;
                return;
            }
        }

        private void btnCargos_Click(object sender, EventArgs e)
        {
            var cargos = new FormCargos();
            OpenChildForm(cargos);
        }

        private void btnRegresar_Click(object sender, EventArgs e)
        {
            Regresar();
        }
    }
}
