using System;


namespace XCom
{
	/// <summary>
	/// A <c>class</c> that associates any object with a readable label.
	/// </summary>
	/// <remarks>This could be deleted and done w/ <c>Tuple</c> or
	/// <c>Dictionary</c> or <c>HashTable</c> or <c>SortedSet</c> or whatever.
	/// Hence <c>Pterodactyl</c>. Because it should go the way of the dinosaurs.</remarks>
	public class Pterodactyl
	{
		#region Fields
		/// <summary>
		/// A string, preferably readable.
		/// </summary>
		private readonly string _label;
		#endregion Fields


		#region Properties
		private readonly object _o;
		/// <summary>
		/// Anything - it's boxed.
		/// </summary>
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
		#endregion Methods (override)
	}
}
