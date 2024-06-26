﻿using Newtonsoft.Json;

namespace Tetris
{
		public class ConsoleColorConverter : JsonConverter
		{
			public override bool CanConvert(Type objectType)
			{
				return objectType == typeof(ConsoleColor);
			}

			public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
			{
				if (reader.TokenType == JsonToken.String)
				{
					ConsoleColor result;
          if (reader.Value == null) return ConsoleColor.White;
					string colorString = (string)reader.Value;
					if (Enum.TryParse(colorString, true, out result)) return result;
					else return ConsoleColor.White;
				}
				return existingValue ?? ConsoleColor.White;
			}

			public override void WriteJson(JsonWriter writer, object? existingValue, JsonSerializer serializer)
			{
				throw new NotImplementedException();
			}
		}
}