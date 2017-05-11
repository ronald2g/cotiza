using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace cotiza
{
    public partial class Form1 : Form
    {
        DateTime fecha;
        static public int grupo_auxiliar=0;

        public Form1()
        {
            InitializeComponent();
        }


        private String[] cadena_partir(int[] corte, String texto)
        {
            int lugar = 0;
            String[] partes = new String[corte.Length];
            for (int i = 0; i < corte.Length; i++)
            {
                if (texto.Length >= lugar + corte[i])
                {
                    partes[i] = texto.Substring(lugar, corte[i]);
                }
                else
                {
                    partes[i] = String.Empty;
                }
                lugar += corte[i];
            }
            return partes;
        }

        private void cadena_captura(String cadena,int pagina,int linea)
        {
            String[] partes = cadena_partir(Configura.columnas, cadena);
            int cotizacion = Datos.codigo_numero(partes[1].Trim());
            if(cotizacion>0)
            {
                String empresa = partes[2].Trim();
                if (!String.IsNullOrEmpty(empresa))
                {
                    DataTable empresa_id = Datos.empresa_recuperar(empresa);
                    if(empresa_id.Rows.Count>1)
                    {
                        // todo OK se encontro la empresa, guardar datos
                    }
                    else if (empresa_id.Rows.Count==0)
                    {
                        // buscar en los nombre completos si alguno corresponde
                        empresa_id.Dispose();
                        empresa_id = Datos.empresa_buscar(empresa);
                        if(empresa_id.Rows.Count==1)
                        {
                            // se encontro un sola empresa
                            DataRow fila = empresa_id.Rows[0];
                            Datos.empresa_modificar(fila["aux_id"].ToString(), empresa);
                        }
                        
                    }

                    if(empresa_id.Rows.Count==0)
                    {
                        Form2 formulario = new Form2();
                        formulario.textBox1.Text = empresa;
                        formulario.textBox2.Text = empresa;
                        if (formulario.ShowDialog() == DialogResult.OK)
                        {
                            //ListViewItem a = formulario.listView1.SelectedItems[0];
                        }
                    }
                }
            }
            else
            {
                int indice = Array.IndexOf(Datos.empresa_tipo,cadena.Trim()); 
                if(indice>=0)
                {
                    grupo_auxiliar = Datos.empresa_cuenta[indice];
                }
            }
        }

        private void datos_recuperar(String nombre)
        {
            System.IO.StreamReader archivo;
            archivo = new System.IO.StreamReader(nombre);
            int pagina = 1;
            int linea = 1;
            String cadena = archivo.ReadLine();
            while (cadena != null)
            {
                if(linea>=Configura.inicio && linea<=Configura.final)
                    cadena_captura(cadena,pagina,linea);
                linea++;
                if(linea>Configura.pagina)
                {
                    pagina++;
                    linea = 1;
                }
                cadena = archivo.ReadLine();
            }
            archivo.Close();
        }

        private String fecha_recuperar(String nombre,int linea,int columna,int ancho)
        {
            System.IO.StreamReader archivo;
            archivo = new System.IO.StreamReader(nombre);
            int contador = 1;
            String cadena = archivo.ReadLine();
            while (cadena != null)
            {
                if (contador == linea) break;
                cadena = archivo.ReadLine();
                contador++;
            }
            archivo.Close();
            if (cadena != null)
            {
                if(cadena.Length >=columna+ancho)
                {
                    return cadena.Substring(columna, ancho);
                }
            }
            return null;
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            // recuperar configuración
            if(Configura.recuperar()==false)
            {
                MessageBox.Show("Error al leer el archivo de configuración");
                Close();
            }

            // abrir la base de datos
            Exception ex=Datos.abrir();
            if(ex!=null)
            {
                MessageBox.Show(ex.Message);
                Close();
            }

            // si existe un parametro recuperar archivo
            String [] argumento = Environment.GetCommandLineArgs();
            if (argumento.Length > 1)
            {
                String nombre = Path.GetFileName(argumento[1]);
                if(nombre.Equals(Configura.documento))
                {
                    String fecha_cadena = fecha_recuperar(argumento[1], Configura.fecha[0], Configura.fecha[1], Configura.fecha[2]);
                
                    if (String.IsNullOrEmpty(fecha_cadena))
                    {
                        MessageBox.Show("Error al leer la fecha del archivo");
                        Close();
                    }
                    else
                    {
                        fecha = DateTime.ParseExact(fecha_cadena, "dd/MM/yyyy", null);
                    }
                    datos_recuperar(argumento[1]);
                }
                else 
                {
                    MessageBox.Show("El nombre del archivo no coincide con: cotrep003");
                    Close();
                }
            }
        }
    }
}
