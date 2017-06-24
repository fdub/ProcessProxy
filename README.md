# ProcessProxy v0.1

Prototype of library to isolate a class by creating a proxy and instantiating the class in another process.

Proxies are generated using Castle DynamicProxy, communication uses Akka.Remote. 

Disclaimer:
I recently started experimenting with Akka so do not expect a well done piece of software. It is just a raw prototype to verify the idea.