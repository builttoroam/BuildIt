// Currency.cs
//
// This file was automatically generated and should not be edited.
//

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using CoreML;
using CoreVideo;
using Foundation;

namespace BuildIt.ML.Sample.iOS {
	/// <summary>
	/// Model Prediction Input Type
	/// </summary>
	public class CurrencyInput : NSObject, IMLFeatureProvider
	{
		static readonly NSSet<NSString> featureNames = new NSSet<NSString> (
			new NSString ("data")
		);

		CVPixelBuffer data;

		/// <summary>
		///  as color (kCVPixelFormatType_32BGRA) image buffer, 227 pizels wide by 227 pixels high
		/// </summary>
		/// <value></value>
		public CVPixelBuffer Data {
			get { return data; }
			set {
				if (value == null)
					throw new ArgumentNullException (nameof (value));

				data = value;
			}
		}

		public NSSet<NSString> FeatureNames {
			get { return featureNames; }
		}

		public MLFeatureValue GetFeatureValue (string featureName)
		{
			switch (featureName) {
			case "data":
				return MLFeatureValue.Create (Data);
			default:
				return null;
			}
		}

		public CurrencyInput (CVPixelBuffer data)
		{
			if (data == null)
				throw new ArgumentNullException (nameof (data));

			Data = data;
		}
	}

	/// <summary>
	/// Model Prediction Output Type
	/// </summary>
	public class CurrencyOutput : NSObject, IMLFeatureProvider
	{
		static readonly NSSet<NSString> featureNames = new NSSet<NSString> (
			new NSString ("loss"), new NSString ("classLabel")
		);

		NSDictionary<NSObject, NSNumber> loss;
		string classLabel;

		/// <summary>
		///  as dictionary of strings to doubles
		/// </summary>
		/// <value></value>
		public NSDictionary<NSObject, NSNumber> Loss {
			get { return loss; }
			set {
				if (value == null)
					throw new ArgumentNullException (nameof (value));

				loss = value;
			}
		}

		/// <summary>
		///  as string value
		/// </summary>
		/// <value></value>
		public string ClassLabel {
			get { return classLabel; }
			set {
				if (value == null)
					throw new ArgumentNullException (nameof (value));

				classLabel = value;
			}
		}

		public NSSet<NSString> FeatureNames {
			get { return featureNames; }
		}

		public MLFeatureValue GetFeatureValue (string featureName)
		{
			MLFeatureValue value;
			NSError err;

			switch (featureName) {
			case "loss":
				value = MLFeatureValue.Create (Loss, out err);
				if (err != null)
					err.Dispose ();
				return value;
			case "classLabel":
				return MLFeatureValue.Create (ClassLabel);
			default:
				return null;
			}
		}

		public CurrencyOutput (NSDictionary<NSObject, NSNumber> loss, string classLabel)
		{
			if (loss == null)
				throw new ArgumentNullException (nameof (loss));

			if (classLabel == null)
				throw new ArgumentNullException (nameof (classLabel));

			Loss = loss;
			ClassLabel = classLabel;
		}
	}

	/// <summary>
	/// Class for model loading and prediction
	/// </summary>
	public class Currency : NSObject
	{
		readonly MLModel model;

		public Currency ()
		{
			var url = NSBundle.MainBundle.GetUrlForResource ("Currency", "mlmodelc");
			NSError err;

			model = MLModel.Create (url, out err);
		}

		Currency (MLModel model)
		{
			this.model = model;
		}

		public static Currency Create (NSUrl url, out NSError error)
		{
			if (url == null)
				throw new ArgumentNullException (nameof (url));

			var model = MLModel.Create (url, out error);

			if (model == null)
				return null;

			return new Currency (model);
		}

		/// <summary>
		/// Make a prediction using the standard interface
		/// </summary>
		/// <param name="input">an instance of CurrencyInput to predict from</param>
		/// <param name="error">If an error occurs, upon return contains an NSError object that describes the problem.</param>
		public CurrencyOutput GetPrediction (CurrencyInput input, out NSError error)
		{
			var prediction = model.GetPrediction (input, out error);

			if (prediction == null)
				return null;

			var lossValue = prediction.GetFeatureValue ("loss").DictionaryValue;
			var classLabelValue = prediction.GetFeatureValue ("classLabel").StringValue;

			return new CurrencyOutput (lossValue, classLabelValue);
		}

		/// <summary>
		/// Make a prediction using the convenience interface
		/// </summary>
		/// <param name="data"> as color (kCVPixelFormatType_32BGRA) image buffer, 227 pizels wide by 227 pixels high</param>
		/// <param name="error">If an error occurs, upon return contains an NSError object that describes the problem.</param>
		public CurrencyOutput GetPrediction (CVPixelBuffer data, out NSError error)
		{
			var input = new CurrencyInput (data);

			return GetPrediction (input, out error);
		}
	}
}
