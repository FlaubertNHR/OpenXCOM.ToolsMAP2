namespace XCom
{
	/// <summary>
	/// A struct that associates an enumerated case with a readable string.
	/// @note This could be deleted and done w/ Tuple or Dictionary or HashTable
	/// or SortedSet or whatever. Hence 'Pterodactyl'. Because it should go the
	/// way of the dinosaurs.
	/// </summary>
	public struct Pterodactyl
	{
		/// <summary>
		/// An enumerated case. Actually, anything - it's boxed.
		/// </summary>
		private readonly object _case;
		public object Case
		{
			get { return _case; }
		}

		/// <summary>
		/// A string, preferably readable.
		/// </summary>
		private readonly string _st;


		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="st"></param>
		/// <param name="case"></param>
		internal Pterodactyl(string st, object @case)
		{
			_st   = st;
			_case = @case;
		}


		/// <summary>
		/// Returns the string-value of the boxed-value.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return _st;
		}
	}
}
