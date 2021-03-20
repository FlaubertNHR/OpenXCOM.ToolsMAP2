namespace XCom
{
	/// <summary>
	/// A struct that associates any object with a readable label.
	/// @note This could be deleted and done w/ Tuple or Dictionary or HashTable
	/// or SortedSet or whatever. Hence 'Pterodactyl'. Because it should go the
	/// way of the dinosaurs.
	/// </summary>
	public struct Pterodactyl
	{
		#region Fields
		/// <summary>
		/// A string, preferably readable.
		/// </summary>
		private readonly string _label;
		#endregion Fields


		#region Properties
		/// <summary>
		/// Anything - it's boxed.
		/// </summary>
		private readonly object _o;
		public object O
		{
			get { return _o; }
		}
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="label"></param>
		/// <param name="o"></param>
		internal Pterodactyl(string label, object o)
		{
			_label = label;
			_o = o;
		}
		#endregion cTor


		#region Methods (override)
		/// <summary>
		/// Returns the string-value of the boxed-value.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return _label;
		}

		// TODO: override Equals()
		// TODO: override operators == and !=
		#endregion Methods (override)
	}
}
