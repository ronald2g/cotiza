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
        private int grado;
        private int anterior;

        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Datos.empresa_crear(textBox1.Text, textBox2.Text);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void empresas_similares()
        {
            MySqlDataReader puntero = Datos.empresa_lista(Form1.grupo_auxiliar);
            int medida = textBox1.Text.Length;
            int distancia;
            while (puntero.Read())
            {
                String empresa = puntero.GetString("aux_nombre");
                if (empresa.Length > Configura.maximo)
                    distancia = Datos.distancia_levenshtein(textBox1.Text, empresa.Substring(0, Configura.maximo));
                else
                    distancia = Datos.distancia_levenshtein(textBox1.Text, empresa);

                if (anterior <= distancia && distancia < grado)
                {
                    ListViewItem item = new ListViewItem(distancia.ToString().PadLeft(2, '0'));
                    item.SubItems.Add(puntero.GetString("aux_etiqueta"));
                    item.SubItems.Add(empresa);
                    listView1.Items.Add(item);
                }
            }
            puntero.Close();
            puntero.Dispose();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            this.grado    = 3 ;
            this.anterior = 0 ;
            empresas_similares();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                ListViewItem a = listView1.SelectedItems[0];
                textBox2.Text = a.SubItems[2].Text;
            }
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                ListViewItem a = listView1.SelectedItems[0];
                Datos.empresa_reemplazar(a.SubItems[1].Text, textBox1.Text, textBox2.Text);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            anterior = grado ;
            grado   += 3     ;
            empresas_similares();
        }
    }
}
