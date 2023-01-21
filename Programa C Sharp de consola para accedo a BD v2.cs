using System;
using Microsoft.Data.SqlClient;

namespace DBConnectV2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Interactuando con la BD desde la consola");
//            string connString = "Data Source=v-w7-des;Initial Catalog=Exam2Verif;Integrated Security=True";
            string connString = "Data Source = V-W7-DES; Initial Catalog = Exam2Verif; User ID = TestDBExam; Password = examen";
            SqlConnection conn = new SqlConnection(connString);
            try
            {
                Console.WriteLine("Abrimos la conexión ...");
                conn.Open();
                Console.WriteLine("Connexión exitosa");

            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                Environment.Exit(0);
            }


// Primer ejercicio: un comando SQL directo
            Console.WriteLine("Ejecutamos un comando SQL directamente");
            SqlCommand comando = new SqlCommand("select * from Autores", conn);
            SqlDataReader ejecutor = comando.ExecuteReader();
            while (ejecutor.Read())
            {
                Console.WriteLine(ejecutor["IdAutor"] + " | " + ejecutor["Nombre"] + " | " + ejecutor["Nacionalidad"]);
            }
            Console.WriteLine("**_______________________**");
            Console.ReadKey();
            ejecutor.Close();


// Segundo ejercicio: un comando SQL directo
            comando.Dispose();  // Borrar comando
            Console.WriteLine("Ejecutamos un segundo comando SQL directamente");
            comando.CommandText = "insert into Autores values ('A18', 'Pedro Shimose', 'Boliviana')";
            comando.ExecuteNonQuery();
            Console.WriteLine("**_______________________**");
            Console.ReadKey();
            ejecutor.Close();


//Tercer ejercicio: procedimiento almacenado sin parametros
            comando.Dispose();
            Console.WriteLine("Ejecutamos un procedimiento almacenado sin parametros");
            comando.CommandType = System.Data.CommandType.StoredProcedure;
            comando.CommandText = "ListaAlumnos";
            ejecutor = comando.ExecuteReader();
            while (ejecutor.Read())
            {
                Console.WriteLine(ejecutor["NroRegistro"] + " | " + ejecutor["Nombre"] + " | " + ejecutor["Carrera"] + " | " + ejecutor["Sigla"]);
            }
            Console.WriteLine("**_______________________**");
            Console.ReadKey();

//Cuarto Ejercicio
            comando.Dispose();
            ejecutor.Close();
            Console.WriteLine("Ejecutamos un procedimiento almacenado con parametros de entrada y salida");
            comando.CommandText = "NombreAlumno";
            comando.CommandType = System.Data.CommandType.StoredProcedure;
            comando.Parameters.AddWithValue("@Registro", "92113281");
            SqlParameter Nombre = new SqlParameter("@Nombre", System.Data.SqlDbType.VarChar, 40);
            Nombre.Direction = System.Data.ParameterDirection.Output;
            comando.Parameters.Add(Nombre);
            comando.ExecuteNonQuery();
            Console.WriteLine("El numero de registro corresponde a:" + Nombre.Value);
            Console.WriteLine("**_______________________**");
            Console.ReadKey();

        }
    }
}

