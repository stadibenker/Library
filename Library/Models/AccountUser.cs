using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Library.Models
{
    public class AccountUser
    {
        public string UserName { get; set; }
        public string UserRoots { get; set; }
        public enum Roots { User, Administrator }
    }


    public class UserOperations
    {
        public AccountUser FindUser(string userName)
        {
            SqlConnection connect = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=UserAccounts;Integrated Security=True");
            connect.Open();
            SqlDataReader read = null;
            SqlCommand command = new SqlCommand("select * from Users where Us_name = '" + userName + "'", connect);

            read = command.ExecuteReader();
            AccountUser user = new AccountUser();
            if (read.Read())
            {
                user.UserName = String.Format("{0}", read["Us_name"].ToString());
                user.UserRoots = String.Format("{0}", read["Us_root"].ToString());
            }
            else
            {
                user.UserName = "empty";
            }
            connect.Close();
            return user;
        }


        public int AddUser(AccountUser user)
        {
            SqlConnection connect = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=UserAccounts;Integrated Security=True");
            connect.Open();

            //добавление юзера в таблицу "Users"
            string insertQuery = "insert into Users values ('"
                + user.UserName + "','"
                + user.UserRoots + "')";
            SqlCommand command = new SqlCommand(insertQuery, connect);
            int res = command.ExecuteNonQuery();
            connect.Close();
            return res;
        }


        public bool Authentication(string userName)
        {
            AccountUser user = new AccountUser();
            user = FindUser(userName);
            if (user.UserRoots == "admin")
            {
                return true;
            }
            else return false;
        }
    }
}