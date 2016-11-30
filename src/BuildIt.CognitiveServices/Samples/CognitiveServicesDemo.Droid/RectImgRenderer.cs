//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using Android.Graphics;
//using CognitiveServicesDemo.Droid;
//using CognitiveServicesDemo.Service;
//using Xamarin.Forms;
//using Xamarin.Forms.Platform.Android;

//[assembly: ExportRenderer(typeof(BuildIt.CognitiveServices.Models.RectImg), typeof(RectImgRenderer))]
//namespace CognitiveServicesDemo.Droid
//{
//    public class RectImgRenderer : ImageRenderer
//    {
//        public bool isFirstTime = true;
//        public Android.Views.View SavedView { get; set; }
//        public Canvas Savedcanvas { get; set; } = new Canvas();
//        BuildIt.CognitiveServices.Models.RectImg formsControl;
//        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Image> e)
//        {
//            base.OnElementChanged(e);
//            if (e.NewElement != null)
//            {
//                formsControl = e.NewElement as BuildIt.CognitiveServices.Models.RectImg;
//            }
//        }

//        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
//        {
//            base.OnElementPropertyChanged(sender, e);
//            System.Diagnostics.Debug.WriteLine("property " + e.PropertyName);
//            if (string.Equals(e.PropertyName, "RectLeft", StringComparison.InvariantCultureIgnoreCase))
//            {

//            }
//        }

//        protected override bool DrawChild(Canvas canvas, Android.Views.View child, long drawingTime)
//        {
//            Savedcanvas = canvas;
//            var result = base.DrawChild(Savedcanvas, child, drawingTime);
//            Paint paint = new Paint();
//            //paint.AntiAlias = true;
//            paint.SetStyle(Paint.Style.Stroke);
//            paint.Color = Android.Graphics.Color.Red;
//            int stokeWidth = 2;
//            paint.StrokeWidth = stokeWidth;
//            //if (formsControl.Faces != null)
//            //{
//            //    foreach (Face face in formsControl.Faces)
//            //    {
//            //        FaceRectangle faceRectangle = face.FaceRectangle;
//            //        canvas.DrawRect(
//            //                faceRectangle.Left,
//            //                faceRectangle.Top,
//            //                faceRectangle.Left + faceRectangle.Width,
//            //                faceRectangle.Top + faceRectangle.Height,
//            //                paint);
//            //    }
//            //}
//            //formsControl.RectLeft = 200;
//            //formsControl.RectTop = 200;
//            //formsControl.RectRight = 400;
//            //formsControl.RectBottom = 400;

//            if (isFirstTime)
//            {
//                //formsControl.RectLeft = new List<float>();
//                //formsControl.RectTop = new List<float>();
//                //formsControl.RectRight = new List<float>();
//                //formsControl.RectBottom = new List<float>();
//                //formsControl.RectLeft.Add(0);
//                //formsControl.RectTop.Add(0);
//                //formsControl.RectRight.Add(0);
//                //formsControl.RectBottom.Add(0);
//                isFirstTime = false;
//                formsControl.Rectangle = new List<Rectangle>();
//                formsControl.Rectangle.Add(new Rectangle()
//                {
//                    Left = 0,
//                    Top = 0,
//                    Width = 0,
//                    Height = 0
//                });
//            }
//            else
//            {
//                //for (int i = 0; i < formsControl.RectLeft.Count; i++)
//                //{
//                //    Savedcanvas.DrawRect((float)formsControl.RectLeft[i], (float)formsControl.RectTop[i], (float)formsControl.RectRight[i] + formsControl.RectLeft[i], (float)formsControl.RectBottom[i] + (float)formsControl.RectTop[i], paint);
//                //}
//                foreach (Rectangle t in formsControl.Rectangle)
//                {
//                    Savedcanvas.DrawRect((float)t.Left, (float)t.Top, (float)t.Width+ (float)t.Left, (float)t.Height+ (float)t.Top,paint);
//                }
//                formsControl.Rectangle.Clear();
//                //formsControl.RectLeft.Clear();
//                //formsControl.RectTop.Clear();
//                //formsControl.RectRight.Clear();
//                //formsControl.RectBottom.Clear();
//            }
            

//            paint.Dispose();

//            return result;

//        }
//    }
//}