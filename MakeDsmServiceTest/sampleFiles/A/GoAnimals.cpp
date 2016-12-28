#include <iostream>
#include <vector>
#include "Factory.h"
#include <iterator>
using namespace std;

int main(){
	std::vector<IAnimal*> arr;
	IAnimal* dog = Factory::GetAnimal("dog");
	IAnimal* cat = Factory::GetAnimal("cat");
	arr.push_back(dog);
	arr.push_back(cat);

	for (unsigned i = 0; i<2; i++)
	{
		IAnimal* animal = arr[i];
		std::string s = (*animal).MakeSound();
		std::cout << "I do  " << s<<"\n";//<< endl;
	}

	
	string str;
	getline(cin, str);
	//for (vector<IAnimal*>::iterator animal = arr.begin(); animal != arr.end(); animal++)
	//{
	//	std::string s = (*animal).MakeSound();
	//	std::cout << "I do  " << s;//<< endl;
	//}
    
    
    return 0;
}
