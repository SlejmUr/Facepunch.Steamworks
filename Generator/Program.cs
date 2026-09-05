using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Generator
{
    class Program
    {
		public static SteamApiDefinition Definitions;

		static void Main( string[] args )
        {
            var content = System.IO.File.ReadAllText( "steam_sdk/steam_api.json" );
            var def = Newtonsoft.Json.JsonConvert.DeserializeObject<SteamApiDefinition>( content );

			Definitions = def;

			Directory.CreateDirectory( "../Facepunch.Steamworks" );
			Directory.CreateDirectory( "../Facepunch.Steamworks/Generated" );
			Directory.CreateDirectory( "../Facepunch.Steamworks/Generated/Interfaces" );

			var generator = new CodeWriter( def );
			
            generator.ToFolder( "../Facepunch.Steamworks/Generated/" );
        }
    }
}


