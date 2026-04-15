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
What this means is that we have our base *class*, which all of our current program in contained in. It is important that this is the same name as your file that you're writing the program in. Next we have our first *function*, which is called when the program is run. This then runs our first line of real code, `Console.WriteLine("Hello, World!");`. This writes a line of text to the console. It writes whater ever you pass as an *argument*, in this case the *string* `"Hello, World!"`.

However, the goal of this guide is to use a *library* that goes by the name of Spectre.Console, which is used to help display elaborate layouts to the console. So we are now going to tell the program that we are using the library. At the top of the file, we now write `using Spectre.Console;`. So your project now should look like the following:
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
So now we can finally start using out library. But how do we access the new *functions* that are now available to us? You can access attributes and functions of a given object by using a period (.) immediately after a word. For example, in our line `Console.WriteLine("Hello, World!");` we are accessing the console, and then saying, use the function "WriteLine" to write whatever you put in the parentheses. And because we write at the top that we're using `Spectre.Console`, we don't have to reference that explicitly.

so now to use our first function we can replace our WriteLine with `AnsiConsole.MarkupLine("Hello, World!");`. This does a very similar thing, just using out new library. We are accessing the AnsiConsole and telling it to write a Markup Line, which has extra capabilities like color and style. To make what we just wrote colorful, we can just add determiners to the string. `"Hello, World!"` becomes `"[green]Hello, World![/]"` to make the text green. It's very simple. In brackets, add the color you want to change to, and when you're done, add a forward slash (/) in brackets to stop the color.

Now we are going to start the basis of out inventory system. We are going to create a *List* of items for out inventory. Our inventory right now is just going to be a list of item names. the names will be stored in a *data type* called a *string*. You can think of it as a string of letters. So to create our first item name, we need to tell the program the variable type, a string, the name, which can be whetever you want, and the value, the actual name of your item. For example: `string item1 = "Wooden Sword";`. We'll do that a few times so we can have things to put in out inventory.
```
using Spectre.Console;

class Guide
{
    static void Main()
    {
        string item1 = "Wooden Sword";
        string item2 = "Iron Shield";
        string item3 = "Leather Breastplate";
        string item4 = "Health Potion";
    }
}
```
Now we need to put these in a *List* for our inventory. To do this, its similar to a normal variable, but we also have to tell the list what *type* it will be storing. So to do this, we write `List<string> inventory = new();`. What this does is create a list to store *string*s, the list is named inventory, and it's empty as denoted by the `new()`. This tells the program to create a list with nothing in it.

However there are multiple ways to add things to a list. When it starts empty, we can use the Add function which adds the specified variable to the end of the list. This is done like so: `inventory.Add(item1);`. We say, using the inventory variable, add item1 to the end of the list. you can then do this for every item you created. Another way to add items is to add them when the list is created.

Instead of using `new()` with no arguments to create the list, we can specify variables to use, seperated by commas.
```
List<string> inventory = new()
{
    item1,
    item2,
    item3,
    item4
};
```
By adding braces after the parenteses, we can specify what we want the list to start as.

Now we need to create another list for items and weapons we actually have equipped. This one will start empty. So we will create it much like we did for the first inventory one.
`List<string> equipped = new();`

Now the fun part starts. We will now be using Spectre to display things. We will be using a table to display our two lists side by side. We will start by making a new table variable
like we did for the lists. `Table inventoryTable = new Table();` If you'd like, you can name this inventoryTable variable anything you want, like you can for any other variable. Their names do not matter, just as long as you remember them. We can now manipulate this like we can do for lists, just using Spectre's functions. We are going to start by creating two columns in our table, one titled "Inventory" and the other titled "Equipped".

This can be done by accessing our table variable and calling the AddColumn function. For example: `inventoryTable.AddColumn("Inventory");` this adds a column to our table titles "Inventory. You can now the the other column on your own, name it "Equipped" or whatever you prefer.

If you'd like to see your progress so far, you can write the line `AnsiConsole.Write(inventoryTable);` which writes the table we've just created to the console.

Now we have to display our lists. Hopefully you're already familiar with the repetition structure because we'll be using loops for this. We will be using a foreach loop which does what it sounds like. It goes through every item in a list and for each item, performs a set of instructions. We'll start the loop with `foreach(string item in inventory) {}`. What this does is say that for each variable of the type string, in the list inventory, name it item. so now on every iteration, the current variable will always be named item, which is convienent. Now in the braces, we'll put our logic.

In the braces we can add the statement `inventoryTable.AddRow(item, item);`. This will, for every item in inventory, add a new row to our table, and the two arguments are what will go in the columns. In the first column, we will display item, which is a string. Right now both columns will display the item, next we will check if it's equipped. for now the program looks something like this:
```
using Spectre.Console;

class Guide
{
    static void Main()
    {
        string item1 = "Wooden Sword";
        string item2 = "Iron Shield";
        string item3 = "Leather Breastplate";
        string item4 = "Health Potion";

        List<string> inventory = new()
        {
            item1,
            item2,
            item3,
            item4
        };

        List<string> equipped = new();

        Table inventoryTable = new Table();

        inventoryTable.AddColumn("Inventory");
        inventoryTable.AddColumn("Equipped");

        foreach(string item in inventory)
        {
            inventoryTable.AddRow(item, item);
        }

        AnsiConsole.Write(inventoryTable);
    }
}
```
But the next step is to add logic to check if an item is equipped. In our loop, but above the statement to add rows, we will add another variable. It will be a string named col2Text. I initialized it as such: `string col2Text = "";`. this means it exists, but is empty. We will use it to store a string that tells us wether items are equipped or not. Immediately after that statement, we will write an if statement. Since we want to check if `item` is in `equipped`, we we write just that. `if(equipped.Contains(item)) {}` This means that we reference the `equipped` list, and check to see if it contains the item. If it's true, it will execute the if statement. The correct code to execute would be `col2Text = "Equipped";`. And if it's not true, we can set `col2Text = "Not Equipped";`

Now that we've done that, we can replace the variable in the second column of AddRow so it reads `inventoryTable.AddRow(item, col2Text);`. The whole loop should now look like this:
```
foreach(string item in inventory)
{
    string col2Text = "";
    if(equipped.Contains(item))
    {
        col2Text = "Equipped";
    }
    else
    {
        col2Text = "Not Equipped";
    }
    inventoryTable.AddRow(item, col2Text);
}
```
If you run this, you've now built a small inventory system where you can tell which items are equipped and which aren't. Of course, right now there's nothing in the `equipped` list. Let's change that. Starting at the line `Table inventoryTable = new Table();`, we are going to be encasing everything in a while loop. this is so that we can keep asking for input to change the lists. We are also going to clear the screen every loop and so we'll add `AnsiConsole.Clear();` to the top of the loop. You should now have something along these lines:
```
using Spectre.Console;

class Guide
{
    static void Main()
    {
        string item1 = "Wooden Sword";
        string item2 = "Iron Shield";
        string item3 = "Leather Breastplate";
        string item4 = "Health Potion";

        List<string> inventory = new()
        {
            item1,
            item2,
            item3,
            item4
        };

        List<string> equipped = new();
        while(true)
        {
            AnsiConsole.Clear();

            Table inventoryTable = new Table();

            inventoryTable.AddColumn("Inventory");
            inventoryTable.AddColumn("Equipped");

            foreach(string item in inventory)
            {
                string col2Text = "";
                if(equipped.Contains(item))
                {
                    col2Text = "Equipped";
                }
                else
                {
                    col2Text = "Not Equipped";
                }
                inventoryTable.AddRow(item, col2Text);
            }

            AnsiConsole.Write(inventoryTable);
        }
    }
}
```
Now we are going to start adding things after `AnsiConsole.Write(inventoryTable);`, but still in the while loop. Spectre has a powerful feature that allows you to give the user prompts and allow the user to make selections. We are going to create a new variable to hold our SelectionPrompt. We do this by writing `var prompt = new SelectionPrompt<string>();`. this gives a new SelectionPrompt object that was can interact with, named prompt.

Now we need to add choices to our prompt, and we want the choices to be the items in our inventory so we can select them to equip them. To acheive this, we'll do another foreach loop. In this loop, we need to add a choice to our prompt. We do this through the use of `AddChoice` and in out case specifically it's `prompt.AddChoice(item);`. so we should have something along the lines of
```
foreach(string item in inventory)
{
    prompt.AddChoice(item);
}
```
Now we need to store the user's choice in something and actually ask them the question, all we've done so far is create a prompt that we can display whenever. To do this we create a variable to store the choice, `var choice = ` and to ask the question we pass out `prompt` to `AnsiConsole.Prompt(prompt);`. So now our variable assignment looks like `var choice = AnsiConsole.Prompt(prompt);`

so now we have the user's choice, which will be the weapon they've selected to equip. Now we could just add it to the `equipped` list immediately but what if it's already there? We dont want to equip it more than once. So we need to use an if statement to check wether or not it's already equipped. So we just have to check `if(!equipped.Contains(choice))`. The exclamation point there means 'not', so if `equipped` does *not* contain `choice` then we can add it using `equipped.Add(choice);`.

That should complete this basic program. After you equip the item, it restarts the loop and does it again. After experimenting though, you might realize there's nothing to do once you equip everything. I challenge you to use what you've already learned to unequip an item if it is equipped. hints are below if necessary.



Hint 1: Turn the final if statement into an if-else.
Hint 2: There's .Add for lists, there's also .Remove