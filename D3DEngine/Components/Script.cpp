#include "Script.h"

namespace d3d::script {

	//对script实现double indice
	namespace {
		utl::vector<detail::script_ptr>		entity_scripts;
		utl::vector<id::id_type>			id_mapping;
		utl::vector<id::generation_type>	generations;
		utl::vector<script_id>				free_ids;

		using script_registry = std::unordered_map<size_t, detail::script_creator>;

		script_registry& registery()
		{
			//设置为static变量，我们可以确保数据被访问之前以及被初始化
			static script_registry reg;
			return reg;
		}
		
		bool exists(script_id id)
		{
			//先看id是否有效
			assert(id::is_valid(id));
			//获取index部分
			const id::id_type index{ id::index(id) };

			assert(index < generations.size() && id_mapping[index] < entity_scripts.size());
			assert(generations[index] == id::generation(id));
			return (generations[index] == id::generation(id) &&
				entity_scripts[id_mapping[index]] &&
				entity_scripts[id_mapping[index]]->is_valid());
		}
	}

	namespace detail
	{
		u8 register_script(size_t tag, script_creator func)
		{
			bool result{ registery().insert(script_registry::value_type{tag,func}).second };
			assert(result);
			return result;
		}

	}// namespace detail

	component create(init_info info, game_entity::entity entity)
	{
		assert(entity.is_valid());
		assert(info.script_creator);

		script_id id{};
		if (free_ids.size() > id::min_deleted_elements)
		{
			id = free_ids.front();
			
			assert(!exists(id));
			free_ids.pop_back();

			id = script_id{ id::new_generation(id) };
			++generations[id::index(id)];

		}
		else
		{
			id = script_id{ (id::id_type)id_mapping.size() };
			id_mapping.emplace_back();
			generations.push_back(0);
		}
		assert(id::is_valid(id));
		const id::id_type index{ (id::id_type)entity_scripts.size() };
		//加到script里面去，在后面
		entity_scripts.emplace_back(info.script_creator(entity));
		//看看是否和entity的id一样
		assert(entity_scripts.back()->get_id() == entity.get_id());
		
		
		id_mapping[id::index(id)] = index;

	

		return component{ id };
	}

	
	void remove(component c)
	{
		assert(c.is_valid() && exists(c.get_id()));
		//获取script id
		const script_id id{ c.get_id() };
		//获取index,因为id_mapping需要与总id同步，这里的index指的是entity_scripts的index。id_mapping中有对应entity scripts中的index。
		const id::id_type index{ id_mapping[id::index(id)] };
		//获取最后一个script的script id
		const script_id last_id{ entity_scripts.back()->script().get_id() };
		utl::erase_unordered(entity_scripts, index);
		//交换之后要把最后id mapping的id指向script
		id_mapping[id::index(last_id)] = index;
		id_mapping[id::index(id)] = id::invalid_id;
	}

}
