using System;
namespace WURFL
{
	public interface ICache<K, V>
	{
		int Size
		{
			get;
		}
		V Get(K key);
		void Put(K key, V value);
		void Remove(K key);
		void Clear();
	}
}
