#include "Entity.h"
#include "Transform.h"

namespace d3d::game_entity
{
//匿名namespace
namespace {
		
utl::vector<transform::component>		transforms;
//用来记录generations的vector，对应的index就是id中的index部分
utl::vector<id::generation_type>		generations;
utl::deque<entity_id>					free_ids;
}

entity
create_game_entity(const entity_info& info)
{
	assert(info.transform);
	if (!info.transform) return entity{};

	entity_id id;
	//这里的逻辑因为我们的generations最大数是255，因为是8bits， 所以如果我们在同一个slot里删除重写多次的话，generation会极速增加，所以我们定义一个min_deleted_elements，这里是1024
	//当移除的元素，也就是entity_id的数量大于最小移除数量的话，我们才开始重新写入这些被删除的slot里面，否则，我们将直接在entity数组后面增加一个slot来创建实体
	if (free_ids.size() > id::min_deleted_elements)
	{
		//这里表示remove的slot已经大于最小删除元素了
		id = free_ids.front();
		//假定这个entity已经不再活跃
		assert(!is_alive(entity{ id }));
		//把这个list的第一个位置pop了，表示重复开始利用
		free_ids.pop_front();
		//使用new_generation获取一个新id
		id = entity_id{ id::new_generation(id)};
		//应该是用这个id记录一下对应index的generation?
		++generations[id::index(id)];

	}
	else
	{
		//还没到最大删除数量的话，弄个新的，然后generation是0，generation最后那个元素后面面一个则是新的index.
		id = entity_id{ (id::id_type)generations.size() };
		generations.push_back(0);

		//使用emplace_back保证内存allocations保持低
		transforms.emplace_back();
	}

	//用entity构造函数返回一个entity
	const entity new_entity{ id };

	//定义了他的index，使用掩码得出
	const id::id_type index{ id::index(id) };

	//Create transform component
	assert(!transforms[index].is_valid());
	transforms[index] = transform::create_transform(*info.transform, new_entity);
	if (!transforms[index].is_valid()) return {};

	return new_entity; 


		
}

void 
remove_game_entity(entity e)
{
	const entity_id id{ e.get_id() };
	const id::id_type index{ id::index(id) };
	assert(is_alive(e));
	if (is_alive(e))
	{
		transform::remove_transform(transforms[index]);
		transforms[index] = {};
		//把这个id加入removel的lIst
		free_ids.push_back(id);
	}
	
}

bool 
is_alive(entity e)
{
	//先判断这个实体是否是valid的
	assert(e.is_valid());
	//获取其id
	const entity_id id{ e.get_id() };
	//获取id中的index
	const id::id_type index{ id::index(id) };
	//断言这个index是小于generations的大小，因为超过了就不是合规的index了，index是根据generations数组中的元素来排的
	assert(index < generations.size());
	//断言这个数组中记录当前index的generation是否和Id中标识的generation一致
	assert(generations[index] == id::generation(id));
	

	return (generations[index] == id::generation(id) && transforms[index].is_valid());
}
transform::component
entity::transform() const
{
	assert(is_alive(*this));
	const id::id_type index{ id::index(_id) };
	
	return transforms[index];

}
}

