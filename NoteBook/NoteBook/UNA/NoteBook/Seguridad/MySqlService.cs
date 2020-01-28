﻿using DataBaseAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UNA.Notebook;
using UNA.NoteBook;

namespace NoteBook.UNA.NoteBook.Seguridad
{
    public class MySqlService
    {
        private static MySqlService _instance = null;
        private static MySqlAccess mySqlAccess; 
        private MySqlService()
        {

        }
        public static MySqlService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MySqlService();
                    mySqlAccess = new MySqlAccess();
                    mySqlAccess.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DateBaseNoteBook"].ConnectionString;
                }
                return _instance;
            }
        }

        public List<User> CargarUsuarios()
        {
            try
            {
                mySqlAccess.OpenConnection();
                DataTable result = new DataTable();
                result = mySqlAccess.QuerySQL("SELECT * FROM usuarios");
                List<User> usuarios = new List<User>();
                for (int x = 0; x < result.Rows.Count; x++)
                {
                    User user = new User();
                    user.NameUser = result.Rows[x]["Nombre_Usuario"].ToString();
                    user.PasswordUser = result.Rows[x]["Contraseña"].ToString();
                    user.Name = result.Rows[x]["Nombre"].ToString();
                    user.LastName = result.Rows[x]["Apellido"].ToString();
                    usuarios.Add(user);
                }
                mySqlAccess.CloseConnection();
                return usuarios;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public void CrearUsuario(User user)
        {
            mySqlAccess.OpenConnection();
            mySqlAccess.EjectSQL("Insert into usuarios values ('"+user.NameUser+"','"+user.PasswordUser+"','"+user.Name+"','"+user.LastName+"');");
            mySqlAccess.CommitTransaction();
            mySqlAccess.CloseConnection();
        }

        public void CrearLibro(Book book)
        {
            mySqlAccess.OpenConnection();
            mySqlAccess.EjectSQL("Insert Into libros (`Nombre`, `Privacidad`, `Imagen`, `Usuarios_Nombre_Usuario`) VALUES ('" + book.NameBook + "',"+ book.AccessBook +", '"+DireccionImagen(book) +"' ,'" + book.User.NameUser + "');");
            mySqlAccess.CommitTransaction();
            mySqlAccess.CloseConnection();
        }

        public void ModificarContraseña(User user)
        {
            mySqlAccess.OpenConnection();
            mySqlAccess.EjectSQL("Update usuarios set Contraseña = '"+user.PasswordUser+"' where Nombre_usuario='"+user.NameUser+"'");
            mySqlAccess.CommitTransaction();
            mySqlAccess.CloseConnection();
        }

        public List<Book> CargarLibros(User user)
        {
            try
            {
                mySqlAccess.OpenConnection();
                List<Book> books = new List<Book>();
                DataTable result = mySqlAccess.QuerySQL("SELECT * FROM libros where Usuarios_Nombre_Usuario = '" + user.NameUser + "' or Privacidad = 1;");
                for (int x = 0; x < result.Rows.Count; x++)
                {
                    int id_book = Convert.ToInt32(result.Rows[x]["ID_Libro"]);
                    Book book = new Book();
                    book.NameBook = result.Rows[x]["Nombre"].ToString();
                    book.ImageBook = result.Rows[x]["Imagen"].ToString();
                    book.AccessBook = Convert.ToBoolean(result.Rows[x]["Privacidad"]);
                    DataTable categories = mySqlAccess.QuerySQL("Select Categorias_Nombre from libros_categorias Where Libros_ID_Libro = " + id_book + "");
                    for (int y = 0; y < categories.Rows.Count; y++)
                    {
                        book.CategorieBook.Add(Convert.ToString(categories.Rows[y]["Categorias_Nombre"]));
                    }
                    string usuario = Convert.ToString(result.Rows[x]["Usuarios_Nombre_Usuario"]);
                    if (usuario != user.NameUser)
                    {
                        User usuarioDistinto = new User();
                        usuarioDistinto.NameUser = usuario;
                        book.User = usuarioDistinto;
                    }
                    else
                    {
                        book.User = user;
                    }
                    books.Add(book);
                }
                mySqlAccess.CloseConnection();
                return books;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public void CrearCategorias(string categoria, string imagen)
        {
            try
            {
                mySqlAccess.OpenConnection();
                mySqlAccess.EjectSQL("Insert Into categorias(`Nombre`, `Imagen`) values('"+ categoria+"', '"+ DireccionImagen(imagen)+"');");
                mySqlAccess.CommitTransaction();
                mySqlAccess.CloseConnection();
            }
            catch(Exception ex)
            {

            }
        }
        public Dictionary<string, string> ObtenerCategorias()
        {
            mySqlAccess.OpenConnection();
            DataTable result = new DataTable();
            result = mySqlAccess.QuerySQL("Select * From categorias");
            Dictionary<string, string> categorieImage = new Dictionary<string, string>();
            for(int x = 0; x < result.Rows.Count; x++)
            {
                categorieImage.Add(Convert.ToString(result.Rows[x]["Nombre"]),Convert.ToString(result.Rows[x]["Imagen"]));
            }
            return categorieImage;
        }

        public void AsociarLibroCategoria(List<string> categorias)
        {
            mySqlAccess.OpenConnection();
            DataTable result = mySqlAccess.QuerySQL("SELECT * FROM libros ORDER BY ID_Libro desc LIMIT 1;");
            int id_libro = Convert.ToInt32(result.Rows[0]["ID_libro"]);
            for(int x = 0; x<categorias.Count; x++)
            {
                mySqlAccess.EjectSQL("Insert Into libros_categorias(`categorias_Nombre`, `libros_ID_Libro`) values('"+categorias[x]+"', '"+ id_libro+"');");
            }
            mySqlAccess.CloseConnection();
            Console.WriteLine(result.Rows[0]["ID_libro"]);
        }
        private string DireccionImagen(Book book)
        {
            string result = book.ImageBook;
            result = result.Replace('\\', '/');
            Console.WriteLine(result);
            return result;
        }
        private string DireccionImagen(string imagen)
        {
            string result = imagen;
            result = result.Replace('\\', '/');
            Console.WriteLine(result);
            return result;
        }
        public int BuscarLibro(Book book)
        {
            mySqlAccess.OpenConnection();
            DataTable result = mySqlAccess.QuerySQL("Select ID_Libro from libros where Usuarios_Nombre_Usuario = '" + book.User.NameUser + "' and Nombre='" + book.NameBook + "'");
            int id_libro = Convert.ToInt32(result.Rows[0]["ID_libro"]);
            mySqlAccess.CloseConnection();
            return id_libro;
        }
        public void ActualizarLibro(int id_libro, Book book)
        {
            Console.WriteLine(id_libro);
            mySqlAccess.OpenConnection();
            mySqlAccess.EjectSQL("Update libros set Nombre='"+book.NameBook+ "', Privacidad=" + book.AccessBook+ ", Imagen='"+book.ImageBook+"' where ID_Libro =" + id_libro+";");
            mySqlAccess.CommitTransaction();
            mySqlAccess.EjectSQL("Delete from libros_categorias where Libros_ID_Libro = "+id_libro+";");
            mySqlAccess.CommitTransaction();
            for(int x = 0; x< book.CategorieBook.Count; x++)
            {
                mySqlAccess.EjectSQL("Insert Into libros_categorias(`categorias_Nombre`, `libros_ID_Libro`) values('" + book.CategorieBook[x]+"',"+id_libro+")");
                mySqlAccess.CommitTransaction();
            }
            mySqlAccess.CloseConnection();
        }
        public void BorrarLibro(int id_libro)
        {
            mySqlAccess.OpenConnection();
            mySqlAccess.EjectSQL("Delete from libros_categorias where Libros_ID_Libro = " + id_libro + ";");
            mySqlAccess.CommitTransaction();
            mySqlAccess.EjectSQL("Delete From `libros` Where (`ID_Libro` =  "+id_libro+");");
            mySqlAccess.CommitTransaction();
            mySqlAccess.CloseConnection();
        }
    }
}