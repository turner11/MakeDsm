#ifndef DOG_H
#define DOG_H

#include "IAnimal.h"
class Dog:public IAnimal
{
public:
	virtual std::string MakeSound();
private:

};

#endif

