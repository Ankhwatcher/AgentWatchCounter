/* Created by Rory Glynn */

using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using System.Threading;

namespace AgentWatchApplication1
{
    public class Program
    {
        /*Constants*/
        static int DISPLAY_HEIGHT = 128;
        static int DISPLAY_WIDTH = 128;

        static Bitmap _display;
        static InterruptPort _buttonUp;
        static InterruptPort _buttonMenu;

        static int _countVal = 0;
        static string _resetLabel = "Reset";
        static string _incrementLabel = "Increment";
        public static void Main()
        {
            // initialize display buffer
            _display = new Bitmap(Bitmap.MaxWidth, Bitmap.MaxHeight);

            _countVal = -1;
            incrementCount(0, 0, DateTime.Now);


            _buttonUp = new InterruptPort(HardwareProvider.HwProvider.GetButtonPins(Button.VK_UP), false, Port.ResistorMode.PullDown, Port.InterruptMode.InterruptEdgeLow);
            _buttonMenu = new InterruptPort(HardwareProvider.HwProvider.GetButtonPins(Button.VK_MENU), false, Port.ResistorMode.PullDown, Port.InterruptMode.InterruptEdgeLow);


            _buttonUp.OnInterrupt += incrementCount;
            _buttonMenu.OnInterrupt += resetCount;


            // go to sleep; all further code should be timer-driven or event-driven
            Thread.Sleep(Timeout.Infinite);
        }

        private static void resetCount(uint data1, uint data2, DateTime time)
        {
            _countVal = -1;
            incrementCount(data1, data2, time);
        }

        private static void incrementCount(uint data1, uint data2, DateTime time)
        {
            if (_countVal < 9999999)
            {
                _countVal++;

                //Increase values at a greater rate to test the font sizing.
                /* 
                if (_countVal > 9)
                    _countVal = _countVal + 10;

                if (_countVal > 99)
                    _countVal = _countVal + 100;
                if (_countVal > 999)
                    _countVal = _countVal + 1000;
                if (_countVal > 9999)
                    _countVal = _countVal + 10000;
                if (_countVal > 99999)
                    _countVal = _countVal + 100000;
                 */
            }
            _display.Clear();

            Font fontNinaB = Resources.GetFont(Resources.FontResources.NinaB);

            Font fontStencil = Resources.GetFont(Resources.FontResources.stencil72);
            // Use a smaller version of the Stencil font for larger numbers
            if (_countVal > 99999)
                fontStencil = Resources.GetFont(Resources.FontResources.stencil24);
            else if (_countVal > 999)
                fontStencil = Resources.GetFont(Resources.FontResources.stencil32);
            else if (_countVal > 99)
                fontStencil = Resources.GetFont(Resources.FontResources.stencil48);


            int charsAcross = DISPLAY_WIDTH / fontStencil.CharWidth('0');
            _display.DrawText(_countVal.ToString("d" + charsAcross), fontStencil, Color.White,
                (DISPLAY_WIDTH % fontStencil.CharWidth('0')) / 2,
                (DISPLAY_HEIGHT - fontStencil.Height) / 2);

            // Draw labels for Reset and Increment buttons.
            _display.DrawText(_resetLabel, fontNinaB, Color.White, 2, DISPLAY_HEIGHT - fontNinaB.Height - 2);
            int increaseLabelWidth = 0;
            char[] charArray = _incrementLabel.ToCharArray(0, _incrementLabel.Length);
            for (int letterNo = 0; letterNo < charArray.Length; letterNo++)
            {
                increaseLabelWidth += fontNinaB.CharWidth(charArray[letterNo]);
            }
            _display.DrawText(_incrementLabel, fontNinaB, Color.White, DISPLAY_WIDTH - increaseLabelWidth - 2, 2);


            _display.Flush();
        }
    }
}