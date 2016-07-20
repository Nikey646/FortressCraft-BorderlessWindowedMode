using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace BorderlessWindowedMode
{
	internal class NativeMethods
	{

		private const Int32 MonitorDefaultToPrimary = 1;
		private const Int32 MonitorDefaultToNearest = 2;

		private const Int32 GwlStyle = -16;

		private const Int32 WsCaption = 0x00C00000;
		private const Int32 WsThickframe = 0x00040000;
		private const Int32 SwpFrameChanged = 0x0020;

		[DllImport("user32", SetLastError = true, CharSet = CharSet.Auto)]
		internal static extern IntPtr GetForegroundWindow();

		[DllImport("user32", SetLastError = true, CharSet = CharSet.Auto)]
		internal static extern IntPtr MonitorFromWindow(IntPtr handle, UInt32 flags);

		[DllImport("user32", SetLastError = true, CharSet = CharSet.Auto)]
		internal static extern Boolean GetMonitorInfo(IntPtr handle, out MonitorInfo monitorInfo);

		[DllImport("user32", SetLastError = true, CharSet = CharSet.Auto)]
		internal static extern IntPtr SetWindowLong(IntPtr handle, Int32 index, Int32 newLong);

		[DllImport("user32", SetLastError = true, CharSet = CharSet.Auto)]
		internal static extern IntPtr GetWindowLong(IntPtr handle, Int32 index);

		[DllImport("user32", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern IntPtr SetWindowPos(IntPtr handle, Int32 handleInsertAfter, Int32 x, Int32 y, Int32 cx, Int32 cy,
			Int32 flags);

		[DllImport("user32", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern Int32 GetWindowText(IntPtr handle, StringBuilder text, Int32 maxLength);

		public static Boolean TrySetBorderless(Boolean primaryMonitor)
		{
			var handle = GetForegroundWindow(); // Dangerous, Might not be FortressCraft
			
			var titleBuilder = new StringBuilder(256);
			var result = GetWindowText(handle, titleBuilder, 255);
			if (result == 0)
				return false;

			var title = titleBuilder.ToString(0, result);
			if (title != "FortressCraft")
				return false;

			var standardFlags = (Int32) GetWindowLong(handle, GwlStyle);

			if (!Flags.Contains(standardFlags, WsCaption))
				return true;

			Flags.Unset(ref standardFlags, WsCaption);
			Flags.Unset(ref standardFlags, WsThickframe);

			var monitorFlags = primaryMonitor ? MonitorDefaultToPrimary : MonitorDefaultToNearest;
			var monitor = MonitorFromWindow(handle, (UInt32) monitorFlags);

			var info = new MonitorInfo();
			info.Size = Marshal.SizeOf(info);
			if (!GetMonitorInfo(monitor, out info))
			{
				Debug.LogError($"Unable to find Monitor Information for Monitor Handle: {monitor}"); // Should not ever happen.
				return false;
			}

			var width = info.MonitorRect.Right - info.MonitorRect.Left; // Support negative offsets by calculating the size, rather than just using right / bottom
			var height = info.MonitorRect.Bottom - info.MonitorRect.Top;
			var x = info.MonitorRect.Left;
			var y = info.MonitorRect.Top;

			SetWindowLong(handle, GwlStyle, standardFlags);
			SetWindowLong(handle, GwlStyle, standardFlags); // Repeat to ensure it's applied

			// Move the window after we set the style so that Unity aligns the UI Display and the UI Interaction positions
			SetWindowPos(handle, 0, x, y, width, height, SwpFrameChanged);

			standardFlags = (Int32) GetWindowLong(handle, GwlStyle); // Check to see if we still have the caption.
			return !Flags.Contains(standardFlags, WsCaption);
		}
		
		public struct MonitorInfo
		{
			public Int32 Size;
			public Rect MonitorRect;
			public Rect WorkAreaRect;
			public Int32 Flags;
		}
		
		public struct Rect
		{
			public Int32 Left;
			public Int32 Top;
			public Int32 Right;
			public Int32 Bottom;
		}

	}
}
