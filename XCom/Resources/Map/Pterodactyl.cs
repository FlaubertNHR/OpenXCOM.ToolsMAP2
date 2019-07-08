namespace XCom
{
	/// <summary>
	/// A struct that associates an enumerated case with a readable string.
	/// </summary>
	public struct Pterodactyl
	{
		/// <summary>
		/// An enumerated case.
		/// </summary>
		private readonly object _case;
		public object Case
		{
			get { return _case; }
		}

		/// <summary>
		/// A readable string.
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
		/// Returns the string-value of the case-value.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return _st;
		}
	}
}
