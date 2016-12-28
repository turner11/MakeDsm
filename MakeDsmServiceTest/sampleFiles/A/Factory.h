#ifndef FACTORY_H
#define FACTORY_H

#include "IAnimal.h"


class Factory
{
public:
	static IAnimal* GetAnimal(std::string name);


};



#endif