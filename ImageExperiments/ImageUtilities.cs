﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace ImageExperiments
{
    public static class ImageUtilities
    {
        public static Image DrawString(string str, Image bitmap, DrawSettings drawSettings)
        {
            if (drawSettings.DrawStyle == DrawStyle.Bordered)
                return DrawBorderedString(str, bitmap, drawSettings);
            Graphics graphicsImage = Graphics.FromImage(bitmap);
            
            //strFormat.FormatFlags |= StringFormatFlags.FitBlackBox;
            Color StringColor = drawSettings.Color;
            Point point = new Point(bitmap.Width / 2, bitmap.Height / 2);
            if (drawSettings.WrapWidth > 0)
            {
                str = WordWrap(str, drawSettings.WrapWidth);
            }
            Font font = GetAdjustedFont(graphicsImage, str, drawSettings.Font, bitmap.Width, drawSettings.MaxTextSize, drawSettings.MinTextSize, true);
            graphicsImage.DrawString(str, font, new SolidBrush(StringColor), point,
                drawSettings.StringFormat);
            bitmap.Save("testOutput.png");
            return bitmap;
        }

        public static Image DrawBorderedString(string str, Image bitmap, DrawSettings drawSettings)
        {
            Graphics g = Graphics.FromImage(bitmap);
            StringFormat stringFormat = drawSettings.StringFormat;

            Color StringColor = drawSettings.Color;
            string adjustedText = WordWrap(str, drawSettings.WrapWidth);
            Point point = new Point(bitmap.Width / 2, bitmap.Height / 2 - 40);
            Font font = drawSettings.Font;
            font = GetAdjustedFont(g, adjustedText, font, bitmap.Width, drawSettings.MaxTextSize, drawSettings.MinTextSize, true);

            GraphicsPath p = new GraphicsPath();
            float moveX = bitmap.Width / 2f + 20;
            float moveY = bitmap.Height / 2f - 40 + 20;
            g.TranslateTransform(moveX, moveY);
            g.RotateTransform(-35f);
            g.TranslateTransform(-moveX, -moveY);
            p.AddString(adjustedText, FontFamily.GenericSansSerif, (int)FontStyle.Regular, g.DpiY * font.Size / 72, point, stringFormat);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            g.CompositingQuality = CompositingQuality.HighQuality;
            Pen pen = new Pen(Color.Black, 10f);
            g.DrawPath(pen, p);
            p.FillMode = FillMode.Winding;
            //g.FillPath(new TextureBrush(fillMap, new Rectangle(0, 0, bitmap.Width, bitmap.Height)), p);
            RectangleF pathRect = p.GetBounds();
            Rectangle gradRect = new Rectangle(0, 0, 20, 20);
            var gradBrush = new LinearGradientBrush(gradRect, Color.BlueViolet, Color.ForestGreen, LinearGradientMode.ForwardDiagonal);
            //gradBrush.CenterColor = Color.Aqua;
            //gradBrush.SurroundColors = new[] { Color.Coral, Color.Crimson, Color.ForestGreen, Color.Pink, Color.White };
            g.FillPath(gradBrush, p);

            //graphicsImage.DrawString(adjustedText, font, new SolidBrush(StringColor), point,
            //    centerFormat);
            g.ResetTransform();
            return bitmap;
        }

        public static Font GetAdjustedFont(Graphics GraphicRef, string GraphicString, Font OriginalFont, int ContainerWidth, int MaxFontSize, int MinFontSize, bool SmallestOnFail)
        {
            // We utilize MeasureString which we get via a control instance           
            for (int AdjustedSize = MaxFontSize; AdjustedSize >= MinFontSize; AdjustedSize--)
            {
                Font TestFont = new Font(OriginalFont.Name, AdjustedSize, OriginalFont.Style);

                // Test the string with the new size
                SizeF AdjustedSizeNew = GraphicRef.MeasureString(GraphicString, TestFont);

                if (ContainerWidth > Convert.ToInt32(AdjustedSizeNew.Width))
                {
                    // Good font, return it
                    return TestFont;
                }
            }

            // If you get here there was no fontsize that worked
            // return MinimumSize or Original?
            if (SmallestOnFail)
            {
                return new Font(OriginalFont.Name, MinFontSize, OriginalFont.Style);
            }
            else
            {
                return OriginalFont;
            }
        }

        /// <summary>
        /// Word wraps the given text to fit within the specified width.
        /// </summary>
        /// <param name="text">Text to be word wrapped</param>
        /// <param name="width">Width, in characters, to which the text
        /// should be word wrapped</param>
        /// <returns>The modified text</returns>
        private static string WordWrap(string text, int width)
        {
            int pos, next;
            StringBuilder sb = new StringBuilder();

            // Lucidity check
            if (width < 1)
                return text;

            // Parse each line of text
            for (pos = 0; pos < text.Length; pos = next)
            {
                // Find end of line
                int eol = text.IndexOf(Environment.NewLine, pos);
                if (eol == -1)
                    next = eol = text.Length;
                else
                    next = eol + Environment.NewLine.Length;

                // Copy this line of text, breaking into smaller lines as needed
                if (eol > pos)
                {
                    do
                    {
                        int len = eol - pos;
                        if (len > width)
                            len = BreakLine(text, pos, width);
                        sb.Append(text, pos, len);
                        sb.Append(Environment.NewLine);

                        // Trim whitespace following break
                        pos += len;
                        while (pos < eol && Char.IsWhiteSpace(text[pos]))
                            pos++;
                    } while (eol > pos);
                }
                else sb.Append(Environment.NewLine); // Empty line
            }
            return sb.ToString();
        }

        /// <summary>
        /// Locates position to break the given line so as to avoid
        /// breaking words.
        /// </summary>
        /// <param name="text">String that contains line of text</param>
        /// <param name="pos">Index where line of text starts</param>
        /// <param name="max">Maximum line length</param>
        /// <returns>The modified line length</returns>
        private static int BreakLine(string text, int pos, int max)
        {
            // Find last whitespace in line
            int i = max;
            while (i >= 0 && !Char.IsWhiteSpace(text[pos + i]))
                i--;

            // If no whitespace found, break at maximum length
            if (i < 0)
                return max;

            // Find start of whitespace
            while (i >= 0 && Char.IsWhiteSpace(text[pos + i]))
                i--;

            // Return length of text before whitespace
            return i + 1;
        }

    }

    public struct DrawSettings
    {
        public StringFormat StringFormat;
        public Color Color;
        public Font Font;
        public int WrapWidth;
        public int MinTextSize;
        public int MaxTextSize;
        public DrawStyle DrawStyle;
    }

    public enum DrawStyle
    {
        Normal,
        Bordered
    }
}
