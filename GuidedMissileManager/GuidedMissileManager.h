#pragma once

#define LIG_DLL_EXPORT

#ifdef LIG_DLL_EXPORT
#define LIG_DLL_EXPORT __declspec(dllexport)
#else
#define LIG_DLL_EXPORT __deslspec(dllimport)
#endif

class LIG_DLL_EXPORT GuidedMissileManager
{
public:
	GuidedMissileManager();
};


class GuidedMissile
{
	int id;
	int start_x;
	int start_y;
	int end_x;
	int end_y;
	int speed;

	int current_x;
	int current_y;

	bool is_detected;
	bool is_touched;
	bool is_launched;
	bool is_exploded;

public:
	GuidedMissile(int id, int start_x, int start_y, int end_x, int end_y, int speed, 
				  int current_x, int current_y, bool is_detected, bool is_touched, bool is_launched, bool is_exploded);
	void UpdatePosition();
	bool GetbTouchedRadarRange(int x, int y);
};