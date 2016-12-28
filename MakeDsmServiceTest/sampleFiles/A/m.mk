all: GoAnimals

GoAnimals: GoAnimals.o Factory.o Dog.o Cat.o IAnimal.o
    g++ GoAnimals.o Factory.o Dog.o Cat.o IAnimal.o -o GoAnimals

GoAnimals.o: GoAnimals.cpp Factory.o
    g++ -c GoAnimals.cpp Factory.o 

Factory.o: Factory.cpp IAnimal.o Dog.o Cat.o
    g++ -c Factory.cpp IAnimal.o Dog.o Cat.o

Dog.o: Dog.cpp IAnimal.o
    g++ -c Dog.cpp IAnimal.o

Cat.o: Cat.cpp IAnimal.o
    g++ -c Cat.cpp IAnimal.o

IAnimal.o: IAnimal.cpp
    g++ -c IAnimal.cpp

clean:
    del *o GoAnimals