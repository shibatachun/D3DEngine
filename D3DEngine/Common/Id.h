 #pragma once
#include "CommonHeaders.h"

//DOD������
namespace d3d::id {
//id���������޷���32λ��������00000000,00000000,000000000,00000000, ����using�������
using id_type = u32;
//internal namespace
namespace detail {

constexpr u32 generation_bits{ 8 };
//��Ϊ������λ32λ������ֵ��4���ֽڣ���Ϊ32��4��byte��һ��byte��8bits��������Ҫ����8��Ȼ���ȥ�ոն����8λ������ʣ�µ�index��λ��������24λ
//ps:����Ҳ�����ڱ�����ʼ���������������ʺ�ֱ�ӳ�ʼ����С�����ʺϾ�������֮���ʼ���ĳ���
constexpr u32 index_bits(sizeof(id_type) * 8 - generation_bits);

//������ȡһ��id����������������е����� �� https://chatgpt.com/c/66df5460-0c80-8013-bc89-ce86096703b1
constexpr id_type index_mask{ (id_type{1} << index_bits) - 1 };
constexpr id_type generation_mask{ (id_type{1} << generation_bits) - 1 };
}
	

constexpr id_type invalid_id{ id_type(-1)};

constexpr u32 min_deleted_elements{ 1024 };

using generation_type = std::conditional_t< detail::generation_bits <= 16, std::conditional_t<detail::generation_bits <= 8, u8, u16>, u32>;

//ȷ��generation type�ܹ�����Ŷgeneration bit
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
�����µ�Generation�����ӣ� �����ǰindex��00000000 00000000 00000000 00000010�� ��ʹ��generation��ȡgeneration����ȡ���Ժ��Ǹ�8λ�����֣�����
01010110��֮������������ϱ��id�Ļ������ǵ��Ȱ����ƻظ�λ8�� ������Ҫ����Index��ô����λ����Ȼ��ͨ��|����index���кϲ�
*/
constexpr id_type
new_generation(id_type id)
{
	const id_type generation{ id::generation(id) + 1 };
	//u64,��ʾһ��64λ�޷��ŵ�������ʼֵΪ1, Ȼ�����Ʋ�������ǰgeneration��λ����8�������256��256-1�ع��8λ��00000001 00000000���00000000 11111111 ��˼��255.���ǲ�����generation����255����Ȼ�������ͻ
	assert(generation < (( (u64)1 << detail::generation_bits)-1) );
	return index(id) | (generation << detail::index_bits);
}

//��DeBugģʽ��releaseģʽ��id type������һ�����Ͱ�ȫ��ID����name�����ⲻͬ��ID����
#if _DEBUG
	//������һ���ڲ��ṹ��
	namespace detail {
		struct id_base
		{
			//�������������explicit��id_base������캯�������ڱ�����ִ�У�exlicity��ֹ��ʽת��
			constexpr explicit id_base(id_type id) : _id{ id } {}
			//һ���������ط�������ͨ��id_type����id_base����Ϊid_type
			constexpr operator id_type() const { return _id; }
		private:
			id_type _id;
		};
	}
	//�����Ϊ name ������һ�����Ͱ�ȫ�� ID �ṹ�壬�̳��� id::internal::id_base
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