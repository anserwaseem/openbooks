using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using openbooks.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace openbooks.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Changepassword()
        {
            return View();
        }
        public ActionResult changeUserPassword(string currentPassword, string newPassword, string confirmNewPassword)
        {
            if (HttpContext.Session.GetInt32("userID") == null)
            {
                String Data = "No such User ID found in database.";
                return View("Login", (object)Data);
            }
            else
            {

                int ID = Convert.ToInt32(HttpContext.Session.GetInt32("userID").ToString());

                Tuple<int, User> userTuple = CRUD.getUserInfo(ID);
                if (userTuple.Item1 == -1)
                {
                    String Data = "Something went wrong while connecting with the database.";
                    Tuple<string, User> dbErrorInGettingUserInfo = new Tuple<string, User>(Data, userTuple.Item2);
                    return View("Profile", dbErrorInGettingUserInfo);
                }
                int result = CRUD.EditPassword(ID, currentPassword, newPassword, confirmNewPassword);
                if (result == -10)//for admin
                {
                    String Data = "No such User ID found in database.";
                    return View("Settings", (object)Data);
                }
                if (result == -9)//for admin
                {
                    String Data = "Password changed successfuly.";
                    return View("Settings", (object)Data);
                }
                if (result == -8)
                {
                    String Data = "Your Current Password and New Password are same.";
                    return View("Changepassword", (object)Data);
                }
                else if (result == -7)
                {
                    String Data = "Your new password and confirm password aren't equal.";
                    return View("Changepassword", (object)Data);
                }
                else if (result == -6)
                {
                    String Data = "Your Confirm Password is empty.";
                    return View("Changepassword", (object)Data);
                }
                else if (result == -5)
                {
                    String Data = "Your New Password is empty.";
                    return View("Changepassword", (object)Data);
                }
                else if (result == -4)
                {
                    String Data = "Your Current Password is empty.";
                    return View("Changepassword", (object)Data);
                }
                else if (result == -3)
                {
                    String Data = "Password's length must be >= 8 and it must contain atleast 1 number and 1 alphabet.";
                    return View("Changepassword", (object)Data);
                }
                else if (result == -2)
                {
                    String Data = "Your given password is incorrect.";
                    return View("Changepassword", (object)Data);
                }
                else if (result == -1)
                {
                    String Data = "Something went wrong while connecting with the database.";
                    Tuple<string, User> dbErrorInEditingPass = new Tuple<string, User>(Data, userTuple.Item2);
                    return View("Profile", (object)dbErrorInEditingPass);
                }
                else if (result == 0)
                {
                    String Data = "No such User ID found in database.";
                    Tuple<string, User> IDErrorInEditingPass = new Tuple<string, User>(Data, userTuple.Item2);
                    return View("Profile", (object)IDErrorInEditingPass);
                }

                String data = "Profile details updated successfuly.";
                Tuple<string, User> passwordUpdateSuccess = new Tuple<string, User>(data, userTuple.Item2);
                return View("Profile", (object)passwordUpdateSuccess);
            }
        }
        public ActionResult Editprofile()
        {
            return View();
        }
        public ActionResult EditUserProfile(string fullname, string email, string phone, string gender, string line, string city, string state, string country, string zipCode)
        {
            if (HttpContext.Session.GetInt32("userID") == null)
            {
                String Data = "No such User ID found in database.";
                return View("Login", (object)Data);
            }
            else
            {
                int ID = Convert.ToInt32(HttpContext.Session.GetInt32("userID").ToString());

                Tuple<int, User> userTuple = CRUD.getUserInfo(ID);
                if (userTuple.Item1 == -1)
                {
                    String Data = "Something went wrong while connecting with the database.";
                    Tuple<string, User> dbErrorInGettingUserInfo = new Tuple<string, User>(Data, userTuple.Item2);
                    return View("Profile", (object)dbErrorInGettingUserInfo);
                }
                int result = CRUD.EditProfile(ID, fullname, email, phone, gender, line, city, state, country, zipCode);


                if (result == -4)
                {
                    String Data = "No textbox populated by the user.";
                    Tuple<string, User> userErrorInPopulatingFields = new Tuple<string, User>(Data, userTuple.Item2);
                    return View("Profile", (object)userErrorInPopulatingFields);
                }
                if (result == -1)
                {
                    String Data = "Something went wrong while connecting with the database.";
                    Tuple<string, User> dbErrorInEditingProfile = new Tuple<string, User>(Data, userTuple.Item2);
                    return View("Profile", (object)dbErrorInEditingProfile);
                }
                else if (result == 0)
                {
                    String Data = "No such User ID found in database.";
                    Tuple<string, User> IDErrorInEditingProfile = new Tuple<string, User>(Data, userTuple.Item2);
                    return View("Profile", (object)IDErrorInEditingProfile);
                }

                String data = "Profile details updated successfuly.";
                Tuple<string, User> profileUpdateSuccess = new Tuple<string, User>(data, userTuple.Item2);
                //++
                return RedirectToAction("Profile", (object)profileUpdateSuccess);
            }
        }
        public ActionResult Profile()
        {
            if (HttpContext.Session.GetInt32("userID") == null)
            {
                return View("Login");
            }
            else
            {
                int ID = Convert.ToInt32(HttpContext.Session.GetInt32("userID").ToString());
                Tuple<int, User> userTuple = CRUD.getUserInfo(ID);
                Tuple<string, User> dataAndUser;
                if (userTuple.Item1 == -1)
                {
                    String data = "Something went wrong while connecting with the database.";
                    dataAndUser = new Tuple<string, User>(data, userTuple.Item2);

                    return View((object)dataAndUser);
                }
                //else if (userTuple.Item1 == 0)
                //{

                //    dataAndUser = new Tuple<string, User>(data, userTuple.Item2);

                //    return View(dataAndUser);
                //}
                dataAndUser = new Tuple<string, User>("", userTuple.Item2);
                return View(dataAndUser);
            }
        }
        public ActionResult Settings()
        {//every view is returned from an action method

            return View();
        }
        public ActionResult Books()
        {
            return View();
        }
        public ActionResult UsersDetails()
        {
            return View();

        }
        public ActionResult Orderdetails()
        {
            return View();
        }
        public ActionResult getAllOrders(string orderType, string orderDateFrom, string orderDateTo, string orderid)
        {
            if (string.IsNullOrEmpty(orderid))
                orderid = "0";
            Tuple<int, Tuple<List<Order>, List<User>, List<Book>>> AllOrders = CRUD.getAllOrders(orderType, orderDateFrom, orderDateTo, orderid);

            ViewBag.AllOrders = AllOrders;
            return View("Orderdetails");
        }
        public ActionResult Adminpanel()
        {//every view is returned from an action method

            return View();
        }
        public ActionResult Discount()
        {//every view is returned from an action method

            return View();
        }
        public ActionResult Adminsettings()
        {//every view is returned from an action method

            return View();
        }
        public ActionResult Adminshow()
        {//every view is returned from an action method

            return View();
        }
        public ActionResult Adminar()
        {//every view is returned from an action method

            return View();
        }
        public ActionResult Adminlogin()
        {//every view is returned from an action method

            return View();
        }

        public ActionResult register()
        {//every view is returned from an action method

            return View();
        }
        public ActionResult AddNewUser(String fullname, String email, String password)
        {//every view is returned from an action method
            Tuple<int, int> t = CRUD.SignUp(fullname, email, password);

            if (t.Item1 == -1)
            {
                String data = "Something went wrong while connecting with the database.";
                return View("Register", (object)data);
            }
            else if (t.Item1 == 0)
            {
                String data = "User already exists";
                return View("Register", (object)data);
            }

            HttpContext.Session.SetString("email", email);
            HttpContext.Session.SetString("password", password);
            HttpContext.Session.SetInt32("userID", t.Item2);
            //Session["email"] = email;
            //Session["password"] = password;
            //Session["userID"] = t.Item2;
            //int update = CRUD.updateOrderStatus(t.Item2);
            return RedirectToAction("Index");
        }

        public ActionResult Login()
        {
            return View();
        }
        public ActionResult authenticate(String email, String password)
        {
            int result = CRUD.Login(email, password);
            if (result == 3)//if this user is deactivated; deny login access
            {
                String data = "This user is deactivated.";
                return View("Login", (object)data);
            }
            if (result == -1)
            {
                String data = "Something went wrong while connecting with the database.";
                return View("Login", (object)data);
            }
            else if (result == 0)
            {

                String data = "Incorrect Credentials";
                return View("Login", (object)data);
            }
            HttpContext.Session.SetString("email", email);
            HttpContext.Session.SetString("password", password);
            //Session["email"] = email;
            //Session["password"] = password;

            int temp = CRUD.GetUserID(email);
            if (temp == -1)
            {
                String data = "Couldn't get userID. Something went wrong while connecting with the database.";
                return View("Login", (object)data);
            }
            else if (temp == 0)
            {

                String data = "No such User ID found against given credentials in the database.";
                return View("Login", (object)data);
            }
            HttpContext.Session.SetInt32("userID", CRUD.CuserID);
            //Session["userID"] = CRUD.CuserID;


            int update = CRUD.updateOrderStatus();

            if (result == 1)//user
                return RedirectToAction("Index");
            else//admin: result=2
            {
                //return RedirectToAction("Adminpanel");
                return RedirectToAction("Orderdetails");
            }
        }

        public ActionResult Logout()
        {
            int result = CRUD.logout();
            string data = "logged out successfuly.";
            if (result == -1)
                data = "Something went wrong while connecting with the database.";
            if (result == 0)
                data = "No such User ID found in database.";
            return View("Login", (object)data);
        }
        public ActionResult Index()
        {
            bool cat = true, d0 = true, d30 = true, d50 = true, so = true;
            Tuple<int, List<string>> categories = CRUD.getCategories();
            if (categories.Item1 == -1)
                cat = false;

            Tuple<int, List<Book>> newArrivalBooks = CRUD.getSectionBooks(1);
            if (newArrivalBooks.Item1 == -1)
                d0 = false;

            Tuple<int, List<Book>> thirtyPercentBooks = CRUD.getSectionBooks(2);
            if (thirtyPercentBooks.Item1 == -1)
                d30 = false;

            Tuple<int, List<Book>> fiftyPercentBooks = CRUD.getSectionBooks(3);
            if (fiftyPercentBooks.Item1 == -1)
                d50 = false;
            //Tuple<int, List<Book>> soldOutBooks= CRUD.getSectionBooks(5);
            //if (soldOutBooks.Item1 == -1)
            //	so = false;
            if (cat && d0 && d30 && d50 && so)
            {
                Tuple<List<Book>, List<Book>, List<Book>> AllBooks = new Tuple<List<Book>, List<Book>, List<Book>>(newArrivalBooks.Item2, thirtyPercentBooks.Item2, fiftyPercentBooks.Item2);
                Tuple<List<string>, Tuple<List<Book>, List<Book>, List<Book>>> categoriesANDbooks = new Tuple<List<string>, Tuple<List<Book>, List<Book>, List<Book>>>(categories.Item2, AllBooks);
                return View(categoriesANDbooks);
            }

            String Data = "Something went wrong while connecting with the database.";
            return RedirectToAction("Index");
        }
        public ActionResult DeactivateUser(string userid)
        {
            int result = CRUD.deactivateUser(Convert.ToInt32(userid));
            if (result == -1)
            {
                String Data = "Something went wrong while connecting with the database.";
                return RedirectToAction("UsersDetails");
            }
            if (result == 0)
            {
                String Data = "User is already deactivated.";
                return RedirectToAction("UsersDetails");
            }
            string data = "User deactivated.";
            return RedirectToAction("UsersDetails");
        }
        public ActionResult ReactivateUser(string userid)
        {
            int result = CRUD.reactivateUser(Convert.ToInt32(userid));
            if (result == -1)
            {
                String Data = "Something went wrong while connecting with the database.";
                return RedirectToAction("UsersDetails");
            }
            if (result == 0)
            {
                String Data = "User is already active.";
                return RedirectToAction("UsersDetails");
            }
            string data = "User activated.";
            return RedirectToAction("UsersDetails");
        }
        public ActionResult Cart()
        {
            Tuple<int, List<Book>, string> cartItems = CRUD.getCartItems();
            return View(cartItems);
        }
        public ActionResult AddToCart(string bookid)
        {
            int result = CRUD.addToDBCart(Convert.ToInt32(bookid));
            if (result == -1)
            {
                String Data = "Something went wrong while connecting with the database.";
                return RedirectToAction("Index");
            }
            if (result == 0)
            {
                String Data = "Couldn't get userID.";
                return RedirectToAction("Index");
            }
            string data = "Cart updated successfuly.";
            return RedirectToAction("Index");
        }
        public ActionResult RemoveFromCart(string bookid)
        {
            int result = CRUD.removeFromDBCart(Convert.ToInt32(bookid));
            if (result == -1)
            {
                String Data = "Something went wrong while connecting with the database.";
                return RedirectToAction("Cart");
            }
            if (result == 0)
            {
                String Data = "Couldn't get userID.";
                return RedirectToAction("Cart");
            }
            string data = "Cart updated successfuly.";
            return RedirectToAction("Cart");
        }
        public ActionResult getbook(string bookid)
        {
            Tuple<int, Book> requiredBook = CRUD.getThisBook(Convert.ToInt32(bookid));
            Tuple<int, List<string>> categories = CRUD.getCategories();
            int rating = CRUD.getAverageRating(bookid);
            Tuple<Book, List<string>, int> reqBookANDcatANDrating = new Tuple<Book, List<string>, int>(requiredBook.Item2, categories.Item2, rating);

            return View(reqBookANDcatANDrating);
        }
        public ActionResult Viewall(string sectionNo)
        {
            int atw = -1;
            if (Convert.ToInt32(sectionNo) == 5)
                atw = Convert.ToInt32(sectionNo);

            Tuple<int, List<Book>> listOfAllBooks = CRUD.getSectionBooks(Convert.ToInt32(sectionNo));
            if (listOfAllBooks.Item1 == -1)
            {
                String Data = "Something went wrong while connecting with the database.";
            }
            //Tuple<int, List<Rating>> avgRating = CRUD.getAverageRating(userid, bookid);
            Tuple<int, List<string>> categories = CRUD.getCategories();
            Tuple<List<string>, List<Book>, int> allBooksANDcat = new Tuple<List<string>, List<Book>, int>(categories.Item2, listOfAllBooks.Item2, atw);
            return View(allBooksANDcat);
        }
        public ActionResult getBooksForAdmin(string sectionNo)
        {
            Tuple<int, List<Book>> AllBooks = CRUD.getSectionBooks(Convert.ToInt32(sectionNo));
            ViewBag.AllBooks = AllBooks;
            return View("Books");
        }
        public ActionResult Wishlist()
        {
            Tuple<int, List<Book>> wishlistBooks = CRUD.GetWishlist();
            string data = "";
            if (wishlistBooks.Item1 == -1)
                data = "Something went wrong while connecting with the database.";
            Tuple<string, List<Book>> wishlist = new Tuple<string, List<Book>>(data, wishlistBooks.Item2);
            return View(wishlist);
        }
        public ActionResult Category(string categoryType)
        {
            bool cat = true, reqCat = true;
            Tuple<int, List<string>> categories = CRUD.getCategories();
            if (categories.Item1 == -1)
                cat = false;

            Tuple<int, List<Book>> getRequiredCategoryBooks = CRUD.getRequiredCategoryBooks(categoryType);
            if (getRequiredCategoryBooks.Item1 == -1)
                reqCat = false;

            if (cat && reqCat)
            {
                Tuple<List<string>, List<Book>> requiredCategoryBooks = new Tuple<List<string>, List<Book>>(categories.Item2, getRequiredCategoryBooks.Item2);
                return View(requiredCategoryBooks);
            }
            return RedirectToAction("Index");
        }
        public ActionResult Checkout(string data)
        {
            Tuple<int, List<Book>, string> cartItems = CRUD.getCartItems();

            if (!string.IsNullOrEmpty(data))
                cartItems = new Tuple<int, List<Book>, string>(CRUD.getCartItems().Item1, CRUD.getCartItems().Item2, data);
            return View(cartItems);
        }
        public ActionResult ConfirmOrder()
        {
            int confirmation = CRUD.confirmOrder();
            string data = "order placed successfuly";
            if (confirmation == -1)
                data = "Something went wrong while connecting with the database.";
            else if (confirmation == 0)
                data = "There are no items in cart against this user.";

            return RedirectToAction("Checkout", (object)data);
        }
        public ActionResult Orders(string bookid, string stars)
        {
            Tuple<int, Tuple<List<Order>, List<User>, List<Book>>> activeOrders = CRUD.getAllOrders("5", "1900-01-01", "1900-01-01", "0");
            Tuple<int, Tuple<List<Order>, List<User>, List<Book>>> deliveredOrders = CRUD.getAllOrders("6", "1900-01-01", "1900-01-01", "0");
            Tuple<int, Tuple<List<Order>, List<User>, List<Book>>> cancelledOrders = CRUD.getAllOrders("7", "1900-01-01", "1900-01-01", "0");

            ViewBag.activeOrders = activeOrders;
            ViewBag.deliveredOrders = deliveredOrders;
            ViewBag.cancelledOrders = cancelledOrders;

            if (string.IsNullOrEmpty(bookid) || string.IsNullOrEmpty(stars))
                ViewBag.Ratings = -1;
            else
            {
                if (Convert.ToInt32(stars) < 0 || Convert.ToInt32(stars) > 5)
                {
                    ViewBag.Ratings = -1;
                }
                else
                {
                    int id = CRUD.CuserID;
                    int result = CRUD.rateThisBook(id, bookid, stars);
                    ViewBag.Ratings = result;
                }
            }

            return View();
        }
        public ActionResult cancelOrder(string userType, string orderid)
        {
            int result = -3;
            if (userType == "user")
            {
                result = CRUD.cancelOrder(CRUD.CuserID, 0);
                return RedirectToAction("Orders");
            }
            result = CRUD.cancelOrder(0, Convert.ToInt32(orderid));
            return RedirectToAction("Orderdetails");
        }
        public ActionResult rateThisBook(string bookid, string stars)
        {
            int id = CRUD.CuserID;
            if (Convert.ToInt32(stars) < 0 || Convert.ToInt32(stars) > 5)
            {

                return View("Orders");
            }

            int result = CRUD.rateThisBook(id, bookid, stars);
            ViewBag.Ratings = result;

            return View("Orders");
        }
        public ActionResult request(string data)
        {
            Tuple<int, List<Request>> reqBooks = CRUD.getPreviousRequests();
            string Data = "";
            if (string.IsNullOrEmpty(data))
            {
                if (reqBooks.Item1 == -1)
                    Data = "Something went wrong while connecting with the database.";
                else if (reqBooks.Item1 == 0)
                    Data = "no previous requested books against this user";
            }
            else
                Data = data;
            Tuple<string, List<Request>> requests = new Tuple<string, List<Request>>(Data, reqBooks.Item2);
            return View(requests);
        }
        public ActionResult requestThisBook(string title, string author, string isbn)
        {
            int result = CRUD.requestThisBook(title, author, isbn);
            string data = "book successfuly requested";
            if (result == 0)
                data = "no userID found agianst this user.";
            else if (result == -2)
                data = "requested book is already present.";
            return RedirectToAction("request", (object)data);
        }

        public ActionResult search(string searchterm)
        {
            if (searchterm == " " || searchterm == "")
                searchterm = "+=+";

            Tuple<int, List<Book>> search_results = CRUD.getsearchresults(searchterm);
            if (search_results.Item1 == 1)
            {
                Tuple<int, List<Book>> searching_results1 = new Tuple<int, List<Book>>(search_results.Item1, search_results.Item2);
                return View("Results", searching_results1);
            }
            Tuple<int, List<Book>> searching_results2 = new Tuple<int, List<Book>>(search_results.Item1, search_results.Item2);
            return View("Results", searching_results2);

        }
        public ActionResult searchadminbook(string searchterm)
        {
            if (searchterm == " " || searchterm == "")
                searchterm = "+=+";
            Tuple<int, List<Book>> search_results = CRUD.getsearchresults(searchterm);
            ViewBag.SearchedBook = search_results;
            return View("Books");
        }
        public ActionResult adminsearchbook(string searchterm)
        {

            Tuple<int, List<Book>> search_results = CRUD.getsearchresults(searchterm);
            if (search_results.Item1 == 1)
            {
                Tuple<int, List<Book>> searching_results1 = new Tuple<int, List<Book>>(search_results.Item1, search_results.Item2);
                return View("Results", searching_results1);
            }
            Tuple<int, List<Book>> searching_results2 = new Tuple<int, List<Book>>(search_results.Item1, search_results.Item2);
            return View("Results", searching_results2);

        }

        public ActionResult searchuser(string searchterm)
        {
            if (searchterm == " " || searchterm == "")
                searchterm = "+=+";

            Tuple<int, List<User>> search_results = CRUD._searchusers(searchterm);
            //if (search_results.Item1 == 1)
            //{
            //	Tuple<int, List<User>, string> searching_results1 = new Tuple<int, List<User>, string>(search_results.Item1, search_results.Item2, "");
            //	return View("UsersDetails", searching_results1);
            //	//return RedirectToAction("UsersDetails");
            //}
            //Tuple<int, List<User>, string> searching_results2 = new Tuple<int, List<User>, string>(search_results.Item1, search_results.Item2, "");

            ViewBag.SearchedUser = search_results;
            return View("UsersDetails");
            //return RedirectToAction("UsersDetails", (object)searchterm);
        }

        public ActionResult Addbook(string title, string author, string isbn, string language, string description, string price, string discount, string category, string noofcopies, string pdate)
        {
            int result = CRUD.addBook(title, author, isbn, language, description, price, discount, category, noofcopies, pdate);
            string data = "Book added.";
            if (result == -2)
                data = "Invalid input.";
            else if (result == -1)
                data = "Something went wrong while connecting with the database.";
            ViewBag.BookAdded = data;
            return RedirectToAction("Books", (object)data);
        }
        public ActionResult editBookInfo(string bookid, string title, string author, string isbn, string language, string description, string price, string discount, string category, string noofcopies, string pdate)
        {
            int result = CRUD.editBookInfo(bookid, title, author, isbn, language, description, price, discount, category, noofcopies, pdate);
            string data = "Book edited.";
            if (result == -2)
                data = "Invalid input.";
            else if (result == -1)
                data = "Something went wrong while connecting with the database.";
            return RedirectToAction("Books", (object)data);
        }
        public ActionResult removeBook(string bookid)
        {
            int result = CRUD.removeBook(bookid);
            string data = "Book removed.";
            if (result == -2)
                data = "Invalid input.";
            else if (result == -1)
                data = "Something went wrong while connecting with the database.";
            return RedirectToAction("Books", (object)data);
        }
        public ActionResult AddToWishlist(string bookid)
        {
            int result = CRUD.addToWishlist(Convert.ToInt32(bookid));
            if (result == -1)
            {
                String Data = "Something went wrong while connecting with the database.";
                return RedirectToAction("Index");
            }
            if (result == 0)
            {
                String Data = "Couldn't get userID.";
                return RedirectToAction("Index");
            }
            string data = "Wishlist updated successfuly.";
            return RedirectToAction("Index");
        }
        public ActionResult RemoveFromWishlist(string bookid)
        {
            int result = CRUD.removeFromWishlist(Convert.ToInt32(bookid));
            if (result == -1)
            {
                String Data = "Something went wrong while connecting with the database.";
                return RedirectToAction("Wishlist");
            }
            if (result == 0)
            {
                String Data = "not removed from wishlist.";
                return RedirectToAction("Wishlist");
            }
            string data = "Wishlist updated successfuly.";
            return RedirectToAction("Wishlist");
        }

        public ActionResult RequestsDetails()
        {
            return View();
        }
        public ActionResult SearchRequest(string searchterm)
        {
            Tuple<int, List<User>, List<Request>> req = CRUD.searchRequest(searchterm);

            ViewBag.Requests = req;
            return View("RequestsDetails");
        }
        public ActionResult WishlistsDetails()
        {
            return View();
        }
        public ActionResult getWishlistsForAdmin(string searchterm)
        {
            Tuple<int, List<Wishlist>, List<User>, List<Book>> Wishlist = CRUD.getWishlistsForAdmin(searchterm);

            ViewBag.Wishlist = Wishlist;
            return View("WishlistsDetails");
        }
        public ActionResult AdminOrderDelivered(string orderid, string userid)
        {
            int result = CRUD.adminOrderDelivered(orderid, userid);
            return View("Orderdetails");
        }
        public ActionResult ForgetPassword()
        {
            return View();
        }
        public ActionResult RenewPassword(string email, string pass, string cpass)
        {
            int result = CRUD.renewPassword(email, pass, cpass);
            string data = "Password renewed";
            if (result == 1)
            {
                HttpContext.Session.SetString("email", email);
                HttpContext.Session.SetString("password", pass);
                //Session["email"] = email;
                //Session["password"] = pass;
                int id = CRUD.GetUserID(email);
                HttpContext.Session.SetInt32("userID", id);
                //Session["userID"] = id;

                int update = CRUD.updateOrderStatus();
                return RedirectToAction("Index");
            }
            else if (result == 0)
                data = "No such email found.";
            else if (result == -1)
                data = "error while connecting with the database.";
            else if (result == -2)
                data = "password rules violated.";
            else if (result == -4)
                data = "Your Email field is empty.";
            else if (result == -5)
                data = "Your Password field is empty.";
            else if (result == -6)
                data = "Your Confirm Password field is empty.";
            else if (result == -7)
                data = "Your Password and Confirm Password don't match. Try Again.";
            return View("ForgetPassword", (object)data);
        }
        public ActionResult FulfilRequest(string requestid)
        {
            int result = CRUD.fulfilRequest(requestid);
            return View("RequestsDetails");
        }
    }

}