using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;


namespace DSShared
{
	/// <summary>
	/// Provides a method for performing a deep copy of an object. Binary
	/// Serialization is used to perform the copy.
	/// Reference articles:
	/// http://www.codeproject.com/KB/tips/SerializedObjectCloner.aspx
	/// https://stackoverflow.com/questions/78536/deep-cloning-objects/78612#78612
	/// </summary>
	public static class ObjectCopier
	{
		/// <summary>
		/// Performs a deep clone of a specified serializable object.
		/// </summary>
		/// <typeparam name="T">the type of object being cloned</typeparam>
		/// <param name="src">the object to clone</param>
		/// <returns>the cloned object</returns>
		/// <exception cref="ArgumentException"><typeparamref name="T"/> is not
		/// serializable</exception>
		/// <remarks>If <paramref name="src"/> is <c>null</c> a default instance
		/// of <typeparamref name="T"/> is returned.
		/// 
		/// 
		/// See also <c>XCom.SpriteLoader.Clone()</c> - for whatever reason
		/// they're not interchangeable.</remarks>
		public static T Clone<T>(T src)
		{
			if (!typeof(T).IsSerializable)
			{
				throw new ArgumentException("The type must be serializable.", "src");
			}

			// don't serialize a null object - return the default for that object
			if (Object.ReferenceEquals(src, null))
			{
				return default(T);
			}

			IFormatter bf = new BinaryFormatter();
			using (Stream str = new MemoryStream())
			{
				bf.Serialize(str, src);
				str.Seek(0, SeekOrigin.Begin);
				return (T)bf.Deserialize(str);
			}
		}
	}
}
