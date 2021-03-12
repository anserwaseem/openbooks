using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace openbooks.Models
{
	public class User
	{
		public int userID;
		public string fullname;
		public string email;
		public string password;
		public DateTime userRegisteredDatetime;

		public string phone;
		public string gender;
		public int type;
		public string status;
		public string line;
		public string city;
		public string state;
		public string country;
		public int zipCode;
		public DateTime userDetailsAdditionDatetime;

		public DateTime orderDate;
		public DateTime requestedBookDate;
		public DateTime wishlistedTime;
	}
}