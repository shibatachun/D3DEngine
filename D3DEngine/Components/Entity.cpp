#include "Entity.h"


namespace d3d::game_entity
{
//����namespace
namespace {
		
utl::vector<transform::component>		transforms;
//������¼generations��vector����Ӧ��index����id�е�index����
utl::vector<id::generation_type>		generations;
utl::vector<script::component>			scripts;
utl::deque<entity_id>					free_ids;
}


entity create(entity_info info)
{
	assert(info.transform);
	if (!info.transform) return entity{};

	entity_id id;
	//������߼���Ϊ���ǵ�generations�������255����Ϊ��8bits�� �������������ͬһ��slot��ɾ����д��εĻ���generation�Ἣ�����ӣ��������Ƕ���һ��min_deleted_elements��������1024
	//���Ƴ���Ԫ�أ�Ҳ����entity_id������������С�Ƴ������Ļ������ǲſ�ʼ����д����Щ��ɾ����slot���棬�������ǽ�ֱ����entity�����������һ��slot������ʵ��
	if (free_ids.size() > id::min_deleted_elements)
	{
		//�����ʾremove��slot�Ѿ�������Сɾ��Ԫ����
		id = free_ids.front();
		//�ٶ����entity�Ѿ����ٻ�Ծ
		assert(!is_alive( id ));
		//�����list�ĵ�һ��λ��pop�ˣ���ʾ�ظ���ʼ����
		free_ids.pop_front();
		//ʹ��new_generation��ȡһ����id
		id = entity_id{ id::new_generation(id)};
		//Ӧ���������id��¼һ�¶�Ӧindex��generation?
		++generations[id::index(id)];

	}
	else
	{
		//��û�����ɾ�������Ļ���Ū���µģ�Ȼ��generation��0��generation����Ǹ�Ԫ�غ�����һ�������µ�index.
		id = entity_id{ (id::id_type)generations.size() };
		generations.push_back(0);

		//ʹ��emplace_back��֤�ڴ�allocations���ֵ�
		transforms.emplace_back();
		scripts.emplace_back();
	}

	//��entity���캯������һ��entity
	const entity new_entity{ id };

	//����������index��ʹ������ó�
	const id::id_type index{ id::index(id) };

	//Create transform component
	assert(!transforms[index].is_valid());
	transforms[index] = transform::create(*info.transform, new_entity);
	if (!transforms[index].is_valid()) return {};

	//����Script Component

	if (info.script && info.script->script_creator)
	{
		assert(!scripts[index].is_valid());
		scripts[index] = script::create(*info.script, new_entity);
		assert(scripts[index].is_valid()); 

	}

	return new_entity; 
		
}
void remove(entity_id id)
{
	
	const id::id_type index{ id::index(id) };
	assert(is_alive(id));
	if (scripts[index].is_valid())
	{
		script::remove(scripts[index]);
		scripts[index] = {};
	}
	transform::remove(transforms[index]);
	transforms[index] = {};
	//�����id����removel��lIst
	free_ids.push_back(id);
	
}
bool is_alive(entity_id id)
{
	//���ж����ʵ���Ƿ���valid��
	assert(id::is_valid(id));
	//��ȡid�е�index
	const id::id_type index{ id::index(id) };
	//�������index��С��generations�Ĵ�С����Ϊ�����˾Ͳ��ǺϹ��index�ˣ�index�Ǹ���generations�����е�Ԫ�����ŵ�
	assert(index < generations.size());
	//������������м�¼��ǰindex��generation�Ƿ��Id�б�ʶ��generationһ��
	assert(generations[index] == id::generation(id));
	

	return (generations[index] == id::generation(id) && transforms[index].is_valid());
}

transform::component entity::transform() const
{
	assert(is_alive(_id));
	const id::id_type index{ id::index(_id) };
	
	return transforms[index];

}

script::component entity::script() const
{
	assert(is_alive(_id));
	const id::id_type index{ id::index(_id) };
	return scripts[index];
}
}

