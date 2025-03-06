#pragma once
#include "..\Components\ComponentsCommon.h"
#include "TransformComponent.h"
#include "ScriptComponent.h"

namespace d3d {


	namespace game_entity {

		DEFINE_TYPED_ID(entity_id);


		class entity {
		public:
			constexpr explicit entity(entity_id id) : _id{ id } {}
			constexpr entity() : _id{ id::invalid_id } {}
			constexpr entity_id get_id() const { return _id; }
			constexpr bool is_valid() const { return id::is_valid(_id); }

			//这里把需要的component加到里面俩
			transform::component transform() const;
			script::component script() const;

		private:
			entity_id _id;
		};
	}
	//namespace script
	namespace script
	{
		class entity_script : public game_entity::entity
		{
		public:
			virtual ~entity_script() = default;
			virtual void begin_play() {}
			virtual void update(float){}
			
		protected:
			//创建script的构造函数，绑定对应的entity
			constexpr explicit entity_script(game_entity::entity entity)
				: game_entity::entity{entity.get_id()}{}
		};
		//namespace detial
		namespace detail {


			using script_ptr = std::unique_ptr<entity_script>;
			//类型别名，代表任意一个返回script_ptr并且接受game entity的函数指针
			using script_creator = script_ptr(*)(game_entity::entity entity);

			using string_hash = std::hash<std::string>;

			u8 register_script(size_t, script_creator);

;			template<class script_class>
			script_ptr create_script(game_entity::entity entity)
			{
				assert(entity.is_valid());
				//创建一个script实例，然后返回他的指针
				return std::make_unique<script_class>(entity);

			}
		}//namespace detail
		//宏定义，注册游戏脚本的类
#define REGISTER_SCRIPT(TYPE)													\
		class TYPE;																\
		namespace {																\
		const u8 _reg_##TYPE													\
		{ d3d::script::detail::register_script(									\
				d3d::script::detail::string_hash()(#TYPE),						\
				&d3d::script::detail::create_script<TYPE>)};					\
		}
	}//namespace script
}
