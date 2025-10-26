#include <iostream>
#include "GuidedMissileManager.h"

GuidedMissileManager::GuidedMissileManager()
{
}

GuidedMissile::GuidedMissile(int id, int start_x, int start_y, int end_x, int end_y, int speed, int current_x, int current_y, bool is_detected, bool is_touched, bool is_launched, bool is_exploded)
{
	this->id = id;
	this->start_x = start_x;
	this->start_y = start_y;
	this->end_x = end_x;
	this->end_y = end_y;
	this->speed = speed;
	this->current_x = current_x;
	this->current_y = current_y;
	this->is_detected = is_detected;
	this->is_touched = is_touched;
	this->is_launched = is_launched;
	this->is_exploded = is_exploded;
}
