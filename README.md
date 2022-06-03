# Tabalog
 
## What is Tabalog?
**Tabalog is a easy to use and easy to understand tool for quickly Loading and Saving data**. The file format is super simple, like maybe too simple. Named Tabalog because it kinda looks like a Catalog, and for it's use of tabs.

A .tabalog file file can look like this:

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
