using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ButterflyGame
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        //create butterfly
        private Buttefly butterfly;

        //flowers
        private List<Flower> flowers = new List<Flower>();

        //audio
        private MediaElement mediaElement;

        //game loop timer
        private DispatcherTimer timer;
        // which keys are pressed
        private bool UpPressed; // true false?
        private bool RightPressed;
        private bool LeftPressed;


        public MainPage()
        {
            this.InitializeComponent();
            // try open 800x600 window
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            ApplicationView.PreferredLaunchViewSize = new Size (800, 600);

            //key listeners
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
            Window.Current.CoreWindow.KeyUp += CoreWindow_KeyUp;
            Window.Current.CoreWindow.PointerPressed += CoreWindow_PointerPressed;

            InitAudio();
            CreateButterfly();
            StartGame();

        }

        //load audio from assets
        private async void InitAudio()
        {
            mediaElement = new MediaElement();
            mediaElement.AutoPlay = false;
            StorageFolder folder =
                await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Assets");
            StorageFile file = await folder.GetFileAsync("tada.wav");
            var stream = await file.OpenAsync(FileAccessMode.Read);
            mediaElement.SetSource(stream, file.ContentType);

        }


        private void CoreWindow_PointerPressed(CoreWindow sender, PointerEventArgs args)
        {   //create a new flower
            Flower flower = new Flower();
            flower.LocationX = args.CurrentPoint.Position.X - flower.Width / 2;
            flower.LocationY = args.CurrentPoint.Position.Y - flower.Height / 2;
            // add to canvas
            MyCanvas.Children.Add(flower);
            flower.SetLocation();

            // add flower to list
            flowers.Add(flower);
        }

        private void CoreWindow_KeyUp(CoreWindow sender, KeyEventArgs args)
        {
            switch (args.VirtualKey)
            {
                case VirtualKey.Up:
                    UpPressed = false;
                    break;
                case VirtualKey.Left:
                    LeftPressed = false;
                    break;
                case VirtualKey.Right:
                    RightPressed = false;
                    break;
                default:
                    break;

            }
        }

        //tapahtuman käsittelijä napin painallukselle
        private void CoreWindow_KeyDown(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        {
            switch (args.VirtualKey)
            {
                case VirtualKey.Up:
                    UpPressed = true;
                    break;
                case VirtualKey.Left:
                    LeftPressed = true;
                    break;
                case VirtualKey.Right:
                    RightPressed = true;
                    break;
                default:
                    break;

            }
        }

        // create butterfly
        private void CreateButterfly()
        {
            butterfly = new Buttefly
            {
                LocationX = MyCanvas.Width / 2 - 75,
                LocationY = MyCanvas.Height / 2 - 66
            };

            //add to canvas
            MyCanvas.Children.Add(butterfly);
            //show in right location
            butterfly.SetLocation();
        }
        //start game loop
        private void StartGame()
        {
            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = new TimeSpan(0,0,0,0,1000/60); //60 kertaa sekunnissa
            timer.Start(); // stop!

        }
        // Game loop
        private void Timer_Tick(object sender, object e)
        {
            //move butterfly
           if (UpPressed) butterfly.Move();

            // rotate
           
            if (LeftPressed) butterfly.Rotate(-1);
            if (RightPressed) butterfly.Rotate(1);
            //update butterfly location
            butterfly.SetLocation();

            //collision butterfly with flowers
            CheckCollision();
        }

        //check collision with flowers and butterfly
        private void CheckCollision()
            {
            foreach (Flower flower in flowers)
            {
                //get rectangle
                Rect r1 = new Rect(butterfly.LocationX, butterfly.LocationY, butterfly.ActualWidth, butterfly.ActualHeight); //
                Rect r2 = new Rect(flower.LocationX, flower.LocationY, flower.ActualWidth, flower.ActualHeight);
                // törmääkö ne?
                r1.Intersect(r2);
                //jos ei tyhjä, eli ne törmää
                if (!r1.IsEmpty)
                {
                    //play audio
                    mediaElement.Play();
                    //poista kukka
                    MyCanvas.Children.Remove(flower);
                    flowers.Remove(flower);

                    break; 
                }
            }
            }
    }



}
