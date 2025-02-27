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

}//匿名命名空间


component 
create(init_info info, game_entity::entity entity)
{
	assert(entity.is_valid());
	const id::id_type entity_index{ id::index(entity.get_id()) };

	//因为这些旋转数据不会像entity那样复用以前的slot，因此如果某个在position或者这些数据之前的entity被删除的话
	//要创一个新的entity需要新的transform组件，则要绑定到这个新的entity的话，因为新的entity是用回老的index
	// 因此index有可能是比这些数据的size小的，所以判断小于的话直接更新数据数组中相应的位置就行，而不需要往内存里多一个slot
	if (positions.size() > entity_index)
	{
		rotations[entity_index] = math::v4(info.rotation);
		positions[entity_index] = math::v3(info.position);
		scales[entity_index] = math::v3(info.scale);
	}
	else
	{
		//这里则是新创一个slot了，entity没有复用，这里也直接添加一个对应的transform component并添加到原有数组中去
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
