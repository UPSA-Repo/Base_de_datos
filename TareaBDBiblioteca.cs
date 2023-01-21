using Microsoft.Data.SqlClient;
using System;

namespace EjercicioEnClase
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Conexion con base de datos");
            string connString = "Data Source=DESKTOP-S5G74PE;Initial Catalog=Exam2Verif;Integrated Security=True";
            SqlConnection sqlConnection = new SqlConnection(connString);
            SqlDataReader sqlDataReader = null;

            try
            {
                Console.WriteLine("Abriendo conexion...");
                sqlConnection.Open();
                Console.WriteLine("Conectado");
            }catch(Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                Environment.Exit(0);
            }

            Menu(sqlConnection, ref sqlDataReader);
            Environment.Exit(0);
        }

        static void Menu(SqlConnection sqlConnection, ref SqlDataReader sqlDataReader)
        {
            int op;
            do
            {
                Console.Clear();
                Console.WriteLine("==========MENU==========");
                Console.WriteLine("1. Obtener todos los libros");
                Console.WriteLine("2. Obtener todos los autores");
                Console.WriteLine("3. Obtener todos las autorias");
                Console.WriteLine("4. Obtener todos los prestamos");
                Console.WriteLine("5. Obtener todos los estudiantes");
                Console.WriteLine("6. Consultar libro y autor");
                Console.WriteLine("0. Salir");
                op = Convert.ToInt32(Console.ReadLine());

                switch (op)
                {
                    case 1:
                        ObtenerListaLibros(sqlConnection, ref sqlDataReader);
                        break;
                    case 2:
                        ObtenerListaAutores(sqlConnection, ref sqlDataReader);
                        break;
                    case 3:
                        ObtenerListaAutorias(sqlConnection, ref sqlDataReader);
                        break;
                    case 4:
                        ObtenerListaPrestamos(sqlConnection, ref sqlDataReader);
                        break;
                    case 5:
                        ObtenerListaEstudiantes(sqlConnection, ref sqlDataReader);
                        break;
                    case 6:
                        Ejercicio(sqlConnection, ref sqlDataReader);
                        break;
                    case 0:
                        Console.WriteLine("Saliending...");
                        break;
                    default:
                        Console.WriteLine("Opcion invalida");
                        break;
                }
                Console.ReadLine();
            } while (op != 0);

        }

        static void ObtenerListaLibros(SqlConnection sqlConnection, ref SqlDataReader sqlDataReader)
        {
            SqlCommand sqlCommand = new SqlCommand("select * from Libros", sqlConnection);
            sqlDataReader = sqlCommand.ExecuteReader();
            while (sqlDataReader.Read())
                Console.WriteLine($"IDLibro: {sqlDataReader["IdLibro"]}Titulo:{sqlDataReader["Titulo"]}{sqlDataReader["Subtitulo"]} Editorial:{sqlDataReader["Editorial"]} Area:{sqlDataReader["Area"]} AnoPublicacion:{sqlDataReader["AnoPublicacion"]} Paginas:{sqlDataReader["TotalPaginas"]} Edicion:{sqlDataReader["Edicion"]}");
            sqlDataReader.Close();
        }

        static void ObtenerListaAutores(SqlConnection sqlConnection, ref SqlDataReader sqlDataReader)
        {
            SqlCommand sqlCommand = new SqlCommand("select * from Autores", sqlConnection);
            sqlDataReader = sqlCommand.ExecuteReader();
            while (sqlDataReader.Read())
                Console.WriteLine($"Nombre: {sqlDataReader["Nombre"]} IDAutor: {sqlDataReader["IdAutor"]} Nacionalidad: {sqlDataReader["Nacionalidad"]}");
            sqlDataReader.Close();
        }

        static void ObtenerListaAutorias(SqlConnection sqlConnection, ref SqlDataReader sqlDataReader)
        {
            SqlCommand sqlCommand = new SqlCommand("select * from Autorias", sqlConnection);
            sqlDataReader = sqlCommand.ExecuteReader();
            while (sqlDataReader.Read())
                Console.WriteLine($"IdAutor: {sqlDataReader["IdAutor"]} IdLibro: {sqlDataReader["IdLibro"]}");
            sqlDataReader.Close();
        }

        static void ObtenerListaPrestamos(SqlConnection sqlConnection, ref SqlDataReader sqlDataReader)
        {
            SqlCommand sqlCommand = new SqlCommand("select * from Prestamos", sqlConnection);
            sqlDataReader = sqlCommand.ExecuteReader();
            while (sqlDataReader.Read())
                Console.WriteLine($"Registro: {sqlDataReader["NroRegistro"]} IdLibro: {sqlDataReader["IdLibro"]} Prestamo: {sqlDataReader["FechaPrestamo"]} Hasta: {sqlDataReader["FechaDebeDevolver"]} Devolvio: {sqlDataReader["FechaDevolucion"]}");
            sqlDataReader.Close();
        }

        static void ObtenerListaEstudiantes(SqlConnection sqlConnection, ref SqlDataReader sqlDataReader)
        {
            SqlCommand sqlCommand = new SqlCommand("select * from Estudiantes", sqlConnection);
            sqlDataReader = sqlCommand.ExecuteReader();
            while (sqlDataReader.Read())
                Console.WriteLine($"Nombre: {sqlDataReader["Nombre"]} Registro: {sqlDataReader["NroRegistro"]} Carrera: {sqlDataReader["Carrera"]} Edad: {sqlDataReader["Edad"]} CI: {sqlDataReader["CI"]} Direccion: {sqlDataReader["Direccion"]} Telefono: {sqlDataReader["Telefono"]}");
            sqlDataReader.Close();
        }

        static void Ejercicio(SqlConnection sqlConnection, ref SqlDataReader sqlDataReader)
        {
            // OBTENER DATOS
            String libro = "", autor = "";
            ObtenerDatos(ref libro, ref autor);

            // VERIFICAR EXISTENCIA DEL LIBRO
            bool existeIDLibro = ExisteLibro(libro, sqlConnection, ref sqlDataReader);  // ref -> & (C++)
            sqlDataReader.Close();
            if (existeIDLibro)
                Console.WriteLine("LIBRO ENCONTRADO");
            else
            {
                // DEBERIA REGISTRAR LIBRO
                Console.WriteLine("LIBRO NO ENCONTRADO");
                RegistrarLibro(libro, sqlConnection, ref sqlDataReader);
            }

            // VERIFICAR EXISTENCIA DE AUTORES
            String[] autores = autor.Split(' ');  // ALMACENAR TODOS LOS AUTORES

            foreach (String aut in autores)
            {
                if (!sqlDataReader.IsClosed)
                    sqlDataReader.Close();
                if (!ExisteAutor(aut, sqlConnection, ref sqlDataReader))
                {
                    sqlDataReader.Close();  // REGISTRAR EN CASO DE QUE NO EXISTA
                    RegistrarAutor(aut, sqlConnection, ref sqlDataReader);
                }
            }

            // VERIFICAR AUTORIAS
            foreach (String aut in autores)
            {
                if (!VerificarAutoria(aut, libro, sqlConnection, ref sqlDataReader))
                {
                    // REGISTRAR AUTORIA EN CASO DE QUE NO EXISTA
                    sqlDataReader.Close();
                    RegistrarAutoria(aut, libro, sqlConnection, ref sqlDataReader);
                    sqlDataReader.Close();
                }

            }

            sqlDataReader.Close();
            SqlCommand command = new SqlCommand($"select * from Autorias where IdLibro = '{libro}'", sqlConnection);
            sqlDataReader = command.ExecuteReader();

            while (sqlDataReader.Read())
                Console.WriteLine($"Autor: {sqlDataReader["IdAutor"]} Libro: {sqlDataReader["IdLibro"]}");
            sqlDataReader.Close();
        }

        static void ObtenerDatos(ref String libro, ref String autor)
        {
            Console.WriteLine("IDLibro: ");
            libro = Console.ReadLine().ToUpper();
            Console.WriteLine("Autor: ");
            autor = Console.ReadLine().ToUpper();
        }
        // FUNCIONES LIBRO
        static bool ExisteLibro(String idLibro, SqlConnection sqlConnection, ref SqlDataReader sqlDataReader)
        {
            SqlCommand command = new SqlCommand($"select * from Libros where IdLibro = '{idLibro}'", sqlConnection);
            sqlDataReader = command.ExecuteReader();
            return sqlDataReader.HasRows;
        }

        static void RegistrarLibro(String idLibro, SqlConnection sqlConnection, ref SqlDataReader sqlDataReader) 
        {
            Console.WriteLine("----------Registrando libro----------");
            Console.WriteLine("Titulo: ");
            String titulo = Console.ReadLine();

            Console.WriteLine("Subtitulo: ");
            String subTitulo = Console.ReadLine();

            Console.WriteLine("Editorial: ");
            String editorial = Console.ReadLine();

            Console.WriteLine("Area: ");
            String area = Console.ReadLine();

            Console.WriteLine("Año de publicacion: ");
            String publicacion = Console.ReadLine();

            Console.WriteLine("Paginas: ");
            String paginas = Console.ReadLine();

            Console.WriteLine("Edicion: ");
            String edicion = Console.ReadLine();

            SqlCommand command = new SqlCommand($"insert into Libros values('{idLibro}', '{titulo}', '{subTitulo}', '{editorial}', '{area}', '{publicacion}', '{paginas}', '{edicion}')", sqlConnection);
            sqlDataReader = command.ExecuteReader();
            sqlDataReader.Close();
        }
        // FUNCIONES AUTORES
        static bool ExisteAutor(String idAutor, SqlConnection sqlConnection, ref SqlDataReader sqlDataReader)
        {
            if (!sqlDataReader.IsClosed)
                sqlDataReader.Close();
            SqlCommand command = new SqlCommand($"select * from Autores where IdAutor = '{idAutor}'", sqlConnection);
            sqlDataReader = command.ExecuteReader();
            return sqlDataReader.HasRows;
        }

        static void RegistrarAutor(String idAutor, SqlConnection sqlConnection, ref SqlDataReader sqlDataReader)
        {
            if (!sqlDataReader.IsClosed)
                sqlDataReader.Close();
            Console.WriteLine("----------Registrando autor----------");
            Console.WriteLine($"ID Autor: {idAutor}");
            
            Console.WriteLine("Nombre: ");
            String nombre = Console.ReadLine();

            Console.WriteLine("Nacionalidad: ");
            String nacionalidad = Console.ReadLine();

            SqlCommand command = new SqlCommand($"insert into Autores values('{idAutor}', '{nombre}', '{nacionalidad}')", sqlConnection);
            sqlDataReader = command.ExecuteReader();
            sqlDataReader.Close();
        }

        // FUNCIONES AUTORIAS
        static bool VerificarAutoria(String idAutor, String idLibro, SqlConnection sqlConnection, ref SqlDataReader sqlDataReader)
        {
            if (!sqlDataReader.IsClosed)
                sqlDataReader.Close();
            SqlCommand command = new SqlCommand($"select * from Autorias where IdAutor = '{idAutor}' and IdLibro = '{idLibro}'", sqlConnection);
            sqlDataReader = command.ExecuteReader();
            return sqlDataReader.HasRows;
        }

        static void RegistrarAutoria(String idAutor, String idLibro, SqlConnection sqlConnection, ref SqlDataReader sqlDataReader)
        {
            if (!sqlDataReader.IsClosed)
                sqlDataReader.Close();
            SqlCommand command = new SqlCommand($"insert into Autorias values('{idAutor}', '{idLibro}')", sqlConnection);
            sqlDataReader = command.ExecuteReader();
            sqlDataReader.Close();
        }
    }
}