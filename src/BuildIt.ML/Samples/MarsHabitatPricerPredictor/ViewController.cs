using BuildIt.ML.Platforms.Ios;
using CoreML;
using Foundation;
using System;
using System.Collections.Generic;
using UIKit;

namespace MarsHabitatPricerPredictor
{
    public partial class ViewController : UIViewController
    {
        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override async void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Perform any additional setup after loading the view, typically from a nib.
            var classifier = new CustomVisionClassifier();
            var outputFeatures = new List<Feature>();
            await classifier.InitAsync("MarsHabitatPricer", outputFeatures);
            var inputFeatures = new List<Feature>
            {
                new DoubleFeatureValue() { Name = "solarPanels", Value = 2.0 },
                new DoubleFeatureValue() { Name = "greenhouses", Value = 2.0 },
                new DoubleFeatureValue() { Name = "size", Value = 2000.0 }
            };

            var outputFeatureValues = await classifier.ClassifyAsync(inputFeatures);
            var price = outputFeatureValues.GetFeatureValue("price").DoubleValue;
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }
    }
}