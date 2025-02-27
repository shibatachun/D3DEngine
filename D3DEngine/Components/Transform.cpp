#include "Transform.h"
#include "Entity.h"
#include "CommonHeaders.h"

namespace d3d::transform
{
namespace
{
	utl::vector< math::v3> positions;
	utl::vector< math::v4> rotations;
	utl::vector<math::v3> scales;

}//���������ռ�


component 
create(init_info info, game_entity::entity entity)
{
	assert(entity.is_valid());
	const id::id_type entity_index{ id::index(entity.get_id()) };

	//��Ϊ��Щ��ת���ݲ�����entity����������ǰ��slot��������ĳ����position������Щ����֮ǰ��entity��ɾ���Ļ�
	//Ҫ��һ���µ�entity��Ҫ�µ�transform�������Ҫ�󶨵�����µ�entity�Ļ�����Ϊ�µ�entity���û��ϵ�index
	// ���index�п����Ǳ���Щ���ݵ�sizeС�ģ������ж�С�ڵĻ�ֱ�Ӹ���������������Ӧ��λ�þ��У�������Ҫ���ڴ����һ��slot
	if (positions.size() > entity_index)
	{
		rotations[entity_index] = math::v4(info.rotation);
		positions[entity_index] = math::v3(info.position);
		scales[entity_index] = math::v3(info.scale);
	}
	else
	{
		//���������´�һ��slot�ˣ�entityû�и��ã�����Ҳֱ�����һ����Ӧ��transform component����ӵ�ԭ��������ȥ
		assert(positions.size() == entity_index);
		rotations.emplace_back(info.rotation);
		positions.emplace_back(info.position);
		scales.emplace_back(info.scale);
	}
	return component(transform_id{ entity.get_id()});
}

void 
remove(component c)
{
	assert(c.is_valid());
}
math::v4 
component::rotation() const
{
	assert(is_valid());
	return rotations[id::index(_id)];
}
math::v3 
component::position() const
{
	assert(is_valid());
	return positions[id::index(_id)];
}
math::v3 
component::scale() const
{
	assert(is_valid());
	return scales[id::index(_id)];
}

}
