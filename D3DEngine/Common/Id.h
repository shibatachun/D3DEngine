 #pragma once
#include "CommonHeaders.h"

//DOD编程设计
namespace d3d::id {
//id的类型是无符号32位数，比如00000000,00000000,000000000,00000000, 这里using是起别名
using id_type = u32;
//internal namespace
namespace detail {

constexpr u32 generation_bits{ 8 };
//因为被定义位32位，返回值是4个字节，因为32有4个byte，一个byte有8bits，所以需要乘以8，然后减去刚刚定义的8位，表明剩下的index的位数可以是24位
//ps:这里也是属于变量初始化，不过大括号适合直接初始化，小括号适合经过计算之后初始化的场景
constexpr u32 index_bits(sizeof(id_type) * 8 - generation_bits);

//用来提取一个id里面的索引和生成中的内容 ： https://chatgpt.com/c/66df5460-0c80-8013-bc89-ce86096703b1
constexpr id_type index_mask{ (id_type{1} << index_bits) - 1 };
constexpr id_type generation_mask{ (id_type{1} << generation_bits) - 1 };
}
	

constexpr id_type invalid_id{ id_type(-1)};

constexpr u32 min_deleted_elements{ 1024 };

using generation_type = std::conditional_t< detail::generation_bits <= 16, std::conditional_t<detail::generation_bits <= 8, u8, u16>, u32>;

//确保generation type能够容纳哦generation bit
static_assert(sizeof(generation_type) * 8 >= detail::generation_bits);

static_assert(sizeof(id_type) - sizeof(generation_type) > 0);

constexpr bool
is_valid(id_type id)
{
	
	return id != invalid_id;
}

constexpr id_type
index(id_type id)
{
	id_type index{ id & detail::index_mask };
	assert(index != detail::index_mask);
	return index;
}


constexpr id_type
generation(id_type id)
{
	return (id >> detail::index_bits) & detail::generation_mask;
}
	
/*
生成新的Generation，例子： 如果当前index是00000000 00000000 00000000 00000010， 先使用generation提取generation，提取出以后是个8位的数字，比如
01010110，之后我们重新组合变成id的话，我们得先把它移回高位8， 所以需要左移Index那么长的位数，然后通过|来与index进行合并
*/
constexpr id_type
new_generation(id_type id)
{
	const id_type generation{ id::generation(id) + 1 };
	//u64,表示一个64位无符号的数，初始值为1, 然后左移操作，当前generation的位数是8，结果是256，256-1回归第8位，00000001 00000000变成00000000 11111111 意思是255.我们不能让generation超过255，不然会引起冲突
	assert(generation < (( (u64)1 << detail::generation_bits)-1) );
	return index(id) | (generation << detail::index_bits);
}

//分DeBug模式和release模式的id type，定义一种类型安全的ID类型name，以免不同的ID混淆
#if _DEBUG
	//定义了一个内部结构体
	namespace detail {
		struct id_base
		{
			//编译过程中声明explicit，id_base这个构造函数可以在编译中执行，exlicity防止隐式转换
			constexpr explicit id_base(id_type id) : _id{ id } {}
			//一个运算重载符，可以通过id_type返回id_base对象为id_type
			constexpr operator id_type() const { return _id; }
		private:
			id_type _id;
		};
	}
	//这个宏为 name 定义了一个类型安全的 ID 结构体，继承自 id::internal::id_base
#define DEFINE_TYPED_ID(name)									\
		struct name final : id::detail::id_base				\
		{														\
			constexpr explicit name(id::id_type id)				\
				:id_base{ id } {}								\
			constexpr name() : id_base{0}{}						\
		};
#else
#define DEFINE_TYPED_ID(name) using name = id:id_type;
#endif

}