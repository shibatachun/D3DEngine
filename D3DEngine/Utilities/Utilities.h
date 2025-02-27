#pragma once

#define USE_STL_VECTOR 1
#define USE_STL_DEQUE 1

#if USE_STL_VECTOR
#include <algorithm>
#include <vector>
namespace d3d::utl {
	template<typename T>
	using vector = std::vector<T>;

	template<typename T>
	//交换并且抹除，交换最后一个元素到当前元素，然后把最后一个元素pop back。
	void erase_unordered(std::vector<T>& v, size_t index)
	{
		if (v.size() > 1)
		{
			std::iter_swap(v.begin() + index, v.end() - 1);
			v.pop_back();
		}
		else
		{
			v.clear();
		}
	}
}
#endif


#if USE_STL_DEQUE
#include <deque>
namespace d3d::utl {
	template<typename T>
	using deque = std::deque<T>;
}
#endif

namespace d3d::utl {
	//TODO: implement our own containers
}
