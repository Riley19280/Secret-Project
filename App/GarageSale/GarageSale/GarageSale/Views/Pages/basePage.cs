﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GarageSale.Views.Pages
{
	public class basePage : ContentPage
	{
		public basePage()
		{
			Style = (Style)Application.Current.Resources["contentPageStyle"];
			Resources = new ResourceDictionary();
		}
	}
}
