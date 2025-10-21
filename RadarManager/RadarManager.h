#pragma once
#define LIG_DLL_EXPORT

#ifdef LIG_DLL_EXPORT
#define LIG_DLL_EXPORT __declspec(dllexport)
#else
#define LIG_DLL_EXPORT __deslspec(dllimport)
#endif

class LIG_DLL_EXPORT RadarManager
{
public:
	RadarManager();
};