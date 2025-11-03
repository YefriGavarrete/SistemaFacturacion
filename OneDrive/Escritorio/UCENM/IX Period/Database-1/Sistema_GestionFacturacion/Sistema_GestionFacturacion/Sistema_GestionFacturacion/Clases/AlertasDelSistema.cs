using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sistema_GestionFacturacion.Clases
{
    internal class AlertasDelSistema
    {
        MessageBoxButtons botones;
        MessageBoxIcon icono;

        public void Advertencia(string msg_Advertencia)
        {
            botones = MessageBoxButtons.OK;
            icono = MessageBoxIcon.Warning;
            MessageBox.Show(msg_Advertencia, "Advertencia", botones, icono);
        }

        public void Realizado(string msg_Realizado)
        {
            botones = MessageBoxButtons.OK;
            icono = MessageBoxIcon.Information;
            MessageBox.Show(msg_Realizado, "Operacion Exitosa", botones, icono);

        }

        public bool Confirmacion(string msg_Confirmacion)
        {
            bool resp = false;
            botones = MessageBoxButtons.YesNo;
            icono = MessageBoxIcon.Question;
            if (MessageBox.Show(msg_Confirmacion, "Confirmar", botones, icono) == DialogResult.Yes)
            {
                resp = true;
            }
            return resp;
        }
    }
}
