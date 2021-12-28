using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
namespace CSharpLib.WinForms
{
    /// <summary>
    /// Class containing Windows Forms methods.
    /// </summary>
    public class Forms
    {
        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
        private const UInt32 SWP_NOSIZE = 0x0001;
        private const UInt32 SWP_NOMOVE = 0x0002;
        private const UInt32 SWP_NOACTIVATE = 0x0010;
        private const UInt32 TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        /// <summary>
        /// Keeps the specified form on top of other forms, even if the form isn't in focus. 
        /// </summary>
        /// <param name="form">The form to keep on top of other applications.</param>
        public static void KeepOnTop(Form form)
        {
            SetWindowPos(form.Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
        }
        /// <summary>
        /// Keeps the specified form behind other forms, even if the form isn't in focus. 
        /// </summary>
        /// <param name="form">The form to keep behind other applications.</param>
        public static void KeepOnBottom(Form form)
        {
            SetWindowPos(form.Handle, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
        }      
        /// <summary>
        ///  Asyncronously updates the text of the specified label.
        /// </summary>
        /// <param name="form">Form containing label to update.</param>
        /// <param name="label">Label to update.</param>
        /// <param name="text">Text to set.</param>
        public static void UpdateLabel(Form form, Label label, string text)
        {
            MethodInvoker inv = delegate
            {
                label.Text = text;
            };
            form.Invoke(inv);
        }
        /// <summary>
        /// Checks if the specified control is, in part or in whole, out of the bounds of its parent form.
        /// </summary>
        /// <param name="form">The parent form of the control.</param>
        /// <param name="control">The forms' child control to check.</param>
        /// <returns></returns>
        public static bool IsOutofBounds(Form form, Control control)
        {
            int controlEnd_X = control.Location.X + control.ClientSize.Width;
            int controlEnd_Y = control.Location.Y + control.ClientSize.Height;
            if (form.ClientSize.Width < controlEnd_X || form.ClientSize.Height < controlEnd_Y)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    /// <summary>
    /// Class for setting a Windows Form to fullscreen.
    /// </summary>
    public class FullScreen
    {
        /// <summary>
        /// Fills the screen with the specified form.  WARNING: This method will completely fullscreen your form and remove the taskbar. If you do not have another way of closing or resizing your form, it will not be removable.
        /// </summary>
        /// <param name="targetForm">The form to resize.</param>
        public void EnterFullScreenMode(Form targetForm)
        {
            targetForm.WindowState = FormWindowState.Normal;
            targetForm.FormBorderStyle = FormBorderStyle.None;
            targetForm.WindowState = FormWindowState.Maximized;
        }
        /// <summary>
        /// Returns your form to its non-fullscreen state.
        /// </summary>
        /// <param name="targetForm">The form to resize.</param>
        public void LeaveFullScreenMode(Form targetForm)
        {
            targetForm.FormBorderStyle = FormBorderStyle.Sizable;
            targetForm.WindowState = FormWindowState.Normal;
        }
    }
}
