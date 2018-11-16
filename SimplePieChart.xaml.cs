using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

//using static AniLyst_5._0.CustomControls.PieChart;

namespace AniLyst_5._0.CustomControls
{
    /// <summary>
    /// Interaction logic for SimplePieChart.xaml
    /// </summary>
    public partial class SimplePieChart : UserControl
    {
        public SimplePieChart()
        {
            InitializeComponent();
            Slice = new _Slice(this);
            this.SizeChanged += PieChart_SizeChanged;
            LengendLocation = LengendLocation.Right;
        }

        public string Title
        {
            get { return TitleTextBlock.Text; }
            set { TitleTextBlock.Text = value; }
        }

        public string LengendTitle
        {
            get { return Lengend.Header.ToString(); }
            set { Lengend.Header = value; }
        }

        #region Main Methods

        private void PieChart_SizeChanged(object sender, SizeChangedEventArgs e) => Refresh();

        public void Refresh()
        {
            if (Slice.Slices.Count > 0)
            {
                double total = Slice.Slices.Values.Sum(item => item.Value);
                if (total != 0)
                {
                    double PreEndAngle = 0;
                    foreach (KeyValuePair<string, PieSlice> KVP in Slice.Slices)
                    {
                        if (KVP.Value.Value == 0) continue;
                        double Angle = (KVP.Value.Value / total) * 360;
                        KVP.Value.StartAngle = PreEndAngle;

                        PreEndAngle = KVP.Value.EndAngle = Angle + PreEndAngle;

                        if (PreEndAngle > 360) PreEndAngle -= 360;
                    }
                }
            }
        }

        public void Clear()
        {
            Slice.Slices = new Dictionary<string, PieSlice>();
            PieGrid.Children.Clear();
            LengendBox.Children.Clear();
        }

        #endregion Main Methods

        #region Slice

        public _Slice Slice { get; set; }

        #endregion Slice

        #region Events

        public delegate void _SelectedSliceChanged();

        public event _SelectedSliceChanged SelectedSliceChanged;

        #endregion Events

        private LengendLocation _LengendLocation = LengendLocation.Right;

        public LengendLocation LengendLocation
        {
            get { return _LengendLocation; }
            set
            {
                _LengendLocation = value;
                switch (_LengendLocation)
                {
                    case LengendLocation.Left:
                        Grid.SetColumn(LengendGrid, 0);
                        Grid.SetColumn(PieViewBox, 1);
                        MainGrid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
                        MainGrid.ColumnDefinitions[1].Width = new GridLength(2, GridUnitType.Star);
                        break;

                    case LengendLocation.Right:
                        Grid.SetColumn(PieViewBox, 0);
                        Grid.SetColumn(LengendGrid, 1);
                        MainGrid.ColumnDefinitions[0].Width = new GridLength(2, GridUnitType.Star);
                        MainGrid.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);
                        break;
                }
            }
        }
    }

    public partial class SimplePieChart
    {
        public class _Slice
        {
            public _Slice(SimplePieChart _PC) => PC = _PC;

            private SimplePieChart PC { get; set; }
            public static int ColorInt = -1;

            public void IncrementColorInt()
            {
                ColorInt++;
                if (ColorInt > BrushBook.Keys.Max()) ColorInt = 0;
            }

            public void Add(string Key)
            {
                IncrementColorInt();
                Add(Key, BrushBook[ColorInt]);
            }

            public void Add(string Key, double Value, double InnerSlicePercent)
            {
                IncrementColorInt();
                Add(Key, Value, InnerSlicePercent, BrushBook[ColorInt]);
            }

            public void Add(string Key, Brush BackGround) => Add(Key, 0, 100, BackGround, BackGround);

            public void Add(string Key, Brush BackGround, Brush StrokeColor) => Add(Key, 0, 100, BackGround, StrokeColor);

            public void Add(string Key, double Value, double InnerSlicePercent, Brush BackGround) => Add(Key, Value, InnerSlicePercent, BackGround, BackGround);

            public void Add(string Key, double Value, double InnerSlicePercent, Brush BackGround, Brush StrokeColor)
            {
                PieSlice PS = new PieSlice();
                PS.Text = Key;
                PS.Value = Value;
                PS.SliceColor = BackGround;
                PS.StrokeColor = StrokeColor;
                PS.Slice.MouseDown += Slice_MouseDown;
                PS.LengendItem.MouseDown += CheckBox_MouseDown;
                PS.LengendItem.Checked += CheckBox_Checked;
                PC.PieGrid.Children.Add(PS.Slice);
                PC.LengendBox.Children.Add(PS.LengendItem);
                Slices.Add(Key, PS);
            }

            private void CheckBox_Checked(object sender, RoutedEventArgs e) => SetSelected((sender as CheckBox).Tag as PieSlice);

            private void CheckBox_MouseDown(object sender, MouseButtonEventArgs e) => SetSelected((sender as CheckBox).Tag as PieSlice);

            private void Slice_MouseDown(object sender, MouseButtonEventArgs e) => SetSelected((sender as Xceed.Wpf.Toolkit.Pie).Tag as PieSlice);

            private void SetSelected(PieSlice PS)
            {
                Selected = PS;
                PC.SelectedSliceChanged?.Invoke();
            }

            public PieSlice Selected { get; set; }

            public void Remove(string SliceText)
            {
                PieSlice PS = Slices[SliceText];
                PC.PieGrid.Children.Remove(PS.Slice);
                PC.LengendBox.Children.Remove(PS.LengendItem);
                Slices.Remove(SliceText);
            }

            public PieSlice this[string key] { get { return Slices[key]; } set { Slices[key] = value; } }

            internal Dictionary<string, PieSlice> Slices = new Dictionary<string, PieSlice>();
            public int Count { get { return Slices.Count; } }

            public bool Contains(string SliceText)
            {
                return Slices.Keys.Contains(SliceText);
            }

            private static int U = 0;

            public static Dictionary<int, Brush> BrushBook = new Dictionary<int, Brush>
            {
               { U++, Colors.DarkViolet.ToBrush() },
               //{ U++, Colors.Beige.ToBrush() },
               { U++, Colors.SaddleBrown.ToBrush() },
               { U++, Colors.Salmon.ToBrush() },
               { U++, Colors.LightSteelBlue.ToBrush() },
               { U++, Colors.Black.ToBrush() },
               { U++, Colors.MediumAquamarine.ToBrush() },
               { U++, Colors.DarkTurquoise.ToBrush() },
               { U++, Colors.SeaGreen.ToBrush() },
               { U++, Colors.SlateGray.ToBrush() },
               { U++, Colors.Cyan.ToBrush() },
               //{ U++, Colors.Pink.ToBrush() },
               { U++, Colors.Firebrick.ToBrush() },
               { U++, Colors.Tomato.ToBrush() },
               { U++, Colors.Magenta.ToBrush() },
               { U++, Colors.Goldenrod.ToBrush() },
               { U++, Colors.Sienna.ToBrush() },
               { U++, Colors.LimeGreen.ToBrush() },
               //{ U++, Colors.WhiteSmoke.ToBrush() },
               { U++, Colors.Olive.ToBrush() },
               { U++, Colors.Orange.ToBrush() },
               //{ U++, Colors.Moccasin.ToBrush() },
               { U++, Colors.Aqua.ToBrush() },
               { U++, Colors.RoyalBlue.ToBrush() },
               { U++, Colors.LightSlateGray.ToBrush() },
               //{ U++, Colors.Cornsilk.ToBrush() },
               //{ U++, Colors.White.ToBrush() },
               //{ U++, Colors.PaleGoldenrod.ToBrush() },
               { U++, Colors.Gold.ToBrush() },
               { U++, Colors.LightCoral.ToBrush() },
               { U++, Colors.Red.ToBrush() },
               { U++, Colors.DarkGreen.ToBrush() },
               { U++, Colors.MediumSeaGreen.ToBrush() },
               { U++, Colors.SlateBlue.ToBrush() },
               //{ U++, Colors.Wheat.ToBrush() },
               { U++, Colors.Peru.ToBrush() },
               { U++, Colors.DarkOrchid.ToBrush() },
               { U++, Colors.Chocolate.ToBrush() },
               { U++, Colors.Crimson.ToBrush() },
               { U++, Colors.DodgerBlue.ToBrush() },
               { U++, Colors.DarkBlue.ToBrush() },
               { U++, Colors.DarkMagenta.ToBrush() },
               //{ U++, Colors.Honeydew.ToBrush() },
               { U++, Colors.DarkKhaki.ToBrush() },
               //{ U++, Colors.Snow.ToBrush() },
               //{ U++, Colors.LemonChiffon.ToBrush() },
               { U++, Colors.DarkSlateBlue.ToBrush() },
               //{ U++, Colors.Bisque.ToBrush() },
               { U++, Colors.Green.ToBrush() },
               { U++, Colors.DarkGray.ToBrush() },
               { U++, Colors.LightSkyBlue.ToBrush() },
               { U++, Colors.DarkRed.ToBrush() },
               { U++, Colors.Lavender.ToBrush() },
               //{ U++, Colors.BlanchedAlmond.ToBrush() },
               { U++, Colors.DeepSkyBlue.ToBrush() },
               { U++, Colors.PaleTurquoise.ToBrush() },
               { U++, Colors.DarkSalmon.ToBrush() },
               { U++, Colors.LightSalmon.ToBrush() },
               { U++, Colors.Lime.ToBrush() },
               { U++, Colors.PaleGreen.ToBrush() },
               { U++, Colors.Yellow.ToBrush() },
               //{ U++, Colors.AliceBlue.ToBrush() },
               //{ U++, Colors.OldLace.ToBrush() },
               { U++, Colors.Blue.ToBrush() },
               { U++, Colors.LightSeaGreen.ToBrush() },
               //{ U++, Colors.AntiqueWhite.ToBrush() },
               { U++, Colors.MediumBlue.ToBrush() },
               { U++, Colors.MediumSlateBlue.ToBrush() },
               { U++, Colors.Teal.ToBrush() },
               { U++, Colors.Orchid.ToBrush() },
               //{ U++, Colors.MistyRose.ToBrush() },
               { U++, Colors.DarkSlateGray.ToBrush() },
               //{ U++, Colors.Gainsboro.ToBrush() },
               { U++, Colors.Coral.ToBrush() },
               { U++, Colors.MidnightBlue.ToBrush() },
               { U++, Colors.BlueViolet.ToBrush() },
               //{ U++, Colors.Linen.ToBrush() },
               { U++, Colors.RosyBrown.ToBrush() },
               { U++, Colors.Tan.ToBrush() },
               { U++, Colors.ForestGreen.ToBrush() },
               //{ U++, Colors.Ivory.ToBrush() },
               //{ U++, Colors.MintCream.ToBrush() },
               { U++, Colors.SpringGreen.ToBrush() },
               { U++, Colors.YellowGreen.ToBrush() },
               { U++, Colors.HotPink.ToBrush() },
               { U++, Colors.SkyBlue.ToBrush() },
               //{ U++, Colors.LightGoldenrodYellow.ToBrush() },
               { U++, Colors.GreenYellow.ToBrush() },
               //{ U++, Colors.LightCyan.ToBrush() },
               { U++, Colors.Fuchsia.ToBrush() },
               { U++, Colors.LightGreen.ToBrush() },
               { U++, Colors.DarkOrange.ToBrush() },
               { U++, Colors.BurlyWood.ToBrush() },
               //{ U++, Colors.Plum.ToBrush() },
               { U++, Colors.LightBlue.ToBrush() },
               //{ U++, Colors.Thistle.ToBrush() },
               { U++, Colors.DarkGoldenrod.ToBrush() },
               { U++, Colors.SandyBrown.ToBrush() },
               { U++, Colors.OrangeRed.ToBrush() },
               //{ U++, Colors.LightGray.ToBrush() },
               //{ U++, Colors.PapayaWhip.ToBrush() },
               { U++, Colors.MediumPurple.ToBrush() },
               { U++, Colors.DarkOliveGreen.ToBrush() },
               { U++, Colors.IndianRed.ToBrush() },
               { U++, Colors.DimGray.ToBrush() },
               { U++, Colors.MediumTurquoise.ToBrush() },
               //{ U++, Colors.LavenderBlush.ToBrush() },
               { U++, Colors.CadetBlue.ToBrush() },
               { U++, Colors.Brown.ToBrush() },
               { U++, Colors.DeepPink.ToBrush() },
               { U++, Colors.Gray.ToBrush() },
               { U++, Colors.MediumOrchid.ToBrush() },
               { U++, Colors.PaleVioletRed.ToBrush() },
               { U++, Colors.DarkSeaGreen.ToBrush() },
               { U++, Colors.Maroon.ToBrush() },
               //{ U++, Colors.LightYellow.ToBrush() },
               { U++, Colors.SteelBlue.ToBrush() },
               { U++, Colors.Silver.ToBrush() },
               { U++, Colors.LawnGreen.ToBrush() },
               { U++, Colors.Chartreuse.ToBrush() },
               { U++, Colors.Navy.ToBrush() },
               //{ U++, Colors.NavajoWhite.ToBrush() },
               { U++, Colors.Indigo.ToBrush() },
               //{ U++, Colors.GhostWhite.ToBrush() },
               //{ U++, Colors.FloralWhite.ToBrush() },
               { U++, Colors.DarkCyan.ToBrush() },
               { U++, Colors.LightPink.ToBrush() },
               //{ U++, Colors.PeachPuff.ToBrush() },
               { U++, Colors.Violet.ToBrush() },
               { U++, Colors.Turquoise.ToBrush() },
               { U++, Colors.Purple.ToBrush() },
               { U++, Colors.MediumSpringGreen.ToBrush() },
               { U++, Colors.Khaki.ToBrush() },
               { U++, Colors.CornflowerBlue.ToBrush() },
               //{ U++, Colors.Azure.ToBrush() },
               { U++, Colors.OliveDrab.ToBrush() },
               { U++, Colors.MediumVioletRed.ToBrush() },
               //{ U++, Colors.PowderBlue.ToBrush() },
               //{ U++, Colors.SeaShell.ToBrush() },
               { U++, Colors.Aquamarine.ToBrush() },
            };
        }
    }

    public enum LengendLocation { Left, Right }

    public partial class PieSlice
    {
        public PieSlice()
        {
            //LengendItem.IsHitTestVisible = false;
            SliceColor = AppColors.White;
            StrokeColor = AppColors.White;
            Slice.SnapsToDevicePixels = true;
            LengendItem.Tag = Slice.Tag = this;
            LengendItem.Checked += LengendItem_Checked;
        }

        #region LengendItem

        public CheckBox LengendItem = new CheckBox();

        private void LengendItem_Checked(object sender, RoutedEventArgs e) => LengendItem.IsChecked = false;

        private void SetLengendItem() => LengendItem.Content = _Text + " (" + Value + ")";

        private string _Text = "";

        public string Text { get { return _Text; } set { _Text = value; SetLengendItem(); } }

        private double _Value = 0;

        public double Value { get { return _Value; } set { _Value = value; SetLengendItem(); } }

        #endregion LengendItem

        #region Slice

        public Xceed.Wpf.Toolkit.Pie Slice = new Xceed.Wpf.Toolkit.Pie { Mode = Xceed.Wpf.Toolkit.PieMode.EndAngle, StartAngle = 0, EndAngle = 0, StrokeThickness = 0, SnapsToDevicePixels = true, UseLayoutRounding = true };

        public double StartAngle { get { return Slice.StartAngle; } set { Slice.StartAngle = value; } }

        public double EndAngle { get { return Slice.EndAngle; } set { Slice.EndAngle = value; } }

        private Brush _SliceColor = AppColors.Transparent;

        public Brush SliceColor
        {
            get { return _SliceColor; }
            set { LengendItem.Background = Slice.Fill = _SliceColor = value; }
        }

        private Brush _StrokeColor = AppColors.Transparent;

        public Brush StrokeColor
        {
            get { return _StrokeColor; }
            set { /*LengendItem.BorderBrush = Slice.Stroke =*/ LengendItem.BorderBrush = _StrokeColor = value; }
        }

        #endregion Slice

        private object _Tag = null;

        public object Tag { get { return _Tag; } set { _Tag = value; } }
    };
}