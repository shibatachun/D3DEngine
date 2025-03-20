#include "Common.h"
#include "CommonHeaders.h"

#ifndef WIN32_MEAN_AND_LEAN
#define WIN32_MEAN_AND_LEAN
#endif // !WIN32_MEAN_AND_LEAN

#include <Windows.h>

using namespace d3d;
namespace {
	HMODULE game_code_dll{ nullptr };
}
EDITOR_INTERFACE u32
LoadGameCodeDll(const char* dll_path)
{
	if (game_code_dll) return 0;
	game_code_dll = LoadLibraryA(dll_path);
	assert(game_code_dll);

	return game_code_dll ? TRUE : FALSE;
}

EDITOR_INTERFACE u32
UnloadGameCodeDll(const char* dll_path)
{
	if (!game_code_dll) return FALSE;
	assert(game_code_dll);
	int result{ FreeLibrary(game_code_dll) };
	assert(result);
	game_code_dll = nullptr;
	return TRUE;
}

