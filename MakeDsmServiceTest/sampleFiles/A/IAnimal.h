#ifndef IANIMAL_H
#define IANIMAL_H
#include <string>
class IAnimal
{
public:
	virtual std::string MakeSound() = 0;
	std::string MakeGrrrrr();
private:

};
#endif