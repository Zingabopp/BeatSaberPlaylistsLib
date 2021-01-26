using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ImageExperiments.ViewModels
{
    public class DrawSettingsViewModel : INotifyPropertyChanged
    {
        private StringAlignment _horizontalAlignment = StringAlignment.Center;

        public StringAlignment HorizontalAlignment
        {
            get { return _horizontalAlignment; }
            set
            {
                if (_horizontalAlignment == value) return;
                _horizontalAlignment = value;
                NotifyPropertyChanged();
            }
        }

        private StringAlignment _verticalAlignment = StringAlignment.Center;

        public StringAlignment VerticalAlignment
        {
            get { return _verticalAlignment; }
            set
            {
                if (_verticalAlignment == value) return;
                _verticalAlignment = value;
                NotifyPropertyChanged();
            }
        }

        private Color _textColor = ColorTranslator.FromHtml("blue");

        public Color TextColor
        {
            get { return _textColor; }
            set
            {
                if (_textColor == value) return;
                _textColor = value;
                NotifyPropertyChanged();
            }
        }

        private Font _font = new Font("arial", 60, FontStyle.Regular);

        public Font Font
        {
            get { return _font; }
            set { _font = value; }
        }

        private int _wrapWidth = 50;

        public int WrapWidth
        {
            get { return _wrapWidth; }
            set
            {
                if (_wrapWidth == value) return;
                _wrapWidth = value;
                NotifyPropertyChanged();
            }
        }

        private int _minTextSize = 20;

        public int MinTextSize
        {
            get { return _minTextSize; }
            set
            {
                if (_minTextSize == value) return;
                _minTextSize = value;
                NotifyPropertyChanged();
            }
        }

        private int _maxTextSize = 100;

        public int MaxTextSize
        {
            get { return _maxTextSize; }
            set
            {
                if (_maxTextSize == value) return;
                _maxTextSize = value;
                NotifyPropertyChanged();
            }
        }

        private DrawStyle _drawStyle;

        public DrawStyle DrawStyle
        {
            get { return _drawStyle; }
            set
            {
                if (_drawStyle == value) return;
                _drawStyle = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(NormalDraw));
                NotifyPropertyChanged(nameof(BorderedDraw));
            }
        }


        public bool NormalDraw
        {
            get { return DrawStyle == DrawStyle.Normal; }
            set
            {
                if (value)
                    DrawStyle = DrawStyle.Normal;
            }
        }

        public bool BorderedDraw
        {
            get { return DrawStyle == DrawStyle.Bordered; }
            set
            {
                if (value)
                    DrawStyle = DrawStyle.Bordered;
            }
        }






        public DrawSettings GetDrawSettings()
        {
            return new DrawSettings()
            {
                StringFormat = new StringFormat()
                {
                    Alignment = HorizontalAlignment,
                    LineAlignment = VerticalAlignment
                },
                Color = TextColor,
                Font = Font,
                WrapWidth = WrapWidth,
                MinTextSize = MinTextSize,
                MaxTextSize = MaxTextSize,
                DrawStyle = DrawStyle

            };
        }












        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
