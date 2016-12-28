#include "Factory.h"
#include "Cat.h"
#include "Dog.h"

#include <string>

IAnimal* Factory::GetAnimal(std::string name)
{
	IAnimal* a;
	if (name == "dog")
	{
		a = new Dog();
	}
	else
	{
		a = new Cat();
	}
	return a;
}
