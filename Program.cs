using System;
using System.Collections.Generic;
using Spectre.Console;
using Spectre.Console.Rendering;

// Game Modes
enum GameMode
{
    Actions,
    Inventory,
    ItemInteraction
}

// Item Types
enum ItemType
{
    Consumable,
    Weapon,
    Offhand,
    Helmet,
    Chestplate,
    Gloves,
    Greaves,
    Boots,
    Accessory
}

// Stats System
public class Stats
{
    public int Attack { get; set; } = 0;
    public int Defense { get; set; } = 0;
    public int Health { get; set; } = 0;
    public int Agility { get; set; } = 0;
    public int Luck { get; set; } = 0;

    // Percent bonuses (0.10 = +10%)
    public float AttackPercent { get; set; } = 0f;
    public float DefensePercent { get; set; } = 0f;
    public float HealthPercent { get; set; } = 0f;

    public static Stats operator +(Stats a, Stats b)
    {
        return new Stats
        {
            Attack = a.Attack + b.Attack,
            Defense = a.Defense + b.Defense,
            Health = a.Health + b.Health,
            Agility = a.Agility + b.Agility,
            Luck = a.Luck + b.Luck,

            AttackPercent = a.AttackPercent + b.AttackPercent,
            DefensePercent = a.DefensePercent + b.DefensePercent,
            HealthPercent = a.HealthPercent + b.HealthPercent
        };
    }
}

// Item class, represents both inventory items and equipped items
class Item
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public int Quantity { get; set; } = 1;
    public string Extras { get; set; } = ""; // For things like "x3"
    public ItemType Type { get; set; }
    public Stats Stats { get; set; } = new Stats();
}

// All Game State data
class GameState
{
    public string PlayerName = "";
    public List<string> Log = new();
    public bool Running = true;
    public Stats BaseStats = new Stats
    {
        Attack = 2,
        Defense = 1,
        Health = 10,
        Agility = 1,
        Luck = 0
    };
    public List<Stats> ActiveBuffs = new();
    public GameMode Mode = GameMode.Actions;

    public List<Item> Inventory = new()
    {
        new Item { Name = "Health Potion", Description = "Restores health.", Type = ItemType.Consumable },
        new Item { Name = "Iron Sword", Description = "A sharp blade.", Type = ItemType.Weapon, Stats = new Stats { Attack = 5 } },
        new Item { Name = "Leather Vest", Description = "Basic protection.", Type = ItemType.Chestplate, Stats = new Stats { Defense = 3 } },
        new Item { Name = "Lucky Ring", Description = "Feels strange...", Type = ItemType.Accessory, Stats = new Stats { Attack = 1, Defense = 1, Luck = 3, Agility = -1 } }
    };

    public Item? SelectedItem = null;

    // Equipment slots
    public Dictionary<ItemType, Item> Equipment = new();
    //                Key  /  Value

    public bool InventoryChanged = true;

    public void AddLog(string message)
    {
        Log.Add(message);

        if (Log.Count > 14)
            Log.RemoveAt(0);
    }
    public Stats GetTotalStats()
    {
        Stats total = new Stats
        {
            Attack = BaseStats.Attack,
            Defense = BaseStats.Defense,
            Health = BaseStats.Health,
            Agility = BaseStats.Agility,
            Luck = BaseStats.Luck
        };

        Stats modifiers = new Stats();

        foreach (var eq in Equipment)
            modifiers += eq.Value.Stats;

        foreach (var buff in ActiveBuffs)
            modifiers += buff;

        // Apply flat bonuses first
        total += modifiers;

        // Apply percent bonuses AFTER
        total.Attack = (int)(total.Attack * (1 + modifiers.AttackPercent));
        total.Defense = (int)(total.Defense * (1 + modifiers.DefensePercent));
        total.Health = (int)(total.Health * (1 + modifiers.HealthPercent));

        return total;
    }
}

// Menu system, all menus are just instances of this with different options and display functions
class Menu<T>
{
    public T[] Options { get; private set; } = Array.Empty<T>();
    public int SelectedIndex { get; private set; }

    private readonly Func<T, string> _display;

    /*
        We pass in a function to determine how to display each option, 
        this allows us to use the same menu class for both the action menu 
        (which just displays strings) and the inventory menu (which needs to 
        display item names but also has a "Back" option which is a string)
    */
    public Menu(Func<T, string> display)
    {
        _display = display;
    }

    public void SetOptions(T[] options) // Ensures the selected index is always valid when changing options
    {
        Options = options;

        if (SelectedIndex >= Options.Length)
            SelectedIndex = Options.Length - 1;

        if (SelectedIndex < 0)
            SelectedIndex = 0;
    }

    public void HandleInput(ConsoleKey key)
    {
        switch (key)
        {
            case ConsoleKey.UpArrow:
                SelectedIndex = (SelectedIndex - 1 + Options.Length) % Options.Length;
                break;

            case ConsoleKey.DownArrow:
                SelectedIndex = (SelectedIndex + 1) % Options.Length;
                break;
        }
    }

    public T GetSelected() => Options[SelectedIndex]; // Returns the currently selected option

    public IRenderable Render()
    {
        var text = "";

        for (int i = 0; i < Options.Length; i++)
        {
            var label = _display(Options[i]);

            if (i == SelectedIndex)
                text += $"[black on yellow]> {label}[/]\n";
            else
                text += $"  {label}\n";
        }

        return new Markup(text);
    }
}

// UI System
class GameUI
{
    public float GetCritChance(Stats stats)
    {
        return MathF.Min(1f, stats.Luck * 0.02f); // max 100%, 1 Luck = 2% crit
    }
    public int GetComboCount(Stats stats)
    {
        return 1 + (stats.Agility / 5);
    }
    private readonly Layout _layout;

    public GameUI() // Sets initial layout structure, we will just update the content of each panel later
    {
        _layout = new Layout("Root")
            .SplitColumns(
                new Layout("Left").SplitRows(
                    new Layout("UpperLeft").Ratio(2).SplitRows(
                        new Layout("World").Ratio(3),
                        new Layout("Stats").Ratio(1)),
                    new Layout("Actions").Ratio(1)
                ),
                new Layout("Right").SplitRows(
                    new Layout("UpperRight").Ratio(3).SplitColumns(
                        new Layout("Inventory").Ratio(1),
                        new Layout("Equipment").Ratio(1)),
                    new Layout("Log").Ratio(1)
                )
            );
    }

    public Layout GetLayout() => _layout;

    public void Render(GameState state, IRenderable menu) // This is where we update all the panels with the current game state and menu
    {
        // Initialize inventory table
        var inventoryTable = new Table()
                    .AddColumn("Item")
                    .AddColumn("Details", col => col.RightAligned())
                    .NoBorder()
                    .Expand();
        // Add items to inventory table with appropriate details and coloring for equipped items
        foreach (var item in state.Inventory)
        {
            item.Extras = "";
            if (item.Type == ItemType.Consumable && item.Quantity >= 0)
                item.Extras = $"x{item.Quantity}";
            else if (item.Type != ItemType.Consumable)
                foreach (var stat in typeof(Stats).GetProperties())
                {

                    if (stat.Name.Contains("Percent"))
                        continue;

                    var value = stat.GetValue(item.Stats);

                    if (value is int intValue && intValue != 0)
                        item.Extras += $"{(intValue > 0 ? "+" : "")}{intValue} {stat.Name}, ";
                }

            if (state.Equipment.TryGetValue(item.Type, out var eq) && eq == item) // If item is equipped, show green and (E)
                inventoryTable.AddRow($"[green]{item.Name} (E)[/]", $"{item.Extras}");
            else
                inventoryTable.AddRow(item.Name, item.Extras);
        }

        // Log to display
        var logText = string.Join("\n", state.Log);

        // Equipment slots (always visible)
        var equipText = "";

        foreach (ItemType type in Enum.GetValues(typeof(ItemType)))
        {
            if (type == ItemType.Consumable) // If it's a consumable, we don't have an equipment slot for it
                continue;

            if (state.Equipment.TryGetValue(type, out var item))
                equipText += $"{type}: [green]{item.Name}[/]\n";
            else
                equipText += $"{type}: [grey]None[/]\n";
        }

        // Get Stats to display
        var stats = state.GetTotalStats();

        float crit = GetCritChance(stats);
        int combo = GetComboCount(stats);
        var statsText = "";
        foreach (var stat in typeof(Stats).GetProperties())
        {
            if (stat.Name.Contains("Percent"))
                continue;

            var value = stat.GetValue(stats);

            statsText += $"{stat.Name}:{new string(' ', 8 - stat.Name.Length)}{value}\n";
        }
        statsText += $"Crit Chance: {crit * 100:0}%\n";
        statsText += $"Combo Hits: {combo}\n";

        // World
        _layout["World"].Update(
            new Panel($"[red]{state.PlayerName} stands in an open field...[/]\n\n")
                .Header("[white]World[/]")
                .BorderColor(Color.Red3_1)
                .Expand()
        );

        // Stats
        _layout["Stats"].Update(
            new Panel(statsText)
                .Header("[white]Stats[/]")
                .BorderColor(Color.Red3_1)
                .Expand()
        );

        // Actions
        _layout["Actions"].Update(
            new Panel(menu)
                .Header("[white]Actions[/]")
                .BorderColor(Color.Red3_1)
                .Expand()
        );

        // Inventory
        _layout["Inventory"].Update(
            new Panel(inventoryTable)
                .Header("[white]Inventory[/]")
                .BorderColor(Color.Red3_1)
                .Expand()
        );

        // Equipment
        _layout["Equipment"].Update(
            new Panel(equipText)
                .Header("[white]Equipment[/]")
                .BorderColor(Color.Red3_1)
                .Expand()
        );

        // Log
        _layout["Log"].Update(
            new Panel(logText)
                .Header("[white]Log[/]")
                .BorderColor(Color.Red3_1)
                .Expand()
        );
    }
}

// Username Input
static class Input
{
    public static string GetUserName()
    {
        while (true)
        {
            var name = AnsiConsole.Ask<string>("What is your name [red]traveller[/]?");

            if (AnsiConsole.Confirm($"[red]{name}[/], is that right?"))
                return name;
        }
    }
}

// Main Program
class Program
{
    static void temp()
    {
        AnsiConsole.Clear();

        var state = new GameState
        {
            PlayerName = Input.GetUserName()
        };

        var actionMenu = new Menu<string>(x => x);
        actionMenu.SetOptions(new[]
        {
            "Attack",
            "Flee",
            "Open Inventory",
            "Quit"
        });

        var inventoryMenu = new Menu<object>(x => // We use object here because we want to display both Items and the "Back" option, which is a string
        {
            return x switch // Pattern matching to determine how to display each option
            {
                Item item => item.Name, // If it's an item, display its name
                string s => s, // If it's a string (the "Back" option), display it as is
                _ => x?.ToString() ?? "" // Fallback for any other types, should never happen in this case
            };
        });

        var itemMenu = new Menu<string>(x => x);
        itemMenu.SetOptions(new[]
        {
            "Use",
            "Inspect",
            "Drop",
            "Back"
        });

        var UI = new GameUI();

        AnsiConsole.Clear();

        AnsiConsole.Live(UI.GetLayout()).Start(ctx =>
        {
            state.AddLog($"[green]Welcome, {state.PlayerName}![/]");

            while (state.Running)
            {
                IRenderable currentMenu;

                if (state.Mode == GameMode.Actions)
                {
                    currentMenu = actionMenu.Render();
                }
                else if (state.Mode == GameMode.Inventory)
                {
                    if (state.InventoryChanged)
                    {
                        var items = new List<object>(state.Inventory);
                        items.Add("Back");

                        inventoryMenu.SetOptions(items.ToArray());
                        state.InventoryChanged = false;
                    }

                    currentMenu = inventoryMenu.Render();
                }
                else
                {
                    currentMenu = itemMenu.Render();
                }

                UI.Render(state, currentMenu);
                ctx.Refresh();

                var key = Console.ReadKey(true);

                if (key.Key != ConsoleKey.Enter)
                {
                    if (state.Mode == GameMode.Actions)
                        actionMenu.HandleInput(key.Key);
                    else if (state.Mode == GameMode.Inventory)
                        inventoryMenu.HandleInput(key.Key);
                    else
                        itemMenu.HandleInput(key.Key);

                    continue;
                }

                if (state.Mode == GameMode.Actions)
                {
                    var choice = actionMenu.GetSelected();

                    switch (choice)
                    {
                        case "Attack":
                            state.AddLog("[red]You strike the enemy![/]");
                            break;

                        case "Flee":
                            state.AddLog("[yellow]You attempt to flee...[/]");
                            break;

                        case "Open Inventory":
                            state.Mode = GameMode.Inventory;
                            state.InventoryChanged = true;
                            state.AddLog("[blue]You open your inventory.[/]");
                            break;

                        case "Quit":
                            state.AddLog("[grey]You abandon your quest.[/]");
                            state.Running = false;
                            break;
                    }
                }
                else if (state.Mode == GameMode.Inventory)
                {
                    var choice = inventoryMenu.GetSelected();

                    if (choice is "Back")
                    {
                        state.Mode = GameMode.Actions;
                        state.AddLog("[grey]You close your inventory.[/]");
                    }
                    else if (choice is Item item)
                    {
                        state.SelectedItem = item;
                        state.Mode = GameMode.ItemInteraction;
                        state.AddLog($"[green]Selected {item.Name}.[/]");
                    }
                }
                else
                {
                    var choice = itemMenu.GetSelected();
                    var item = state.SelectedItem;

                    if (item == null)
                        continue;

                    switch (choice)
                    {
                        case "Use":
                            if (item.Type == ItemType.Consumable)
                            {
                                state.AddLog($"[green]You use {item.Name}.[/]");
                            }
                            else
                            {
                                if (state.Equipment.TryGetValue(item.Type, out var equipped) &&
                                    equipped == item)
                                {
                                    state.Equipment.Remove(item.Type);
                                    state.AddLog($"[grey]Unequipped {item.Name}.[/]");
                                }
                                else
                                {
                                    state.Equipment[item.Type] = item;
                                    state.AddLog($"[green]Equipped {item.Name}.[/]");
                                }
                            }
                            break;

                        case "Inspect":
                            state.AddLog($"[yellow]{item.Description}[/]");
                            state.AddLog($"[grey]Type: {item.Type}[/]");
                            break;

                        case "Drop":
                            state.Inventory.Remove(item);
                            state.InventoryChanged = true;
                            state.Mode = GameMode.Inventory;
                            state.AddLog($"[red]Dropped {item.Name}.[/]");
                            break;

                        case "Back":
                            state.Mode = GameMode.Inventory;
                            break;
                    }
                }
            }
        });

        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("[grey]Game over.[/]");
    }
}