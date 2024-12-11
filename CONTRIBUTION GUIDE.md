# NOTICE: THIS WILL PROB BE REWRITTEN SOON



# Github Contributions
- Always create new branches whenever you're working on a new piece/pieces of content to avoid conflicts with other people's code.
- NEVER directly commit to `development` or `public-release` unless it's a very small change or you have permission to do so.
- PRs should be reviewed by at least 2 other programmers on the team before being accepted.

# Programming Contributions
## Naming Conventions

1. Use **PascalCase** when naming Public and Private global fields, methods, constructors, properties, enums, constants, namespaces, classes and delegates.

```c#
// Correct.
public float PlayerSpeed;
public int Time left;

// Avoid.
public int playerName;
public void getPlayerName() { }
```

2. Use **camelCase** when naming local fields and method arguments.

```c#
// Correct.
private int playerSpeed;
private void GetPlayerAcceleration(float playerAcceleration) { }

// Avoid.
private int PlayerHeight;
public void MeasurePlayerDimensions(int Width, int Height) { }
```

3. Do not use **Screaming Caps** when naming constant variables.

```c#
// Correct.
public constant int Projectiles = 2;

// Avoid.
public constant int PROJECTILES = 1;
```

4. Do not use abbreviations.

```c#
// Correct.
ModPlayer modPlayer;
ResplendentRoarPlayer resplendentRoarPlayer;

// Avoid.
OrbitalGravitySystem orbGravSys;
ModAchievement modAchvmnt;
```

## Formatting
### General Formatting Rules
- Use new lines to block related chunks of code together and as padding between class members.
- Always use accessibility modifiers on global members, with the only exceptions being explicit interface implementations.
- Only use the `var` keyword in areas where the type it is replacing is long. Please avoid using it otherwise.

### Braces
1. Always stick to using allman-styled braces. 
```c#
if (boolean is true)
{
    code running 1;
    code running 2;
}
```

2. Do not use braces for single-line expressions.
```c#
// Correct.
public override bool CanDamage() => true;

// Avoid.
public override bool CanDamage()
{
    return true;
}
```

The above also applies to `if` statements and loops. In the case of nested-single line loops, all loops except the innermost should use brackets.
```c#
// Correct.
if (cause)
    effect();

for (int i = 0; i < 10; i++)
{
    for (int j = 0; j < 20; j++)
        TheThingToLoop();
}

// Avoid.
if (cause)
{
    effect();
}

for (int i = 0; i < 10; i++)
{
    for (int j = 0; j < 20; j++)
    {
        TheThingToLoop();
    }
}
```

### Tabs
Tabs should be set as Spaces with an indention of size 4. Make sure this is set properly in VS Studio.

### .cs file Ordering
This is show code should be ordered in a .cs file.
```c#
usings

namespace
{
    Class/Struct/Interface/Enum
    {
        Nested classes/interfaces/enums/structs
        Fields
        Properties
        Delegates
        Events
        Constructors
        Deconstructors
        Public Instance Methods
        Private Instance Methods
        Public Static Methods
        Private Static Methods
    }
}
```
