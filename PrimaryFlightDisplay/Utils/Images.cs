using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace PrimaryFlightDisplay
{
    public static class Images
    {
       public static readonly BitmapImage PitchIndicatorImage = (BitmapImage)Application.Current.Resources["PitchIndicatorImage"];
       public static readonly BitmapImage PitchOpacityMaskImage = (BitmapImage)Application.Current.Resources["PitchOpacityMaskImage"];
       public static readonly BitmapImage PitchScaleImage = (BitmapImage)Application.Current.Resources["PitchScaleImage"];
       public static readonly BitmapImage RollIndicatorImage = (BitmapImage)Application.Current.Resources["RollIndicatorImage"];
       public static readonly BitmapImage RollScaleImage = (BitmapImage)Application.Current.Resources["RollScaleImage"];
    }
}
