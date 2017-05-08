using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace cotiza
{
    class Configura
    {
        // pagina de un documento
        public static String documento;
        public static int [] fecha;
        public static int maximo;
        public static int pagina;
        public static int inicio;
        public static int final;
        public static int [] columnas;

        // base de datos
        public static String datos_servidor;
        public static String datos_almacen;
        public static String datos_usuario;
        public static String datos_clave;


        public static bool recuperar()
        {
            try
            {
                // nombre del documento
                documento = ConfigurationManager.AppSettings["documento"];
                // en el archivo donde se encuentra la fecha: linea, columna, ancho
                fecha  = lista_leer(ConfigurationManager.AppSettings["fecha"]);
                // largo maximo del nombre de la empresa
                maximo = Int32.Parse(ConfigurationManager.AppSettings["empresa_maximo"]);
                // número de líneas por página
                pagina = Int32.Parse(ConfigurationManager.AppSettings["pagina"]);

                // número de línea de inicio y final de contenido de una página
                inicio = Int32.Parse(ConfigurationManager.AppSettings["linea_inicio"]);
                final  = Int32.Parse(ConfigurationManager.AppSettings["linea_final"]);

                // columnas de una página
                columnas = lista_leer(ConfigurationManager.AppSettings["columnas"]);

                // configurar servidor de base de datos mysql
                datos_servidor = ConfigurationManager.AppSettings["servidor"];
                datos_almacen  = ConfigurationManager.AppSettings["almacen"];
                datos_usuario  = ConfigurationManager.AppSettings["usuario"];
                datos_clave    = ConfigurationManager.AppSettings["clave"];

                return true;
            }
            catch
            {
                return false;
            }
        }

        private static int[][] arreglo_leer(String cadena)
        {
            int[][] arreglo;
            String[] filas = cadena.Split(';');
            arreglo = new int[filas.Length][];
            for (int i = 0; i < filas.Length; i++)
            {
                String[] columnas = filas[i].Split(',');
                arreglo[i] = new int[columnas.Length];
                for (int j = 0; j < columnas.Length; j++)
                {
                    arreglo[i][j] = int.Parse(columnas[j]);
                }
            }
            return arreglo;
        }

        private static int[] lista_leer(String cadena)
        {
            int[] lista;
            String[] columnas = cadena.Split(',');
            lista = new int[columnas.Length];
            for (int j = 0; j < columnas.Length; j++)
            {
                lista[j] = int.Parse(columnas[j]);
            }
            return lista;
        }
    }
}
