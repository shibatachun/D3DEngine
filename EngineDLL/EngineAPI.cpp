
#include "Common.h"
#include "CommonHeaders.h"
#include "..\D3DEngine\Components\Script.h"


#ifndef WIN32_MEAN_AND_LEAN
#define WIN32_MEAN_AND_LEAN
#endif // !WIN32_MEAN_AND_LEAN

#include <Windows.h>

using namespace d3d;
namespace {
	HMODULE game_code_dll{ nullptr };

	using _get_script_creator = d3d::script::detail::script_creator(*)(size_t);
	_get_script_creator get_script_creator{ nullptr };
	using _get_script_names = LPSAFEARRAY(*)(void);
	_get_script_names get_script_names{ nullptr };
}
EDITOR_INTERFACE u32
LoadGameCodeDll(const char* dll_path)
{
	if (game_code_dll) return 0;
	game_code_dll = LoadLibraryA(dll_path);
	assert(game_code_dll);

	get_script_creator = (_get_script_creator)GetProcAddress(game_code_dll, "get_script_creator");
	get_script_names = (_get_script_names)GetProcAddress(game_code_dll, "get_script_names");
	return (game_code_dll && get_script_creator && get_script_names) ? TRUE : FALSE;
}

EDITOR_INTERFACE u32
UnloadGameCodeDll()
{
	if (!game_code_dll) return FALSE;
	assert(game_code_dll);
	int result{ FreeLibrary(game_code_dll) };
	assert(result);
	game_code_dll = nullptr;
	return TRUE;
}

EDITOR_INTERFACE script::detail::script_creator
GetScriptCreator(const char* name)
{
	return (game_code_dll && get_script_creator) ? get_script_creator(script::detail::string_hash()(name)) : nullptr;
}

EDITOR_INTERFACE LPSAFEARRAY
GetScriptNames()
{
	auto names = (game_code_dll && get_script_names) ? get_script_names() : nullptr;
	return names;
}