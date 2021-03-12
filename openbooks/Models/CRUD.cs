using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using System.Data.SqlClient;
using System.Data.Sql;
using System.Data;
using Microsoft.Data.SqlClient;

namespace openbooks.Models
{
    public class CRUD
    {
        //We are using localhost, our database name is 'openbooks' windows authentication
        //host.docker.internal resolves to the internal IP address used by the host.
        public static string connectionString = "Server=host.docker.internal,1433;Database=openbooks;User=SA;Password=Answer5922;";
        public static int CuserID = 0;
        public static int Login(string email, string password)
        {
            int result = -2;
            // Instantiate the connection 
            SqlConnection con = new SqlConnection(connectionString);
            // Open the connection 
            con.Open();
            try//executing the stored procedure UserLoginProc with email & password as input parameters and output as output parameter.
            {
                SqlCommand cmd = new SqlCommand("UserLoginProc", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@email", SqlDbType.VarChar, 50).Value = email;
                cmd.Parameters.Add("@password", SqlDbType.VarChar, 50).Value = password;
                cmd.Parameters.Add("@output", SqlDbType.Int).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();
                result = Convert.ToInt32(cmd.Parameters["@output"].Value);
            }

            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error" + ex.Message.ToString());
                result = -1; //-1 will be interpreted as "error while connecting with the database."
            }
            finally
            {
                con.Close();
            }
            return result;

        }
        public static Tuple<int, int> SignUp(string fullname, string email, string password)
        {
            int result = -2;
            Tuple<int, int> Out = new Tuple<int, int>(result, CuserID);
            // Instantiate the connection 
            SqlConnection con = new SqlConnection(connectionString);
            // Open the connection 
            con.Open();
            try//executing the stored procedure UserSignupProc with fullname, email & password as input parameters and output as output parameter.
            {
                SqlCommand cmd = new SqlCommand("UserSignupProc", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@fullname", SqlDbType.VarChar, 50).Value = fullname;
                cmd.Parameters.Add("@email", SqlDbType.VarChar, 50).Value = email;
                cmd.Parameters.Add("@password", SqlDbType.VarChar, 50).Value = password;
                cmd.Parameters.Add("@userID", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@output", SqlDbType.Int).Direction = ParameterDirection.Output;

                int rowsAffected = cmd.ExecuteNonQuery();
                result = Convert.ToInt32(cmd.Parameters["@output"].Value);
                CuserID = Convert.ToInt32(cmd.Parameters["@userID"].Value);
                Out = new Tuple<int, int>(result, CuserID);

            }

            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error" + ex.Message.ToString());
                result = -1; //-1 will be interpreted as "error while connecting with the database."
            }
            finally
            {
                con.Close();
            }

            return Out;

        }
        
        public static int logout()
		{
            int result = -2;
            // Instantiate the connection 
            SqlConnection con = new SqlConnection(connectionString);
            // Open the connection 
            con.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("UserLogout", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@userid", SqlDbType.Int).Value = CuserID;
                cmd.Parameters.Add("@output", SqlDbType.Int).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();
                result = Convert.ToInt32(cmd.Parameters["@output"].Value);
            }

            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error" + ex.Message.ToString());
                result = -1; //-1 will be interpreted as "error while connecting with the database."
            }
            finally
            {
                con.Close();
            }
            return result;
        }
        public static int updateOrderStatus()
        {
            int result = -2;
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("updateOrderStatus", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@userid", SqlDbType.Int).Value = CuserID;
                cmd.Parameters.Add("@output", SqlDbType.Int).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();
                result = Convert.ToInt32(cmd.Parameters["@output"].Value);
            }

            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error" + ex.Message.ToString());
                result = -1; //-1 will be interpreted as "error while connecting with the database."
            }
            finally
            {
                con.Close();
            }
            return result;
        }
        public static Tuple<int, List<Book>> getsearchresults(string searchterm)
        {
            int result = -3;
            List<Book> list = new List<Book>();
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("searchresult", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@input", SqlDbType.VarChar).Value = searchterm;
                cmd.Parameters.Add("@output", SqlDbType.Int).Direction = ParameterDirection.Output;

                

                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())//read the table row wise
                {
                    Book bk = new Book();
                    bk.bookID = Convert.ToInt32(rdr["bookid"].ToString());
                    bk.title = rdr["title"].ToString();
                    bk.author = rdr["author"].ToString();
                    bk.isbn = rdr["isbn"].ToString();
                    bk.language = rdr["language"].ToString();
                    bk.description = rdr["description"].ToString();
                    bk.publishingDate = rdr["publishingDate"].ToString();
                    bk.price = Convert.ToInt32(rdr["price"].ToString());
                    bk.discount = Convert.ToInt32(rdr["discount"].ToString());
                    bk.category = rdr["category"].ToString();
                    bk.numberOfCopies = Convert.ToInt32(rdr["numberOfCopies"].ToString());
                    list.Add(bk);
                }
                
                rdr.Close();
                cmd.ExecuteNonQuery();
                result = Convert.ToInt32(cmd.Parameters["@output"].Value);

            }

            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error" + ex.Message.ToString());
                result = -1; //-1 will be interpreted as "error while connecting with the database."
            }
            finally
            {
                con.Close();
            }
            Tuple<int, List<Book>> booksList = new Tuple<int, List<Book>>(result, list);

            return booksList;
        }
        public static int EditProfile(int userID, string fullname, string email, string phone, string gender, string line, string city, string state, string country, string zipCode)
        {
            if (userID == 0)
            {
                return -3; //-3 will be interpreted as "No such User ID found in Session."
            }
            if (string.IsNullOrEmpty(fullname) && string.IsNullOrEmpty(email) && string.IsNullOrEmpty(phone) && string.IsNullOrEmpty(gender) && string.IsNullOrEmpty(line) && string.IsNullOrEmpty(city) && string.IsNullOrEmpty(state) && string.IsNullOrEmpty(country) && string.IsNullOrEmpty(zipCode))
            {
                return -4; //-4 will be interpreted as "No textbox populated."
            }

            int result = -2;

            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            SqlCommand cmd;

            try
            {
                cmd = new SqlCommand("editUserInfo", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@input", SqlDbType.VarChar, 50).Value = userID;
                cmd.Parameters.Add("@fullname", SqlDbType.VarChar, 50).Value = fullname;
                cmd.Parameters.Add("@email", SqlDbType.VarChar, 50).Value = email;
                cmd.Parameters.Add("@phone", SqlDbType.VarChar, 50).Value = phone;
                cmd.Parameters.Add("@gender", SqlDbType.VarChar, 50).Value = gender;
                cmd.Parameters.Add("@line", SqlDbType.VarChar, 50).Value = line;
                cmd.Parameters.Add("@city", SqlDbType.VarChar, 50).Value = city;
                cmd.Parameters.Add("@state", SqlDbType.VarChar, 50).Value = state;
                cmd.Parameters.Add("@country", SqlDbType.VarChar, 50).Value = country;
                cmd.Parameters.Add("@zipCode", SqlDbType.Int).Value = (string.IsNullOrEmpty(zipCode) || zipCode[0] == ' ') ? 0 : Convert.ToInt32(zipCode);
                cmd.Parameters.Add("@output", SqlDbType.Int).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();
                result = Convert.ToInt32(cmd.Parameters["@output"].Value);
            }

            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error" + ex.Message.ToString());
                result = -1; //-1 will be interpreted as "error while connecting with the database."
            }
            finally
            {
                con.Close();
            }
            return result;
        }
        public static Tuple<int, User> getUserInfo(int ID)
        {
            int result = -1;
            User userInfo = new User();
            Tuple<int, User> userTuple = new Tuple<int, User>(result, userInfo);

            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            SqlCommand cmd;

            try
            {
                cmd = new SqlCommand("ViewUserInfo", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@input", SqlDbType.Int).Value = ID;
                cmd.Parameters.Add("@output", SqlDbType.Int).Direction = ParameterDirection.Output;

                //cmd.ExecuteNonQuery();
                result = Convert.ToInt32(cmd.Parameters["@output"].Value);

                SqlDataReader rdr = cmd.ExecuteReader();
                //if (result == 1)
                {

                    if (rdr.Read())
                    {
                        userInfo.fullname = rdr["fullname"].ToString();
                        userInfo.email = rdr["email"].ToString();
                        userInfo.password = rdr["password"].ToString();
                        userInfo.userRegisteredDatetime = Convert.ToDateTime(rdr["userRegisteredDatetime"].ToString());
                        userInfo.phone = rdr["phone"].ToString();
                        userInfo.gender = rdr["gender"].ToString();
                        userInfo.status = rdr["status"].ToString();
                        userInfo.line = rdr["line"].ToString();
                        userInfo.city = rdr["city"].ToString();
                        userInfo.state = rdr["state"].ToString();
                        userInfo.country = rdr["country"].ToString();
                        userInfo.zipCode = rdr["zipCode"].Equals(DBNull.Value) ? 0 : Convert.ToInt32(rdr["zipCode"].ToString());
                        if (!rdr["userDetailsAdditionDatetime"].Equals(DBNull.Value))
                            userInfo.userDetailsAdditionDatetime = Convert.ToDateTime(rdr["userDetailsAdditionDatetime"].ToString());

                        rdr.Close();
                        con.Close();
                        userTuple = new Tuple<int, User>(result, userInfo);

                        return userTuple;
                    }
                }
                rdr.Close();
                con.Close();
                return userTuple;//in this case, userTuple.Item1 = 0, which means, "No such User ID found in database."
            }

            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error" + ex.Message.ToString());
                return userTuple;//in this case, userTuple.Item1 = -1, which means, sql error
            }
        }
        public static int GetUserID(string email)
        {
            int result = -2;
            // Instantiate the connection 
            SqlConnection con = new SqlConnection(connectionString);
            // Open the connection 
            con.Open();
            try//executing the stored procedure getUserID with email as input parameters and userID as output parameter.
            {
                SqlCommand cmd = new SqlCommand("getUserID", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@email", SqlDbType.VarChar, 50).Value = email;
                cmd.Parameters.Add("@userID", SqlDbType.Int).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();
                result = Convert.ToInt32(cmd.Parameters["@userID"].Value);
                CuserID = result;
            }

            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error" + ex.Message.ToString());
                result = -1; //-1 will be interpreted as "error while connecting with the database."
            }
            finally
            {
                con.Close();
            }
            return result;

        }
        public static int getUserType(int userID)
		{
            int result = -2;
            // Instantiate the connection 
            SqlConnection con = new SqlConnection(connectionString);
            // Open the connection 
            con.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("getUserType", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@userid", SqlDbType.Int).Value = userID;
                cmd.Parameters.Add("@output", SqlDbType.Int).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();
                result = Convert.ToInt32(cmd.Parameters["@output"].Value);
            }

            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error" + ex.Message.ToString());
                result = -1; //-1 will be interpreted as "error while connecting with the database."
            }
            finally
            {
                con.Close();
            }
            return result;
        }
        public static int EditPassword(int userID, string currentPassword, string newPassword, string confirmNewPassword)
        {
            int type = CRUD.getUserType(userID);
            if (type == -1)
            {
                return -10;
            }
            if (string.IsNullOrEmpty(currentPassword))
            {
                return -4;
            }
            if (string.IsNullOrEmpty(newPassword))
            {
                return -5;
            }
            if (string.IsNullOrEmpty(confirmNewPassword))
            {
                return -6;
            }
            if (!Equals(newPassword, confirmNewPassword))
            {
                return -7;
            }
            if (Equals(newPassword, currentPassword))
            {
                return -8;
            }

            int result = -11;
            // Instantiate the connection 
            SqlConnection con = new SqlConnection(connectionString);
            // Open the connection 
            con.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("changePassword", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@input", SqlDbType.VarChar, 50).Value = userID;
                cmd.Parameters.Add("@curr", SqlDbType.VarChar, 50).Value = currentPassword;
                cmd.Parameters.Add("@new", SqlDbType.VarChar, 50).Value = confirmNewPassword;
                cmd.Parameters.Add("@output", SqlDbType.Int).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();
                result = Convert.ToInt32(cmd.Parameters["@output"].Value);
            }

            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error" + ex.Message.ToString());
                result = -1; //-1 will be interpreted as "error while connecting with the database."
            }
            finally
            {
                con.Close();
            }

            if (type == 2)//if admin
                return -9;
            return result;
        }
        public static Tuple<int, List<Book>> getSectionBooks(int section)
        {
            int result = -2;
            // Instantiate the connection 
            SqlConnection con = new SqlConnection(connectionString);
            // Open the connection 
            con.Open();
            List<Book> list = new List<Book>();
            try
            {
                string cmdText = "";
                if (section == 0)
                    cmdText = "getAllBooks";
                else if (section == 1)
                    cmdText = "getBooksNEW";
                else if (section == 2)
                    cmdText = "getBooks30";
                else if (section == 3)
                    cmdText = "getBooks50";
                else if (section == 4)
                    cmdText = "getBooksWishlist";
                else if (section == 5)
                    cmdText = "getBooksSoldOut";
                SqlCommand cmd = new SqlCommand(cmdText, con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                //using ExecuteReader because a table will be returned from the called procedure
                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())//read the table row wise
                {
                    Book bk = new Book();
                    bk.bookID = Convert.ToInt32(rdr["bookid"].ToString());
                    bk.title = rdr["title"].ToString();
                    bk.author = rdr["author"].ToString();
                    bk.isbn = rdr["isbn"].ToString();
                    bk.language = rdr["language"].ToString();
                    bk.description = rdr["description"].ToString();
                    bk.publishingDate = rdr["publishingDate"].ToString();
                    bk.price = Convert.ToInt32(rdr["price"].ToString());
                    bk.discount = Convert.ToInt32(rdr["discount"].ToString());
                    bk.category = rdr["category"].ToString();
                    bk.numberOfCopies = Convert.ToInt32(rdr["numberOfCopies"].ToString());
                    list.Add(bk);
                }
                result = 1;
                rdr.Close();
            }
            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error" + ex.Message.ToString());
                result = -1;
            }
            finally
            {
                con.Close();
            }

            Tuple<int, List<Book>> booksList = new Tuple<int, List<Book>>(result, list);
            return booksList;
        }
        public static Tuple<int, List<Book>> GetWishlist()
        {
            int result = -2;
            List<Book> list = new List<Book>();
            
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("getBooksWishlist", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@userid", SqlDbType.Int).Value = CuserID;
                //using ExecuteReader because a table will be returned from the called procedure
                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())//read the table row wise
                {
                    Book bk = new Book();
                    bk.bookID = Convert.ToInt32(rdr["bookid"].ToString());
                    bk.title = rdr["title"].ToString();
                    bk.author = rdr["author"].ToString();
                    bk.isbn = rdr["isbn"].ToString();
                    bk.language = rdr["language"].ToString();
                    bk.description = rdr["description"].ToString();
                    bk.publishingDate = rdr["publishingDate"].ToString();
                    bk.price = Convert.ToInt32(rdr["price"].ToString());
                    bk.discount = Convert.ToInt32(rdr["discount"].ToString());
                    bk.category = rdr["category"].ToString();
                    list.Add(bk);
                }
                result = 1;
                rdr.Close();
            }

            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error" + ex.Message.ToString());
                result = -1; //-1 will be interpreted as "error while connecting with the database."
            }
            finally
            {
                con.Close();
            }
            Tuple<int, List<Book>> wishlist = new Tuple<int, List<Book>>(result, list);
            return wishlist;
        }
        public static int addToWishlist(int bookid)
        {
            int result = -2;
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("addToWishlist", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@userid", SqlDbType.Int).Value = CuserID;
                cmd.Parameters.Add("@bookid", SqlDbType.Int).Value = bookid;
                cmd.Parameters.Add("@output", SqlDbType.Int).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();
                result = Convert.ToInt32(cmd.Parameters["@output"].Value);
            }

            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error" + ex.Message.ToString());
                result = -1; //-1 will be interpreted as "error while connecting with the database."
            }
            finally
            {
                con.Close();
            }
            return result;
        }
        public static int removeFromWishlist(int bookid)
        {
            int result = -2;
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("removeFromWishlist", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@userid", SqlDbType.Int).Value = CuserID;
                cmd.Parameters.Add("@bookid", SqlDbType.Int).Value = bookid;
                cmd.Parameters.Add("@output", SqlDbType.Int).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();
                result = Convert.ToInt32(cmd.Parameters["@output"].Value);
            }

            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error" + ex.Message.ToString());
                result = -1; //-1 will be interpreted as "error while connecting with the database."
            }
            finally
            {
                con.Close();
            }
            return result;
        }
        public static int addToDBCart(int bookid)
        {
            int result = -2;
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("addToCart", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@userid", SqlDbType.Int).Value = CuserID;
                cmd.Parameters.Add("@bookid", SqlDbType.Int).Value = bookid;
                cmd.Parameters.Add("@output", SqlDbType.Int).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();
                result = Convert.ToInt32(cmd.Parameters["@output"].Value);
            }

            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error" + ex.Message.ToString());
                result = -1; //-1 will be interpreted as "error while connecting with the database."
            }
            finally
            {
                con.Close();
            }
            return result;
        }
        public static Tuple<int, List<Book>, string> getCartItems()
        {
            int cartCount = CRUD.getCartCount();
            String Data = "";
            if (cartCount == -1)
            {
                Data = "Something went wrong while connecting with the database.";
            }

            Tuple<int, List<Book>> gotItems = CRUD.getItemsFromCart(cartCount);
            if (gotItems.Item1 == -1)
            {
                Data = "Something went wrong while connecting with the database.";
            }
            else if (gotItems.Item1 == 0)
            {
                Data = "Couldn't get userID.";
            }
            Tuple<int, List<Book>, string> cartItems = new Tuple<int, List<Book>, string>(cartCount, gotItems.Item2, Data);
            return cartItems;
        }
        public static int getCartCount()
        {
            int result = -3;
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("getCartCount", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@userid", SqlDbType.Int).Value = CuserID;
                cmd.Parameters.Add("@count", SqlDbType.Int).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();
                result = Convert.ToInt32(cmd.Parameters["@count"].Value);
            }

            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error" + ex.Message.ToString());
                result = -1; //-1 will be interpreted as "error while connecting with the database."
            }
            finally
            {
                con.Close();
            }
            return result;
        }
        public static Tuple<int, List<Book>> getItemsFromCart(int cartCount)
        {
            int result = -2;
            List<Book> list = new List<Book>();

            SqlConnection con = new SqlConnection(connectionString);
            con.Open();

            try
            {
                SqlCommand cmd = new SqlCommand("getCartItems", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@userid", SqlDbType.Int).Value = CuserID;
                cmd.Parameters.Add("@output", SqlDbType.Int).Direction = ParameterDirection.Output;

                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    Book bk = new Book();
                    bk.bookID = Convert.ToInt32(rdr["bookid"].ToString());
                    bk.title = rdr["title"].ToString();
                    bk.author = rdr["author"].ToString();
                    bk.isbn = rdr["isbn"].ToString();
                    bk.language = rdr["language"].ToString();
                    bk.description = rdr["description"].ToString();
                    bk.publishingDate = rdr["publishingDate"].ToString();
                    bk.price = Convert.ToInt32(rdr["price"].ToString());
                    bk.discount = Convert.ToInt32(rdr["discount"].ToString());
                    bk.category = rdr["category"].ToString();
                    bk.numberOfCopies = Convert.ToInt32(rdr["numberOfCopies"].ToString());
                    bk.book_count = Convert.ToInt32(rdr["book_count"].ToString());
                    list.Add(bk);
                }
                rdr.Close();
                cmd.ExecuteNonQuery();
                result = Convert.ToInt32(cmd.Parameters["@output"].Value);

            }

            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error" + ex.Message.ToString());
                result = -1; //-1 will be interpreted as "error while connecting with the database."
            }
            finally
            {
                con.Close();
            }
            Tuple<int, List<Book>> cartItems = new Tuple<int, List<Book>>(result, list);
            return cartItems;
        }
        public static int removeFromDBCart(int bookid)
        {
            int result = -2;
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("removeFromCart", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@userid", SqlDbType.Int).Value = CuserID;
                cmd.Parameters.Add("@bookid", SqlDbType.Int).Value = bookid;
                cmd.Parameters.Add("@output", SqlDbType.Int).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();
                result = Convert.ToInt32(cmd.Parameters["@output"].Value);
            }

            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error" + ex.Message.ToString());
                result = -1; //-1 will be interpreted as "error while connecting with the database."
            }
            finally
            {
                con.Close();
            }
            return result;
        }
        public static Tuple<int, Book> getThisBook(int bookid)
        {
            int result = -2;
            Book bk = new Book();

            SqlConnection con = new SqlConnection(connectionString);
            con.Open();

            try
            {
                SqlCommand cmd = new SqlCommand("getThisBook", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@bookid", SqlDbType.Int).Value = bookid;
                cmd.Parameters.Add("@output", SqlDbType.Int).Direction = ParameterDirection.Output;

                SqlDataReader rdr = cmd.ExecuteReader();

                if (rdr.Read())
                {
                    bk.bookID = Convert.ToInt32(rdr["bookid"].ToString());
                    bk.title = rdr["title"].ToString();
                    bk.author = rdr["author"].ToString();
                    bk.isbn = rdr["isbn"].ToString();
                    bk.language = rdr["language"].ToString();
                    bk.description = rdr["description"].ToString();
                    bk.publishingDate = rdr["publishingDate"].ToString();
                    bk.price = Convert.ToInt32(rdr["price"].ToString());
                    bk.discount = Convert.ToInt32(rdr["discount"].ToString());
                    bk.category = rdr["category"].ToString();
                    bk.numberOfCopies = Convert.ToInt32(rdr["numberOfCopies"].ToString());
                }
                rdr.Close();
                cmd.ExecuteNonQuery();
                result = Convert.ToInt32(cmd.Parameters["@output"].Value);

            }

            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error" + ex.Message.ToString());
                result = -1; //-1 will be interpreted as "error while connecting with the database."
            }
            finally
            {
                con.Close();
            }
            Tuple<int, Book> requiredBook = new Tuple<int, Book>(result, bk);
            return requiredBook;
        }
        public static Tuple<int, List<string>> getCategories()
        {
            int result = -2;
            List<string> categories = new List<string>();

            SqlConnection con = new SqlConnection(connectionString);
            con.Open();

            try
            {
                SqlCommand cmd = new SqlCommand("getCategories", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    categories.Add(rdr["category"].ToString());
                }
                rdr.Close();
            }

            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error" + ex.Message.ToString());
                result = -1; //-1 will be interpreted as "error while connecting with the database."
            }
            finally
            {
                con.Close();
            }
            Tuple<int, List<string>> allCategories = new Tuple<int, List<string>>(result, categories);
            return allCategories;
        }
        public static Tuple<int, List<Book>> getRequiredCategoryBooks(string categoryType)
        {
            int result = -2;
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            List<Book> list = new List<Book>();
            try
            {
                SqlCommand cmd = new SqlCommand("getRequiredCategoryBooks", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@category", SqlDbType.VarChar, 50).Value = categoryType;
                cmd.Parameters.Add("@output", SqlDbType.Int).Direction = ParameterDirection.Output;

                //using ExecuteReader because a table will be returned from the called procedure
                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())//read the table row wise
                {
                    Book bk = new Book();
                    bk.bookID = Convert.ToInt32(rdr["bookid"].ToString());
                    bk.title = rdr["title"].ToString();
                    bk.author = rdr["author"].ToString();
                    bk.isbn = rdr["isbn"].ToString();
                    bk.language = rdr["language"].ToString();
                    bk.description = rdr["description"].ToString();
                    bk.publishingDate = rdr["publishingDate"].ToString();
                    bk.price = Convert.ToInt32(rdr["price"].ToString());
                    bk.discount = Convert.ToInt32(rdr["discount"].ToString());
                    bk.category = rdr["category"].ToString();
                    bk.numberOfCopies = Convert.ToInt32(rdr["numberOfCopies"].ToString());
                    list.Add(bk);
                }
                rdr.Close();
                cmd.ExecuteNonQuery();
                result = Convert.ToInt32(cmd.Parameters["@output"].Value);
            }
            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error" + ex.Message.ToString());
                result = -1;
            }
            finally
            {
                con.Close();
            }

            Tuple<int, List<Book>> booksList = new Tuple<int, List<Book>>(result, list);
            return booksList;
        }
        public static int confirmOrder()
        {
            int result = -2;
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("placeOrder", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@userid", SqlDbType.Int).Value = CuserID;
                cmd.Parameters.Add("@output", SqlDbType.Int).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();
                result = Convert.ToInt32(cmd.Parameters["@output"].Value);
            }
            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error" + ex.Message.ToString());
                result = -1;
            }
            finally
            {
                con.Close();
            }
            return result;
        }
        public static int rateThisBook(int userid, string bookid, string stars)
        {
            int result = -3;
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("rateThisBook", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@userid", SqlDbType.Int).Value = userid;
                cmd.Parameters.Add("@bookid", SqlDbType.Int).Value = Convert.ToInt32(bookid);
                cmd.Parameters.Add("@stars", SqlDbType.Int).Value = Convert.ToInt32(stars);
                cmd.Parameters.Add("@output", SqlDbType.Int).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();
                result = Convert.ToInt32(cmd.Parameters["@output"].Value);
            }

            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error" + ex.Message.ToString());
                result = -1; //-1 will be interpreted as "error while connecting with the database."
            }
            finally
            {
                con.Close();
            }
            return result;
        }
        
        public static int getAverageRating(string bookid)
        {
            int result = -2;
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            List<Rating> list = new List<Rating>();
            try
            {
                SqlCommand cmd = new SqlCommand("getAVGratings", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@bookid", SqlDbType.Int).Value = Convert.ToInt32(bookid); ;
                cmd.Parameters.Add("@output", SqlDbType.Int).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();
                result = Convert.ToInt32(cmd.Parameters["@output"].Value);
            }
            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error" + ex.Message.ToString());
                result = -1;
            }
            finally
            {
                con.Close();
            }
            //Tuple<int, List<Rating>> avgRating = new Tuple<int, List<Rating>>(result, list);
            return result;
        }
        
        public static int requestThisBook(string title, string author, string isbn)
        {
            int result = -3;
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("requestThisBook", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@title", SqlDbType.VarChar, 100).Value = title;
                cmd.Parameters.Add("@author", SqlDbType.VarChar, 100).Value = author;
                cmd.Parameters.Add("@userid", SqlDbType.VarChar, 50).Value = CuserID;
                cmd.Parameters.Add("@output", SqlDbType.Int).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();
                result = Convert.ToInt32(cmd.Parameters["@output"].Value);
            }

            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error" + ex.Message.ToString());
                result = -1; //-1 will be interpreted as "error while connecting with the database."
            }
            finally
            {
                con.Close();
            }
            return result;
        }
        
        public static Tuple<int, List<Request>> getPreviousRequests()
        {
            int result = -2;
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            List<Request> list = new List<Request>();
            try
            {
                SqlCommand cmd = new SqlCommand("getPreviousRequests", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@userid", SqlDbType.Int).Value = CuserID;
                cmd.Parameters.Add("@output", SqlDbType.Int).Direction = ParameterDirection.Output;

                //using ExecuteReader because a table will be returned from the called procedure
                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())//read the table row wise
                {
                    Request rq = new Request();
                    rq.title = rdr["title"].ToString();
                    rq.author = rdr["author"].ToString();
                    rq.requestedBookDate = Convert.ToDateTime(rdr["requestedBookDate"].ToString());
                    list.Add(rq);
                }
                rdr.Close();
                cmd.ExecuteNonQuery();
                result = Convert.ToInt32(cmd.Parameters["@output"].Value);
            }
            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error" + ex.Message.ToString());
                result = -1;
            }
            finally
            {
                con.Close();
            }

            Tuple<int, List<Request>> requestsList = new Tuple<int, List<Request>>(result, list);
            return requestsList;
        }
        
        public static int cancelOrder(int userid, int orderid)
        {
            int result = -3;
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("cancelOrder", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@userid", SqlDbType.Int).Value = userid;
                cmd.Parameters.Add("@orderid", SqlDbType.Int).Value = orderid;
                cmd.Parameters.Add("@output", SqlDbType.Int).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();
                result = Convert.ToInt32(cmd.Parameters["@output"].Value);
            }

            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error" + ex.Message.ToString());
                result = -1; //-1 will be interpreted as "error while connecting with the database."
            }
            finally
            {
                con.Close();
            }
            return result;
        }
        
        public static int addBook(string title, string author, string isbn, string language, string description, string price, string discount, string category, string noofcopies, string pdate)
        {
            int result = -2;
            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(author) || string.IsNullOrEmpty(isbn) || string.IsNullOrEmpty(language) || string.IsNullOrEmpty(description) || string.IsNullOrEmpty(price) || string.IsNullOrEmpty(discount) || string.IsNullOrEmpty(category) || string.IsNullOrEmpty(noofcopies) || string.IsNullOrEmpty(pdate))
                return result;
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("addBook", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@title", SqlDbType.VarChar, 100).Value = title;
                cmd.Parameters.Add("@author", SqlDbType.VarChar, 100).Value = author;
                cmd.Parameters.Add("@isbn", SqlDbType.VarChar, 14).Value = isbn;
                cmd.Parameters.Add("@language", SqlDbType.VarChar, 50).Value = language;
                cmd.Parameters.Add("@description", SqlDbType.VarChar, 1000).Value = description;
                cmd.Parameters.Add("@publishingDate", SqlDbType.VarChar, 10).Value = pdate;
                cmd.Parameters.Add("@price", SqlDbType.Int).Value = Convert.ToInt32( price);
                cmd.Parameters.Add("@discount", SqlDbType.Int).Value = Convert.ToInt32(discount);
                cmd.Parameters.Add("@category", SqlDbType.VarChar, 50).Value = category;
                cmd.Parameters.Add("@numberOfCopies", SqlDbType.Int).Value = Convert.ToInt32(noofcopies);

                cmd.ExecuteNonQuery();
                result = 1;
            }

            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error" + ex.Message.ToString());
                result = -1; //-1 will be interpreted as "error while connecting with the database."
            }
            finally
            {
                con.Close();
            }
            return result;
        }
        public static int editBookInfo(string bookid, string title, string author, string isbn, string language, string description, string price, string discount, string category, string noofcopies, string pdate)
        {
            if (string.IsNullOrEmpty(bookid))
                return -3; //-3 will be interpreted as "No book ID found."
            if (string.IsNullOrEmpty(title) && string.IsNullOrEmpty(author) && string.IsNullOrEmpty(isbn) && string.IsNullOrEmpty(language) && string.IsNullOrEmpty(description) && string.IsNullOrEmpty(price) && string.IsNullOrEmpty(discount) && string.IsNullOrEmpty(category) && string.IsNullOrEmpty(noofcopies) && string.IsNullOrEmpty(pdate))
                return -4; //-4 will be interpreted as "No textbox populated."

            int result = -2;

            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            SqlCommand cmd;

            try
            {
                cmd = new SqlCommand("editBookInfo", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@bookid", SqlDbType.Int).Value = Convert.ToInt32(bookid) ;
                cmd.Parameters.Add("@title", SqlDbType.VarChar, 100).Value = title;
                cmd.Parameters.Add("@author", SqlDbType.VarChar, 100).Value = author;
                cmd.Parameters.Add("@isbn", SqlDbType.VarChar, 14).Value = isbn;
                cmd.Parameters.Add("@language", SqlDbType.VarChar, 50).Value = language;
                cmd.Parameters.Add("@description", SqlDbType.VarChar, 1000).Value = description;
                cmd.Parameters.Add("@publishingDate", SqlDbType.VarChar, 10).Value = pdate;

                if(!string.IsNullOrEmpty(price))
                    cmd.Parameters.Add("@price", SqlDbType.Int).Value = Convert.ToInt32(price);
                else
                    cmd.Parameters.Add("@price", SqlDbType.Int).Value = 0;

                if (!string.IsNullOrEmpty(discount))
                    cmd.Parameters.Add("@discount", SqlDbType.Int).Value = Convert.ToInt32(discount);
                else
                    cmd.Parameters.Add("@discount", SqlDbType.Int).Value = 0;

                cmd.Parameters.Add("@category", SqlDbType.VarChar, 50).Value = category;

                if (!string.IsNullOrEmpty(noofcopies))
                    cmd.Parameters.Add("@numberOfCopies", SqlDbType.Int).Value = Convert.ToInt32(noofcopies);
                else
                    cmd.Parameters.Add("@numberOfCopies", SqlDbType.Int).Value = 0;

                cmd.Parameters.Add("@output", SqlDbType.Int).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();
                result = Convert.ToInt32(cmd.Parameters["@output"].Value);
            }

            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error" + ex.Message.ToString());
                result = -1; //-1 will be interpreted as "error while connecting with the database."
            }
            finally
            {
                con.Close();
            }
            return result;
        }
        public static int removeBook(string bookid)
        {
            int result = -2;
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("removeBook", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@bookid", SqlDbType.Int).Value = bookid;
                cmd.Parameters.Add("@output", SqlDbType.Int).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();
                result = Convert.ToInt32(cmd.Parameters["@output"].Value);
            }

            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error" + ex.Message.ToString());
                result = -1; //-1 will be interpreted as "error while connecting with the database."
            }
            finally
            {
                con.Close();
            }
            return result;
        }
        public static Tuple<int, Tuple<Order,List<Book>>> getorderslist(string searchterm)
        {
            int result = -3, st, Ocount=0;
            Order o = new Order();
            List<Book> list = new List<Book>();
            if (int.TryParse(searchterm, out st))
            {

                SqlConnection con = new SqlConnection(connectionString);
                con.Open();
                try
                {
                    SqlCommand cmd = new SqlCommand("searchorder", con);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    if (string.IsNullOrEmpty(searchterm))
                    {
                        cmd.Parameters.Add("@input", SqlDbType.Int).Value = st;
                    }
                    else
                        cmd.Parameters.Add("@input", SqlDbType.Int).Value = Convert.ToInt32(searchterm);
                    cmd.Parameters.Add("@output", SqlDbType.Int).Direction = ParameterDirection.Output;

                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())//read the table row wise
                    {
                        if (Ocount == 0)//it'll run only once
                        {
                            o.orderid = Convert.ToInt32(rdr["orderid"].ToString());
                            o.userid = Convert.ToInt32(rdr["userid"].ToString());
                            o.status = Convert.ToInt32(rdr["status"].ToString());
                            o.orderDate = Convert.ToDateTime(rdr["orderDate"].ToString());
                            o.deliveryDate = Convert.ToDateTime(rdr["deliveryDate"].ToString());
                            Ocount = 1;
                        }

                        Book bk = new Book();
                        bk.bookID = Convert.ToInt32(rdr["bookid"].ToString());
                        bk.title = rdr["title"].ToString();
                        bk.author = rdr["author"].ToString();
                        bk.isbn = rdr["isbn"].ToString();
                        bk.language = rdr["language"].ToString();
                        bk.description = rdr["description"].ToString();
                        bk.publishingDate = rdr["publishingDate"].ToString();
                        bk.price = Convert.ToInt32(rdr["price"].ToString());
                        bk.discount = Convert.ToInt32(rdr["discount"].ToString());
                        bk.category = rdr["category"].ToString();
                        bk.book_count = Convert.ToInt32(rdr["book_count"].ToString());
                        list.Add(bk);
                    }

                    rdr.Close();
                    cmd.ExecuteNonQuery();
                    result = Convert.ToInt32(cmd.Parameters["@output"].Value);

                }

                catch (SqlException ex)
                {
                    Console.WriteLine("SQL Error" + ex.Message.ToString());
                    result = -1; //-1 will be interpreted as "error while connecting with the database."
                }
                finally
                {
                    con.Close();
                }
            }
            Tuple<Order, List<Book>> orderedBooks = new Tuple<Order, List<Book>>(o, list);
            Tuple<int, Tuple<Order, List<Book>>> orderedBooksList = new Tuple<int, Tuple<Order, List<Book>>>(result, orderedBooks);

            return orderedBooksList;
        }
        public static Tuple<int, Tuple<List<Order>, List<User>, List<Book>>> getAllOrders(string orderType, string orderDateFrom, string orderDateTo, string orderid)
		{
            int result = -3, oid = 0;
            bool searchterm = int.TryParse(orderid, out oid);
            List<Order> olist = new List<Order>();
			List<Book> blist = new List<Book>();
            List<User> ulist = new List<User>();

			SqlConnection con = new SqlConnection(connectionString);
			con.Open();
			try
			{
                SqlCommand cmd = new SqlCommand("getAllOrders", con);
				cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@orderType", SqlDbType.Int).Value = Convert.ToInt32(orderType);
                cmd.Parameters.Add("@orderDateFrom", SqlDbType.DateTime).Value = orderDateFrom;
                cmd.Parameters.Add("@orderDateTo", SqlDbType.DateTime).Value = orderDateTo;
                cmd.Parameters.Add("@userid", SqlDbType.Int).Value = CRUD.CuserID;
                cmd.Parameters.Add("@orderid", SqlDbType.Int).Value = oid;
                cmd.Parameters.Add("@output", SqlDbType.Int).Direction = ParameterDirection.Output;

				SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())//read the table row wise
                {
                    Order o = new Order
                    {
                        orderid = Convert.ToInt32(rdr["orderid"].ToString()),
                        userid = Convert.ToInt32(rdr["userid"].ToString()),
                        status = Convert.ToInt32(rdr["status"].ToString()),
                        orderDate = Convert.ToDateTime(rdr["orderDate"].ToString()),
                        deliveryDate = Convert.ToDateTime(rdr["deliveryDate"].ToString())
                    };
                    olist.Add(o);

                    User u = new User
                    {
                        userID = Convert.ToInt32(rdr["userid"].ToString()),
                        fullname = rdr["fullname"].ToString(),
                        email = rdr["email"].ToString(),
                        password = rdr["password"].ToString(),
                        userRegisteredDatetime = Convert.ToDateTime(rdr["userRegisteredDatetime"].ToString()),
                        phone = rdr["phone"].ToString(),
                        gender = rdr["gender"].ToString(),
                        status = rdr["status"].ToString(),
                        line = rdr["line"].ToString(),
                        city = rdr["city"].ToString(),
                        state = rdr["state"].ToString(),
                        country = rdr["country"].ToString(),
                        zipCode = rdr["zipCode"].Equals(DBNull.Value) ? 0 : Convert.ToInt32(rdr["zipCode"].ToString()),
                        orderDate = Convert.ToDateTime(rdr["orderDate"].ToString())
                    };
                    if (!rdr["userDetailsAdditionDatetime"].Equals(DBNull.Value))
                        u.userDetailsAdditionDatetime = Convert.ToDateTime(rdr["userDetailsAdditionDatetime"].ToString());
                    ulist.Add(u);
                }

                rdr.Close();
				cmd.ExecuteNonQuery();
				result = Convert.ToInt32(cmd.Parameters["@output"].Value);
			}

			catch (SqlException ex)
			{
				Console.WriteLine("SQL Error" + ex.Message.ToString());
				result = -1; //-1 will be interpreted as "error while connecting with the database."
			}
			finally
			{
				con.Close();
			}

            con = new SqlConnection(connectionString);
            con.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("getAllOrdersDetails", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@orderType", SqlDbType.Int).Value = Convert.ToInt32(orderType);
                cmd.Parameters.Add("@orderDateFrom", SqlDbType.DateTime).Value = orderDateFrom;
                cmd.Parameters.Add("@orderDateTo", SqlDbType.DateTime).Value = orderDateTo;
                cmd.Parameters.Add("@userid", SqlDbType.Int).Value = CRUD.CuserID;
                cmd.Parameters.Add("@orderid", SqlDbType.Int).Value = oid;
                cmd.Parameters.Add("@output", SqlDbType.Int).Direction = ParameterDirection.Output;

                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())//read the table row wise
                {
                    Book bk = new Book
                    {
                        bookID = Convert.ToInt32(rdr["bookid"].ToString()),
                        title = rdr["title"].ToString(),
                        author = rdr["author"].ToString(),
                        isbn = rdr["isbn"].ToString(),
                        language = rdr["language"].ToString(),
                        description = rdr["description"].ToString(),
                        publishingDate = rdr["publishingDate"].ToString(),
                        price = Convert.ToInt32(rdr["price"].ToString()),
                        discount = Convert.ToInt32(rdr["discount"].ToString()),
                        category = rdr["category"].ToString(),
                        book_count = Convert.ToInt32(rdr["book_count"].ToString()),
                        orderid = Convert.ToInt32(rdr["orderid"].ToString())
                    };
                    blist.Add(bk);
                }

                rdr.Close();
                cmd.ExecuteNonQuery();
                result = Convert.ToInt32(cmd.Parameters["@output"].Value);
            }
            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error" + ex.Message.ToString());
                result = -1; //-1 will be interpreted as "error while connecting with the database."
            }
            finally
            {
                con.Close();
            }

            Tuple<List<Order>, List<User>, List<Book>> orderedBooks = new Tuple<List<Order>, List<User>, List<Book>>(olist, ulist, blist);
            Tuple<int, Tuple<List<Order>, List<User>, List<Book>>> orderedBooksList = new Tuple<int, Tuple<List<Order>, List<User>, List<Book>>>(result, orderedBooks);

			return orderedBooksList;
		}
        public static Tuple<int, List<Order>> getOrdersInBetweenTwoDates(string orderDateFrom, string orderDateTo)
        {
            int result = -2;
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            List<Order> list = new List<Order>();
            try
            {
                SqlCommand cmd = new SqlCommand("getOrdersInBetweenTwoDates", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@orderDateFrom", SqlDbType.DateTime).Value = orderDateFrom;
                cmd.Parameters.Add("@orderDateTo", SqlDbType.DateTime).Value = orderDateTo;
                cmd.Parameters.Add("@output", SqlDbType.Int).Direction = ParameterDirection.Output;

                //using ExecuteReader because a table will be returned from the called procedure
                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())//read the table row wise
                {
                    Order o = new Order();
                    o.orderid = Convert.ToInt32(rdr["orderid"].ToString());
                    o.bookid = Convert.ToInt32(rdr["bookid"].ToString());
                    o.book_count = Convert.ToInt32(rdr["book_count"].ToString());
                    o.userid = Convert.ToInt32(rdr["userid"].ToString());
                    o.status = Convert.ToInt32(rdr["status"].ToString());
                    o.orderDate = Convert.ToDateTime(rdr["orderDate"].ToString());
                    o.deliveryDate = Convert.ToDateTime(rdr["deliveryDate"].ToString());
                    //o.ordered_book_title = CRUD.getThisBook(o.bookid).Item2.title;
                    list.Add(o);
                }
                rdr.Close();
                cmd.ExecuteNonQuery();
                result = Convert.ToInt32(cmd.Parameters["@output"].Value);
            }
            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error" + ex.Message.ToString());
                result = -1;
            }
            finally
            {
                con.Close();
            }

            Tuple<int, List<Order>> ordersList = new Tuple<int, List<Order>>(result, list);
            return ordersList;
        }
        public static Tuple<int, List<User>> _searchusers(string searchterm)
        {
            int result = -3;
            List<User> list = new List<User>();
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("searchuser", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@input", SqlDbType.VarChar, 50).Value = searchterm;
                cmd.Parameters.Add("@output", SqlDbType.Int).Direction = ParameterDirection.Output;

                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())//read the table row wise
                {
                    User userInfo = new User();
                    userInfo.userID = Convert.ToInt32(rdr["userid"].ToString());
                    userInfo.fullname = rdr["fullname"].ToString();
                    userInfo.email = rdr["email"].ToString();
                    userInfo.password = rdr["password"].ToString();
                    userInfo.userRegisteredDatetime = Convert.ToDateTime(rdr["userRegisteredDatetime"].ToString());
                    userInfo.phone = rdr["phone"].ToString();
                    userInfo.gender = rdr["gender"].ToString();
                    userInfo.status = rdr["status"].ToString();
                    userInfo.line = rdr["line"].ToString();
                    userInfo.city = rdr["city"].ToString();
                    userInfo.state = rdr["state"].ToString();
                    userInfo.country = rdr["country"].ToString();
                    userInfo.zipCode = rdr["zipCode"].Equals(DBNull.Value) ? 0 : Convert.ToInt32(rdr["zipCode"].ToString());
                    if (!rdr["userDetailsAdditionDatetime"].Equals(DBNull.Value))
                        userInfo.userDetailsAdditionDatetime = Convert.ToDateTime(rdr["userDetailsAdditionDatetime"].ToString());

                    list.Add(userInfo);
                }
                rdr.Close();
                cmd.ExecuteNonQuery();
                result = Convert.ToInt32(cmd.Parameters["@output"].Value);
            }

            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error" + ex.Message.ToString());
                result = -1; //-1 will be interpreted as "error while connecting with the database."
            }
            finally
            {
                con.Close();
            }
            Tuple<int, List<User>> UsersList = new Tuple<int, List<User>>(result, list);

            return UsersList;
        }
        public static int deactivateUser(int userid)
        {
            int result = -2;
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("deactivateUser", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@userid", SqlDbType.Int).Value = userid;
                cmd.Parameters.Add("@output", SqlDbType.Int).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();
                result = Convert.ToInt32(cmd.Parameters["@output"].Value);
            }

            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error" + ex.Message.ToString());
                result = -1; //-1 will be interpreted as "error while connecting with the database."
            }
            finally
            {
                con.Close();
            }
            return result;
        }
        public static int reactivateUser(int userid)
        {
            int result = -2;
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("reactivateUser", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@userid", SqlDbType.Int).Value = userid;
                cmd.Parameters.Add("@output", SqlDbType.Int).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();
                result = Convert.ToInt32(cmd.Parameters["@output"].Value);
            }

            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error" + ex.Message.ToString());
                result = -1; //-1 will be interpreted as "error while connecting with the database."
            }
            finally
            {
                con.Close();
            }
            return result;
        }
        public static Tuple<int, List<User>, List<Request>> searchRequest(string searchterm)
		{
            int result = -3;
            List<User> ulist = new List<User>();
            List<Request> rlist = new List<Request>();
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("searchrequest", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@input", SqlDbType.VarChar, 50).Value = searchterm;
                cmd.Parameters.Add("@output", SqlDbType.Int).Direction = ParameterDirection.Output;

                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())//read the table row wise
                {
                    User userInfo = new User();
                    userInfo.userID = Convert.ToInt32(rdr["userid"].ToString());
                    userInfo.fullname = rdr["fullname"].ToString();
                    userInfo.email = rdr["email"].ToString();
                    userInfo.password = rdr["password"].ToString();
                    userInfo.userRegisteredDatetime = Convert.ToDateTime(rdr["userRegisteredDatetime"].ToString());
                    userInfo.phone = rdr["phone"].ToString();
                    userInfo.gender = rdr["gender"].ToString();
                    userInfo.status = rdr["status"].ToString();
                    userInfo.line = rdr["line"].ToString();
                    userInfo.city = rdr["city"].ToString();
                    userInfo.state = rdr["state"].ToString();
                    userInfo.country = rdr["country"].ToString();
                    userInfo.zipCode = rdr["zipCode"].Equals(DBNull.Value) ? 0 : Convert.ToInt32(rdr["zipCode"].ToString());
                    if (!rdr["userDetailsAdditionDatetime"].Equals(DBNull.Value))
                        userInfo.userDetailsAdditionDatetime = Convert.ToDateTime(rdr["userDetailsAdditionDatetime"].ToString());
                    userInfo.requestedBookDate = Convert.ToDateTime(rdr["requestedBookDate"].ToString());
                    ulist.Add(userInfo);

                    Request req = new Request();
                    req.requestid = Convert.ToInt32(rdr["requestid"].ToString());
                    req.userid = Convert.ToInt32(rdr["userid"].ToString());
                    req.title = rdr["title"].ToString();
                    req.author = rdr["author"].ToString();
                    req.status = rdr["status"].ToString();
                    req.requestedBookDate = Convert.ToDateTime(rdr["requestedBookDate"].ToString());
                    rlist.Add(req);
                }
                rdr.Close();
                cmd.ExecuteNonQuery();
                result = Convert.ToInt32(cmd.Parameters["@output"].Value);
            }

            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error" + ex.Message.ToString());
                result = -1; //-1 will be interpreted as "error while connecting with the database."
            }
            finally
            {
                con.Close();
            }
            Tuple<int, List<User>, List<Request>> UsersRequests = new Tuple<int, List<User>, List<Request>>(result, ulist, rlist);

            return UsersRequests;
        }
        public static Tuple<int, List<Wishlist>, List<User>, List<Book>> getWishlistsForAdmin(string searchterm)
		{
            int result = -3;
            List<Wishlist> wlist = new List<Wishlist>();
            List<Book> blist = new List<Book>();
            List<User> ulist = new List<User>();

            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("getWishlists", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@input", SqlDbType.VarChar, 50).Value = searchterm;
                cmd.Parameters.Add("@output", SqlDbType.Int).Direction = ParameterDirection.Output;

                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())//read the table row wise
                {
                    Wishlist w = new Wishlist
                    {
                        wishlistid = Convert.ToInt32(rdr["wishlistid"].ToString()),
                        userid = Convert.ToInt32(rdr["userid"].ToString()),
                        bookid = Convert.ToInt32(rdr["bookid"].ToString()),
                        wishlistedTime = Convert.ToDateTime(rdr["wishlistedTime"].ToString())
                    };
                    wlist.Add(w);

                    User u = new User
                    {
                        userID = Convert.ToInt32(rdr["userid"].ToString()),
                        fullname = rdr["fullname"].ToString(),
                        email = rdr["email"].ToString(),
                        password = rdr["password"].ToString(),
                        userRegisteredDatetime = Convert.ToDateTime(rdr["userRegisteredDatetime"].ToString()),
                        phone = rdr["phone"].ToString(),
                        gender = rdr["gender"].ToString(),
                        status = rdr["status"].ToString(),
                        line = rdr["line"].ToString(),
                        city = rdr["city"].ToString(),
                        state = rdr["state"].ToString(),
                        country = rdr["country"].ToString(),
                        zipCode = rdr["zipCode"].Equals(DBNull.Value) ? 0 : Convert.ToInt32(rdr["zipCode"].ToString()),
                        wishlistedTime = Convert.ToDateTime(rdr["wishlistedTime"].ToString())
                    };
                    if (!rdr["userDetailsAdditionDatetime"].Equals(DBNull.Value))
                        u.userDetailsAdditionDatetime = Convert.ToDateTime(rdr["userDetailsAdditionDatetime"].ToString());
                    ulist.Add(u);

                    Book bk = new Book
                    {
                        bookID = Convert.ToInt32(rdr["bookid"].ToString()),
                        title = rdr["title"].ToString(),
                        author = rdr["author"].ToString(),
                        isbn = rdr["isbn"].ToString(),
                        language = rdr["language"].ToString(),
                        description = rdr["description"].ToString(),
                        publishingDate = rdr["publishingDate"].ToString(),
                        price = Convert.ToInt32(rdr["price"].ToString()),
                        discount = Convert.ToInt32(rdr["discount"].ToString()),
                        category = rdr["category"].ToString(),
                        wishlistid = Convert.ToInt32(rdr["wishlistid"].ToString())
                    };
                    blist.Add(bk);
                }

                rdr.Close();
                cmd.ExecuteNonQuery();
                result = Convert.ToInt32(cmd.Parameters["@output"].Value);
            }

            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error" + ex.Message.ToString());
                result = -1; //-1 will be interpreted as "error while connecting with the database."
            }
            finally
            {
                con.Close();
            }
            Tuple<int, List<Wishlist>, List<User>, List<Book>> Wishlist = new Tuple<int, List<Wishlist>, List<User>, List<Book>>(result, wlist, ulist, blist) ;
            return Wishlist;
        }
        public static int adminOrderDelivered(string orderid, string userid)
		{
            int result = -4;
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("adminOrderDelivered", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@orderid", SqlDbType.Int).Value = Convert.ToInt32(orderid);
                cmd.Parameters.Add("@userid", SqlDbType.Int).Value = Convert.ToInt32(userid); 
                cmd.Parameters.Add("@output", SqlDbType.Int).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();
                result = Convert.ToInt32(cmd.Parameters["@output"].Value);
            }

            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error" + ex.Message.ToString());
                result = -1; //-1 will be interpreted as "error while connecting with the database."
            }
            finally
            {
                con.Close();
            }
            return result;
        }
        public static int renewPassword(string email, string pass, string cpass)
		{
            if (string.IsNullOrEmpty(email))
            {
                return -4;
            }
            if (string.IsNullOrEmpty(pass))
            {
                return -5;
            }
            if (string.IsNullOrEmpty(cpass))
            {
                return -6;
            }
            if (!Equals(pass, cpass))
            {
                return -7;
            }

            int result = -3;
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("renewPassword", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@email", SqlDbType.VarChar,50).Value = email;
                cmd.Parameters.Add("@pass", SqlDbType.VarChar, 50).Value = pass;
                cmd.Parameters.Add("@cpass", SqlDbType.VarChar, 50).Value = cpass;
                cmd.Parameters.Add("@output", SqlDbType.Int).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();
                result = Convert.ToInt32(cmd.Parameters["@output"].Value);
            }

            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error" + ex.Message.ToString());
                result = -1; //-1 will be interpreted as "error while connecting with the database."
            }
            finally
            {
                con.Close();
            }
            return result;
        }
        public static int fulfilRequest(string requestid)
        {
            int result = -2;
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("fulfilRequest", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@requestid", SqlDbType.Int).Value = Convert.ToInt32(requestid);
                cmd.Parameters.Add("@output", SqlDbType.Int).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();
                result = Convert.ToInt32(cmd.Parameters["@output"].Value);
            }

            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error" + ex.Message.ToString());
                result = -1; //-1 will be interpreted as "error while connecting with the database."
            }
            finally
            {
                con.Close();
            }
            return result;
        }




        //      public static Tuple<int, List<Book>> getBooksList()
        //{
        //          int result = -2;
        //          // Instantiate the connection 
        //          SqlConnection con = new SqlConnection(connectionString);
        //          // Open the connection 
        //          con.Open();
        //          List<Book> list = new List<Book>();
        //          try
        //          {
        //              SqlCommand cmd = new SqlCommand("getBooksList", con);
        //              cmd.CommandType = System.Data.CommandType.StoredProcedure;

        //              //using ExecuteReader because a table will be returned from the called procedure
        //              SqlDataReader rdr = cmd.ExecuteReader();

        //              while (rdr.Read())//read the table row wise
        //              {
        //                  Book bk = new Book();
        //                  bk.bookID = Convert.ToInt32(rdr["bookid"].ToString());
        //                  bk.title = rdr["title"].ToString();
        //                  bk.author = rdr["author"].ToString();
        //                  bk.isbn = rdr["isbn"].ToString();
        //                  bk.language = rdr["language"].ToString();
        //                  bk.publishingDate = rdr["publishingDate"].ToString();
        //                  bk.price = Convert.ToInt32(rdr["price"].ToString());
        //                  bk.discount = Convert.ToInt32(rdr["discount"].ToString());
        //                  bk.category = rdr["category"].ToString();
        //                  bk.numberOfCopies = Convert.ToInt32(rdr["numberOfCopies"].ToString());
        //                  bk.book_count= Convert.ToInt32(rdr["book_count"].ToString());
        //                  list.Add(bk);
        //              }
        //              result = 1;
        //              rdr.Close();
        //          }
        //          catch (SqlException ex)
        //          {
        //              Console.WriteLine("SQL Error" + ex.Message.ToString());
        //              result = -1;
        //          }
        //          finally
        //          {
        //              con.Close();
        //          }

        //          Tuple<int, List<Book>> booksList = new Tuple<int, List<Book>>(result, list);
        //          return booksList;
        //      }

    }
}