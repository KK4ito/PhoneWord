using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Phoneword
{
    [Activity(Label = "Contacts")]
    public class ContactsActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.SetContentView(Resource.Layout.Contacts);
            var c = Intent.Extras.GetStringArrayList("listC") ?? new string[0];
            // Create your application here
            if (c.Count < 0) 
            {
                Finish();
            }
            
            ListView lst = FindViewById<ListView>(Resource.Id.listContact);
            /*ArrayAdapter<String> arrayAdapter = new ArrayAdapter<string>(
                this,
                
                c);*/
            



        }
    }
}