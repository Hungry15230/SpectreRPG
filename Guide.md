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
What this means is that we have our base *class*, which all of our current program in contained in. It is important that this is the same name as your
file that you're writing the program in. Next we have our first *function*, which is called when the program is run. This then runs our first line of real
code, `Console.WriteLine("Hello, World!");`. This writes a line of text to the console. It writes whater ever you pass as an *argument*, in this case the
*string* `"Hello, World!"`.

However, the goal of this guide is to use a *library* that goes by the name of Spectre.Console, which is used to help display elaborate layouts to the console.
So we are now going to tell the program that we are using the library. At the top of the file, we now write `using Spectre.Console;`. So your project now should
look like the following:
```
using Spectre.Console;

class Guide
{
    static void Main()
    {
        Console.WriteLine("Hello, World!");
    }
}
```
So now we can finally start using out library. But how do we access the new *functions* that are now available to us? You can access attributes and functions of a given
object by using a period (.) immediately after a word. For example, in our line `Console.WriteLine("Hello, World!");` we are accessing the console, and then saying, use the function "WriteLine"
to write whatever I put in the parentheses. And because we write at the top that we're using `Spectre.Console`, we don't have to reference that explecitely.

so now to use our first function we can replace our WriteLine with `AnsiConsole.MarkupLine("Hello, World!");`. This does a very similar thing, just using out new library.
We are accessing the AnsiConsole and telling it to write a Markup Line, which has extra capabilities like color and style. To make what we just wrote colourful, we can
just add determiners to the string. "Hello, World!" becomes "[green]Hello, World![/]" to make the text green. It's very simple. In brackets, add the colour you want to change to,
and when you're done, add a forward slash (/) in brackets to stop the colour.