
#if !defined(SHIPPING)
#include "..\Content\ContentLoader.h"
#include "..\Components\\Script.h"
#include <thread>

bool engine_initialize()
{
	bool result{ d3d::content::load_game() };
	return result;
}
void engine_update()
{
	d3d::script::update(10.f);
	std::this_thread::sleep_for(std::chrono::milliseconds(10));
}
void engine_shutdown()
{
	d3d::content::unload_game();
}
#endif