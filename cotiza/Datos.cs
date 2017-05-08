using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;

namespace cotiza
{
    public class Datos
    {
        public static Enlace enlace1;
        //public static Enlace enlace2;

        public static Exception abrir()
        {
            return (enlace1 = new Enlace(
                    Configura.datos_servidor,
                    Configura.datos_almacen,
                    Configura.datos_usuario,
                    Configura.datos_clave)).abrir();
        }

        private static String fecha_mysql(String fecha)
        {
            String[] a = fecha.Split('/');
            if (a.Length == 3) return a[2] + "-" + a[1] + "-" + a[0];
            return String.Empty;
        }

        public static int cotizacion_guardar(String fecha, String monto, String etiqueta, String concepto, String detalle)
        {
            String fecha2 = fecha_mysql(fecha);
            String monto2 = monto.Replace(",", "");
            etiqueta = etiqueta.Trim();
            concepto = concepto.Trim();
            detalle = detalle.Trim();
            if (!String.IsNullOrEmpty(etiqueta))
            {
                String codigo = enlace1.valor(String.Format("select reg_codigo from registro where reg_etiqueta='{0}' and reg_fecha='{1}' and reg_tipo=1 and reg_estado=1", etiqueta, fecha2));
                if (String.IsNullOrEmpty(codigo))
                    return enlace1.ejecutar(String.Format("insert into registro (reg_fecha,reg_tipo,reg_monto,reg_etiqueta,reg_concepto,reg_detalle,reg_creado,reg_estado) values('{0}',1,{1},'{2}','{3}','{4}',curdate(),1)", fecha2, monto2, etiqueta, concepto, detalle));
            }
            return 0;
        }

        public static int empresa_crear(String corto,String empresa)
        {
            return enlace1.ejecutar(String.Format("insert into auxiliar (aux_etiqueta,aux_corto,aux_nombre,aux_estado) values (espacio_buscar(4011),'{0}','{1}',1)", corto, empresa));
        }

        public static DataTable empresa_recuperar(String nombre)
        {
            return enlace1.lista(String.Format("select aux_id from auxiliar where aux_corto='{0}'",nombre));
        }

        public static int empresa_reemplazar(String codigo,String corto,String empresa)
        {
            return enlace1.ejecutar(String.Format("update auxiliar set aux_corto='{0}',aux_nombre='{1}' where aux_etiqueta={2} and aux_estado=1", corto,empresa,codigo));
        }

        public static MySqlDataReader empresa_lista()
        {
            return enlace1.consulta(String.Format("select * from auxiliar where aux_estado=1"));
        }

        public static MySqlDataReader empresa_buscar(String nombre)
        {
            return enlace1.consulta(String.Format("select aux_id from auxiliar where left(aux_nombre,{0})='{1}'",Configura.maximo, nombre));
        }

        public static String codigo_cadena(String ci)
        {
            int numero = 0;
            Int32.TryParse(ci, out numero);
            if (numero > 0) return numero.ToString();
            else return String.Empty;
        }

        public static int codigo_numero(String ci)
        {
            int numero = 0;
            Int32.TryParse(ci, out numero);
            return numero;
        }

        // distancia entre dos palabras
        public static int distancia_levenshtein(String s, String t)
        {
            int costo = 0;
            int m = s.Length;
            int n = t.Length;
            int[,] d = new int[m + 1, n + 1];
            if (n == 0) return m;
            if (m == 0) return n;
            for (int i = 0; i <= m; d[i, 0] = i++) ;
            for (int j = 0; j <= n; d[0, j] = j++) ;
            for (int i = 1; i <= m; i++)
            {
                for (int j = 1; j <= n; j++)
                {
                    costo = (s[i - 1] == t[j - 1]) ? 0 : 1;
                    d[i, j] = System.Math.Min(System.Math.Min(d[i - 1, j] + 1,  //Eliminacion
                                  d[i, j - 1] + 1),                             //Inserccion 
                                  d[i - 1, j - 1] + costo);                     //Sustitucion
                }
            }
            return d[m, n];
        }
    }

    public class Enlace
    {
        public MySqlConnection enlace;
        String servidor;
        String almacen;
        String usuario;
        String clave;

        public Enlace(String servidor, String almacen, String usuario, String clave)
        {
            this.servidor = servidor;
            this.almacen  = almacen;
            this.usuario  = usuario;
            this.clave    = clave;
        }

        public Exception abrir()
        {
            try
            {
                enlace = new MySqlConnection(String.Format("server={0};database={1};uid={2};password={3}", this.servidor, this.almacen, this.usuario, this.clave));
                enlace.Open();
                return null;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        public int ejecutar(String consulta)
        {
            MySqlCommand comando = new MySqlCommand(consulta, enlace);
            return comando.ExecuteNonQuery();
        }

        public String indice(String consulta)
        {
            MySqlCommand comando = new MySqlCommand(consulta, enlace);
            comando.ExecuteNonQuery();
            comando = new MySqlCommand("select last_insert_id()", enlace);
            return comando.ExecuteScalar().ToString();
        }

        public String ultimo(String consulta)
        {
            MySqlCommand comando = new MySqlCommand(consulta, enlace);
            comando.ExecuteNonQuery();
            return valor("select last_insert_id()");
        }


        public String valor(String consulta, String defecto = "")
        {
            MySqlCommand comando = new MySqlCommand(consulta, enlace);
            object respuesta = comando.ExecuteScalar();
            return respuesta == null ? defecto : respuesta.ToString();
        }

        public DataRow primero(String consulta)
        {
            DataTable tabla = new DataTable();
            MySqlDataAdapter a = new MySqlDataAdapter(consulta, enlace);
            a.Fill(tabla);
            if (tabla.Rows.Count > 0) return tabla.Rows[0]; else return null;
        }

        public DataTable lista(String consulta)
        {
            DataTable tabla2 = new DataTable();
            MySqlDataAdapter a = new MySqlDataAdapter(consulta, enlace);
            a.Fill(tabla2);
            return tabla2;
        }

        public MySqlDataReader consulta(String consulta)
        {
            MySqlCommand comando;
            comando = new MySqlCommand(consulta, enlace);
            return comando.ExecuteReader();
        }
    }
}
