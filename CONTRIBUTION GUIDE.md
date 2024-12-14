# Github Contributions
- Always create new branches whenever you're working on a new piece/pieces of content to avoid conflicts with other people's code.
- NEVER directly commit to `development` or `public-release` unless it's a very small change or you have permission to do so.
- PRs should be reviewed by at least 2 other programmers on the team before being accepted.

## File Structure Conventions
### Source Files
C# source files for content should be placed in the Content root directory such that its path will generally look like this:

Content/[Type]/[Theme]/[InternalName]{.MethodDescriptor}

where [Type] is the type of content the source file contains (i.e. Item, NPC, or Tile), [Theme] is the artistic theme
or set identity, and [InternalName] is the internal name of the content (or the primary class of the file). If a partial class,
it should have a suffix {.MethodDescriptor} describing what methods are within that part. If not a partial class do not include a
suffix.

Source files for large systems or anything not specifically related to any one piece of content should be placed in the Core root 
directory. Core source files should descriptively name the functionality they manage in their file name.

You should only place multiple classes in a file if the content they describe are closely linked (like an item and its holdout projectile)
and the file's length is less than 350 lines.

Please make sure namespaces match folder structure.

### Non-source files
Unless it is a localization file, anything that isn't a source file should go in the Assets root directory, with the only exception being AssetRegistry.cs. 
Within Assets it should be placed so that its path mirrors the path of the content using it, with [Type] being replaced with the asset's type 
(sounds go in Sounds, textures in Textures, structures in Structures, 3d models in Models, etc). If the asset is used by multiple pieces of
content or is not used by content, it should go in its type's Extra directory.

Asset names should describe their application in the source. For example noise maps that are used by multiple shaders should be 
called whatever their noise is (i.e. SimplexNoise). If you require a suffix in the name, like say for a glowmask, format it with a _ at the 
start. (i.e. CosmostoneGeode_Glow).

Some extra things:
- If a texture is an atlas, it goes in the Atlases directory instead of the Textures directory.
- Effects go in AutoloadedEffects, not Effects, and do not mirror the Content directory.
- The Structures directory doesn't include a [Type] directory, and instead is organized primarily by [Theme].
- Always include .fx source files with the compiled shaders.
- Make sure textures are all .png and sounds are .ogg

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
This is how code should be ordered in a .cs file:
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
