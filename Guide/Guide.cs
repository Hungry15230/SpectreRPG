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

            var prompt = new SelectionPrompt<string>();

            foreach(string item in inventory)
            {
                prompt.AddChoice(item);
            }

            var choice = AnsiConsole.Prompt(prompt);

            if(!equipped.Contains(choice))
            {
                equipped.Add(choice);
            }
            else
            {
                equipped.Remove(choice);
            }
        }
    }
}