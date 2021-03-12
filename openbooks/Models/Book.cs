using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace openbooks.Models
{
	public class Book
	{
		public int bookID;
		public string title;
		public string author;
		public string isbn;
		public string language;
		public string description;
		public string publishingDate;
		public int price;
		public int discount;
		public string category;
		public int numberOfCopies;

		public int book_count;
		public int orderid;
		public int wishlistid;
	}
}