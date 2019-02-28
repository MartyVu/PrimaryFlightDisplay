using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PrimaryFlightDisplay
{
    public class MainController : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            Draw();
        }

        public ObservableCollection<UIElement> PFDLayers { get; }

        #region Parameters
        private double _pitch;
        public double Pitch
        {
            get => _pitch;
            set
            {
                _pitch = Math.Round(value, 2).Clamp(Globals.PitchMin, Globals.PitchMax);
                OnPropertyChanged();
            }
        }
        private double _roll;
        public double Roll
        {
            get { return _roll; }
            set
            {
                _roll = Math.Round(value, 2).Clamp(Globals.RollMin, Globals.RollMax);
                OnPropertyChanged();
            }
        }
        private double _ias;
        public double IAS
        {
            get { return _ias; }
            set
            {
                _ias = Math.Round(value, 2).Clamp(Globals.IASMin, Globals.IASMax);
                OnPropertyChanged();
            }
        }
        private double _altitude;
        public double Altitude
        {
            get { return _altitude; }
            set
            {
                _altitude = Math.Round(value, 2).Clamp(Globals.AltitudeMin, Globals.AltitudeMax);
                OnPropertyChanged();
            }
        }
        private double _heading;
        public double Heading
        {
            get { return _heading; }
            set
            {
                _heading = Math.Round(value, 1);
                OnPropertyChanged();
            }
        }
        #endregion

        #region UI
        bool _skyGround;
        public bool SkyGround
        {
            get { return _skyGround; }
            set
            {
                _skyGround = value;
                OnPropertyChanged();
            }
        }
        bool _pitchClip;
        public bool PitchClip
        {
            get { return _pitchClip; }
            set
            {
                _pitchClip = value;
                OnPropertyChanged();
            }
        }
        bool _opacityMask;
        public bool OpacityMask
        {
            get { return _opacityMask; }
            set
            {
                _opacityMask = value;
                OnPropertyChanged();
            }
        }
        bool _pitchScale;
        public bool PitchScale
        {
            get { return _pitchScale; }
            set
            {
                _pitchScale = value;
                OnPropertyChanged();
            }
        }
        bool _rollScale;
        public bool RollScale
        {
            get { return _rollScale; }
            set
            {
                _rollScale = value;
                OnPropertyChanged();
            }
        }
        bool _indicator;
        public bool Indicator
        {
            get { return _indicator; }
            set
            {
                _indicator = value;
                OnPropertyChanged();
            }
        }
        #endregion

        public MainController()
        {
            PFDLayers = new ObservableCollection<UIElement>();

            _skyGround = true;
            _pitchClip = true;
            _pitchScale = true;
            _rollScale = true;
            _indicator = true;
        }

        public void Draw()
        {
            PFDLayers.Clear();

            DrawView();
            DrawIAS();
            DrawAltitude();
            DrawHeading();
        }

        private void DrawView()
        {
            var View = new Canvas
            {
                Width = Globals.ViewWidth,
                Height = Globals.ViewHeight,
                ClipToBounds = true
            };

            if (SkyGround)
                DrawSkyGround(View);
            if (PitchScale)
                DrawPitchScale(View);
            if (RollScale)
                DrawRollScale(View);
            if (Indicator)
                DrawIndicator(View);
            if (OpacityMask)
                DrawOpacityMask(View);

            TransformGroup viewTransform = new TransformGroup();
            viewTransform.Children.Add(new TranslateTransform(Globals.PFDWidth * 0.465 - Globals.ViewWidth * 0.5, Globals.PFDHeight * 0.486 - Globals.ViewHeight * 0.5));
            View.RenderTransform = viewTransform;

            PFDLayers.Add(View);
        }
        private void DrawSkyGround(Canvas View)
        {
            double pitch = Pitch.Clamp(-13.5, 13.5);

            Rectangle sky = new Rectangle
            {
                Fill = new SolidColorBrush(new Color() { R = 0, G = 115, B = 166, A = 255 }),
                Width = Globals.ViewWidth * 3.5,
                Height = Globals.ViewHeight * 3.5
            };
            Canvas.SetLeft(sky, Globals.ViewOriginX - sky.Width * 0.5);
            Canvas.SetTop(sky, Globals.ViewOriginY - sky.Height);

            TransformGroup skyTransform = new TransformGroup();
            skyTransform.Children.Add(new RotateTransform(Roll + Globals.RollScaleOffset * Math.Sin(Math.PI * Roll / 90.0), sky.Width * 0.5, sky.Height - Globals.ViewHeight * Globals.PitchScaleOffset * pitch / Globals.PitchMax));
            skyTransform.Children.Add(new TranslateTransform(0, Globals.ViewHeight * Globals.PitchScaleOffset * pitch / Globals.PitchMax));
            sky.RenderTransform = skyTransform;

            Rectangle ground = new Rectangle
            {
                Fill = new SolidColorBrush(new Color() { R = 166, G = 115, B = 0, A = 255 }),
                Width = Globals.ViewWidth * 3.5,
                Height = Globals.ViewHeight * 3.5
            };
            Canvas.SetLeft(ground, Globals.ViewOriginX - ground.Width * 0.5);
            Canvas.SetTop(ground, Globals.ViewOriginY);

            TransformGroup groundTransform = new TransformGroup();
            groundTransform.Children.Add(new RotateTransform(Roll + Globals.RollScaleOffset * Math.Sin(Math.PI * Roll / 90.0), ground.Width * 0.5, 0 - Globals.ViewHeight * Globals.PitchScaleOffset * pitch / Globals.PitchMax));
            groundTransform.Children.Add(new TranslateTransform(0, Globals.ViewHeight * Globals.PitchScaleOffset * pitch / Globals.PitchMax));
            ground.RenderTransform = groundTransform;

            Rectangle line = new Rectangle
            {
                Fill = Brushes.White,
                Width = Globals.ViewWidth * 3.5,
                Height = Globals.ViewHeight * 0.01
            };
            Canvas.SetLeft(line, Globals.ViewOriginX - line.Width * 0.5);
            Canvas.SetTop(line, Globals.ViewOriginY - line.Height * 0.5);

            TransformGroup lineTransform = new TransformGroup();
            lineTransform.Children.Add(new RotateTransform(Roll + Globals.RollScaleOffset * Math.Sin(Math.PI * Roll / 90.0), line.Width * 0.5, line.Height * 0.5 - Globals.ViewHeight * Globals.PitchScaleOffset * pitch / Globals.PitchMax));
            lineTransform.Children.Add(new TranslateTransform(0, Globals.ViewHeight * Globals.PitchScaleOffset * pitch / Globals.PitchMax));
            line.RenderTransform = lineTransform;

            View.Children.Add(sky);
            View.Children.Add(ground);
            View.Children.Add(line);
        }
        private void DrawPitchScale(Canvas View)
        {
            Image pitchScale = new Image
            {
                Source = Images.PitchScaleImage,
                Width = Images.PitchScaleImage.PixelWidth * Globals.ViewWidth / Images.PitchScaleImage.PixelWidth,
                Height = Images.PitchScaleImage.PixelHeight * Globals.ViewWidth / Images.PitchScaleImage.PixelWidth
            };

            TransformGroup pitchScaleTransform = new TransformGroup();
            pitchScaleTransform.Children.Add(new RotateTransform(Roll + Globals.RollScaleOffset * Math.Sin(Math.PI * Roll / 90.0), pitchScale.Width * 0.5, pitchScale.Height * 0.5 - Globals.ViewHeight * Globals.PitchScaleOffset * Pitch / Globals.PitchMax));
            pitchScaleTransform.Children.Add(new TranslateTransform((Globals.ViewWidth - pitchScale.Width) * 0.5, Globals.ViewOriginY - pitchScale.Height * 0.5));
            pitchScaleTransform.Children.Add(new TranslateTransform(0, Globals.ViewHeight * Globals.PitchScaleOffset * Pitch / Globals.PitchMax));
            pitchScale.RenderTransform = pitchScaleTransform;

            if (PitchClip)
            {
                Image pitchMask = new Image
                {
                    Source = Images.PitchOpacityMaskImage,
                    Width = Globals.ViewWidth,
                    Height = Globals.ViewHeight
                };
                Canvas.SetLeft(pitchMask, 0);
                Canvas.SetTop(pitchMask, 0);

                VisualBrush opacityMask = new VisualBrush
                {
                    Visual = pitchMask,
                    Stretch = Stretch.None,
                    Transform = new TranslateTransform(0, Globals.ViewHeight * 0.5 - Globals.ViewOriginY - Globals.ViewHeight * Globals.PitchScaleOffset * Pitch / Globals.PitchMax)
                };

                pitchScale.OpacityMask = opacityMask;
            }

            View.Children.Add(pitchScale);
        }
        private void DrawRollScale(Canvas View)
        {
            Image rollScale = new Image
            {
                Source = Images.RollScaleImage,
                Width = Globals.ViewWidth,
                Height = Globals.ViewHeight
            };

            View.Children.Add(rollScale);
        }
        private void DrawIndicator(Canvas View)
        {
            Image pitchIndicator = new Image
            {
                Source = Images.PitchIndicatorImage,
                Width = Globals.ViewWidth,
                Height = Globals.ViewHeight
            };

            Image rollIndicator = new Image
            {
                Source = Images.RollIndicatorImage,
                Width = Images.RollIndicatorImage.PixelWidth * Globals.ViewWidth / 1000.0,
                Height = Images.RollIndicatorImage.PixelHeight * Globals.ViewHeight / 1140.0
            };

            TransformGroup rollIndicatorTransform = new TransformGroup();
            rollIndicatorTransform.Children.Add(new TranslateTransform(Globals.ViewOriginX - rollIndicator.Width * 0.5, Globals.ViewOriginY - rollIndicator.Height));
            rollIndicatorTransform.Children.Add(new RotateTransform(Roll + Globals.RollScaleOffset * Math.Sin(Math.PI * Roll / 90.0), Globals.ViewOriginX, Globals.ViewOriginY));
            rollIndicator.RenderTransform = rollIndicatorTransform;

            View.Children.Add(pitchIndicator);
            View.Children.Add(rollIndicator);
        }
        private void DrawOpacityMask(Canvas View)
        {
            Image pitchMask = new Image
            {
                Source = Images.PitchOpacityMaskImage,
                Opacity = 0.5,
                Width = Globals.ViewWidth,
                Height = Globals.ViewHeight
            };

            pitchMask.RenderTransform = new RotateTransform(Roll + Globals.RollScaleOffset * Math.Sin(Math.PI * Roll / 90.0), Globals.ViewOriginX, Globals.ViewOriginY);

            View.Children.Add(pitchMask);
        }

        private void DrawAltitude()
        {
            Rectangle panel = new Rectangle
            {
                Fill = new SolidColorBrush(new Color() { R = 64, G = 64, B = 64, A = 255 }),
                Width = Globals.PFDWidth * 0.213,
                Height = Globals.PFDHeight
            };

            TransformGroup panelTransform = new TransformGroup();
            panelTransform.Children.Add(new TranslateTransform(Globals.PFDWidth - panel.Width, Globals.PFDHeight * 0.5 - panel.Height * 0.5));
            panel.RenderTransform = panelTransform;

            PFDLayers.Add(panel);

            double[] xOffset = { 0.14, 0.32, 0.54, 0.74, 0.88 };
            int[] digitValue = new int[5];



            for (int i = 0; i < 5; i++)
                digitValue[i] = (int)(Math.Round(Altitude) / Math.Pow(10, 4 - i) % 10);

            double d = Math.Floor(Altitude / 100) % 2 == 0 ? Math.Floor(Altitude / 100) * 100 : (Math.Floor(Altitude / 100) - 1) * 100;



            double decimalValue = Altitude - Math.Floor(Altitude);


            double scaleY = Globals.PFDHeight * 0.045;
            double scaleX = Globals.PFDWidth * 0.028 * 2;

            double offsetY = Globals.PFDHeight * 0.486 - Globals.ViewHeight * 0.5 + Globals.ViewOriginY;
            int count = 0;

            if (Altitude < Globals.AltitudeMax)
            {
                while (offsetY > -200)
                {
                    int number = (int)Math.Abs(d + count * 100);
                    if (number > Globals.AltitudeMax)
                        break;

                    double y = (((digitValue[2] * 100 + digitValue[3] * 10 + digitValue[4]) % 200) + decimalValue) / 200 * 2 * Globals.PFDHeight * 0.104;

                    if (count == 0)
                    {
                        count++;
                        offsetY -= Globals.PFDHeight * 0.104;
                        continue;
                    }
                    double startX = 0.18;

                    if (count % 2 == 0)
                    {
                        if (d + count * 100 < 0)
                        {
                            GeometryGroup geometry = new GeometryGroup
                            {
                                Children = Geometries.Minus,
                                Transform = new ScaleTransform(scaleX, scaleY)
                            };

                            Path digit = new Path
                            {
                                Data = geometry,
                                Stroke = Brushes.White,
                                StrokeThickness = 3,
                                StrokeStartLineCap = PenLineCap.Round,
                                StrokeEndLineCap = PenLineCap.Round
                            };

                            TransformGroup transform = new TransformGroup();
                            transform.Children.Add(new TranslateTransform(panel.Width * startX - scaleX * 0.5 * 0.5, offsetY - scaleY * 0.5 + y));
                            transform.Children.Add(panel.RenderTransform.Clone());
                            digit.RenderTransform = transform;

                            PFDLayers.Add(digit);
                            startX += 0.158;
                        }
                        if (number == 0)
                            startX += 2 * 0.158;
                        for (int i = 0; i < number.ToString().Length; i++)
                        {
                            int value = (int)(number / Math.Pow(10, number.ToString().Length - 1 - i) % 10);

                            GeometryGroup geometry = new GeometryGroup
                            {
                                Children = Geometries.Numbers[value],
                                Transform = new ScaleTransform(scaleX, scaleY)
                            };

                            Path digit = new Path
                            {
                                Data = geometry,
                                Stroke = Brushes.White,
                                StrokeThickness = 3,
                                StrokeStartLineCap = PenLineCap.Round,
                                StrokeEndLineCap = PenLineCap.Round
                            };

                            TransformGroup transform = new TransformGroup();
                            transform.Children.Add(new TranslateTransform(panel.Width * (startX + 0.158 * i) - scaleX * 0.5 * 0.5, offsetY - scaleY * 0.5 + y));
                            transform.Children.Add(panel.RenderTransform.Clone());
                            digit.RenderTransform = transform;

                            PFDLayers.Add(digit);
                        }
                    }
                    else
                    {
                        Path tick = new Path
                        {
                            Data = new LineGeometry(new Point(0.0, 0.0), new Point(panel.Width * 0.153, 0.0)),
                            Stroke = Brushes.White,
                            StrokeThickness = 6,
                            StrokeStartLineCap = PenLineCap.Flat,
                            StrokeEndLineCap = PenLineCap.Round
                        };
                        tick.Width = tick.Data.Bounds.Width;

                        TransformGroup tickTransform = new TransformGroup();
                        tickTransform.Children.Add(panel.RenderTransform.Clone());
                        tickTransform.Children.Add(new TranslateTransform(0, offsetY + y));
                        tick.RenderTransform = tickTransform;

                        PFDLayers.Add(tick);
                    }

                    count++;
                    offsetY -= Globals.PFDHeight * 0.104;
                }
            }

            offsetY = Globals.PFDHeight * 0.486 - Globals.ViewHeight * 0.5 + Globals.ViewOriginY;
            count = 0;
            while (offsetY < Globals.PFDHeight + 100)
            {
                int number = (int)Math.Abs(d - count * 100);

                double y = (((digitValue[2] * 100 + digitValue[3] * 10 + digitValue[4]) % 200) + decimalValue) / 200 * 2 * Globals.PFDHeight * 0.104;

                double startX = 0.18;

                if (count % 2 == 0)
                {
                    if (d - count * 100 < 0)
                    {
                        GeometryGroup geometry = new GeometryGroup
                        {
                            Children = Geometries.Minus,
                            Transform = new ScaleTransform(scaleX, scaleY)
                        };

                        Path digit = new Path
                        {
                            Data = geometry,
                            Stroke = Brushes.White,
                            StrokeThickness = 3,
                            StrokeStartLineCap = PenLineCap.Round,
                            StrokeEndLineCap = PenLineCap.Round
                        };

                        TransformGroup transform = new TransformGroup();
                        transform.Children.Add(new TranslateTransform(panel.Width * startX - scaleX * 0.5 * 0.5, offsetY - scaleY * 0.5 + y));
                        transform.Children.Add(panel.RenderTransform.Clone());
                        digit.RenderTransform = transform;

                        PFDLayers.Add(digit);

                        startX += 0.158;
                    }
                    if (number == 0)
                        startX += 2 * 0.158;
                    for (int i = 0; i < number.ToString().Length; i++)
                    {
                        GeometryGroup geometry = new GeometryGroup();

                        int value = (int)(number / Math.Pow(10, number.ToString().Length - 1 - i) % 10);

                        geometry.Children = Geometries.Numbers[value];
                        geometry.Transform = new ScaleTransform(scaleX, scaleY);

                        Path digit = new Path
                        {
                            Data = geometry,
                            Stroke = Brushes.White,
                            StrokeThickness = 3,
                            StrokeStartLineCap = PenLineCap.Round,
                            StrokeEndLineCap = PenLineCap.Round
                        };

                        TransformGroup transform = new TransformGroup();
                        transform.Children.Add(panel.RenderTransform.Clone());
                        transform.Children.Add(new TranslateTransform(panel.Width * (startX + 0.158 * i) - scaleX * 0.5 * 0.5, offsetY + y - scaleY * 0.5));
                        digit.RenderTransform = transform;

                        PFDLayers.Add(digit);
                    }
                }
                else
                {
                    Path tick = new Path
                    {
                        Data = new LineGeometry(new Point(0.0, 0.0), new Point(panel.Width * 0.153, 0.0)),
                        Stroke = Brushes.White,
                        StrokeThickness = 6,
                        StrokeStartLineCap = PenLineCap.Flat,
                        StrokeEndLineCap = PenLineCap.Round
                    };
                    tick.Width = tick.Data.Bounds.Width;

                    TransformGroup tickTransform = new TransformGroup();
                    tickTransform.Children.Add(panel.RenderTransform.Clone());
                    tickTransform.Children.Add(new TranslateTransform(0, offsetY + y));
                    tick.RenderTransform = tickTransform;

                    PFDLayers.Add(tick);
                }

                count++;
                offsetY += Globals.PFDHeight * 0.104;
            }

            Rectangle altitudeBorderBrush = new Rectangle
            {
                Stroke = Brushes.White,
                StrokeThickness = 3
            };
            altitudeBorderBrush.Width = Globals.PFDWidth * 0.287 + altitudeBorderBrush.StrokeThickness;
            altitudeBorderBrush.Height = Globals.PFDHeight * 0.140 + 2 * altitudeBorderBrush.StrokeThickness;

            Border altitudeBorder = new Border
            {
                Background = Brushes.Black,
                BorderBrush = new VisualBrush(altitudeBorderBrush),
                BorderThickness = new Thickness(altitudeBorderBrush.StrokeThickness, altitudeBorderBrush.StrokeThickness, 0, altitudeBorderBrush.StrokeThickness),
                Width = altitudeBorderBrush.Width,
                Height = altitudeBorderBrush.Height
            };

            TransformGroup altitudeBorderTransform = new TransformGroup();
            altitudeBorderTransform.Children.Add(new TranslateTransform(Globals.PFDWidth - altitudeBorder.Width, Globals.PFDHeight * 0.486 - Globals.ViewHeight * 0.5 + Globals.ViewOriginY - altitudeBorder.Height * 0.5));
            altitudeBorder.RenderTransform = altitudeBorderTransform;

            PFDLayers.Add(altitudeBorder);

            Canvas altitudeCanvas = new Canvas
            {
                Width = altitudeBorder.Width - altitudeBorderBrush.StrokeThickness,
                Height = altitudeBorder.Height - 2 * altitudeBorderBrush.StrokeThickness,
                ClipToBounds = true
            };

            GeometryGroup altitudeClipGeometry = new GeometryGroup();
            altitudeClipGeometry.Children.Add(new RectangleGeometry(new Rect(new Point(altitudeCanvas.Width * 0.05, altitudeCanvas.Height * 0.22), new Point(altitudeCanvas.Width * 0.61, altitudeCanvas.Height * 0.78))));
            altitudeClipGeometry.Children.Add(new RectangleGeometry(new Rect(new Point(altitudeCanvas.Width * 0.61, 0), new Point(altitudeCanvas.Width, altitudeCanvas.Height))));
            altitudeCanvas.Clip = altitudeClipGeometry;

            altitudeBorder.Child = altitudeCanvas;

            double bigScale = altitudeCanvas.Height * 0.50;
            double smallScale = altitudeCanvas.Height * 0.32;

            for (int i = 0; i < 5; i++)
                digitValue[i] = (int)(Altitude / Math.Pow(10, 4 - i) % 10);

            for (int i = 4; i >= 0; i--)
            {
                GeometryGroup geometry = new GeometryGroup();
                GeometryGroup aboveGeometry = new GeometryGroup();
                GeometryGroup belowGeometry = new GeometryGroup();

                // 0000X
                if (i == 4)
                {
                    geometry.Children = Geometries.Numbers[0];
                    aboveGeometry.Children = Geometries.Numbers[0];
                    belowGeometry.Children = Geometries.Numbers[0];
                }
                // 000X0
                else if (i == 3)
                {
                    geometry.Children = Geometries.Numbers[digitValue[i] % 2 == 0 ? digitValue[i] : (digitValue[i] + 1) % 10];
                    aboveGeometry.Children = Geometries.Numbers[digitValue[i] % 2 == 0 ? (digitValue[i] + 2) % 10 : (digitValue[i] + 3) % 10];
                    belowGeometry.Children = Geometries.Numbers[digitValue[i] % 2 == 0 ? (digitValue[i] - 2 + 10) % 10 : (digitValue[i] - 1 + 10) % 10];
                }
                // XXX00
                else
                {
                    geometry.Children = Geometries.Numbers[digitValue[i]];
                    aboveGeometry.Children = Geometries.Numbers[(digitValue[i] + 1) % 10];
                    belowGeometry.Children = Geometries.Numbers[(digitValue[i] - 1 + 10) % 10];
                }

                double scale = i < 2 ? bigScale : smallScale;

                geometry.Transform = new ScaleTransform(scale, scale);
                aboveGeometry.Transform = geometry.Transform.Clone();
                belowGeometry.Transform = geometry.Transform.Clone();

                Path digit = new Path
                {
                    Data = geometry,
                    Stroke = Brushes.White,
                    StrokeEndLineCap = PenLineCap.Round,
                    StrokeThickness = 3,
                    StrokeStartLineCap = PenLineCap.Round
                };


                Path aboveDigit = new Path
                {
                    Data = aboveGeometry,
                    Stroke = Brushes.White,
                    StrokeThickness = 3,
                    StrokeStartLineCap = PenLineCap.Round,
                    StrokeEndLineCap = PenLineCap.Round
                };

                Path belowDigit = new Path
                {
                    Data = belowGeometry,
                    Stroke = Brushes.White,
                    StrokeEndLineCap = PenLineCap.Round,
                    StrokeThickness = 3,
                    StrokeStartLineCap = PenLineCap.Round
                };
                belowDigit.StrokeEndLineCap = PenLineCap.Round;

                double yOffset = 0;

                // 000XX
                if (i > 2)
                {
                    if (digitValue[3] % 2 == 0)
                        yOffset = ((digitValue[3] * 10 + digitValue[4]) % 20) / 20.0 * bigScale * 1.20;
                    else
                        yOffset = ((digitValue[3] * 10 + digitValue[4]) % 20 - 20) / 20.0 * bigScale * 1.20;
                }
                // 00X00
                else if (i == 2)
                {
                    if (digitValue[3] >= 8)
                        yOffset = ((digitValue[3] * 10 + digitValue[4]) % 20) / 20.0 * bigScale * 1.20;
                }
                // 0X000
                else if (i == 1)
                {
                    if (digitValue[2] >= 9 && digitValue[3] >= 8)
                        yOffset = ((digitValue[3] * 10 + digitValue[4]) % 20) / 20.0 * bigScale * 1.20;
                }
                // X0000
                else
                {
                    if (digitValue[1] >= 9 && digitValue[2] >= 9 && digitValue[3] >= 8)
                        yOffset = ((digitValue[3] * 10 + digitValue[4]) % 20) / 20.0 * bigScale * 1.20;
                }

                TransformGroup transform = new TransformGroup();
                transform.Children.Add(new TranslateTransform(altitudeCanvas.Width * xOffset[i] - scale * 0.5 * 0.5, altitudeCanvas.Height * 0.5 - scale * 0.5 + yOffset));

                TransformGroup aboveTransform = transform.Clone();
                aboveTransform.Children.Add(new TranslateTransform(0, -bigScale * 1.20));

                TransformGroup belowTransform = transform.Clone();
                belowTransform.Children.Add(new TranslateTransform(0, bigScale * 1.20));

                digit.RenderTransform = transform;
                aboveDigit.RenderTransform = aboveTransform;
                belowDigit.RenderTransform = belowTransform;

                altitudeCanvas.Children.Add(digit);
                altitudeCanvas.Children.Add(aboveDigit);
                altitudeCanvas.Children.Add(belowDigit);
            }
        }

        private void DrawIAS()
        {
            // Rectangle that contain the scale
            Rectangle background = new Rectangle
            {
                Width = Globals.PFDWidth * 0.142,
                Height = Globals.PFDHeight,
                Fill = new SolidColorBrush(new Color() { R = 64, G = 64, B = 64, A = 255 })
            };
            // Position the left and center of the background at the left and center of the full canvas
            TransformGroup tapeBackgroundTransform = new TransformGroup();
            tapeBackgroundTransform.Children.Add(new TranslateTransform(0, Globals.PFDHeight * 0.5 - background.Height * 0.5));
            background.RenderTransform = tapeBackgroundTransform;
            PFDLayers.Add(background);

            double[] xOffset = { 0.14, 0.45, 0.75 };
            double[] offsetX = { 0.20, 0.46, 0.72 };
            int[] digitValue = new int[3];

            for (int i = 0; i < 3; i++)
                digitValue[i] = (int)(IAS / Math.Pow(10, 2 - i) % 10);

            double decimalValue = IAS - Math.Floor(IAS);

            double scaleY = Globals.PFDHeight * 0.045;
            double scaleX = Globals.PFDWidth * 0.028 * 2;

            double offsetY = Globals.PFDHeight * 0.486 - Globals.ViewHeight * 0.5 + Globals.ViewOriginY;
            int count = 0;

            if (IAS < Globals.IASMax)
            {
                while (offsetY > -200)
                {
                    int d = (int)(Math.Floor(IAS / 10) % 2 == 0 ? Math.Floor(IAS / 10) : Math.Floor(IAS / 10) - 1) + count;
                    if (d > Globals.IASMax / 10 || d < 3 || d < IAS / 10)
                    {
                        count++;
                        offsetY -= Globals.PFDHeight * 0.084;
                        continue;
                    }

                    Path tick = new Path
                    {
                        Stroke = Brushes.White,
                        StrokeThickness = 6,
                        StrokeStartLineCap = PenLineCap.Round,
                        StrokeEndLineCap = PenLineCap.Flat,

                        Data = new LineGeometry(new Point(0.0, 0.0), new Point(background.Width * 0.212, 0.0))
                    };

                    double y = (((digitValue[1] * 10 + digitValue[2]) % 20) + decimalValue) / 20.0 * 2 * Globals.PFDHeight * 0.084;

                    if (count % 2 == 0)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            GeometryGroup geometry = new GeometryGroup();


                            int[] number = new int[2];
                            number[0] = d / 10;
                            number[1] = d % 10;

                            if (i == 0 && number[0] == 0)
                                continue;

                            int value = 0;

                            if (i != 2)
                                value = number[i];

                            geometry.Children = Geometries.Numbers[value];
                            geometry.Transform = new ScaleTransform(scaleX, scaleY);

                            Path digit = new Path
                            {
                                Data = geometry,
                                Stroke = Brushes.White,
                                StrokeThickness = 3,
                                StrokeStartLineCap = PenLineCap.Round,
                                StrokeEndLineCap = PenLineCap.Round
                            };

                            TransformGroup transform = new TransformGroup();
                            transform.Children.Add(new TranslateTransform(background.Width * offsetX[i] - scaleX * 0.5 * 0.5, offsetY - scaleY * 0.5 + y));
                            digit.RenderTransform = transform;

                            PFDLayers.Add(digit);
                        }
                        tick.Data = new LineGeometry(new Point(0.0, 0.0), new Point(background.Width * 0.127, 0.0));
                    }

                    tick.Width = tick.Data.Bounds.Width;

                    TransformGroup tickTransform = new TransformGroup();
                    tickTransform.Children.Add(new TranslateTransform(background.Width - tick.Width, offsetY + y));
                    tick.RenderTransform = tickTransform;

                    if (d <= Globals.IASMax / 10 || d >= 4)
                        PFDLayers.Add(tick);

                    count++;
                    offsetY -= Globals.PFDHeight * 0.084;
                }
            }

            offsetY = Globals.PFDHeight * 0.486 - Globals.ViewHeight * 0.5 + Globals.ViewOriginY;
            count = 0;

            while (offsetY < Globals.PFDHeight + 100)
            {
                int d = (int)(Math.Floor(IAS / 10) % 2 == 0 ? Math.Floor(IAS / 10) : Math.Floor(IAS / 10) - 1) - count;
                if (d < Globals.IASMin || d < 4)
                    break;

                Path tick = new Path
                {
                    Stroke = Brushes.White,
                    StrokeThickness = 6,
                    StrokeStartLineCap = PenLineCap.Round,
                    StrokeEndLineCap = PenLineCap.Flat,

                    Data = new LineGeometry(new Point(0.0, 0.0), new Point(background.Width * 0.212, 0.0))
                };

                double y = (((digitValue[1] * 10 + digitValue[2]) % 20) + decimalValue) / 20.0 * 2 * Globals.PFDHeight * 0.084;

                if (count % 2 == 0)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        GeometryGroup geometry = new GeometryGroup();

                        int[] number = new int[2];
                        number[0] = d / 10;
                        number[1] = d % 10;

                        if (i == 0 && number[0] == 0)
                            continue;

                        int value = 0;

                        if (i != 2)
                            value = number[i];

                        geometry.Children = Geometries.Numbers[value];
                        geometry.Transform = new ScaleTransform(scaleX, scaleY);

                        Path digit = new Path
                        {
                            Data = geometry,
                            Stroke = Brushes.White,
                            StrokeThickness = 3,
                            StrokeStartLineCap = PenLineCap.Round,
                            StrokeEndLineCap = PenLineCap.Round
                        };

                        TransformGroup transform = new TransformGroup();
                        transform.Children.Add(new TranslateTransform(background.Width * offsetX[i] - scaleX * 0.5 * 0.5, offsetY - scaleY * 0.5 + y));
                        digit.RenderTransform = transform;

                        PFDLayers.Add(digit);
                    }
                    tick.Data = new LineGeometry(new Point(0.0, 0.0), new Point(background.Width * 0.127, 0.0));
                }

                tick.Width = tick.Data.Bounds.Width;

                TransformGroup tickTransform = new TransformGroup();
                tickTransform.Children.Add(new TranslateTransform(background.Width - tick.Width, offsetY + y));
                tick.RenderTransform = tickTransform;

                PFDLayers.Add(tick);

                count++;
                offsetY += Globals.PFDHeight * 0.084;
            }

            Rectangle altitudeBorderBrush = new Rectangle
            {
                Stroke = Brushes.White,
                StrokeThickness = 3
            };
            altitudeBorderBrush.Width = Globals.PFDWidth * 0.159 + altitudeBorderBrush.StrokeThickness;
            altitudeBorderBrush.Height = Globals.PFDHeight * 0.140 + 2 * altitudeBorderBrush.StrokeThickness;

            Border altitudeBorder = new Border
            {
                Background = Brushes.Black,
                BorderBrush = new VisualBrush(altitudeBorderBrush),
                BorderThickness = new Thickness(0, altitudeBorderBrush.StrokeThickness, altitudeBorderBrush.StrokeThickness, altitudeBorderBrush.StrokeThickness),
                Width = altitudeBorderBrush.Width,
                Height = altitudeBorderBrush.Height
            };

            TransformGroup altitudeBorderTransform = new TransformGroup();
            altitudeBorderTransform.Children.Add(new TranslateTransform(0, Globals.PFDHeight * 0.486 - Globals.ViewHeight * 0.5 + Globals.ViewOriginY - altitudeBorder.Height * 0.5));
            altitudeBorder.RenderTransform = altitudeBorderTransform;

            PFDLayers.Add(altitudeBorder);

            Canvas altitudeCanvas = new Canvas
            {
                Width = altitudeBorder.Width - altitudeBorderBrush.StrokeThickness,
                Height = altitudeBorder.Height - 2 * altitudeBorderBrush.StrokeThickness,
                ClipToBounds = true
            };

            GeometryGroup altitudeClipGeometry = new GeometryGroup();
            altitudeClipGeometry.Children.Add(new RectangleGeometry(new Rect(new Point(0, altitudeCanvas.Height * 0.22), new Point(altitudeCanvas.Width * 0.58, altitudeCanvas.Height * 0.78))));
            altitudeClipGeometry.Children.Add(new RectangleGeometry(new Rect(new Point(altitudeCanvas.Width * 0.58, 0), new Point(altitudeCanvas.Width, altitudeCanvas.Height))));
            altitudeCanvas.Clip = altitudeClipGeometry;

            altitudeBorder.Child = altitudeCanvas;
            double scale = altitudeCanvas.Height * 0.50;

            for (int i = 0; i < 3; i++)
            {
                //geometryGroup.Children = NumberGeometry.Numbers[(int)(Altitude / Math.Pow(10, 4 - i) % 10)];                
                GeometryGroup geometry = new GeometryGroup
                {
                    Children = Geometries.Numbers[digitValue[i]],
                    Transform = new ScaleTransform(scale, scale)
                };

                Path digit = new Path
                {
                    Data = geometry,
                    Stroke = Brushes.White,
                    StrokeThickness = 3,
                    StrokeStartLineCap = PenLineCap.Round,
                    StrokeEndLineCap = PenLineCap.Round
                };

                double yOffset = 0;

                if (i == 2)
                    yOffset = decimalValue * scale * 1.20;
                else if (i == 1)
                {
                    if (digitValue[2] >= 9)
                        yOffset = decimalValue * scale * 1.20;
                }
                else if (i == 0)
                {
                    if (digitValue[2] >= 9 && digitValue[1] >= 9)
                        yOffset = decimalValue * scale * 1.20;
                }

                TransformGroup transform = new TransformGroup();
                transform.Children.Add(new TranslateTransform(altitudeCanvas.Width * xOffset[i] - scale * 0.5 * 0.5, altitudeCanvas.Height * 0.5 - scale * 0.5 + yOffset));
                digit.RenderTransform = transform;

                altitudeCanvas.Children.Add(digit);

                GeometryGroup aboveGeometry = new GeometryGroup
                {
                    Children = Geometries.Numbers[(digitValue[i] + 1) % 10],
                    Transform = geometry.Transform.Clone()
                };
                Path aboveDigit = new Path
                {
                    Data = aboveGeometry,
                    Stroke = Brushes.White,
                    StrokeThickness = 3,
                    StrokeStartLineCap = PenLineCap.Round,
                    StrokeEndLineCap = PenLineCap.Round
                };

                TransformGroup aboveTransform = transform.Clone();
                aboveTransform.Children.Add(new TranslateTransform(0, -scale * 1.20));
                aboveDigit.RenderTransform = aboveTransform;
                altitudeCanvas.Children.Add(aboveDigit);

                if (i == 2)
                {
                    GeometryGroup belowGeometry = new GeometryGroup();
                    if (decimalValue < 0.5)
                        belowGeometry.Children = Geometries.Numbers[(digitValue[i] - 1 + 10) % 10];
                    else
                        belowGeometry.Children = Geometries.Numbers[(digitValue[i] + 2) % 10];
                    belowGeometry.Transform = geometry.Transform.Clone();
                    Path belowDigit = new Path
                    {
                        Data = belowGeometry,
                        Stroke = Brushes.White,
                        StrokeThickness = 3,
                        StrokeStartLineCap = PenLineCap.Round,
                        StrokeEndLineCap = PenLineCap.Round
                    };

                    TransformGroup belowTransform = transform.Clone();
                    if (decimalValue < 0.5)
                        belowTransform.Children.Add(new TranslateTransform(0, scale * 1.20));
                    else
                        belowTransform.Children.Add(new TranslateTransform(0, -2 * scale * 1.20));
                    belowDigit.RenderTransform = belowTransform;
                    altitudeCanvas.Children.Add(belowDigit);
                }
            }
        }

        private void DrawHeading()
        {
            // Rectangle that contains and clips the big circle
            Canvas canvas = new Canvas
            {
                Width = Globals.HeadingWidth,
                Height = Globals.HeadingHeight,
                ClipToBounds = true
            };
            // Position the center and bottom of the retangle in flush with the view and at the bottom of the full canvas
            TransformGroup canvasTranform = new TransformGroup();
            canvasTranform.Children.Add(new TranslateTransform(Globals.PFDWidth * 0.465 - canvas.Width * 0.5, Globals.PFDHeight - canvas.Height));
            canvas.RenderTransform = canvasTranform;
            PFDLayers.Add(canvas);

            // Big circle that contains the tick marks and numbers
            Ellipse background = new Ellipse
            {
                Width = Globals.PFDHeight * 1.676,
                Height = Globals.PFDHeight * 1.676,
                Fill = new SolidColorBrush(new Color() { R = 64, G = 64, B = 64, A = 255 })
            };
            // Position the top and center of the circle at the top and center of the canvas
            TransformGroup backgroundTransform = new TransformGroup();
            backgroundTransform.Children.Add(new TranslateTransform(canvas.Width * 0.5 - background.Width * 0.5, 0.0));
            background.RenderTransform = backgroundTransform;
            canvas.Children.Add(background);

            // Perecentage of where the heading is relavtive to the next number on the scale
            double rotateOffset = (Heading % 10 + Heading - Math.Floor(Heading)) / 10;

            // Ticks and numbers from the center to the right
            for (int j = 1; j < Globals.HeadingTickNumber / 9; j++)
            {
                Line tick = new Line
                {
                    X1 = 0,
                    X2 = 0,
                    Y1 = 1,
                    // Only draw big ticks on every second tick
                    Y2 = j % Globals.HeadingTickInterval == 0 ? Globals.HeadingBigTickHeight : Globals.HeadingSmallTickHeight,
                    Stroke = Brushes.White,
                    StrokeThickness = 3
                };
                // Position the top of the tick at the top and center of the canvas
                // Rotate the tick along the center of the big circle;
                TransformGroup tickTransform = new TransformGroup();
                tickTransform.Children.Add(new TranslateTransform(canvas.Width * 0.5, 0));
                tickTransform.Children.Add(new RotateTransform(j * Globals.HeadingAngleInterval - rotateOffset * Globals.HeadingAngleInterval * Globals.HeadingTickInterval, canvas.Width * 0.5, background.Height * 0.5));
                tick.RenderTransform = tickTransform;
                canvas.Children.Add(tick);

                // Only draw numbers on even ticks (big ticks)
                if (j % Globals.HeadingTickInterval == 0)
                {
                    int headingDisplay = (int)(Math.Floor(Heading) / 10 + j / Globals.HeadingTickInterval) % 36;
                    // Only draw big numbers on every third number
                    double numberWidth = headingDisplay % Globals.HeadingNumberInterval == 0 ? Globals.HeadingBigNumberWidth : Globals.HeadingSmallNumberWidth;
                    double numberHeight = headingDisplay % Globals.HeadingNumberInterval == 0 ? Globals.HeadingBigNumberHeight : Globals.HeadingSmallNumberHeight;
                    // Spacing between numbers
                    double headingSpacing = numberWidth * 1.20;
                    // Offset to center the number below the tick regardless of the number of digits
                    double offsetX = -headingDisplay.ToString().Length * headingSpacing * 0.5 + headingSpacing * 0.5;

                    // Draw every digit in the number
                    for (int i = 0; i < headingDisplay.ToString().Length; i++)
                    {
                        // Get the value of the digit
                        int value = (int)(headingDisplay / Math.Pow(10, (headingDisplay.ToString().Length - 1) - i) % 10);

                        GeometryGroup geometry = new GeometryGroup
                        {
                            Children = Geometries.Numbers[value],
                            // Double the width because the number's width goes from [0.0 - 0.5] instead of [0.0 - 1.0]
                            Transform = new ScaleTransform(numberWidth * 2, numberHeight)
                        };

                        Path digit = new Path
                        {
                            Data = geometry,
                            Stroke = Brushes.White,
                            StrokeThickness = 3,
                            StrokeStartLineCap = PenLineCap.Round,
                            StrokeEndLineCap = PenLineCap.Round
                        };
                        // Position the number below the tick
                        // Rotate the number with the same rotate transform of the tick
                        TransformGroup transform = new TransformGroup();
                        transform.Children.Add(new TranslateTransform(offsetX - numberWidth * 0.5, canvas.Height * 0.61 - numberHeight * 0.5));
                        transform.Children.Add(tickTransform.Clone());
                        digit.RenderTransform = transform;
                        canvas.Children.Add(digit);

                        offsetX += headingSpacing;
                    }
                }
            }

            // Ticks and numbers from the center to the left
            for (int j = 0; j < Globals.HeadingTickNumber / 9 - 1; j++)
            {
                Line tick = new Line
                {
                    X1 = 0,
                    X2 = 0,
                    Y1 = 1,
                    // Only draw big ticks on every second tick
                    Y2 = j % Globals.HeadingTickInterval == 0 ? Globals.HeadingBigTickHeight : Globals.HeadingSmallTickHeight,
                    Stroke = Brushes.White,
                    StrokeThickness = 3,
                    StrokeStartLineCap = PenLineCap.Round,
                    StrokeEndLineCap = PenLineCap.Round
                };
                // Position the top of the tick at the top and center of the canvas
                // Rotate the tick along the center of the big circle;
                TransformGroup tickTransform = new TransformGroup();
                tickTransform.Children.Add(new TranslateTransform(canvas.Width * 0.5, 0));
                tickTransform.Children.Add(new RotateTransform(j * -Globals.HeadingAngleInterval - rotateOffset * Globals.HeadingAngleInterval * Globals.HeadingTickInterval, canvas.Width * 0.5, background.Height * 0.5));
                tick.RenderTransform = tickTransform;
                canvas.Children.Add(tick);

                // Only draw numbers on even ticks (big ticks)
                if (j % Globals.HeadingTickInterval == 0)
                {
                    int headingDisplay = (int)(Math.Floor(Heading) / 10 - j / Globals.HeadingTickInterval + 36) % 36;
                    // Only draw big numbers on every third number
                    double numberWidth = headingDisplay % Globals.HeadingNumberInterval == 0 ? Globals.HeadingBigNumberWidth : Globals.HeadingSmallNumberWidth;
                    double numberHeight = headingDisplay % Globals.HeadingNumberInterval == 0 ? Globals.HeadingBigNumberHeight : Globals.HeadingSmallNumberHeight;
                    // Spacing between numbers
                    double headingSpacing = numberWidth * 1.20;
                    // Offset to center the number below the tick regardless of the number of digits
                    double offsetX = -headingDisplay.ToString().Length * headingSpacing * 0.5 + headingSpacing * 0.5;

                    for (int i = 0; i < headingDisplay.ToString().Length; i++)
                    {
                        int value = (int)(headingDisplay / Math.Pow(10, headingDisplay.ToString().Length - 1 - i) % 10);

                        GeometryGroup geometry = new GeometryGroup
                        {
                            Children = Geometries.Numbers[value],
                            // Double the width because the number's width goes from [0.0 - 0.5] instead of [0.0 - 1.0]
                            Transform = new ScaleTransform(numberWidth * 2, numberHeight)
                        };

                        Path digit = new Path
                        {
                            Data = geometry,
                            Stroke = Brushes.White,
                            StrokeThickness = 3,
                            StrokeStartLineCap = PenLineCap.Round,
                            StrokeEndLineCap = PenLineCap.Round
                        };
                        // Position the number below the tick
                        // Rotate the number with the same rotate transform of the tick
                        TransformGroup transform = new TransformGroup();
                        transform.Children.Add(new TranslateTransform(offsetX - numberWidth * 0.5, canvas.Height * 0.61 - numberHeight * 0.5));
                        transform.Children.Add(tickTransform.Clone());
                        digit.RenderTransform = transform;
                        canvas.Children.Add(digit);

                        offsetX += headingSpacing;
                    }
                }
            }

            // Hollow triangle that points to the heading
            Polygon indicator = new Polygon();
            indicator.Points.Add(new Point(0.0, 0.0));
            indicator.Points.Add(new Point(Globals.HeadingIndicatorWidth, 0.0));
            indicator.Points.Add(new Point(Globals.HeadingIndicatorWidth * 0.5, Globals.HeadingIndicatorHeight));
            indicator.Stroke = Brushes.White;
            indicator.StrokeThickness = 3;
            indicator.StrokeStartLineCap = PenLineCap.Round;
            indicator.StrokeEndLineCap = PenLineCap.Round;
            // Position the center and top of the indicator at the center and top of the canvas
            TransformGroup indicatorTransform = new TransformGroup();
            indicatorTransform.Children.Add(canvas.RenderTransform.Clone());
            indicatorTransform.Children.Add(new TranslateTransform(canvas.Width * 0.5 - Globals.HeadingIndicatorWidth * 0.5, -canvas.Height * 0.171));
            indicator.RenderTransform = indicatorTransform;
            // Add the indicator outside the canvas to avoid clipping
            PFDLayers.Add(indicator);
        }
    }
}