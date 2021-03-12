using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace openbooks.Models
{
    public class Order
    {
        public int orderid;
        public int userid;
        public int bookid;
        public int book_count;
        public int status;
        public DateTime orderDate;
        public DateTime deliveryDate;

        public string ordered_book_title;
    }
}