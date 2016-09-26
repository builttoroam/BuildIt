namespace BuildIt.AR
{
    public class ScreenOffset
    {
        public double TranslateX { get; set; }
        public double TranslateY { get; set; }
        public double Scale { get; set; }

        public bool IsEmpty => Equals(default(ScreenOffset));
    }
}