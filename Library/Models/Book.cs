using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Library.Models
{
    public class Book
    {
        public int Book_id { get; set; }
        public string Book_name { get; set; }
        public string Author_name { get; set; }
        public string Country { get; set; }
        public int Year_edition { get; set; }
        public int Quantity { get; set; }
    }


    public class Author
    {
        public int Book_id { get; set; }
        public string Author_name { get; set; }
    }


    public class User
    {
        public string Us_name { get; set; }
        public int Book_id { get; set; }
        public DateTime Date_taken { get; set; }
    }


    public class PageInfo
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages
        {
            get { return (int)Math.Ceiling((decimal)TotalItems / PageSize); }
        }
    }


    public class BooksAuthorsModel
    {
        public IEnumerable<Book> Books { get; set; }
        public IEnumerable<Author> Authors { get; set; }
        public PageInfo PageInfo { get; set; }
    }


    public class UserBookModel
    {
        public IEnumerable<User> Users { get; set; }
        public Book book;
    }


    public class DataOperations
    {
        public BooksAuthorsModel GetAllBooks(int currentPage, int pageSize, string sortQuery, string filterQuery, string userName)
        {
            List<Author> authors = new List<Author> { };
            //List<User> users = new List<User> { };
            List<Book> books = new List<Book> { };
            SqlConnection connect = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Library;Integrated Security=True");
            connect.Open();
            SqlDataReader bookRead = null;
            SqlCommand command = new SqlCommand();
            if (filterQuery != "UserBooks")
            {
                command.CommandText = "select * from Books, Authors where Books.Book_id = Authors.Book_id" + filterQuery
                     + "" + sortQuery
                     + " offset " + ((currentPage - 1) * pageSize).ToString() + " rows fetch next "
                     + pageSize.ToString() + " rows only";
            }
            else
            {
                command.CommandText = "select * from Books, Authors, Users where Books.Book_id = Authors.Book_id and Books.Book_id = Users.Book_id and Books.Book_id in " +
                    "(select Book_id from Users where Us_name = '" + userName + "')";
            }
            command.Connection = connect;
            bookRead = command.ExecuteReader();
            int i = 0;
            while (bookRead.Read())
            {
                Book book = new Book
                {
                    Book_id = Convert.ToInt32(String.Format("{0}", bookRead["Book_id"].ToString())),
                    Book_name = String.Format("{0}", bookRead["Book_name"].ToString()),
                    Country = String.Format("{0}", bookRead["Country"].ToString()),
                    Year_edition = Convert.ToInt32(String.Format("{0}", bookRead["Year_edition"].ToString())),
                    Quantity = Convert.ToInt32(String.Format("{0}", bookRead["Quantity"].ToString()))
                };
                //не добавлять повторяющиеся книги
                if(i != 0)
                {
                    Book tmp = books.ElementAt(i-1);
                    if (book.Book_id != tmp.Book_id)
                    {
                        books.Add(book);
                        i++;
                    }
                }
                else
                {
                    books.Add(book);
                    i++;
                }
                Author author = new Author()
                {
                    Book_id = Convert.ToInt32(String.Format("{0}", bookRead["Book_id"].ToString())),
                    Author_name = String.Format("{0}", bookRead["Author_name"].ToString())
                };
                authors.Add(author);
            }
            bookRead.Close();
            connect.Close();
            BooksAuthorsModel authorsBooks = new BooksAuthorsModel()
            {
                Books = books,
                Authors = authors
            };
            return authorsBooks;
        }


        public void Add(Book book)
        {
            SqlConnection connect = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Library;Integrated Security=True");
            connect.Open();

            //добавление книги в таблицу "Books"
            string insertQuery = "insert into Books values ('"
                + book.Book_id.ToString() + "','"
                + book.Book_name + "','"
                + book.Country + "','"
                + book.Year_edition.ToString() + "','"
                + book.Quantity.ToString() + "')";
            SqlCommand command = new SqlCommand(insertQuery, connect);
            command.ExecuteNonQuery();

            //добавление автора в таблицу "Authors"
            insertQuery = "insert into Authors values ";
            string[] authors = book.Author_name.Split(',');
            foreach (string authorName in authors)
            {
                insertQuery += "('";
                insertQuery += authorName + "','"
                + book.Book_id.ToString() + "'),";
            }
            insertQuery = insertQuery.TrimEnd(',');
            command.CommandText = insertQuery;
            command.ExecuteNonQuery();
            connect.Close();
        }


        /*public void Edit(Book book)
        {
            SqlConnection connect = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Library;Integrated Security=True");
            connect.Open();
            //изменение книги в таблице "Books"
            string editQuery = "update Books set "
                + "Book_name='" + book.Book_name + "',"
                + "Country='" + book.Country + "',"
                + "Year_edition=" + book.Year_edition.ToString() + ","
                + "Quantity=" + book.Quantity.ToString()
                + " where Book_id=" + book.Book_id;
            SqlCommand command = new SqlCommand(editQuery, connect);
            int res = command.ExecuteNonQuery();

            //изменение автора в таблице "Authors"
            editQuery = "update Authors set "
                 + "Author_name='" + book.Author_name + "'"
                 + " where Book_id=" + book.Book_id;
            command.CommandText = editQuery;
            command.ExecuteNonQuery();
            connect.Close();
        }*/


        public void Delete(int id)
        {
            SqlConnection connect = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Library;Integrated Security=True");
            connect.Open();

            //удаление книги
            string deleteQuery = "delete from Books where Book_id = '" + id.ToString() + "'";
            SqlCommand command = new SqlCommand(deleteQuery, connect);
            command.ExecuteNonQuery();
            connect.Close();
        }


        public Book Find(int id)
        {
            SqlConnection connect = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Library;Integrated Security=True");
            connect.Open();
            SqlDataReader read = null;
            SqlCommand command = new SqlCommand("select * from Books, Authors where Books.Book_id = Authors.Book_id and Books.Book_id = '" + id.ToString() + "'", connect);

            read = command.ExecuteReader();
            if (read.Read())
            {
                Book book = new Book()
                {
                    Book_id = Convert.ToInt32(String.Format("{0}", read["Book_id"].ToString())),
                    Book_name = String.Format("{0}", read["Book_name"].ToString()),
                    Author_name = String.Format("{0}", read["Author_name"].ToString()),
                    Country = String.Format("{0}", read["Country"].ToString()),
                    Year_edition = Convert.ToInt32(String.Format("{0}", read["Year_edition"].ToString())),
                    Quantity = Convert.ToInt32(String.Format("{0}", read["Quantity"].ToString()))
                };
                connect.Close();
                return book;
            }
            return null;
        }


        public void ChangeQuantity(int id)
        {
            Book book = new Book();
            book = Find(id);
            book.Quantity -= 1;
            SqlConnection connect = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Library;Integrated Security=True");
            connect.Open();
            //изменение книги в таблице "Books"
            string editQuery = "update Books set "
                + "Quantity=" + book.Quantity.ToString()
                + " where Book_id=" + id;
            SqlCommand command = new SqlCommand(editQuery, connect);
            command.ExecuteNonQuery();
            connect.Close();
        }


        public int GetCountRows()
        {
            SqlConnection connect = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Library;Integrated Security=True");
            connect.Open();
            SqlCommand command = new SqlCommand("select count(*) from Books", connect);
            return Convert.ToInt32(command.ExecuteScalar());
        }


        public void AddTakenBook(int id, string userName, DateTime date)
        {
            SqlConnection connect = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Library;Integrated Security=True");
            connect.Open();

            //добавление пользователя, взявшего книгу, в таблицу "Users"
            string insertQuery = "insert into Users values ('"
                + userName + "','"
                + id.ToString() + "','"
                + date.ToString("O") + "')";
            SqlCommand command = new SqlCommand(insertQuery, connect);
            command.ExecuteNonQuery();
            connect.Close();
        }


        public List<User> GetUsersTakenBook(int id)
        {
            List<User> users = new List<User> { };
            SqlConnection connect = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Library;Integrated Security=True");
            connect.Open();
            SqlDataReader read = null;
            SqlCommand command = new SqlCommand(
                "select * from Users where Users.Book_id = '" + id.ToString() + "'", connect);
            read = command.ExecuteReader();
            while (read.Read())
            {
                User user = new User
                {
                    Us_name = String.Format("{0}", read["Us_name"].ToString()),
                    Book_id = Convert.ToInt32(String.Format("{0}", read["Book_id"].ToString())),
                    Date_taken = DateTime.Parse(String.Format("{0}", read["Date_taken"].ToString()))
                };
                users.Add(user);
            }
            connect.Close();
            return users;
        }
    }
}