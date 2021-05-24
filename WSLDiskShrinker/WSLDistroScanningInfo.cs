using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;

namespace WSLDiskShrinker
{
	[JsonObject(MemberSerialization.OptIn)]
	class WSLDistroScanningInfo
	{
		[JsonProperty("identifier")]
		public string Identifier { get; set; }
		[JsonProperty("icon")]
		public PackIconKind Icon { get; set; }

		[JsonProperty("keywords")]
		public string[] Keywords { get; set; }

		[JsonProperty("paths")]
		public string[] Paths { get; set; }

	}
}
