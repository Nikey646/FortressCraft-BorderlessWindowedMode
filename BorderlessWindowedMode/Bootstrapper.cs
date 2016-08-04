using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Common;
using UnityEngine;

namespace BorderlessWindowedMode
{
	public class Bootstrap : FortressCraftMod
	{
		private Ini _settings;
		private UInt64 _counter = 300;

		private void Start()
		{
			var platform = (Int32) Environment.OSVersion.Platform;
			var isOsUnix = platform == 4 || platform == 6 || platform == 128;

			if (isOsUnix || Application.platform != RuntimePlatform.WindowsPlayer)
			{
				Destroy(this); // Eject me!
				return;
			}

			var location = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "settings.ini");
			if (!File.Exists(location))
			{
				using (var fs = File.Create(location))
				using (var stream = new StreamWriter(fs))
				{
					stream.WriteLine("[BorderlessWindowedMode]");
					stream.WriteLine("PrimaryScreen=true");
				}
			}

			this._settings = new Ini(location);
			this._settings.SetSection("BorderlessWindowedMode");
		}

		private void Update()
		{
			this._counter++;

			if (this._counter % (60 * 5) == 0)
			{
				NativeMethods.TrySetBorderless(this._settings.GetBoolean("PrimaryScreen", true));
			}
		}
	}
}
