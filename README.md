# Tabalog
 
## What is Tabalog?
**Tabalog is a easy to use and easy to understand tool for quickly Loading and Saving data**. The file format is super simple, like maybe too simple. Named Tabalog because it kinda looks like a Catalog, and for it's use of tabs.

For example - a `.tabalog` file file can look like this:

```
Control
	Drag Strength : 10000
	Invert Placement Rotation : true
Hookup
	Distances
		Max : 10
		Min : 2
	Ripping
		Chance To Not Destroy On Rip : 4
		Delay : 3
		Force : 15000
		Resistance : 4000
Structure
	Drag Multiplyer : 4
	Rotation Drag Multiplyer : 3
	Weight Multiplyer : 10
```

As you can see it looks awfully similar to a the structure of a folder. The file contains 3 sections, the first is the **Control** section, the second is the **Hookup** section, and the third is the **Structure** section. The control section contains two sections, one is **Drag Strength** and the other is **Invert Placement Rotation**. These two sections have values set to them.

To note, just because a section has a value set to it, does not mean that it cannot contain sections - they can contain sections and have a value.

## How do I use it?
First things first, you need to setup a `.tabalog` file, look at the above section to understand it's structure.

In Unity, create a GameObject and attach the **TabalogHost** script to it. Click on "Open File" to well... open a file...

Open your `.tagalog` file and it will be loaded in. Alot more options will now appear. The dropdown at the top shows the raw data loaded in, simply just showing the text. At the bottom, it will show the rendered out data. Above that, there will be a Text feild in which you can type in a **key**, and either create, remove, or modify it.

#### Working with Keys in Script
You can think of Tabalog as a Dictionary Saver and Loader (*Which is basicaly what it is*). Getting a Key's value is as simple as
```
TabalogHost["Key/Subkey/Subberkey/Even Subber Key"]
```

Creating (and/or Modifying) a Key is as simple as
```
TabalogHost["Key/Subber key/Even Subber Key] = NewValue
```

Tabalog will automatically create a all the keys needed. If you do the above function and `Key` and `Key/Subber key` don't exist, then they will be created with no values.

You can also call this get all the children of a key :
```
TabalogHost.GetSub(desiredkey)
```

And use this to get a bool back if the key... well... exists :
```
TabalogHost.Exists(keyToCheck)
```

Removing a Key is unsurprisingly as simple as calling this, This not only removes the desired key, but all of it's subkeys as well. 
```
TabalogHost.Remove(desiredKey)
```

#### Saving Data
To Save the data, all you have to do is call this and it will save it to the file that the data was orginaly loaded from.
```
TabalogHost.Save()
```

You can also Save the data to any other file using this, if no path is givin, then it will open up a window for you to choose.
```
Tabalog.SaveAs(optionalPath)
```

Both of these saving functions all are so avaliable in the Inspector.

#### Loading Data

To Load Files via Script, you can use this function to to read and process a file (If the path is left unassinged, it will use the previous set path) :
```
TabalogHost.LoadFile(optionalPath)
```

Selecting a file from file exploxer is as simple as :
```
TabalogHost.SelectFile()
```

## Why should I use it?
I'm not saying you should, but I mean... ya could if you want... I don't know if it's any good... This is just a personal project of mine...

¯\\\_(ツ)\_/¯