﻿using GarageSale.Views.ListViews;
using myDataTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using XLabs.Forms.Controls;

namespace GarageSale.Views.Pages
{
	public class commentPage : basePage
	{
		ListView commentListView;

		StackLayout viewStack;
		StackLayout postStack;

		List<comment> comments;

		int item_id;

		public commentPage(int item_id)
		{
			this.item_id = item_id;
			Title = "View Comments";

			commentListView = new commentListView();

			ExtendedEditor commentEditor = new ExtendedEditor
			{
				//BackgroundColor = Color.FromRgb(64, 64, 64),
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Text = "Enter comment.."
			};
			commentEditor.Focused += (s, e) => {
				commentEditor.Text = commentEditor.Text.Replace("Enter comment..", "");
			};


			Button btnAddComment = new Button
			{
				Text = "Log in to add a comment.",
				IsEnabled = false,
				Command = new Command(() =>
				{
					Title = "Add a comment";
					Content = postStack;
				})
			};

			Button btnPostComment = new Button
			{
				Text = "Post Comment",
				Command = new Command(async () =>
				{
					if (string.IsNullOrWhiteSpace(commentEditor.Text))
					{
						await DisplayAlert("Enter Comment", "Enter a comment before posting", "OK");
						return;
					}

					bool sucess = await App.MANAGER.YSSI.AddComment(new comment(-1, item_id, App.CredManager.GetAccountValue("G_id"), App.CredManager.GetAccountValue("G_name"), commentEditor.Text, DateTime.Now));
					if (sucess)
					{
					

						comments.Insert(0,new myDataTypes.comment(-1, item_id, App.CredManager.GetAccountValue("G_id"), App.CredManager.GetAccountValue("G_name"), commentEditor.Text, DateTime.Now));
						commentListView.ItemsSource = comments;
						Title = "View Comments";
						Content = viewStack;
						Navigation.InsertPageBefore(new commentPage(item_id), this);
						Navigation.PopAsync();
					}
					else
						await DisplayAlert("Error adding Comment", "There was an error adding your comment. Please try again", "OK");

				})
			};

			viewStack = new StackLayout
			{
				Padding = new Thickness(25),
				Children = {
					commentListView,
					btnAddComment
				}
			};

			postStack = new StackLayout
			{
				Padding = new Thickness(25),
				Children = {
					commentEditor,
					btnPostComment
				}
			};

			//enabling add comment button if user is logged in
			if (App.CredManager.IsLoggedIn())
			{
				btnAddComment.Text = "Add a comment";
				btnAddComment.IsEnabled = true;
			}
			else {
				btnAddComment.Text = "Log in to add a comment.";
				btnAddComment.IsEnabled = false;
			}

			Content = viewStack;

		}

		void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			commentListView.SelectedItem = null;
		}


		protected async override void OnAppearing()
		{
			base.OnAppearing();
			{


				try
				{
					comments= await App.MANAGER.YSSI.GetComments(item_id);
					if (comments.Count == 0)
						comments.Add(new comment(0, 0, "","", "No Comments to display for this item.",DateTime.Now));
						
					commentListView.ItemsSource = comments;
				}
				catch (Exception e)
				{
					Debug.WriteLine(e.Message);
					Debug.WriteLine(e.StackTrace);
				}
			}
		}
	}
}
