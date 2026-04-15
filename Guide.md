The following guide assumes you have a basic knowledge of logic structures and a gerneral idea of programming. The language used will be C#.

The goal of this guide is to build a basic inventory system you can interact with.
This will help you learn about libraries, classes, data types and code organization.

We start with a basic C# file structure.
```
class Guide
{
    static void Main()
    {
        Console.WriteLine("Hello, World!");
    }
}
```
What this means is that we have our base Class, which all of our current program in contained in. It is important that this is the same name as your
file that you're writing the program in. Next we have our first Function, which is called when the program is run. This then runs our first line of real
code, `Console.WriteLine("Hello, World!");`. This writes a line of text to the console. It writes whater ever you pass as an *argument*, in this case the
*string* `"Hello, World!"`.