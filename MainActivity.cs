using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Com.Microsoft.Projectoxford.Vision;
using Android.Graphics;
using System.IO;
using System;
using Com.Microsoft.Projectoxford.Vision.Contract;
using GoogleGson;
using XamarinRecogCeleb.Model;
using Newtonsoft.Json;
using Java.Lang;

namespace XamarinRecogCeleb
{
    [Activity(Label = "XamarinRecogCeleb", MainLauncher = true, Icon = "@drawable/icon",Theme ="@style/Theme.AppCompat.Light.NoActionBar")]
    public class MainActivity : AppCompatActivity
    {
        public VisionServiceRestClient visionServiceClient = new VisionServiceRestClient("ac585835001b490a941d07984f938e77");

        private Bitmap mBitmap;
        private ImageView imageView;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
             SetContentView (Resource.Layout.Main);

            mBitmap = BitmapFactory.DecodeResource(Resources, Resource.Drawable.steve);
            imageView = FindViewById<ImageView>(Resource.Id.imageView);
            imageView.SetImageBitmap(mBitmap);

            Button btnProcess = FindViewById<Button>(Resource.Id.btnProcess);

            byte[] bitmapData;
            using (var stream = new MemoryStream())
            {
                mBitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);
                bitmapData = stream.ToArray();
            }

            Stream inputStream = new MemoryStream(bitmapData);

                btnProcess.Click += delegate
                {
                    new RecognizeCelebTask(this).Execute(inputStream);
                };
        }
    }

    internal class RecognizeCelebTask:AsyncTask<Stream,string,string>
    {
        private MainActivity mainActivity;
        private ProgressDialog mDialog = new ProgressDialog(Application.Context);

        public RecognizeCelebTask(MainActivity mainActivity)
        {
            this.mainActivity = mainActivity;
        }

        protected override void OnPreExecute()
        {
            mDialog.Window.SetType(Android.Views.WindowManagerTypes.SystemAlert);
            mDialog.Show();
        }

        protected override void OnProgressUpdate(params string[] values)
        {
            mDialog.SetMessage(values[0]);
        }

        protected override string RunInBackground(params Stream[] @params)
        {
            try
            {
                PublishProgress("Detecting...");
                string model = "celebrities";
                AnalysisInDomainResult result = mainActivity.visionServiceClient.AnalyzeImageInDomain(@params[0], model);

                string strResult = new Gson().ToJson(result);
                return strResult;
            }
            catch(Java.Lang.Exception ex)
            {
                return null;
            }
        }

        protected override void OnPostExecute(string result)
        {
            mDialog.Dismiss();
            AnalysisInDomainModel analysisResult = JsonConvert.DeserializeObject<AnalysisInDomainModel>(result);
            StringBuilder strBuilder = new StringBuilder();
            foreach (var element in analysisResult.result.celebrities)
                strBuilder.Append($"Name: {element.name}\n");
            TextView txtDes = mainActivity.FindViewById<TextView>(Resource.Id.txtDescription);
            txtDes.Text = strBuilder.ToString();

        }
    }
}

