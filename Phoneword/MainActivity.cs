using System;
using System.Text;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections;
using System.Collections.Generic;
using Xamarin.Contacts;
using System.Linq;

namespace Phoneword
{
    [Activity(Label = "Phone Word", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        static readonly List<string> phoneNumbers = new List<string>();
        public static ArrayList lstContacts = new ArrayList();
        
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Our code will go here

            // Get our UI controls from the loaded layout:
            EditText phoneNumberText = FindViewById<EditText>(Resource.Id.PhoneNumberText);
            Button translateButton = FindViewById<Button>(Resource.Id.TranslateButton);
            Button callButton = FindViewById<Button>(Resource.Id.CallButton);
            Button callHistoryButton = FindViewById<Button>(Resource.Id.CallHistoryButton);
            Button loadContacts = FindViewById<Button>(Resource.Id.LoadContacts);

            loadContacts.Click += (object sender, EventArgs e) =>
            {
                var book = new AddressBook(this);
                book.RequestPermission().ContinueWith(t =>
                {
                    if (!t.Result)
                    {
                        var deniedPermission = new AlertDialog.Builder(this);
                        deniedPermission.SetMessage("Permission denied");
                        deniedPermission.Show();
                        return;
                    }
                });

                var temp = book
                            .Where(a => !String.IsNullOrEmpty(a.FirstName) && a.Phones.Count() > 0)
                            .OrderBy(x => x.FirstName);

                List<string> lstC = new List<string>();
                foreach(Contact c in temp)
                {
                    if(!(lstC.Any(str => str.Contains($"{c.FirstName},{c.LastName}"))))
                        lstC.Add($"{c.FirstName},{c.LastName},{c.Phones.First().Number}");
                }
                                
                var loadedSucces = new AlertDialog.Builder(this);
                loadedSucces.SetMessage($"{temp.Count()} contacts loaded!");
                loadedSucces.SetNegativeButton("OK", delegate { });

                loadedSucces.SetNeutralButton("Show me", delegate
                {
                    var detailsIntent = new Intent(this, typeof(ContactsActivity));
                    detailsIntent.PutStringArrayListExtra("lstC", lstC);
                    StartActivity(detailsIntent);
                });
                loadedSucces.Show();
            };


            // Disable the "Call" button
            callButton.Enabled = false;

            // Add code to translate number
            string translatedNumber = string.Empty;

            translateButton.Click += (object sender, EventArgs e) =>
            {
                // Translate user's alphanumeric phone number to numeric
                translatedNumber = Core.PhonewordTranslator.ToNumber(phoneNumberText.Text);
                if (String.IsNullOrWhiteSpace(translatedNumber))
                {
                    callButton.Text = "Call";
                    callButton.Enabled = false;
                }
                else
                {
                    callButton.Text = "Call " + translatedNumber;
                    callButton.Enabled = true;
                }
            };
            
            callHistoryButton.Click += (sender, e) =>
            {
                var intent = new Intent(this, typeof(CallHistoryActivity));
                intent.PutStringArrayListExtra("phone_numbers", phoneNumbers);
                StartActivity(intent);
            };
            callButton.Click += (object sender, EventArgs e) =>
            {
                // On "Call" button click, try to dial phone number.
                var callDialog = new AlertDialog.Builder(this);
                callDialog.SetMessage("Call " + translatedNumber + "?");
                callDialog.SetNeutralButton("Call", delegate {
                    // Create intent to dial phone
                    var callIntent = new Intent(Intent.ActionCall);
                    callIntent.SetData(Android.Net.Uri.Parse("tel:" + translatedNumber));
                    StartActivity(callIntent);
                });
                callDialog.SetNegativeButton("Cancel", delegate { });


                callDialog.SetNeutralButton("Call", delegate
                {
                    // add dialed number to list of called numbers.
                    phoneNumbers.Add(translatedNumber);
                    // enable the Call History button
                    callHistoryButton.Enabled = true;
                    // Create intent to dial phone
                    var callIntent = new Intent(Intent.ActionCall);
                    callIntent.SetData(Android.Net.Uri.Parse("tel:" + translatedNumber));
                    StartActivity(callIntent);
                });
                // Show the alert dialog to the user and wait for response.
                callDialog.Show();
                
                
            };
        }
    }
}