﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;


namespace yardSaleWCF
{
	public class DatabaseAccess
	{
		public bool UpdateUser(userWCF user)
		{
			userWCF u = user;
			int affected = 0;


			using (SqlConnection connection = new SqlConnection(Constants.SQLConnectionString))

			using (SqlCommand cmd = new SqlCommand("IF EXISTS (SELECT * FROM dbo.users WHERE id=@id)    UPDATE dbo.users SET id=@id,name=@name,email=@email,pic_url=@pic_url,last_activity=SYSDATETIME(),lastLogon=SYSDATETIME() WHERE id=@id ELSE   INSERT INTO dbo.users(id,name,email,pic_url,account_created,last_activity,lastLogon) VALUES(@id, @name, @email, @pic_url, SYSDATETIME() ,SYSDATETIME() ,SYSDATETIME())", connection))
			{
				cmd.Parameters.AddWithValue("@id", u.id);
				cmd.Parameters.AddWithValue("@name", u.name);
				cmd.Parameters.AddWithValue("@email", u.email);
				cmd.Parameters.AddWithValue("@pic_url", u.pic_url);

				connection.Open();
				affected = cmd.ExecuteNonQuery();

			}
			return affected > 0 ? true : false;
		}

		public bool UpdateUserActivity(string user_id)
		{
			string id = user_id;
			int affected = 0;

			using (SqlConnection connection = new SqlConnection(Constants.SQLConnectionString))

			using (SqlCommand cmd = new SqlCommand("UPDATE dbo.users SET last_activity = SYSDATETIME() where id =  @id", connection))
			{
				cmd.Parameters.AddWithValue("@id", id);

				connection.Open();
				affected = cmd.ExecuteNonQuery();

			}
			return affected > 0 ? true : false;
		}

		public bool AddComment(commentWCF comment)
		{
			commentWCF c = comment;
			int affected = 0;


			using (SqlConnection connection = new SqlConnection(Constants.SQLConnectionString))

			using (SqlCommand cmd = new SqlCommand("INSERT INTO dbo.comments(item_id,user_id,comment,date_added) VALUES(@item_id,@user_id, @comment ,SYSDATETIME())", connection))
			{
				//cmd.Parameters.AddWithValue("@id", c.id);
				cmd.Parameters.AddWithValue("@item_id", c.item_id);
				cmd.Parameters.AddWithValue("@user_id", c.user_id);
				cmd.Parameters.AddWithValue("@comment", c.comment);

				connection.Open();
				affected = cmd.ExecuteNonQuery();

			}

			return affected > 0 ? true : false;
		}

		public bool AddItem(itemWCF item)
		{
			itemWCF i = item;
			int affected = 0;

			//Stream s = new FileStream("", FileMode.Open);

			//i.picture = ReadFully(s);

			using (SqlConnection connection = new SqlConnection(Constants.SQLConnectionString))

			using (SqlCommand cmd = new SqlCommand("INSERT INTO dbo.items(owner_id,chapter_id,name,description,picture,price,quality,sold,date_added) VALUES(@owner_id,@chapter_id,@name,@description,@picture,@price,@quality,@sold,SYSDATETIME())", connection))
			{
				cmd.Parameters.AddWithValue("@owner_id", i.owner_id);
				cmd.Parameters.AddWithValue("@chapter_id", i.chapter_id);
				cmd.Parameters.AddWithValue("@name", i.name);
				cmd.Parameters.AddWithValue("@description", i.description);
				cmd.Parameters.Add(new SqlParameter("@picture", SqlDbType.VarBinary, i.picture.Length) { Value = (SqlBinary)i.picture });
				cmd.Parameters.AddWithValue("@price", i.price);
				cmd.Parameters.AddWithValue("@quality", i.quality);
				cmd.Parameters.AddWithValue("@sold", i.sold);

				connection.Open();
				affected = cmd.ExecuteNonQuery();

			}
			return affected > 0 ? true : false;
		}

		public bool AddBid(bidWCF bid)
		{
			bidWCF b = bid;
			int affected = 0;

			using (SqlConnection connection = new SqlConnection(Constants.SQLConnectionString))

			using (SqlCommand cmd = new SqlCommand("INSERT INTO dbo.bids(item_id,bidder_id,amount,date_added) VALUES(@item_id,@bidder_id,@amount,SYSDATETIME())", connection))
			{
				cmd.Parameters.AddWithValue("@item_id", b.item_id);
				cmd.Parameters.AddWithValue("@bidder_id", b.bidder_id);
				cmd.Parameters.AddWithValue("@amount", b.amount);

				connection.Open();
				affected = cmd.ExecuteNonQuery();

			}
			return affected > 0 ? true : false;
		}

		public bidWCF GetTopBid(int item_id)
		{

			using (SqlConnection connection = new SqlConnection(Constants.SQLConnectionString))
			{

				using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 * from dbo.bids WHERE item_id = @item_id ORDER BY amount DESC", connection))
				{
					cmd.Parameters.AddWithValue("@item_id", item_id);
					connection.Open();
					using (SqlDataReader reader = cmd.ExecuteReader())
					{
						// Check is the reader has any rows at all before starting to read.
						if (reader.HasRows)
						{
							// Read advances to the next row.
							//TODO: chekc fvalues for null
							while (reader.Read())
							{
								bidWCF b = new bidWCF(
									 reader.GetInt32(reader.GetOrdinal("id")),
									 reader.GetInt32(reader.GetOrdinal("item_id")),
									 reader.GetString(reader.GetOrdinal("bidder_id")),
									 reader.GetFloat(reader.GetOrdinal("amount"))
									);

								return b;
							}
						}
					}
				}
			}
			return null;

		}

		public List<itemWCF> GetAllItems()
		{
			List<itemWCF> items = new List<itemWCF>();

			using (SqlConnection connection = new SqlConnection(Constants.SQLConnectionString))
			{

				using (SqlCommand cmd = new SqlCommand("SELECT TOP 25 * from dbo.items ORDER BY date_added DESC,name", connection))
				{
					connection.Open();
					using (SqlDataReader reader = cmd.ExecuteReader())
					{
						// Check is the reader has any rows at all before starting to read.
						if (reader.HasRows)
						{
							while (reader.Read())
							{

								itemWCF i = new itemWCF(
								 reader.GetInt32(reader.GetOrdinal("id")),
								 reader.GetString(reader.GetOrdinal("owner_id")),
								 reader.GetInt32(reader.GetOrdinal("chapter_id")),
								 reader.GetString(reader.GetOrdinal("name")),
								 reader.GetString(reader.GetOrdinal("description")),
								 (byte[])reader.GetSqlBinary(reader.GetOrdinal("picture")) ?? new byte[0],
								 (float)reader.GetDouble(reader.GetOrdinal("price")),
								 (float)reader.GetDouble(reader.GetOrdinal("quality")),
								 reader.GetInt32(reader.GetOrdinal("sold")),
								 reader.GetDateTime(reader.GetOrdinal("date_added"))
								);

								items.Add(i);
							}
						}
					}
				}
			}
			return items;
		}

		public userWCF GetUser(string id)
		{

			using (SqlConnection connection = new SqlConnection(Constants.SQLConnectionString))
			{

				using (SqlCommand cmd = new SqlCommand("SELECT * from dbo.users WHERE id = @id", connection))
				{
					cmd.Parameters.AddWithValue("@id", id);
					connection.Open();
					using (SqlDataReader reader = cmd.ExecuteReader())
					{
						// Check is the reader has any rows at all before starting to read.
						if (reader.HasRows)
						{

							while (reader.Read())
							{
								userWCF u = new userWCF(
									 reader.GetString(reader.GetOrdinal("id")),
									 reader.GetString(reader.GetOrdinal("name")),
									 reader.GetString(reader.GetOrdinal("email")),
									 reader.GetString(reader.GetOrdinal("pic_url"))
									);

								return u;
							}
						}
					}
				}
			}
			return null;
		}

		public List<commentWCF> GetComments(int item_id)
		{
			List<commentWCF> comments = new List<commentWCF>();

			using (SqlConnection connection = new SqlConnection(Constants.SQLConnectionString))
			{

				using (SqlCommand cmd = new SqlCommand("SELECT * from dbo.comments where item_id = @item_id  ORDER BY date_added DESC", connection))
				{
					cmd.Parameters.AddWithValue("@item_id", item_id);
					connection.Open();
					using (SqlDataReader reader = cmd.ExecuteReader())
					{
						// Check is the reader has any rows at all before starting to read.
						if (reader.HasRows)
						{
							while (reader.Read())
							{
								commentWCF c = new commentWCF(
									 reader.GetInt32(reader.GetOrdinal("id")),
									 reader.GetInt32(reader.GetOrdinal("item_id")),
									 reader.GetString(reader.GetOrdinal("user_id")),
									 ResolveUserName(reader.GetString(reader.GetOrdinal("user_id"))),
									 reader.GetString(reader.GetOrdinal("comment")),
									 reader.GetDateTime(reader.GetOrdinal("date_added"))
							  		 );

								comments.Add(c);
							}
						}
					}
				}
			}
			return comments;
		}

		public bool SellItem(int item_id)
		{
			throw new NotImplementedException();
		}

		public List<itemWCF> GetSearchedItems(string search)
		{
			List<itemWCF> items = new List<itemWCF>();

			using (SqlConnection connection = new SqlConnection(Constants.SQLConnectionString))
			{

				using (SqlCommand cmd = new SqlCommand("SELECT TOP (@limit) * FROM dbo.items WHERE LOWER(name) LIKE @term ORDER BY date_added DESC", connection))
				{
					cmd.Parameters.AddWithValue("@limit", 25);
					cmd.Parameters.AddWithValue("@term", "%" + search.ToLower() + "%");
					connection.Open();
					using (SqlDataReader reader = cmd.ExecuteReader())
					{
						// Check is the reader has any rows at all before starting to read.
						if (reader.HasRows)
						{
							// Read advances to the next row.
							//TODO: chekc fvalues for null
							while (reader.Read())
							{
								itemWCF i = new itemWCF(
									 reader.GetInt32(reader.GetOrdinal("id")),
									 reader.GetString(reader.GetOrdinal("owner_id")),
									 reader.GetInt32(reader.GetOrdinal("chapter_id")),
									 reader.GetString(reader.GetOrdinal("name")),
									 reader.GetString(reader.GetOrdinal("description")),
									 (byte[])reader.GetSqlBinary(reader.GetOrdinal("picture")) ?? new byte[0],
									 (float)reader.GetDouble(reader.GetOrdinal("price")),
									 (float)reader.GetDouble(reader.GetOrdinal("quality")),
									 reader.GetInt32(reader.GetOrdinal("sold")),
									 reader.GetDateTime(reader.GetOrdinal("date_added"))
									);

								items.Add(i);
							}
						}
					}
				}
			}
			return items;
		}

		public List<userWCF> GetSearchedUsers(string search)
		{
			List<userWCF> users = new List<userWCF>();

			using (SqlConnection connection = new SqlConnection(Constants.SQLConnectionString))
			{

				using (SqlCommand cmd = new SqlCommand("SELECT TOP (@limit) * FROM dbo.users WHERE LOWER(name) LIKE @term ORDER BY name,account_created DESC", connection))
				{
					cmd.Parameters.AddWithValue("@limit", 25);
					cmd.Parameters.AddWithValue("@term", "%" + search.ToLower() + "%");
					connection.Open();
					using (SqlDataReader reader = cmd.ExecuteReader())
					{
						// Check is the reader has any rows at all before starting to read.
						if (reader.HasRows)
						{
							// Read advances to the next row.
							//TODO: chekc fvalues for null
							while (reader.Read())
							{
								userWCF u = new userWCF(
								 reader.GetString(reader.GetOrdinal("id")),
								 reader.GetString(reader.GetOrdinal("name")),
								 reader.GetString(reader.GetOrdinal("email")),
								 reader.GetString(reader.GetOrdinal("pic_url"))
								);

								users.Add(u);
							}
						}
					}
				}
			}
			return users;
		}

		public List<itemWCF> GetItemsAssociatedWithUser(string user_id)
		{
			List<itemWCF> items = new List<itemWCF>();

			using (SqlConnection connection = new SqlConnection(Constants.SQLConnectionString))
			{

				using (SqlCommand cmd = new SqlCommand("SELECT TOP (@limit) * FROM dbo.items WHERE owner_id = @owner_id", connection))
				{
					cmd.Parameters.AddWithValue("@limit", 25);
					cmd.Parameters.AddWithValue("@owner_id", user_id);
					connection.Open();
					using (SqlDataReader reader = cmd.ExecuteReader())
					{
						// Check is the reader has any rows at all before starting to read.
						if (reader.HasRows)
						{
							// Read advances to the next row.
							//TODO: chekc fvalues for null
							while (reader.Read())
							{
								itemWCF i = new itemWCF(
									 reader.GetInt32(reader.GetOrdinal("id")),
									 reader.GetString(reader.GetOrdinal("owner_id")),
									 reader.GetInt32(reader.GetOrdinal("chapter_id")),
									 reader.GetString(reader.GetOrdinal("name")),
									 reader.GetString(reader.GetOrdinal("description")),
									 (byte[])reader.GetSqlBinary(reader.GetOrdinal("picture")) ?? new byte[0],
									 (float)reader.GetDouble(reader.GetOrdinal("price")),
									 (float)reader.GetDouble(reader.GetOrdinal("quality")),
									 reader.GetInt32(reader.GetOrdinal("sold")),
									 reader.GetDateTime(reader.GetOrdinal("date_added"))
									);

								items.Add(i);
							}
						}
					}
				}
			}
			return items;
		}

		public List<itemWCF> GetFBLAChapterItems(int chapter_id)
		{
			List<itemWCF> items = new List<itemWCF>();

			using (SqlConnection connection = new SqlConnection(Constants.SQLConnectionString))
			{
				#region getting items based on users id (obfuscated)
				//using (SqlCommand cmd = new SqlCommand("select * from dbo.items where owner_id = (select user_id from member_status where chapter_id = @chapter_id)", connection))
				//{
				//	cmd.Parameters.AddWithValue("@chapter_id", chapter_id);
				//	connection.Open();
				//	using (SqlDataReader reader = cmd.ExecuteReader())
				//	{
				//		// Check is the reader has any rows at all before starting to read.
				//		if (reader.HasRows)
				//		{
				//			// Read advances to the next row.
				//			//TODO: chekc fvalues for null
				//			while (reader.Read())
				//			{
				//				itemWCF i = new itemWCF(
				//					 reader.GetInt32(reader.GetOrdinal("id")),
				//					 reader.GetString(reader.GetOrdinal("owner_id")),
				//					 reader.GetInt32(reader.GetOrdinal("chapter_id")),
				//					 reader.GetString(reader.GetOrdinal("name")),
				//					 reader.GetString(reader.GetOrdinal("description")),
				//					 (byte[])reader.GetSqlBinary(reader.GetOrdinal("picture")) ?? new byte[0],
				//					 (float)reader.GetDouble(reader.GetOrdinal("price")),
				//					 (float)reader.GetDouble(reader.GetOrdinal("quality")),
				//					 reader.GetBoolean(reader.GetOrdinal("sold")),
				//					 reader.GetDateTime(reader.GetOrdinal("date_added"))
				//					);

				//				items.Add(i);
				//			}
				//		}
				//	}
				//}
				#endregion

				#region get items directly associated with chapter
				using (SqlCommand cmd = new SqlCommand("select * from dbo.items where chapter_id = @chapter_id", connection))
				{
					cmd.Parameters.AddWithValue("@chapter_id", chapter_id);
					connection.Open();
					using (SqlDataReader reader = cmd.ExecuteReader())
					{
						// Check is the reader has any rows at all before starting to read.
						if (reader.HasRows)
						{
							// Read advances to the next row.
							//TODO: chekc fvalues for null
							while (reader.Read())
							{
								itemWCF i = new itemWCF(
									 reader.GetInt32(reader.GetOrdinal("id")),
									 reader.GetString(reader.GetOrdinal("owner_id")),
									 reader.GetInt32(reader.GetOrdinal("chapter_id")),
									 reader.GetString(reader.GetOrdinal("name")),
									 reader.GetString(reader.GetOrdinal("description")),
									 (byte[])reader.GetSqlBinary(reader.GetOrdinal("picture")) ?? new byte[0],
									 (float)reader.GetDouble(reader.GetOrdinal("price")),
									 (float)reader.GetDouble(reader.GetOrdinal("quality")),
									 reader.GetInt32(reader.GetOrdinal("sold")),
									 reader.GetDateTime(reader.GetOrdinal("date_added"))
									);

								items.Add(i);
							}
						}
					}
				}
				#endregion
			}
			return items;
		}

		public List<userWCF> GetUsersByChapterStatus(int status, int chapter_id)
		{
			List<userWCF> users = new List<userWCF>();
			using (SqlConnection connection = new SqlConnection(Constants.SQLConnectionString))
			{
				SqlCommand cmd = null;

				if (status < 0)//used if we want all users in a chapter 0 is pending member, 1 is member
					cmd = new SqlCommand("select * from dbo.users where id in (select user_id from dbo.member_status where status >= 1 and chapter_id = @chapter_id)", connection);
				else if (status == 0)
					cmd = new SqlCommand("select * from dbo.users where id in (select user_id from yardsale.dbo.member_status where status = @status and chapter_id = @chapter_id)", connection);
				else
					cmd = new SqlCommand("select * from dbo.users where id in (select user_id from yardsale.dbo.member_status where status >=@status and chapter_id = @chapter_id)", connection);
				using (cmd)
				{
					cmd.Parameters.AddWithValue("@status", status);
					cmd.Parameters.AddWithValue("@chapter_id", chapter_id);
					connection.Open();
					using (SqlDataReader reader = cmd.ExecuteReader())
					{
						// Check is the reader has any rows at all before starting to read.
						if (reader.HasRows)
						{

							while (reader.Read())
							{
								userWCF u = new userWCF(
									 reader.GetString(reader.GetOrdinal("id")),
									 reader.GetString(reader.GetOrdinal("name")),
									 reader.GetString(reader.GetOrdinal("email")),
									 reader.GetString(reader.GetOrdinal("pic_url"))
									);
								users.Add(u);

							}
						}
					}
				}
			}
			return users;
		}

		public bool SetChapterStatusOfUser(int status, string user_id)
		{
			int affected = 0;

			SqlConnection connection = new SqlConnection(Constants.SQLConnectionString);
			SqlCommand cmd;

			if (status < 0)
			{
				cmd = new SqlCommand("delete from dbo.member_status where user_id = @user_id", connection);
				cmd.Parameters.AddWithValue("@user_id", user_id);
			}
			else if (status == 0)
			{
				cmd = new SqlCommand("INSERT INTO dbo.member_status (user_id, status) VALUES (@user_id,@status)", connection);
				cmd.Parameters.AddWithValue("@status", status);
				cmd.Parameters.AddWithValue("@user_id", user_id);

			}
			else
			{
				cmd = new SqlCommand("UPDATE dbo.member_status SET status = @status where user_id = @user_id", connection);
				cmd.Parameters.AddWithValue("@status", status);
				cmd.Parameters.AddWithValue("@user_id", user_id);
			}

			connection.Open();
			affected = cmd.ExecuteNonQuery();
			connection.Close();
			return affected > 0 ? true : false;
		}

		public List<fblaChapterWCF> GetSearchedChapters(string search)
		{
			List<fblaChapterWCF> chapters = new List<fblaChapterWCF>();

			using (SqlConnection connection = new SqlConnection(Constants.SQLConnectionString))
			{

				using (SqlCommand cmd = new SqlCommand("SELECT TOP (@limit) * FROM yardsale.dbo.fbla_chapters WHERE LOWER(name) LIKE @term OR LOWER(school) LIKE @term OR LOWER(city) LIKE @term OR LOWER(state) LIKE @term ORDER BY name,school,city,state", connection))
				{
					cmd.Parameters.AddWithValue("@limit", 10);
					cmd.Parameters.AddWithValue("@term", "%" + search + "%");
					connection.Open();
					using (SqlDataReader reader = cmd.ExecuteReader())
					{
						// Check is the reader has any rows at all before starting to read.
						if (reader.HasRows)
						{
							// Read advances to the next row.
							//TODO: chekc fvalues for null
							while (reader.Read())
							{
								fblaChapterWCF f = new fblaChapterWCF(
								 reader.GetInt32(reader.GetOrdinal("id")),
								 reader.GetString(reader.GetOrdinal("name")),
								 reader.GetString(reader.GetOrdinal("state")),
								 reader.GetString(reader.GetOrdinal("city")),
								 reader.GetString(reader.GetOrdinal("school")),
								 reader.GetString(reader.GetOrdinal("contact_email")),
								 reader.GetString(reader.GetOrdinal("payment_email")),
								 (byte[])reader.GetSqlBinary(reader.GetOrdinal("picture")) ?? new byte[0]
								);

								chapters.Add(f);
							}
						}
					}
				}
			}
			return chapters;
		}

		public fblaChapterWCF GetFBLAChapter(int id)
		{
			fblaChapterWCF chapter = null;

			using (SqlConnection connection = new SqlConnection(Constants.SQLConnectionString))
			{

				using (SqlCommand cmd = new SqlCommand("SELECT * FROM dbo.fbla_chapters WHERE id =@id", connection))
				{
					cmd.Parameters.AddWithValue("@id", id);

					connection.Open();
					using (SqlDataReader reader = cmd.ExecuteReader())
					{
						// Check is the reader has any rows at all before starting to read.
						if (reader.HasRows)
						{
							// Read advances to the next row.
							//TODO: chekc fvalues for null
							while (reader.Read())
							{
								fblaChapterWCF f = new fblaChapterWCF(
								 reader.GetInt32(reader.GetOrdinal("id")),
								 reader.GetString(reader.GetOrdinal("name")),
								 reader.GetString(reader.GetOrdinal("state")),
								 reader.GetString(reader.GetOrdinal("city")),
								 reader.GetString(reader.GetOrdinal("school")),
								 reader.GetString(reader.GetOrdinal("contact_email")),
								 reader.GetString(reader.GetOrdinal("payment_email")),
								 (byte[])reader.GetSqlBinary(reader.GetOrdinal("picture")) ?? new byte[0]
								);

								chapter = f;
							}
						}
					}
				}
			}
			return chapter;
		}

		public bool SetFBLAChapterPicture(int id, byte[] picture)
		{

			int affected = 0;

			using (SqlConnection connection = new SqlConnection(Constants.SQLConnectionString))

			using (SqlCommand cmd = new SqlCommand("UPDATE dbo.fbla_chapters SET picture = @picture WHERE id = @id", connection))
			{
				cmd.Parameters.Add(new SqlParameter("@picture", SqlDbType.VarBinary, picture.Length) { Value = (SqlBinary)picture });
				cmd.Parameters.AddWithValue("@id", id);

				connection.Open();
				affected = cmd.ExecuteNonQuery();

			}
			return affected > 0 ? true : false;
		}

		public int[] GetChapterInfoOfUser(string user_id)
		{
			//select status from dbo.member_status where user_id = @user_id
			using (SqlConnection connection = new SqlConnection(Constants.SQLConnectionString))
			{

				using (SqlCommand cmd = new SqlCommand("select chapter_id,status from dbo.member_status where user_id = @user_id", connection))
				{
					cmd.Parameters.AddWithValue("@user_id", user_id);
					connection.Open();
					using (SqlDataReader reader = cmd.ExecuteReader())
					{
						// Check is the reader has any rows at all before starting to read.
						if (reader.HasRows)
						{
							// Read advances to the next row.
							//TODO: chekc fvalues for null
							while (reader.Read())
							{
								return new int[] { reader.GetInt32(reader.GetOrdinal("chapter_id")), reader.GetInt32(reader.GetOrdinal("status")) };

							}
						}
						else
						{
							return new int[] { -1 };
						}
					}
				}
			}
			return new int[] { -1 };
		}

		public itemWCF GetItem(int item_id)
		{

			using (SqlConnection connection = new SqlConnection(Constants.SQLConnectionString))
			{

				using (SqlCommand cmd = new SqlCommand("SELECT * from dbo.items where id = @item_id", connection))
				{
					cmd.Parameters.AddWithValue("@item_id", item_id);

					connection.Open();
					using (SqlDataReader reader = cmd.ExecuteReader())
					{
						// Check is the reader has any rows at all before starting to read.
						if (reader.HasRows)
						{
							while (reader.Read())
							{

								itemWCF i = new itemWCF(
								 reader.GetInt32(reader.GetOrdinal("id")),
								 reader.GetString(reader.GetOrdinal("owner_id")),
								 reader.GetInt32(reader.GetOrdinal("chapter_id")),
								 reader.GetString(reader.GetOrdinal("name")),
								 reader.GetString(reader.GetOrdinal("description")),
								 (byte[])reader.GetSqlBinary(reader.GetOrdinal("picture")) ?? new byte[0],
								 (float)reader.GetDouble(reader.GetOrdinal("price")),
								 (float)reader.GetDouble(reader.GetOrdinal("quality")),
								 reader.GetInt32(reader.GetOrdinal("sold")),
								 reader.GetDateTime(reader.GetOrdinal("date_added"))
								);

								return i;
							}
						}
					}
				}
			}
			return null;
		}

		public bool setItemSellStatus(int item_id, int status)
		{
			int affected = 0;

			using (SqlConnection connection = new SqlConnection(Constants.SQLConnectionString))

			using (SqlCommand cmd = new SqlCommand("UPDATE dbo.items set sold = @status where id = @item_id", connection))
			{
				cmd.Parameters.AddWithValue("@item_id", item_id);
				cmd.Parameters.AddWithValue("@status", status);

				connection.Open();
				affected = cmd.ExecuteNonQuery();

			}
			return affected > 0 ? true : false;
		}

		public bool DeleteItem(int item_id)
		{
			int affected = 0;
			using (SqlConnection connection = new SqlConnection(Constants.SQLConnectionString))
			{
				using (SqlCommand cmd = new SqlCommand("delete from dbo.items where id = @item_id", connection))
				{
					cmd.Parameters.AddWithValue("@item_id", item_id);
					connection.Open();
					affected = cmd.ExecuteNonQuery();

				}
			}
			return affected > 0 ? true : false;
		}


		internal string ResolveUserName(string id)
		{
			using (SqlConnection connection = new SqlConnection(Constants.SQLConnectionString))
			{

				using (SqlCommand cmd = new SqlCommand("select name from dbo.users where id = @user_id", connection))
				{
					cmd.Parameters.AddWithValue("user_id", id);
					connection.Open();
					string s = (string)cmd.ExecuteScalar();
					return s;

				}
			}
		}

		byte[] ReadFully(Stream input)
		{
			byte[] buffer = new byte[16 * 1024];
			using (MemoryStream ms = new MemoryStream())
			{
				int read;
				while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
				{
					ms.Write(buffer, 0, read);
				}
				return ms.ToArray();
			}
		}
	}
}