namespace AltLibrary.Common.AltBiomes {
	public class AltMaterialContext {
		public int EvilOre { get; set; } = -1;
		public int EvilBar { get; set; } = -1;
		public int LightBar { get; set; } = -1;
		public int UnderworldBar { get; set; } = -1;
		public int TropicalBar { get; set; } = -1;
		public int MushroomBar { get; set; } = -1;
		public int EvilSword { get; set; } = -1;
		public int LightSword { get; set; } = -1;
		public int UnderworldSword { get; set; } = -1;
		public int TropicalSword { get; set; } = -1;
		public int CombinationSword { get; set; } = -1;
		public int TrueCombinationSword { get; set; } = -1;
		public int TrueLightSword { get; set; } = -1;
		public int VileInnard { get; set; } = -1;
		public int LightResidue { get; set; } = -1;
		public int LightInnard { get; set; } = -1;
		public int LightComponent { get; set; } = -1;
		public int VileComponent { get; set; } = -1;
		public int EvilBossDrop { get; set; } = -1;
		public int TropicalComponent { get; set; } = -1;
		public int EvilHerb { get; set; } = -1;
		public int UnderworldHerb { get; set; } = -1;
		public int TropicalHerb { get; set; } = -1;
		public int UnderworldForge { get; set; } = -1;

		public AltMaterialContext() { }

		/// <summary>
		/// For Evil alts.<br/>Vanilla values: Demonite Ore, Crimtane Ore
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public AltMaterialContext SetEvilOre(int value) {
			EvilOre = value;
			return this;
		}

		/// <summary>
		/// For Evil alts.<br/>Vanilla values: Demonite Bar, Crimtane Bar
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public AltMaterialContext SetEvilBar(int value) {
			EvilBar = value;
			return this;
		}

		/// <summary>
		/// For Hallow alts.<br/>Vanilla values: Hallowed Bar
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public AltMaterialContext SetLightBar(int value) {
			LightBar = value;
			return this;
		}

		/// <summary>
		/// For Underworld alts.<br/>Vanilla values: Hellstone Bar
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public AltMaterialContext SetUnderworldBar(int value) {
			UnderworldBar = value;
			return this;
		}

		/// <summary>
		/// For Jungle alts.<br/>Vanilla values: Chlorophyte Bar
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public AltMaterialContext SetTropicalBar(int value) {
			TropicalBar = value;
			return this;
		}

		/// <summary>
		/// For Jungle alts.<br/>Vanilla values: Shroomite Bar
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public AltMaterialContext SetMushroomBar(int value) {
			MushroomBar = value;
			return this;
		}

		/// <summary>
		/// For Evil alts.<br/>Vanilla values: Light's Bane, Blood Butcherer
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public AltMaterialContext SetEvilSword(int value) {
			EvilSword = value;
			return this;
		}

		/// <summary>
		/// For Hallow alts.<br/>Vanilla values: Excalibur
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public AltMaterialContext SetLightSword(int value) {
			LightSword = value;
			return this;
		}

		/// <summary>
		/// For Underworld alts.<br/>Vanilla values: Fiery Greatsword
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public AltMaterialContext SetUnderworldSword(int value) {
			UnderworldSword = value;
			return this;
		}

		/// <summary>
		/// For Jungle alts.<br/>Vanilla values: Blade of Grass
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public AltMaterialContext SetTropicalSword(int value) {
			TropicalSword = value;
			return this;
		}

		/// <summary>
		/// For Evil alts.<br/>Vanilla values: Night's Edge
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public AltMaterialContext SetCombinationSword(int value) {
			CombinationSword = value;
			return this;
		}

		/// <summary>
		/// For Evil alts.<br/>Vanilla values: True Night's Edge
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public AltMaterialContext SetTrueCombinationSword(int value) {
			TrueCombinationSword = value;
			return this;
		}

		/// <summary>
		/// For Hallow alts.<br/>Vanilla values: True Excalibur
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public AltMaterialContext SetTrueLightSword(int value) {
			TrueLightSword = value;
			return this;
		}

		/// <summary>
		/// For Evil alts.<br/>Vanilla values: Rotten Chunk, Vertebrae
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public AltMaterialContext SetVileInnard(int value) {
			VileInnard = value;
			return this;
		}

		/// <summary>
		/// For Hallow alts.<br/>Vanilla values: Pixie Dust
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public AltMaterialContext SetLightResidue(int value) {
			LightResidue = value;
			return this;
		}

		/// <summary>
		/// For Hallow alts.<br/>Vanilla values: Unicorn Horn
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public AltMaterialContext SetLightInnard(int value) {
			LightInnard = value;
			return this;
		}

		/// <summary>
		/// For Hallow alts.<br/>Vanilla values: Crystal Shards
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public AltMaterialContext SetLightComponent(int value) {
			LightComponent = value;
			return this;
		}

		/// <summary>
		/// For Evil alts.<br/>Vanilla values: Cursed Flame, Ichor
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public AltMaterialContext SetVileComponent(int value) {
			VileComponent = value;
			return this;
		}

		/// <summary>
		/// For Evil alts.<br/>Vanilla values: Shadow Scale, Tissue Sample
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public AltMaterialContext SetEvilBossDrop(int value) {
			EvilBossDrop = value;
			return this;
		}

		/// <summary>
		/// For Jungle alts.<br/>Vanilla values: Jungle Spores
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public AltMaterialContext SetTropicalComponent(int value) {
			TropicalComponent = value;
			return this;
		}

		/// <summary>
		/// For Evil alts.<br/>Vanilla values: Deathweed
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public AltMaterialContext SetEvilHerb(int value) {
			EvilHerb = value;
			return this;
		}

		/// <summary>
		/// For Underworld alts.<br/>Vanilla values: Fireblossom
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public AltMaterialContext SetUnderworldHerb(int value) {
			UnderworldHerb = value;
			return this;
		}

		/// <summary>
		/// For Jungle alts.<br/>Vanilla values: Moonglow
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public AltMaterialContext SetTropicalHerb(int value) {
			TropicalHerb = value;
			return this;
		}

		/// <summary>
		/// For Underworld alts.<br/>Vanilla values: Hellforge
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public AltMaterialContext SetUnderworldForge(int value) {
			UnderworldForge = value;
			return this;
		}
	}
}
