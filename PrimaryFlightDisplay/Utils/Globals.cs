namespace PrimaryFlightDisplay
{
    public static class Globals
    {
        public static double PFDWidth { get => 1000.0; }
        public static double PFDHeight { get => 1000.0; }

        public static double PFDOriginX  { get =>  PFDWidth * 0.465; }
        public static double PFDOriginY  { get =>  PFDHeight * 0.486; }
        
        public static double ViewWidth  { get =>  PFDWidth * 0.630; }
        public static double ViewHeight  { get =>  PFDHeight * 0.718; }
        
        public static double ViewOriginX  { get =>  ViewWidth * 0.5; }
        public static double ViewOriginY  { get => ViewHeight * 0.574; }
        
        public static double ViewX  { get =>  PFDOriginX - ViewOriginX; }
        public static double ViewY  { get =>  PFDOriginY - ViewOriginY; }
        
        public static double HeadingWidth  { get =>  ViewWidth; }
        public static double HeadingHeight  { get =>  PFDWidth * 0.127; }
        
        public static double HeadingBigNumberWidth  { get =>  PFDWidth * 0.039; }
        public static double HeadingBigNumberHeight  { get =>  PFDHeight * 0.068; }
        
        public static double HeadingSmallNumberWidth  { get =>  PFDWidth * 0.027; }
        public static double HeadingSmallNumberHeight  { get =>  PFDHeight * 0.045; }
        
        public static double HeadingBigTickHeight  { get =>  PFDHeight * 0.034; }
        public static double HeadingSmallTickHeight  { get =>  PFDHeight * 0.022; }
        
        public static double HeadingIndicatorWidth  { get =>  PFDWidth * 0.060; }
        public static double HeadingIndicatorHeight  { get =>  PFDHeight * 0.045; }
        
        public static int HeadingAngleInterval  { get =>  5; }
        public static int HeadingTickNumber  { get =>  360 / HeadingAngleInterval; }
        public static int HeadingTickInterval  { get =>  2; }
        public static int HeadingNumberInterval  { get =>  3; }

        public static double RollScaleOffset { get => 1.8; }
        public static double PitchScaleOffset { get => 2.083; }

        public static double PitchMin { get => -90.0; }
        public static double PitchMax { get => 90.0; }

        public static double RollMax { get => 90.0; }
        public static double RollMin { get => -90.0; }

        public static double IASMin { get => 0.0; }
        public static double IASMax { get => 500.0; }

        public static double AltitudeMin { get => 0.0; }
        public static double AltitudeMax { get => 100000.0; }

        public static double HeadingMin { get => 0.0; }
        public static double HeadingMax { get => 360.0; }
    }
}