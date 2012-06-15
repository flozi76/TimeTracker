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

namespace TimeTracker
{
    [Activity(Label = "My Activity")]
    public class CompleteLocationInput : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.CompleteLocationInputForm);

            var buttonSave = this.FindViewById<Button>(Resource.Id.buttonSaveLocation);

            buttonSave.Click += delegate
                                    {
                                        var backToMain = new Intent(this, typeof(MainActivity));
                                        this.StartActivity(backToMain);
                                    };

            // Create your application here
        }
    }
}