#ifndef CAT_H
#define CAT_H
#include "IAnimal.h"
class Cat:public IAnimal
{
public:
	virtual std::string MakeSound();
	
private:

};
#endif

