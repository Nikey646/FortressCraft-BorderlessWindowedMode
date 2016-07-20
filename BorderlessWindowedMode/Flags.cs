using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BorderlessWindowedMode
{
	// Source: http://www.thewildeternal.com/2014/09/21/devlog-resolution-controller/
	internal static class Flags
	{
		public static void Set<T>(ref T mask, T flag) where T : struct
		{
			var maskValue = (Int32) (object) mask;
			var flagValue = (Int32) (object) flag;

			mask = (T) (object) (maskValue | flagValue);
		}

		private static void UnsetInternal<T>(ref T mask, T flag) where T : struct
		{
			var maskValue = (Int32) (object) mask;
			var flagValue = (Int32) (object) flag;

			mask = (T) (object) (maskValue & ~flagValue);
		}

		public static void Unset<T>(ref T mask, params T[] flags) where T : struct
		{
			foreach (var flag in flags)
			{
				UnsetInternal(ref mask, flag);
			}
		}

		public static void Unset<T>(ref T mask, params object[] flags) where T : struct
		{
			foreach (var flag in flags)
			{
				if (flag is T)
					UnsetInternal(ref mask, (T) flag);
			}
		}

		public static void Toggle<T>(ref T mask, T flag) where T : struct
		{
			if (Contains(mask, flag))
				Unset(ref mask, flag);
			else
				Set(ref mask, flag);
		}

		public static bool Contains<T>(T mask, T flag) where T : struct
		{
			return Contains((Int32) (object) mask, (Int32) (object) flag);
		}

		public static bool Contains(Int32 mask, Int32 flag)
		{
			return (mask & flag) != 0;
		}
	}

}
