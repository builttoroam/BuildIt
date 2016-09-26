using System;

namespace BuildIt.AR
{
    public static class WorldHelpers

    {
        private const double _eQuatorialEarthRadius = 6378.1370D;
        private const double _d2r = (Math.PI / 180D);

        private static int HaversineInM(double lat1, double long1, double lat2, double long2)
        {
            return (int)(1000D * HaversineInKM(lat1, long1, lat2, long2));
        }

        private static double HaversineInKM(double lat1, double long1, double lat2, double long2)
        {
            var dlong = (long2 - long1) * _d2r;
            var dlat = (lat2 - lat1) * _d2r;
            var a = Math.Pow(Math.Sin(dlat / 2D), 2D) + Math.Cos(lat1 * _d2r) * Math.Cos(lat2 * _d2r) * Math.Pow(Math.Sin(dlong / 2D), 2D);
            var c = 2D * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1D - a));
            var d = _eQuatorialEarthRadius * c;

            return d;
        }

        public static double DistanceInMetres(this Location p1, Location p2)
        {
            return Math.Abs(HaversineInM(p1.Latitude, p1.Longitude, p2.Latitude, p2.Longitude));
        }

        public static double DistanceInKilometres(this Location p1, Location p2)
        {
            return Math.Abs(HaversineInKM(p1.Latitude, p1.Longitude, p2.Latitude, p2.Longitude));
        }

        public static double Bearing(this Location p1, Location p2)
        {
            return Bearing(p1.Latitude,p1.Longitude, p2.Latitude,p2.Longitude);
        }

        public static double Bearing(
    double lat1, double lon1,
    double lat2, double lon2)
        {
            //const double R = 6371; //earth’s radius (mean radius = 6,371km)
            var dLon = ToRad(lon2 - lon1);
            var dPhi = Math.Log(
                Math.Tan(ToRad(lat2) / 2 + Math.PI / 4) / Math.Tan(ToRad(lat1) / 2 + Math.PI / 4));
            if (Math.Abs(dLon) > Math.PI)
                dLon = dLon > 0 ? -(2 * Math.PI - dLon) : (2 * Math.PI + dLon);
            return Math.Atan2(dLon, dPhi);
        }

        public static double ToRad(this double degrees)
        {
            return degrees * (Math.PI / 180);
        }

        public static double ToDegrees(this double radians)
        {
            return radians * 180 / Math.PI;
        }

        public static double ToBearing(this double radians)
        {
            // convert radians to degrees (as bearing: 0...360)
            return (ToDegrees(radians) + 360) % 360;
        }

        public static ScreenOffset Offset(Vector3 point, Rectangle bounds, Viewport viewport, Matrix projection,
            Matrix view, Matrix currentAttitude)
        {
            // Create a World matrix for the point.
            var world = Matrix.CreateWorld(point, new Vector3(0, 0, -1), new Vector3(0, 1, 0));

            // Use Viewport.Project to project the point from 3D space into screen coordinates.
            var projected = viewport.Project(Vector3.Zero, projection, view, world * currentAttitude);

            if (projected.Z > 1 || projected.Z < 0)
            {
                // If the point is outside of this range, it is behind the camera.
                // So hide the TextBlock for this point.
                return default(ScreenOffset);
            }

            // Create a TranslateTransform to position the TextBlock.
            // Offset by half of the TextBlock's RenderSize to center it on the point.
            var tt = new ScreenOffset
            {
                TranslateX = projected.X - (bounds.Width / 2),
                TranslateY = projected.Y - (bounds.Height / 2),

                Scale = 1 / projected.Z
            };
            return tt;
        }
    }
}