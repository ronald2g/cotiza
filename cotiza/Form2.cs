using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace cotiza
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void Form2_Load(object sender, EventArgs e)
        {
            MySqlDataReader puntero = Datos.empresa_lista();
            int medida = textBox1.Text.Length;
            int distancia;
            while (puntero.Read())
            {
                String empresa = puntero.GetString("aux_nombre");
                if(empresa.Length>Configura.maximo)
                    distancia = Datos.distancia_levenshtein(textBox1.Text,empresa.Substring(0,Configura.maximo));
                else
                    distancia = Datos.distancia_levenshtein(textBox1.Text, empresa);

                if(distancia<10)
                {
                    ListViewItem item = new ListViewItem(distancia.ToString().PadLeft(2,'0'));
                    item.SubItems.Add(puntero.GetString("aux_etiqueta"));
                    item.SubItems.Add(empresa);
                    listView1.Items.Add(item);
                }
            }
            puntero.Close();
            puntero.Dispose();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
