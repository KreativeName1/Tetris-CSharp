using System.ComponentModel;
  public enum Sound
	{
		[Description("Sound/gameover.mp3")]
		Gameover,

		[Description("Sound/block_land.wav")]
		Land,

		[Description("Sound/line_destroy.wav")]
		Linie,

		[Description("Sound/block_rotate.wav")]
		Drehen,

		[Description("Sound/levelup.wav")]
		Levelup,
	}

	public enum Music
	{
		[Description("Musik/theme_a.mp3")]
		ThemeA,

		[Description("Musik/theme_b.mp3")]
		ThemeB,

		[Description("Musik/theme_c.mp3")]
		ThemeC,

		[Description("Musik/1 - Music 1.mp3")]
		ThemeD,

		[Description("Musik/2 - Music 2.mp3")]
		ThemeE,

		[Description("Musik/theme_f.mp3")]
		ThemeF,

		[Description("Musik/3 - Music 3.mp3")]
		ThemeG,

		[Description("Musik/title.mp3")]
		Title
	}

  public class AudioFiles {

  public static string? GetEnumDescription(Enum value)
		{
			var fi = value.GetType().GetField(value.ToString());
      if (fi == null) return null;
			var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
			return attributes.Length > 0 ? attributes[0].Description : value.ToString();
		}
  }
