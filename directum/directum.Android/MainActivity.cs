using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Widget;
using Android.OS;
using System.Threading;
using System.Threading.Tasks;

/*3. Фоновый расчет
Необходимо реализовать приложение, которое будет выполнять какую-либо математическую операцию в фоне (когда приложение свернуто) с сохранением результата во внутреннее хранилище приложения.
Например, инкрементация какого-либо значения каждую секунду. С возможностью просмотра значения в UI и возможностью его сброса.
Особые требования
· Максимальное время работы приложения в фоне – 10 минут. Нет необходимости обеспечивать более длительную работу.
· Выполнение фоновой операции должно начинаться сразу же после сворачивания приложения.
· Выполнение фоновой операции должно сразу же приостанавливаться при открытии приложения.
· Текущее изменение данных должно сохранятся в хранилище сразу же, а не переносится туда при открытии/сворачивании приложения.*/

namespace directum.Droid
{
    [Activity(Label = "directum", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private static volatile bool isWork = true;
        private static int x = 0;
        private static int SleepsCount = 0;
        private static readonly int MAX_TIME_TO_WORK = 600; // 600 sec = 10 min

        private ManualResetEvent _manualEvent = new ManualResetEvent(true);
        //Run
        private async void Run()
        {
            await Task.Run(() =>
            {
                if (SleepsCount >= MAX_TIME_TO_WORK)
                    return;

                while (isWork)
                {
                    Thread.Sleep(1000);
                    x++;
                    SleepsCount++;
                    FindViewById<TextView>(Resource.Id.textView1).Text = x.ToString();
                }
                return;
            });
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Main);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());

            //reset
            FindViewById<Android.Widget.Button>(Resource.Id.button1).Click += delegate {
                //Если приложение проработало свои 10 минут, запустить заново
                if (SleepsCount >= MAX_TIME_TO_WORK)
                { 
                    SleepsCount = 0;
                    Run();
                }
                
                //Обнуляем счетчик
                x = 0;
                FindViewById<TextView>(Resource.Id.textView1).Text = x.ToString();
            };
            Console.WriteLine("OnCreate");
        }

        //· Выполнение фоновой операции должно сразу же приостанавливаться при открытии приложения.
        protected override void OnResume()
        {
            base.OnResume();
            isWork = false;
            Console.WriteLine("OnResume");
        }

        //· Выполнение фоновой операции должно начинаться сразу же после сворачивания приложения.
        //· Текущее изменение данных должно сохранятся в хранилище сразу же, а не переносится туда при открытии/сворачивании приложения.
        protected override void OnStop()
        {
            base.OnStop();
            isWork = true;
            Run();
            //убиваем, если работает дольше 10 минут
            Console.WriteLine("OnStop");
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}