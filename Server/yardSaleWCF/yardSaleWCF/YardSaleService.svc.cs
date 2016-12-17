﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace yardSaleWCF
{
	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
	// NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
	[ServiceBehavior(IgnoreExtensionDataObject = true)]
	public class YardSaleService : IYardSale
	{
		DatabaseAccess DBACC = new DatabaseAccess();

		public bool AddComment(commentWCF comment)
		{
			return DBACC.AddComment(comment);
		}

		public bool UpdateUser(userWCF user)
		{
			return DBACC.UpdateUser(user);
		}

		public bool UpdateUserActivity(string user_id)
		{
			return DBACC.UpdateUserActivity(user_id);
		}

		public List<itemWCF> GetAllItems()
		{
			return DBACC.GetAllItems();
		}

		public bool AddItem(itemWCF item)
		{
			return DBACC.AddItem(item);
		}

		public bool AddBid(bidWCF bid)
		{
			return DBACC.AddBid(bid);
		}

		public bidWCF GetTopBid(int item_id)
		{
			return DBACC.GetTopBid(item_id);
		}

		public userWCF GetUser(string id)
		{
			return DBACC.GetUser(id);
		}

		public List<commentWCF> GetComments(int item_id)
		{
			return DBACC.GetComments(item_id);
		}

		public bool SellItem(int item_id)
		{
			throw new NotImplementedException();
		}

		public List<itemWCF> GetSearchedItems(string search)
		{
			return DBACC.GetSearchedItems(search);
		}

		public List<userWCF> GetSearchedUsers(string search)
		{
			return DBACC.GetSearchedUsers(search);
		}

		public List<itemWCF> GetItemsAssociatedWithUser(string user_id)
		{
			return DBACC.GetItemsAssociatedWithUser(user_id);
		}

		public List<itemWCF> GetFBLAChapterItems(int chapter_id)
		{
			return DBACC.GetFBLAChapterItems(chapter_id);
		}

		public List<userWCF> GetUsersByChapterStatus(int status, int chapter_id)
		{
			return DBACC.GetUsersByChapterStatus(status, chapter_id);
		}

		public bool SetChapterStatusOfUser(int status, string user_id)
		{
			return DBACC.SetChapterStatusOfUser(status, user_id);
		}

		public List<fblaChapterWCF> GetSearchedChapters(string search)
		{
			return DBACC.GetSearchedChapters(search);
		}

		public fblaChapterWCF GetFBLAChapter(int id)
		{
			return DBACC.GetFBLAChapter(id);
		}

		public bool SetFBLAChapterPicture(int id, byte[] picture)
		{
			return DBACC.SetFBLAChapterPicture(id, picture);
		}

		public int[] GetChapterInfoOfUser(string user_id)
		{
			return DBACC.GetChapterInfoOfUser(user_id);
		}
	}
}
